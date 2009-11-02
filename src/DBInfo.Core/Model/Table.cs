using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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

    private List<string> _PrimaryKeyColumns = new List<string>();
    public List<string> PrimaryKeyColumns {
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

    private List<Trigger> _Triggers = new List<Trigger>();
    public List<Trigger> Triggers {
      get { return _Triggers; }
      set { _Triggers = value; }
    }
    
    private string _TableScript;
    public string TableScript{
      get { return _TableScript;}
      set {_TableScript = value;}
    }
    
    private string _PrimaryKeyScript;
    public string PrimaryKeyScript{
      get { return _PrimaryKeyScript;}
      set { _PrimaryKeyScript = value;}
    }
    
    private string _TableDataScript;
    public string TableDataScript{
      get { return _TableDataScript;}
      set { _TableDataScript = value;}
    }
    
    private DataTable _TableData;
    public DataTable TableData{
      get { return _TableData;}
      set { _TableData = value;}
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
          if (fkcol.Column == AColumn.Name) {
            return fkcol;
          }
        }
      }
      return null;
    }
    
    public string GetIdentityColumn(){
      foreach(Column c in this.Columns){
        if (c.IdentityColumn)
          return c.Name;
      }
      return "";
    }
    
    public Index FindIndex(string IndexName){      
      return 
        (from Index i in this.Indexes
         where i.IndexName.ToLower() == IndexName.ToLower()
         select i).FirstOrDefault<Index>();
    }
    
    public Trigger FindTrigger(string TriggerName){
      return 
        (from Trigger tr in this.Triggers
         where tr.TriggerName.ToLower() == TriggerName.ToLower()
         select tr).FirstOrDefault<Trigger>();
    }
    
    public ForeignKey FindForeignKey(string ForeignKeyName){
      return
        (from ForeignKey fk in this.ForeignKeys
         where fk.ForeignKeyName.ToLower() == ForeignKeyName.ToLower()
         select fk).FirstOrDefault<ForeignKey>();
    }
    
    public CheckConstraint FindCheckConstraint(string CheckConstraintName){
      return 
        (from CheckConstraint cc in this.CheckConstraints
         where cc.CheckConstraintName.ToLower() == CheckConstraintName.ToLower()
         select cc
         ).FirstOrDefault<CheckConstraint>();
    }

  }
}
