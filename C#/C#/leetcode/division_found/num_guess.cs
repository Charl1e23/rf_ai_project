/*
我们正在玩猜数字游戏。猜数字游戏的规则如下：

我会从 1 到 n 随机选择一个数字。 请你猜选出的是哪个数字。（我选的数字在整个游戏中保持不变）。

如果你猜错了，我会告诉你，我选出的数字比你猜测的数字大了还是小了。

你可以通过调用一个预先定义好的接口 int guess(int num) 来获取猜测结果，返回值一共有三种可能的情况：

-1：你猜的数字比我选出的数字大 （即 num > pick）。
1：你猜的数字比我选出的数字小 （即 num < pick）。
0：你猜的数字与我选出的数字相等。（即 num == pick）。
返回我选出的数字。*/

/** 
 * Forward declaration of guess API.
 * @param  num   your guess
 * @return 	     -1 if num is higher than the picked number
 *			      1 if num is lower than the picked number
 *               otherwise return 0
 * int guess(int num);
 */

public class Solution : GuessGame {
    public int GuessNumber(int n) {
        int head=1;
        int tail=n;
        while(head<tail){
            int index=head+(tail-head)/2;
            if(guess(index)<0){
                tail=index;
            }
            else if(guess(index)>0){
                head=index+1;
            }
            else{
                return index;
            }
        }
        return head;
    }
}