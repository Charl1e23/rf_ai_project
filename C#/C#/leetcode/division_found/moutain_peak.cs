/*
给定一个长度为 n 的整数 山脉 数组 arr ，其中的值递增到一个 峰值元素 然后递减。

返回峰值元素的下标。

你必须设计并实现时间复杂度为 O(log(n)) 的解决方案。
*/

public class Solution {
    public int PeakIndexInMountainArray(int[] arr) {
        int head=0;
        int tail=arr.Length-1;
        while(head<tail){
            int index=head+(tail-head)/2;
            if(arr[index]>arr[index+1]){
                    tail=index;
            }
            else if(arr[index]<arr[index+1]){
                head=index+1;
            }
        }
        return head;

    }
}