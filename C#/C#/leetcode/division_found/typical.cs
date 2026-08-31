/*
给定一个 n 个元素有序的（升序）整型数组 nums 和一个目标值 target  ，写一个函数搜索 nums 中的 target，如果 target 存在返回下标，否则返回 -1。

你必须编写一个具有 O(log n) 时间复杂度的算法。
*/

public class Solution {
    public int Search(int[] nums, int target) {
        int head = 0;
        int tail = nums.Length;
        while (head < tail)
        {
            int mid = (head + tail) / 2;
            if (nums[mid] > target)
            {
                tail = mid;
            }
            else if (nums[mid] < target)
            {
                head = mid + 1;
            }
            else
            {
                return mid;
            }
        }
        return -1;
    }
}