//方法一：排列组合
public class Solution {
    static int getC_cul(int k,int M){
            int count=Math.Min(k,M-k);
            long result=1;
            for(int i=1;i<=count;i++){
                 result = result * (M - count + i) / i;
            }
            return (int)result;
        }
    public int UniquePaths(int m, int n) {
        int ways=0;
        ways=getC_cul(Math.Min(m-1,n-1),m+n-2);
        return ways;
    }
}
//方法二：动态规划
public class Solution {
    public int UniquePaths(int m, int n) {
        int[,] dp = new int[m,n];
        // 第一列全部1
        for(int i = 0; i < m; i++) dp[i,0] = 1;
        // 第一行全部1
        for(int j = 0; j < n; j++) dp[0,j] = 1;

        for(int i = 1; i < m; i++)
        {
            for(int j = 1; j < n; j++)
            {
                dp[i,j] = dp[i-1,j] + dp[i,j-1];
            }
        }
        return dp[m-1,n-1];
    }
}