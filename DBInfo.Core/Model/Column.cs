using System;

namespace DBInfo.Core.Model {
  public class Column {
    public enum DBColumnType {
      DBInteger = 1,
      DBVarchar = 2,
      DBChar = 3,
      DBDecimal = 4,
      DBFloat = 5,
      DBMemo = 6,
      DBBlob = 7,
      DBDateTime = 8,
      DBBit = 9,
      DBSmallDateTime = 10,
      DBMoney = 11,
      DBSmallInt = 12,
      DBNumeric = 13,
      DBGUID = 14,
      DBBigInt = 15,
      DBTinyInt = 16,
      DBBinary = 17,
      DBNVarchar = 18,
      DBRowID = 19, //RowID Oracle, RecID Progress
      DBTimeStamp = 20
    }

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
