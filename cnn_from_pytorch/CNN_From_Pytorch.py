import ssl
ssl._create_default_https_context = ssl._create_unverified_context

import torch
import torch.nn as nn
import torch.nn.functional as F
import torch.optim as optim
from torch.utils.data import DataLoader
from torchvision import datasets, transforms
from torchvision.transforms import ToTensor
import time
import os
import sys

# ---------- 1. 模型定义（ResNet18） ----------
class BasicBlock(nn.Module):
    expansion = 1
    def __init__(self, in_planes, planes, stride=1):
        super().__init__()
        self.conv1 = nn.Conv2d(in_planes, planes, kernel_size=3, stride=stride, padding=1, bias=False)
        self.bn1 = nn.BatchNorm2d(planes)
        self.conv2 = nn.Conv2d(planes, planes, kernel_size=3, stride=1, padding=1, bias=False)
        self.bn2 = nn.BatchNorm2d(planes)
        self.shortcut = nn.Sequential()
        if stride != 1 or in_planes != self.expansion * planes:
            self.shortcut = nn.Sequential(
                nn.Conv2d(in_planes, self.expansion * planes, kernel_size=1, stride=stride, bias=False),
                nn.BatchNorm2d(self.expansion * planes)
            )

    def forward(self, x):
        out = F.relu(self.bn1(self.conv1(x)))
        out = self.bn2(self.conv2(out))
        out += self.shortcut(x)
        out = F.relu(out)
        return out

class ResNet(nn.Module):
    def __init__(self, block, num_blocks, num_classes=10):
        super().__init__()
        self.in_planes = 64
        self.conv1 = nn.Conv2d(3, 64, kernel_size=3, stride=1, padding=1, bias=False)
        self.bn1 = nn.BatchNorm2d(64)
        self.layer1 = self._make_layer(block, 64, num_blocks[0], stride=1)
        self.layer2 = self._make_layer(block, 128, num_blocks[1], stride=2)
        self.layer3 = self._make_layer(block, 256, num_blocks[2], stride=2)
        self.layer4 = self._make_layer(block, 512, num_blocks[3], stride=2)
        self.linear = nn.Linear(512 * block.expansion, num_classes)

    def _make_layer(self, block, planes, num_blocks, stride):
        strides = [stride] + [1] * (num_blocks - 1)
        layers = []
        for s in strides:
            layers.append(block(self.in_planes, planes, s))
            self.in_planes = planes * block.expansion
        return nn.Sequential(*layers)

    def forward(self, x):
        out = F.relu(self.bn1(self.conv1(x)))
        out = self.layer1(out)
        out = self.layer2(out)
        out = self.layer3(out)
        out = self.layer4(out)
        out = F.avg_pool2d(out, 4)
        out = out.view(out.size(0), -1)
        out = self.linear(out)
        return out

def ResNet18():
    return ResNet(BasicBlock, [2, 2, 2, 2])

# ---------- 2. 日志工具 ----------
class Tee:
    def __init__(self, filename, mode='w'):
        self.file = open(filename, mode, encoding='utf-8')
        self.stdout = sys.stdout
        sys.stdout = self

    def write(self, data):
        self.file.write(data)
        self.stdout.write(data)
        self.flush()

    def flush(self):
        self.file.flush()
        self.stdout.flush()

    def close(self):
        sys.stdout = self.stdout
        self.file.close()

# ---------- 3. 主程序（必须放在 if __name__ == '__main__' 下以避免Windows多进程问题） ----------
if __name__ == '__main__':
    # 重定向输出到日志文件（放在最前面，以便记录所有输出）
    tee = Tee('trainlog.txt', mode='w')

    # 配置参数
    epochs = 30
    batch_size = 128
    learning_rate = 0.1
    momentum = 0.9
    weight_decay = 5e-4
    data_dir = './data'

    device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
    print(f"使用设备: {device}")
    if device.type == 'cpu':
        print(f"CPU 核心数: {os.cpu_count()}, 将使用全部核心（不限制线程）")

    # ---------- 4. 数据加载 ----------
    transform_train = transforms.Compose([
        transforms.RandomCrop(32, padding=4),
        transforms.RandomHorizontalFlip(),
        ToTensor(),
        transforms.Normalize((0.4914, 0.4822, 0.4465), (0.2023, 0.1994, 0.2010)),
    ])
    transform_test = transforms.Compose([
        ToTensor(),
        transforms.Normalize((0.4914, 0.4822, 0.4465), (0.2023, 0.1994, 0.2010)),
    ])

    train_set = datasets.CIFAR10(root=data_dir, train=True, download=True, transform=transform_train)
    test_set = datasets.CIFAR10(root=data_dir, train=False, download=True, transform=transform_test)

    # 重要：num_workers=0 避免Windows多进程错误，CPU下已足够
    train_loader = DataLoader(train_set, batch_size=batch_size, shuffle=True, num_workers=0, pin_memory=False)
    test_loader = DataLoader(test_set, batch_size=batch_size, shuffle=False, num_workers=0, pin_memory=False)

    # ---------- 5. 模型、损失、优化器 ----------
    model = ResNet18().to(device)
    criterion = nn.CrossEntropyLoss()
    optimizer = optim.SGD(model.parameters(), lr=learning_rate, momentum=momentum, weight_decay=weight_decay)
    scheduler = optim.lr_scheduler.CosineAnnealingLR(optimizer, T_max=epochs)

    print(f"训练轮数: {epochs} 轮\n")

    # ---------- 6. 训练与测试函数 ----------
    def train_epoch(epoch):
        model.train()
        total_loss = 0
        correct = 0
        total = 0
        for batch_idx, (data, target) in enumerate(train_loader):
            data, target = data.to(device), target.to(device)
            optimizer.zero_grad()
            output = model(data)
            loss = criterion(output, target)
            loss.backward()
            optimizer.step()

            total_loss += loss.item()
            _, pred = output.max(1)
            total += target.size(0)
            correct += pred.eq(target).sum().item()

            # 每 50 个 batch 打印一次进度（频繁刷新）
            if batch_idx % 50 == 0:
                current_acc = 100. * correct / total if total > 0 else 0
                print(f"Epoch {epoch:2d} | Batch {batch_idx:4d}/{len(train_loader)} | Loss: {loss.item():.4f} | Acc: {current_acc:.2f}%")

        # 训练结束，只记录不打印平均损失（避免冗余）
        return 100. * correct / total

    def test_epoch(epoch):
        model.eval()
        correct = 0
        total = 0
        with torch.no_grad():
            for data, target in test_loader:
                data, target = data.to(device), target.to(device)
                output = model(data)
                _, pred = output.max(1)
                total += target.size(0)
                correct += pred.eq(target).sum().item()
        acc = 100. * correct / total
        # 每个 epoch 只打印一行测试准确率
        print(f"Epoch {epoch:2d} 测试准确率: {acc:.2f}%\n")
        return acc

    # ---------- 7. 主循环 ----------
    best_acc = 0.0
    start_all = time.time()

    for epoch in range(1, epochs + 1):
        train_acc = train_epoch(epoch)          # 训练（内部频繁打印）
        test_acc = test_epoch(epoch)            # 测试（只打印一行）
        scheduler.step()

        if test_acc > best_acc:
            best_acc = test_acc
            torch.save(model.state_dict(), "best_resnet18_cifar10.pth")
            print(f"💾 新最佳模型保存 (准确率: {best_acc:.2f}%)\n")

    # ---------- 8. 最终简洁报告 ----------
    total_time = (time.time() - start_all) / 60
    print("\n" + "="*40)
    print("          训练结束 - 结果报告")
    print("="*40)
    print(f"总训练轮数: {epochs}")
    print(f"总耗时: {total_time:.2f} 分钟")
    print(f"最佳测试准确率: {best_acc:.2f}%")
    print(f"模型权重: best_resnet18_cifar10.pth")
    print("="*40)

    # 关闭日志重定向
    tee.close()