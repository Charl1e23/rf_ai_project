/*
给定字符串 s 和 t ，判断 s 是否为 t 的子序列。

字符串的一个子序列是原始字符串删除一些（也可以不删除）字符而不改变剩余字符相对位置形成的新字符串。（例如，"ace"是"abcde"的一个子序列，而"aec"不是）。
*/

public class Solution {
    public bool IsSubsequence(string s, string t) {
        int i=0,j=0;
        int len_s=s.Length;
        int len_t=t.Length;
        while(i<len_s && j<len_t){
            if(s[i]==t[j]){
                i++;
            }
            j++;
        }
        return i==len_s;
    }
}