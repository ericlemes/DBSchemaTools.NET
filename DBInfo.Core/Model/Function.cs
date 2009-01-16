using System;

namespace DBInfo.Core.Model {
  public class Function {
  
    private string _Name;    
    public string Name{
      get { return _Name;}
      set { _Name = value;}
    }
    
    private string _Body;
    public string Body{
      get { return _Body;}
      set { _Body = value;}
    }


  }
}
