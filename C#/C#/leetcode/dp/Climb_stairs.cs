public class Solution {
    int max=0;
    Dictionary<int,int> max_order=new Dictionary<int,int>();
    public int ClimbStairs(int n) {
        max_order.Add(0,1);
        max_order.Add(1,1);
        
        return dfs(n);
    }

    public int dfs(int n){
        if(!max_order.ContainsKey(n))
            max_order.Add(n,dfs(n-1)+dfs(n-2));

        return max_order[n];
       
    }
}