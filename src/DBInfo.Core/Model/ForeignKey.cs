using System;
using System.Collections;
using System.Collections.Generic;

namespace DBInfo.Core.Model {
  public class ForeignKey {
    public string TableName{
      get; set;
    }    
  
    private string _ForeignKeyName;    
    public string ForeignKeyName{
      get { return _ForeignKeyName;}
      set { _ForeignKeyName = value;}
    }
    
    private string _RefTableName;
    public string RefTableName{
      get { return _RefTableName;}
      set {_RefTableName = value;}
    }
    
    private List<ForeignKeyColumn> _Columns = new List<ForeignKeyColumn>();
    public List<ForeignKeyColumn> Columns{
      get { return _Columns;}
      set { _Columns = value;}
    }

    private bool _DeleteCascade;    
    public bool DeleteCascade{
      get { return _DeleteCascade;}
      set { _DeleteCascade = value;}
    }
    
    private bool _UpdateCascade;
    public bool UpdateCascade{
      get { return _UpdateCascade;}
      set { _UpdateCascade = value;}
    }
    
    private string _Script;
    public string Script{
      get { return _Script;}
      set { _Script = value;}
    }

  }
}
