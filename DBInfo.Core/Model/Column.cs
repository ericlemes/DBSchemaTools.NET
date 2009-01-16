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
      DBRowID = 19 //RowID Oracle, RecID Progress
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
    }
    
    private bool _IsNull;        
    public bool IsNull{
      get { return _IsNull;}
      set { _IsNull = value;}
    }
    
    private bool _IdentityColumn;
    public bool IdentityColumn{
      get { return _IdentityColumn;}
      set { _IsNull = value;}
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
    
    #region Progress-Specific

    private string _Description;    
    public string Description{
      get {return _Description;}
      set { _Description = value;}
    }
    
    private string _Format;
    public string Format{
      get { return _Format;}
      set { _Format = value;}
    }
    
    private string _Label;    
    public string Label{
      get { return _Label;}
      set { _Label = value;}
    }
    
    private int _Position;
    public int Position {
      get { return _Position;}
      set { _Position = value;}
    }
    
    private int _SqlWidth;
    public int SqlWidth{
      get { return _SqlWidth;}
      set { _SqlWidth = value;}
    }
    
    private string _Help;
    public string Help{
      get { return _Help;}
      set { _Help = value;}
    }
    
    private int _Order;
    public int Order{
      get { return _Order;}
      set { _Order = value;}
    }
    
    private string _ValExp;
    public string ValExp{
      get { return _ValExp;}
      set { _ValExp = value;}
    }
    
    private string _ValMsg;
    public string ValMsg{
      get { return _ValMsg;}
      set { _ValMsg = value;}
    }
    
    private int _Decimals;
    public int Decimals{
      get { return _Decimals;}
      set { _Decimals= value;}
    }
    
    #endregion
    
  }
}
