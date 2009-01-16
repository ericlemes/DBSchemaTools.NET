using System;

namespace DBInfo.Core.Model {
  // This class is Progress-specific. Don't apply to any other database
  // Used to generate the "TABLE-TRIGGER" statement in .df
  public class TableTrigger {
    private string _Event;
    public string Event{
      get { return _Event;}
      set { _Event = value;}
    }
    
    private bool _Override;
    public bool Override{
      get { return _Override;}
      set { _Override = value;}
    }
    
    private string _Procedure;
    public string Procedure{
      get { return _Procedure;}
      set {_Procedure = value;}
    }
    
    private string _CRC;
    public string CRC{
      get { return _CRC;}
      set { _CRC = value;}
    }

  }
}
