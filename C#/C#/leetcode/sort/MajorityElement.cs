/*
给定一个大小为 n 的数组 nums ，返回其中的多数元素。多数元素是指在数组中出现次数 大于 ⌊ n/2 ⌋ 的元素。

你可以假设数组是非空的，并且给定的数组总是存在多数元素。
*/
//摩尔投票算法
public class Solution {
    public int MajorityElement(int[] nums) {
        int count = 0;
        int candidate = 0;
        foreach(int num in nums)
        {
            if(count == 0)
            {
                candidate = num;
            }
            if(num == candidate)
            {
                count++;
            }
            else
            {
                count--;
            }
        }
        return candidate;
    }
}

//哈希遍历
public class Solution {
    public int MajorityElement(int[] nums) {
        Dictionary<int,int> dict = new Dictionary<int,int>();
        int half = nums.Length / 2;
        foreach(var num in nums)
        {
            if(dict.ContainsKey(num))
                dict[num]++;
            else
                dict[num] = 1;

            if(dict[num] > half)
                return num;
        }
        return 0;
    }
}