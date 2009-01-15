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

    public Table Table;
    public string Name;
    public DBColumnType Type;
    public int Size;
    public int Precision;
    public int Scale;
    public bool IsPK;
    public bool IsNull;
    public bool IdentityColumn;
    public string DefaultValue;
    public string ConstraintDefaultName;

    public string Description; //Progress
    public string Format; //Progress
    public string Label; //Progress
    public int Position; //Progress
    public int SqlWidth; //Progress
    public string Help; //Progress
    public int Order; //Progress
    public string ValExp; //Progress
    public string ValMsg; //Progress
    public int Decimals; //Progress

    public Column() {
      IdentityColumn = false;
    }
  }
}
