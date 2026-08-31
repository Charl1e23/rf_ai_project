public class Solution {
    public int MaxProfit(int[] prices) {
        int min_buyin=prices[0],Max_profit=prices[0]-min_buyin;
        for(int i=0;i<prices.Length;i++){
            if(prices[i]<min_buyin){
                min_buyin=prices[i];
            }
            int profit=prices[i]-min_buyin;
            if(profit>Max_profit)
            {
                Max_profit=profit;
            }
        }
    return Max_profit;
    }
}