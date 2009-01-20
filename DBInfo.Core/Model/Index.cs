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

    private List<IndexColumn> _Columns = new List<IndexColumn>();
    public List<IndexColumn> Columns {
      get { return _Columns;}
      set {_Columns = value;}
    }

  }
}
