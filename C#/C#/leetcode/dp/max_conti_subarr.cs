public class Solution {
    public int MaxSubArray(int[] nums) {
       int dp_max=nums[0];
       int current_max=nums[0];
       for(int i=1;i<nums.Length;i++){
        current_max=Math.Max(nums[i],current_max+nums[i]);
        if(dp_max<current_max){
            dp_max=current_max;
        }
       }
       return dp_max;
    
    }
}