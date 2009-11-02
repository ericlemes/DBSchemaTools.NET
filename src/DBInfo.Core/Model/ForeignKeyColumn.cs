using System;

namespace DBInfo.Core.Model {
  public class ForeignKeyColumn {
  
    public string RefTable{
      get; set;
    }    
    
    public string Column{
      get; set;
    } 
    
    public string RefColumn{
      get; set;
    }    
  }
}
