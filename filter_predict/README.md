# 滤波器S参数预测项目
## 项目说明
输入：滤波器几何尺寸参数L_loc1,l_loc2,l_loc3,即三阶微带滤波器的L
输出：对应频点S参数，4Ghz频点的dB(S21)
使用全连接神经网络做回归任务；对比不同网络深度模型的预测性能。
数据集来自ADS仿真批量扫参导出CSV文件。由于ADS操作还较为生疏，所以只修改了三个几何参数，每个修改六个点，总共仅216个数据作为一个练习。

## 📁 数据集
RF_AI\data\filter_dataset_clean.csv存放ADS导出仿真数据
- 输入特征：滤波器几何参数
- 标签：4Ghz点的dB(S21)
## 环境依赖
安装依赖：
mincionda新建环境RF_AI
python3.9   numpy    pandas  matplotlib   pytorch