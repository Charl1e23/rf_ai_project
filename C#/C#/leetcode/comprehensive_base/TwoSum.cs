/*
给定一个整数数组 nums 和一个目标值 target，找出数组中和为目标值的两个数。
*/

public class Solution {
    public int[] TwoSum(int[] nums, int target) {
        Dictionary<int,int> dict = new Dictionary<int,int>();
        for(int i=0;i<nums.Length;i++)
        {
            int need = target - nums[i];
            if(dict.ContainsKey(need))
            {
                return new int[]{dict[need], i};
            }
            dict[nums[i]] = i;
        }
        return new int[]{};
    }
}