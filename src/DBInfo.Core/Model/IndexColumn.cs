using System;

namespace DBInfo.Core.Model {
  public class IndexColumn {
    public enum EnumOrder {
      Ascending = 0,
      Descending = 1
    };

    public string Column{
      get; set;
    } 
    
    private EnumOrder _Order;
    public EnumOrder Order{
      get { return _Order;}
      set { _Order = value;}
    }

  }
}
