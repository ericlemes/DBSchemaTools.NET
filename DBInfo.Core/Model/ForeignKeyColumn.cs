using System;

namespace DBInfo.Core.Model {
  public class ForeignKeyColumn {
    public Table RefTable;
    public Column Column;
    public Column RefColumn;

    public ForeignKeyColumn() {
    }
  }
}
