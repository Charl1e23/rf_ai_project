public class Solution {
    public IList<int> GetRow(int rowIndex) {
     
        if (rowIndex == 0)
        {
            return new List<int> { 1 };
        }
   
        IList<int> prev = new List<int> { 1, 1 };
        if (rowIndex == 1)
        {
            return prev;
        }

      
        for (int index = 2; index <= rowIndex; index++)
        {
            IList<int> curr = new List<int>();
            curr.Add(1);
           
            for (int j = 1; j <= index - 1; j++)
            {
                curr.Add(prev[j - 1] + prev[j]);
            }
            curr.Add(1);
            prev = curr;
        }
        return prev;
    }
}
