using System;

namespace DBInfo.Core.Model {
  public enum DBColumnType {
    Integer = 1,
    VarChar = 2,
    Char = 3,
    Decimal = 4,
    Float = 5,
    Text = 6,
    Image = 7,        
    DateTime = 8,
    Bit = 9,
    SmallDateTime = 10,
    Money = 11,
    SmallInt = 12,
    Numeric = 13,
    UniqueIdentifier = 14,    
    BigInt = 15,
    TinyInt = 16,    
    Binary = 17,
    NChar = 18,
    NText = 19,
    NVarchar = 20, 
    Real = 21,                                     
    SmallMoney = 22,        
    VarBinary = 23,
    Xml = 24,
    TimeStamp = 25
  }

  public class Column {

    private Table _Table;
    public Table Table{
      get { return _Table;}
      set { _Table = value;}
    }
    
    private string _Name;
    public string Name{
      get { return _Name;}
      set {_Name = value;}
    }
    
    private DBColumnType _Type;    
    public DBColumnType Type{
      get { return _Type;}
      set {_Type = value;}
    }
    
    private int _Size;
    public int Size{
      get { return _Size;}
      set {_Size = value;}
    }
    
    private int _Precision;    
    public int Precision{
      get { return _Precision;}
      set { _Precision = value;}
    }
    
    private int _Scale;    
    public int Scale{
      get { return _Scale;}
      set { _Scale = value;}      
    }
    
    private bool _IsPk;
    public bool IsPK{
      get { return _IsPk;}
      set { _IsPk = value;}
    }
    
    private bool _IsNull;        
    public bool IsNull{
      get { return _IsNull;}
      set { _IsNull = value;}
    }
    
    private bool _IdentityColumn;
    public bool IdentityColumn{
      get { return _IdentityColumn;}
      set { _IdentityColumn = value; }
    }
    
    private string _DefaultValue;
    public string DefaultValue{
      get { return _DefaultValue;}
      set { _DefaultValue = value;}
    }
    
    private string _ConstraintDefaultName;
    public string ConstraintDefaultName{
      get { return _ConstraintDefaultName;}
      set { _ConstraintDefaultName = value;}
    }    
    
  }
}
