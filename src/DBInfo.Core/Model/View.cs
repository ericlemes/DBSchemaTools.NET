using System;

namespace DBInfo.Core.Model {
  public class View {
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

    private string _DropViewScript;
    public string DropViewScript{
      get { return _DropViewScript;}
      set { _DropViewScript = value;}
    }
    
    private string _CreateViewScript;
    public string CreateViewScript{
      get { return _CreateViewScript;}
      set { _CreateViewScript = value;}
    }

  }
}
