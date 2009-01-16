using System;

namespace DBInfo.Core.Model {
  public class ForeignKeyColumn {
  
    private Table _RefTable;
    public Table RefTable{
      get { return _RefTable;}
      set { _RefTable = value;}
    }
    
    private Column _Column;
    public Column Column{
      get { return _Column;}
      set { _Column = value;}
    }
    
    private Column _RefColumn;
    public Column RefColumn{
      get { return _RefColumn;}
      set { _RefColumn = value;}
    }    
  }
}
