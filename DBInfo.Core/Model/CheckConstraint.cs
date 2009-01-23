using System;

namespace DBInfo.Core.Model {
  public class CheckConstraint {
    private string _Name;
    public string Name{
      get { return _Name;}
      set { _Name = value;}
    }
    
    private string _Expression;
    public string Expression{
      get { return _Expression;}
      set { _Expression = value;}
    }
    
    private string _Script;
    public string Script{
      get { return _Script;}
      set { _Script = value;}
    }
    
  }
}
