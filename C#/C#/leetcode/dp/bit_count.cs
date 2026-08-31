public class Solution {
    static int Getmaxbit(int x)
    {
        int maxbit=0;
        while((1<<(maxbit+1))<=x){
            maxbit++;
        }
        return maxbit;
    }

    public int[] CountBits(int n) {
        int[] ans=new int[n+1];
        ans[0]=0;
        if(n>=1) ans[1]=1;
       
        for(int i=2;i<=n;i++){
            ans[i]=0;
            int temp=i;
            for(int bits=Getmaxbit(i);bits>=0;bits--){
                int pow2 =1<<bits;
                if(temp>=pow2){
                    
                    ans[i]++;
                    temp=temp-pow2;
                }
            }
        }
        return ans;
            
    }
}