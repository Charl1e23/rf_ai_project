
/*
给定两个字符串 s 和 t ，编写一个函数来判断 t 是否是 s 的 字母异位词。
*/

public class Solution {
    public bool IsAnagram(string s, string t) {
        if(s.Length != t.Length) return false;
        int[] count = new int[26];
        foreach(var c in s) count[c-'a']++;
        foreach(var c in t) count[c-'a']--;
        foreach(var num in count)
        {
            if(num != 0) return false;
        }
        return true;
    }
}