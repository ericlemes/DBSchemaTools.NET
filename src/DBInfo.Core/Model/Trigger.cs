using System;
using System.Collections.Generic;

namespace DBInfo.Core.Model {
  public class Trigger {
  
    private Table _Table;    
    public Table Table{
      get { return _Table;}
      set { _Table = value;}
    }
    
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
    
    private string _DropTriggerScript;
    public string DropTriggerScript{
      get { return _DropTriggerScript;}
      set { _DropTriggerScript = value;}
    }
    
    private string _CreateTriggerScript;
    public string CreateTriggerScript{
      get { return _CreateTriggerScript;}
      set { _CreateTriggerScript = value;}
    }

  }
}
