/*
给定长度为 2n 的整数数组 nums ，你的任务是将这些数分成 n 对, 使得从每对中取最大值时，最大值之和最小。
*/

public class Solution {
    public int ArrayPairSum(int[] nums) {
        QuickSort(nums,0,nums.Length-1);
        int sum = 0;
        for(int i=0;i<nums.Length;i+=2)
        {
            sum += nums[i];
        }
        return sum;
    }

    //手写快速排序
    private void QuickSort(int[] nums,int left,int right)
    {
        if(left >= right) return;
        int pivotIdx = Partition(nums,left,right);
        QuickSort(nums,left,pivotIdx-1);
        QuickSort(nums,pivotIdx+1,right);
    }

    private int Partition(int[] nums,int left,int right)
    {
        int pivot = nums[right];
        int i = left;
        for(int j=left;j<right;j++)
        {
            if(nums[j] < pivot)
            {
                Swap(nums,i,j);
                i++;
            }
        }
        Swap(nums,i,right);
        return i;
    }

    private void Swap(int[] nums,int a,int b)
    {
        int temp = nums[a];
        nums[a] = nums[b];
        nums[b] = temp;
    }
}