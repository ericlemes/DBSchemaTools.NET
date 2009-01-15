using System;
using System.Collections;

namespace DBInfo.Core.Model {
  public class ForeignKey {
    public string ForeignKeyName;
    public string RefTableName;
    
    public ArrayList Columns;

    public bool DeleteCascade;
    public bool UpdateCascade;

    public ForeignKey() {
      Columns = new ArrayList();
      DeleteCascade = false;
      UpdateCascade = false;
    }
  }
}
