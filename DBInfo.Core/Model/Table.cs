using System;
using System.Collections;
using System.Collections.Generic;

namespace DBInfo.Core.Model {
  public class Table {
    private string _TableName;
    public string TableName {
      get { return _TableName; }
      set { _TableName = value; }
    }

    private List<Column> _Columns = new List<Column>();
    public List<Column> Columns {
      get { return _Columns; }
      set { _Columns = value; }
    }

    private List<ForeignKey> _ForeignKeys = new List<ForeignKey>();
    public List<ForeignKey> ForeignKeys {
      get { return _ForeignKeys; }
      set { _ForeignKeys = value; }
    }

    private string _PrimaryKeyName;
    public string PrimaryKeyName {
      get { return _PrimaryKeyName; }
      set { _PrimaryKeyName = value; }
    }

    private List<Column> _PrimaryKeyColumns = new List<Column>();
    public List<Column> PrimaryKeyColumns {
      get { return _PrimaryKeyColumns; }
      set { _PrimaryKeyColumns = value; }
    }

    private List<Index> _Indexes = new List<Index>();
    public List<Index> Indexes {
      get { return _Indexes; }
      set { _Indexes = value; }
    }

    private bool _HasIdentity;
    public bool HasIdentity {
      get { return _HasIdentity; }
      set { _HasIdentity = value; }
    }

    private int _IdentitySeed;
    public int IdentitySeed {
      get { return _IdentitySeed; }
      set { _IdentitySeed = value; }
    }

    private int _IdentityIncrement;
    public int IdentityIncrement {
      get { return _IdentityIncrement; }
      set { _IdentityIncrement = value; }
    }

    private List<CheckConstraint> _CheckConstraints = new List<CheckConstraint>();
    public List<CheckConstraint> CheckConstraints {
      get { return _CheckConstraints; }
      set { _CheckConstraints = value; }
    }    

    public Column FindColumn(string AColumnName) {
      Column TmpColumn = null;
      foreach (Column c in Columns) {
        if (c.Name == AColumnName) {
          TmpColumn = c;
          break;
        }
      }
      return TmpColumn;
    }

    public ForeignKeyColumn ColumnHasForeignKey(Column AColumn) {
      foreach (ForeignKey fk in ForeignKeys) {
        foreach (ForeignKeyColumn fkcol in fk.Columns) {
          if (fkcol.Column == AColumn) {
            return fkcol;
          }
        }
      }
      return null;
    }

  }
}
