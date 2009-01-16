using System;

namespace DBInfo.Core.Model {
  public class IndexColumn {
    public enum EnumOrder {
      Ascending = 0,
      Descending = 1
    };

    private Column _Column;
    public Column Column{
      get { return _Column;}
      set { _Column = value;}
    }
    
    private EnumOrder _Order;
    public EnumOrder Order{
      get { return _Order;}
      set { _Order = value;}
    }

  }
}
