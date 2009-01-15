using System;
using System.Collections;

namespace DBInfo.Core.Model {
  public class Index {
    public string IndexName;
    public bool Unique;
    public string Area; //Progress;
    public bool Primary; //Progress;

    public ArrayList Columns;

    public Index() {
      Columns = new ArrayList();
    }
  }
}
