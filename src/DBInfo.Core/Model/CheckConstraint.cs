using System;

namespace DBInfo.Core.Model {
  public class CheckConstraint {
  
    public string TableName{
      get; set;
    }
    
    public string CheckConstraintName{
      get; set;      
    }
    
    public string Expression{
      get; set;
    }    
    
    public string Script{
      get; set;
    }
    
  }
}
