using System;
using System.Collections.Generic;
using System.IO;

// ====================== 面向对象：复数封装（射频计算基础） ======================
/// <summary>
/// 复数：a + jb
/// </summary>
public class Complex
{
    public double Real { get; set; }
    public double Imag { get; set; }

    public Complex(double real, double imag)
    {
        Real = real;
        Imag = imag;
    }

    // 复数加法
    public static Complex operator +(Complex c1, Complex c2)
    {
        return new Complex(c1.Real + c2.Real, c1.Imag + c2.Imag);
    }

    // 复数减法
    public static Complex operator -(Complex c1, Complex c2)
    {
        return new Complex(c1.Real - c2.Real, c1.Imag - c2.Imag);
    }

    // 复数乘法
    public static Complex operator *(Complex c1, Complex c2)
    {
        double r = c1.Real * c2.Real - c1.Imag * c2.Imag;
        double i = c1.Real * c2.Imag + c1.Imag * c2.Real;
        return new Complex(r, i);
    }

    // 复数除法
    public static Complex operator /(Complex c1, Complex c2)
    {
        double den = c2.Real * c2.Real + c2.Imag * c2.Imag;
        double r = (c1.Real * c2.Real + c1.Imag * c2.Imag) / den;
        double i = (c1.Imag * c2.Real - c1.Real * c2.Imag) / den;
        return new Complex(r, i);
    }

    // 模值
    public double Magnitude()
    {
        return Math.Sqrt(Real * Real + Imag * Imag);
    }

    // 幅角（弧度）
    public double PhaseRad()
    {
        return Math.Atan2(Imag, Real);
    }

    // 幅角（角度）
    public double PhaseDeg()
    {
        return PhaseRad() * 180 / Math.PI;
    }

    public override string ToString()
    {
        if (Imag >= 0)
            return $"{Real:F4} + j{Imag:F4}";
        else
            return $"{Real:F4} - j{Math.Abs(Imag):F4}";
    }
}

// ====================== 计算记录实体（存入Queue） ======================
public class CalcRecord
{
    public double Z0 { get; set; }
    public Complex ZL { get; set; }
    public double ThetaDeg { get; set; }
    public Complex Gamma { get; set; }
    public Complex Zin { get; set; }
    public DateTime Time { get; set; }

    public string GetLogText()
    {
        return $"[{Time:HH:mm:ss}] Z0={Z0}Ω, ZL={ZL}, θ={ThetaDeg:F2}°\n" +
               $"Γ={Gamma}, |Γ|={Gamma.Magnitude():F4}, ∠Γ={Gamma.PhaseDeg():F2}°\n" +
               $"Zin={Zin}\n----------------------------------------\n";
    }
}

// ====================== 射频计算器类（功能封装） ======================
public class RfCalculator
{
    /// <summary>
    /// 计算反射系数 Γ = (ZL - Z0)/(ZL + Z0)
    /// </summary>
    public Complex CalcGamma(Complex ZL, double Z0)
    {
        Complex z0C = new Complex(Z0, 0);
        return (ZL - z0C) / (ZL + z0C);
    }

    /// <summary>
    /// 传输线阻抗变换
    /// </summary>
    /// <param name="ZL">负载阻抗</param>
    /// <param name="Z0">特性阻抗</param>
    /// <param name="thetaDeg">电长度 角度</param>
    public Complex CalcZin(Complex ZL, double Z0, double thetaDeg)
    {
        double thetaRad = thetaDeg * Math.PI / 180.0;
        double tanT = Math.Tan(thetaRad);
        Complex z0 = new Complex(Z0, 0);
        Complex jZ0Tan = new Complex(0, Z0 * tanT);

        Complex numerator = ZL + jZ0Tan;
        Complex denominator = z0 + new Complex(0, tanT) * ZL;
        return z0 * numerator / denominator;
    }
}

class Program
{
    // 队列：保存所有计算记录（学习Queue数据结构）
    private static Queue<CalcRecord> recordQueue = new Queue<CalcRecord>();
    private static RfCalculator calculator = new RfCalculator();
    private static readonly string logPath = Path.Combine(Environment.CurrentDirectory, "rf_calc_log.txt");

    static void Main(string[] args)
    {
        Console.WriteLine("==== C# 反射系数 & 传输线阻抗变换工具 ====");
        Console.WriteLine("指令：");
        Console.WriteLine("1 新建计算 | 2 查看队列记录 | 3 导出日志文件 | 0 退出\n");

        while (true)
        {
            Console.Write("请输入指令：");
            var cmd = Console.ReadLine();
            switch (cmd)
            {
                case "1":
                    RunSingleCalc();
                    break;
                case "2":
                    ShowAllRecords();
                    break;
                case "3":
                    ExportLogToFile();
                    break;
                case "0":
                    Console.WriteLine("程序退出");
                    return;
                default:
                    Console.WriteLine("无效指令");
                    break;
            }
        }
    }

    /// <summary>
    /// 单次计算交互
    /// </summary>
    static void RunSingleCalc()
    {
        try
        {
            Console.Write("输入特性阻抗Z0(Ω): ");
            double Z0 = double.Parse(Console.ReadLine());

            Console.Write("负载阻抗实部RL: ");
            double RL = double.Parse(Console.ReadLine());
            Console.Write("负载阻抗虚部XL: ");
            double XL = double.Parse(Console.ReadLine());
            Complex ZL = new Complex(RL, XL);

            Console.Write("传输线电长度 θ(°)，无传输线填0：");
            double theta = double.Parse(Console.ReadLine());

            // 核心计算
            Complex gamma = calculator.CalcGamma(ZL, Z0);
            Complex zin = calculator.CalcZin(ZL, Z0, theta);

            // 保存记录到队列
            var rec = new CalcRecord
            {
                Z0 = Z0,
                ZL = ZL,
                ThetaDeg = theta,
                Gamma = gamma,
                Zin = zin,
                Time = DateTime.Now
            };
            recordQueue.Enqueue(rec);

            Console.WriteLine("\n===== 计算结果 =====");
            Console.WriteLine(rec.GetLogText());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"输入错误：{ex.Message}\n");
        }
    }

    /// <summary>
    /// 遍历队列展示所有记录
    /// </summary>
    static void ShowAllRecords()
    {
        if (recordQueue.Count == 0)
        {
            Console.WriteLine("暂无计算记录\n");
            return;
        }
        Console.WriteLine($"一共有 {recordQueue.Count} 条记录：\n");
        foreach (var item in recordQueue)
        {
            Console.WriteLine(item.GetLogText());
        }
    }

    /// <summary>
    /// 文件操作：把队列全部记录写入txt
    /// </summary>
    static void ExportLogToFile()
    {
        if (recordQueue.Count == 0)
        {
            Console.WriteLine("没有记录可导出！");
            return;
        }
        using (StreamWriter sw = new StreamWriter(logPath, true)) // true=追加写入
        {
            foreach (var r in recordQueue)
            {
                sw.Write(r.GetLogText());
            }
        }
        Console.WriteLine($"导出成功！文件路径：{logPath}\n");
    }
}