using System;
using System.Collections;
using System.Collections.Generic;

namespace DBInfo.Core.Model {
  public class Index {
    private string _IndexName;
    public string IndexName{
      get { return _IndexName;}
      set { _IndexName = value;}
    }
    
    private bool _Unique;
    public bool Unique{
      get { return _Unique;}
      set { _Unique = value;}
    }
    
    #region Progress-Specific
    
    private string _Area;
    public string Area{
      get { return _Area;}
      set { _Area = value;}
    }
    
    private bool _Primary;
    public bool Primary{
      get { return _Primary;}
      set { _Primary = value;}
    }
    
    #endregion

    private List<IndexColumn> _Columns = new List<IndexColumn>();
    public List<IndexColumn> Columns {
      get { return _Columns;}
      set {_Columns = value;}
    }

  }
}
