using System;
using System.Collections;
using System.Collections.Generic;

namespace DBInfo.Core.Model {
  public class Index {
    public string IndexName;
    public bool Unique;
    public string Area; //Progress;
    public bool Primary; //Progress;

    private List<IndexColumn> _Columns = new List<IndexColumn>();
    public List<IndexColumn> Columns {
      get { return _Columns;}
      set {_Columns = value;}
    }

  }
}
