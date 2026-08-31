import pandas as pd
import numpy as np
import torch
import os
import torch.nn as nn
import torch.optim as optim
from torch.utils.data import Dataset, DataLoader
import matplotlib.pyplot as plt

# ===================== 1. 读取数据集 =====================
df = pd.read_csv(r"C:\Users\Lenovo\Desktop\WORK\RF_AI\code\filter_dataset_clean.csv")

print("数据集形状：", df.shape)
print(df.head())

X_data = df[["L_loc1", "L_loc2", "L_lin"]].values
y_data = df["IL_4G"].values.reshape(-1, 1)   # 输出变成N×1

# 划分训练集、测试集 7:3
split_rate = 0.7
n_total = len(X_data)
n_train = int(n_total * split_rate)

# 简单随机划分
indices = np.random.permutation(n_total)
train_idx = indices[:n_train]
test_idx = indices[n_train:]

X_train = X_data[train_idx]
y_train = y_data[train_idx]
X_test = X_data[test_idx]
y_test = y_data[test_idx]

# 归一化！回归任务非常关键，输入输出都做归一化
from sklearn.preprocessing import StandardScaler
scaler_x = StandardScaler()
scaler_y = StandardScaler()

X_train = scaler_x.fit_transform(X_train)
X_test = scaler_x.transform(X_test)

y_train = scaler_y.fit_transform(y_train)
y_test = scaler_y.transform(y_test)

# 转为torch张量
X_train_tensor = torch.tensor(X_train, dtype=torch.float32)
y_train_tensor = torch.tensor(y_train, dtype=torch.float32)
X_test_tensor = torch.tensor(X_test, dtype=torch.float32)
y_test_tensor = torch.tensor(y_test, dtype=torch.float32)

# 数据集封装
class FilterDataset(Dataset):
    def __init__(self, x, y):
        self.x = x
        self.y = y
    def __len__(self):
        return len(self.x)
    def __getitem__(self, idx):
        return self.x[idx], self.y[idx]

train_ds = FilterDataset(X_train_tensor, y_train_tensor)
test_ds = FilterDataset(X_test_tensor, y_test_tensor)

train_loader = DataLoader(train_ds, batch_size=16, shuffle=True)
test_loader = DataLoader(test_ds, batch_size=16, shuffle=False)

# ===================== 2. 定义3种深度的全连接模型 =====================
# 模型1：浅网络 1隐藏层
class Net_Shallow(nn.Module):
    def __init__(self, in_dim=3, hidden_dim=64):
        super().__init__()
        self.net = nn.Sequential(
            nn.Linear(in_dim, hidden_dim),
            nn.ReLU(),
            nn.Linear(hidden_dim, 1)
        )
    def forward(self, x):
        return self.net(x)

# 模型2：中等深度 2隐藏层
class Net_Medium(nn.Module):
    def __init__(self, in_dim=3, hidden_dim=64):
        super().__init__()
        self.net = nn.Sequential(
            nn.Linear(in_dim, hidden_dim),
            nn.ReLU(),
            nn.Linear(hidden_dim, hidden_dim),
            nn.ReLU(),
            nn.Linear(hidden_dim, 1)
        )
    def forward(self, x):
        return self.net(x)

# 模型3：较深网络 3隐藏层
class Net_Deep(nn.Module):
    def __init__(self, in_dim=3, hidden_dim=64):
        super().__init__()
        self.net = nn.Sequential(
            nn.Linear(in_dim, hidden_dim),
            nn.ReLU(),
            nn.Linear(hidden_dim, hidden_dim),
            nn.ReLU(),
            nn.Linear(hidden_dim, hidden_dim),
            nn.ReLU(),
            nn.Linear(hidden_dim, 1)
        )
    def forward(self, x):
        return self.net(x)

# ===================== 3. 训练通用函数 =====================
def train_model(model, train_loader, test_loader, epochs=300, lr=1e-3):
    criterion = nn.MSELoss()
    optimizer = optim.Adam(model.parameters(), lr=lr)

    train_loss_history = []
    test_loss_history = []

    for epoch in range(epochs):
        model.train()
        train_loss_sum = 0.0
        for batch_x, batch_y in train_loader:
            optimizer.zero_grad()
            pred = model(batch_x)
            loss = criterion(pred, batch_y)
            loss.backward()
            optimizer.step()
            train_loss_sum += loss.item() * batch_x.shape[0]
        train_loss_avg = train_loss_sum / len(train_loader.dataset)

        # test
        model.eval()
        test_loss_sum = 0.0
        with torch.no_grad():
            for batch_x, batch_y in test_loader:
                pred = model(batch_x)
                loss = criterion(pred, batch_y)
                test_loss_sum += loss.item() * batch_x.shape[0]
        test_loss_avg = test_loss_sum / len(test_loader.dataset)

        train_loss_history.append(train_loss_avg)
        test_loss_history.append(test_loss_avg)

        if (epoch+1) % 50 == 0:
            print(f"Epoch {epoch+1:3d} | Train MSE:{train_loss_avg:.4f} | Test MSE:{test_loss_avg:.4f}")

    return train_loss_history, test_loss_history

# ===================== 4. 分别训练三个模型 =====================
if __name__ == "__main__":
    print("\n===== 训练：1层隐藏层（浅网络） =====")
    model_shallow = Net_Shallow()
    loss1_train, loss1_test = train_model(model_shallow, train_loader, test_loader, epochs=300)

    print("\n===== 训练：2层隐藏层（中等网络） =====")
    model_medium = Net_Medium()
    loss2_train, loss2_test = train_model(model_medium, train_loader, test_loader, epochs=300)

    print("\n===== 训练：3层隐藏层（深网络） =====")
    model_deep = Net_Deep()
    loss3_train, loss3_test = train_model(model_deep, train_loader, test_loader, epochs=300)

      # ===================== 5. 绘图对比loss曲线 =====================
    plt.figure(figsize=(10,6))
    plt.plot(loss1_test, label="1 hidden layer(Test MSE)", color="#1f77b4")
    plt.plot(loss2_test, label="2 hidden layer(Test MSE)", color="#ff7f0e")
    plt.plot(loss3_test, label="3 hidden layer(Test MSE)", color="#2ca02c")
    plt.xlabel("Epoch")
    plt.ylabel("MSE loss(normalized space)")
    plt.title("不同网络深度测试集MSE对比")
    plt.legend()
    plt.grid(alpha=0.3)

    # 获取脚本路径，无多余缩进
    script_dir = os.path.dirname(os.path.abspath(__file__))
    save_fig_path = os.path.join(script_dir,"loss_curve.png")
    # 【重要】先保存，再show！
    plt.savefig(save_fig_path, dpi=300, bbox_inches="tight")
    print("loss曲线图片保存至：",save_fig_path)
    plt.show()
    plt.close() # 释放画布内存

    # 输出最终测试集MSE
    print("\n=====最终测试集MSE（归一化空间）====")
    print(f"1隐藏层: {loss1_test[-1]:.5f}")
    print(f"2隐藏层: {loss2_test[-1]:.5f}")
    print(f"3隐藏层: {loss3_test[-1]:.5f}")
