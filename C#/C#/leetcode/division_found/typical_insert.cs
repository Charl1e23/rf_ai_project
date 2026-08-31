/*
给定一个排序数组和一个目标值，在数组中找到目标值，并返回其索引。如果目标值不存在于数组中，返回它将会被按顺序插入的位置。

请必须使用时间复杂度为 O(log n) 的算法。
*/

public class Solution {
    public int SearchInsert(int[] nums, int target) {
        int left = 0;
        int right = nums.Length;

        while(left < right)
        {
            int mid = left + (right - left) / 2; // 防溢出
            if(nums[mid] >= target)
            {
                right = mid;
            }
            else
            {
                left = mid + 1;
            }
        }
        return left;
    }
}