/*爱丽丝和鲍勃一起玩游戏，他们轮流行动。爱丽丝先手开局。

最初，黑板上有一个数字 n 。在每个玩家的回合，玩家需要执行以下操作：

选出任一整数 x，满足 0 < x < n 且 n % x == 0 。
用 n - x 替换黑板上的数字 n 。
如果玩家无法执行这些操作，就会输掉游戏。

只有在爱丽丝在游戏中取得胜利时才返回 true 。假设两个玩家都以最佳状态参与游戏*/
public class Solution {
    public bool DivisorGame(int n) {
        bool[]  dp=new bool[n+1];
        dp[1]=false;
        for(int i=2;i<=n;i++){
            for(int x=1;x<i;x++){
                if(i%x==0 && !dp[i-x]){
                    dp[i]=true;
                    break;
                }
            }
        }
        return dp[n];
    }
}