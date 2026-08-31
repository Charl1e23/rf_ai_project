/*
给你一个整数数组 nums 。如果任一值在数组中出现 至少两次 ，返回 true ；如果数组中每个元素互不相同，返回 false
*/

public class Solution {
    public bool ContainsDuplicate(int[] nums) {
        Dictionary <int,int> dict=new  Dictionary <int,int>();
        foreach(int num in nums){
            if(dict.ContainsKey(num)){
                return true;
            };
            dict[num]=1;
            
        }
        return false;
    }
}