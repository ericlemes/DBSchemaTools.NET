using System;
using System.Collections.Generic;

namespace DBInfo.Core.Model {
  public class Trigger {
    public string TableName{
      get; set;
    }    
        
    public string TriggerName{
      get; set;      
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
