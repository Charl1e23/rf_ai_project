//给你一个按 非递减顺序 排序的整数数组 nums，、
// 返回 每个数字的平方 组成的新数组，要求也按 非递减顺序 排序。

public class Solution {
    public int[] SortedSquares(int[] nums) {
        int n = nums.Length;
        int[] res = new int[n];
        int left = 0;
        int right = n - 1;
        int k = n - 1; //从结果数组末尾向前放

        while(left <= right)
        {
            int lSquare = nums[left] * nums[left];
            int rSquare = nums[right] * nums[right];
            if(lSquare > rSquare)
            {
                res[k] = lSquare;
                left++;
            }
            else
            {
                res[k] = rSquare;
                right--;
            }
            k--;
        }
        return res;
    }
}