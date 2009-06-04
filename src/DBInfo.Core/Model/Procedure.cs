using System;
using System.Collections.Generic;

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
    
    private List<Parameter> _InputParameters = new List<Parameter>();
    public List<Parameter> InputParameters{
      get { return _InputParameters;}
      set { _InputParameters = value;}
    }
    
    private List<RecordSet> _RecordSets = new List<RecordSet>();
    public List<RecordSet> RecordSets {
      get { return _RecordSets;}
      set { _RecordSets = value;}
    }
  }
  
  public class RecordSet{
    public List<Parameter> _Parameters = new List<Parameter>();
    public List<Parameter> Parameters{
      get { return _Parameters; }
      set { _Parameters = value; }
    }
  }
}
