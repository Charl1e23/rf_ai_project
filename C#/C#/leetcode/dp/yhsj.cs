public class Solution {
    public IList<IList<int>> Generate(int numRows) {
       IList<IList<int>> result=new List<IList<int>>();
       IList<int> row1=new List<int>{1};
       
       if(numRows==0) return result;
       if(numRows==1) {
        result.Add(row1);
        return result;
       }
       
       
       
       IList<int> row2=new List<int>{1,1};
       result.Add(row1);
       result.Add(row2);
      IList<int> pre_row=row2;
      for(int index=2;index<numRows;index++){
            IList<int> new_row=new List<int>();
            
            new_row.Add(1);
            for(int i=1;i<index;i++){
                new_row.Add(pre_row[i-1]+pre_row[i]);
            }       
            new_row.Add(1);
            result.Add(new_row);
            pre_row=new_row;
      }
      return result;
    }
}