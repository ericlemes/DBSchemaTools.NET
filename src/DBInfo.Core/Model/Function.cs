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
    
    private string _DropFunctionScript;
    public string DropFunctionScript{
      get { return _DropFunctionScript;}
      set { _DropFunctionScript = value;}
    }
    
    private string _CreateFunctionScript;
    public string CreateFunctionScript{
      get { return _CreateFunctionScript;}
      set { _CreateFunctionScript = value;}
    }


  }
}
