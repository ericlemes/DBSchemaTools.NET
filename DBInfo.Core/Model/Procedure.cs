using System;

namespace DBInfo.Core.Model {
  public class Procedure {
    private string _Name;    
    public string Name{
      get { return _Name;}
      set { _Name = value;}
    }
    
    private string _Body;
    public string Body{
      get { return _Body;}
      set {_Body = value;}
    }

    private string _CreateProcedureScript;
    public string CreateProcedureScript{
      get { return _CreateProcedureScript; }
      set { _CreateProcedureScript = value; }
    }
    
    private string _DropProcedureScript;
    public string DropProcedureScript {
      get { return _DropProcedureScript; }
      set { _DropProcedureScript = value; }
    }

  }
}
