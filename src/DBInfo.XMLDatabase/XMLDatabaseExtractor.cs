using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Extractor;
using System.IO;
using System.Xml.Serialization;

namespace DBInfo.XMLDatabase {
  public class XMLDatabaseExtractor : IExtractor  {   
    public ExtractorType Type {
      get { return ExtractorType.Generic;}
    }
    
    private string _InputDir;
    public string InputDir{
      get { return _InputDir;}
      set { _InputDir = value;}
    }
    
    private string _InputConnectionString;
    public string InputConnectionString{
      get { return _InputConnectionString;}
      set { _InputConnectionString = value;}
    }    
    
    private IDatabaseExtractor _Extractor;
    public IDatabaseExtractor Extractor{
      get { return _Extractor;}
      set { _Extractor = value;}
    }
    
    private List<string> _InputFiles = new List<string>();
    public List<string> InputFiles{
      get { return _InputFiles;}
    }
    
    public DBInfo.Core.Model.Database ExtractXMLDatabase(List<string> XMLFiles){
      DBInfo.Core.Model.Database db = new DBInfo.Core.Model.Database();
      
      foreach(string xmlFile in XMLFiles){
        ParseXMLFile(xmlFile, db, 1);
      }
      foreach (string xmlFile in XMLFiles) {
        ParseXMLFile(xmlFile, db, 2);
      }
      
      return db;
    }
    
    private void ParseXMLFile(string xmlFile, DBInfo.Core.Model.Database db, int Step){
      FileStream fs = new FileStream(xmlFile, FileMode.Open, FileAccess.Read);
      XmlSerializer xs = new XmlSerializer(typeof(StatementCollection));
      StatementCollection sc = (StatementCollection)xs.Deserialize(fs);
      fs.Close();
      
      if (sc.Statement == null)
        return;
      
      foreach(Statement s in sc.Statement){
        if (Step == 1){
          if (s is CreateTable)
            db.Tables.Add(ParseCreateTableStatement((CreateTable)s));
        }
        else {
          if (s is CreateForeignKey)
            ParseCreateForeignKeyStatement((CreateForeignKey)s, db);
          else if (s is CreateIndex)
            ParseCreateIndexStatement((CreateIndex)s, db);
          else if (s is CreateCheckConstraint)
            ParseCreateCheckConstraintStatement((CreateCheckConstraint)s, db);
          else if (s is CreateTrigger)
            ParseCreateTriggerStatement((CreateTrigger)s, db);
          else if (s is CreateProcedure)
            db.Procedures.Add(ParseCreateProcedureStatement((CreateProcedure)s));
          else if (s is CreatePrimaryKey)
            ParseCreatePrimaryKeyStatement((CreatePrimaryKey)s, db);
          else if (s is CreateFunction)
            db.Functions.Add(ParseCreateFunctionStatement((CreateFunction)s));
          else if (s is CreateSequence)
            db.Sequences.Add(ParseCreateSequenceStatement((CreateSequence)s));
          else if (s is CreateView)
            db.Views.Add(ParseCreateViewStatement((CreateView)s));
        }
      }
    }
    
    private DBInfo.Core.Model.Column.DBColumnType GetDBColumnType(ColumnType ct){
      if (ct == ColumnType.BigInt)
        return DBInfo.Core.Model.Column.DBColumnType.DBBigInt;
      else if (ct == ColumnType.Binary)
        return DBInfo.Core.Model.Column.DBColumnType.DBBinary;
      else if (ct == ColumnType.Bit)
        return DBInfo.Core.Model.Column.DBColumnType.DBBit;
      else if (ct == ColumnType.Blob)
        return DBInfo.Core.Model.Column.DBColumnType.DBBlob;
      else if (ct == ColumnType.Char)
        return DBInfo.Core.Model.Column.DBColumnType.DBChar;
      else if (ct == ColumnType.DateTime)
        return DBInfo.Core.Model.Column.DBColumnType.DBDateTime;
      else if (ct == ColumnType.Decimal)
        return DBInfo.Core.Model.Column.DBColumnType.DBDecimal;
      else if (ct == ColumnType.Float)
        return DBInfo.Core.Model.Column.DBColumnType.DBFloat;
      else if (ct == ColumnType.GUID)
        return DBInfo.Core.Model.Column.DBColumnType.DBGUID;
      else if (ct == ColumnType.Integer)
        return DBInfo.Core.Model.Column.DBColumnType.DBInteger;
      else if (ct == ColumnType.Memo)
        return DBInfo.Core.Model.Column.DBColumnType.DBMemo;
      else if (ct == ColumnType.Money)
        return DBInfo.Core.Model.Column.DBColumnType.DBMoney;
      else if (ct == ColumnType.Numeric)
        return DBInfo.Core.Model.Column.DBColumnType.DBNumeric;
      else if (ct == ColumnType.NVarchar)
        return DBInfo.Core.Model.Column.DBColumnType.DBNVarchar;
      else if (ct == ColumnType.RowID)
        return DBInfo.Core.Model.Column.DBColumnType.DBRowID;
      else if (ct == ColumnType.SmallDateTime)
        return DBInfo.Core.Model.Column.DBColumnType.DBSmallDateTime;
      else if (ct == ColumnType.SmallInt)
        return DBInfo.Core.Model.Column.DBColumnType.DBSmallInt;
      else if (ct == ColumnType.TimeStamp)
        return DBInfo.Core.Model.Column.DBColumnType.DBTimeStamp;
      else if (ct == ColumnType.TinyInt)
        return DBInfo.Core.Model.Column.DBColumnType.DBTinyInt;
      else if (ct == ColumnType.Varchar)
        return DBInfo.Core.Model.Column.DBColumnType.DBVarchar;
      else
        throw new Exception(String.Format("Type not supported: {0}", ct.ToString()));      
    }
    
    private DBInfo.Core.Model.Table ParseCreateTableStatement(CreateTable ct){
      DBInfo.Core.Model.Table t = new DBInfo.Core.Model.Table();
      t.TableName = ct.TableName;
      t.HasIdentity = ct.HasIdentity;
      t.IdentitySeed = Convert.ToInt32(ct.IdentitySeed);
      t.IdentityIncrement = Convert.ToInt32(ct.IdentityIncrement);
      if (ct.Columns != null){
        foreach(Column ctc in ct.Columns){
          DBInfo.Core.Model.Column c = new DBInfo.Core.Model.Column();
          c.Name = ctc.Name;
          c.Type = GetDBColumnType(ctc.Type);
          c.Size = Convert.ToInt32(ctc.Size);
          c.Precision = Convert.ToInt32(ctc.Precision);
          c.Scale = Convert.ToInt32(ctc.Scale);
          c.IsNull = ctc.Nullable;
          c.IdentityColumn = ctc.Identity;
          c.DefaultValue = ctc.DefaultValue;
          c.ConstraintDefaultName = ctc.ConstraintDefaultName;
          t.Columns.Add(c);
        }
      }      
      return t;
    }
    
    private void ParseCreateForeignKeyStatement(CreateForeignKey xmlFK, DBInfo.Core.Model.Database db){
      DBInfo.Core.Model.Table t = db.FindTable(xmlFK.TableName, true);
      if (t == null)
        throw new Exception(String.Format("Table {0} not found when creating foreign key {1}", xmlFK.TableName, xmlFK.ForeignKeyName));
      
      DBInfo.Core.Model.ForeignKey fk = new DBInfo.Core.Model.ForeignKey();
      fk.ForeignKeyName = xmlFK.ForeignKeyName;
      fk.RefTableName = xmlFK.RefTableName;
      fk.DeleteCascade = xmlFK.DeleteCascade;
      fk.UpdateCascade = xmlFK.UpdateCascade;
      
      if (xmlFK.Columns != null){
        foreach(ForeignKeyColumn xmlFKCol in xmlFK.Columns){
          DBInfo.Core.Model.ForeignKeyColumn fkCol = new DBInfo.Core.Model.ForeignKeyColumn();
          fkCol.Column = t.FindColumn(xmlFKCol.ColumnName);
          if (fkCol.Column == null)
            throw new Exception(String.Format("Couldn't find column {0} on table {1} when creating foreign key {2}", xmlFKCol.ColumnName, t.TableName, xmlFK.ForeignKeyName));
          fkCol.RefTable = db.FindTable(xmlFK.RefTableName, true);
          if (fkCol.RefTable == null)
            throw new Exception(String.Format("Couldn't find table {0} when creating foreign key {1}", xmlFK.RefTableName, xmlFK.ForeignKeyName));
          fkCol.RefColumn = fkCol.RefTable.FindColumn(xmlFKCol.RefColumnName);
          if (fkCol.RefColumn == null)
            throw new Exception(String.Format("Couldn't find column {0} on table {1} when creating foreign key {2}", xmlFKCol.RefColumnName, xmlFK.RefTableName, xmlFK.ForeignKeyName));          
          fk.Columns.Add(fkCol);
        }
      }
      
      t.ForeignKeys.Add(fk);
    }
    
    private void ParseCreateIndexStatement(CreateIndex xmlIndex, DBInfo.Core.Model.Database db){
      DBInfo.Core.Model.Table t = db.FindTable(xmlIndex.TableName, true);
      if (t == null)
        throw new Exception(String.Format("Table {0} not found when creating index {1}", xmlIndex.TableName, xmlIndex.IndexName));
      
      DBInfo.Core.Model.Index i = new DBInfo.Core.Model.Index();
      i.IndexName = xmlIndex.IndexName;
      i.Unique = xmlIndex.Unique;
      i.IsClustered = xmlIndex.Clustered;
      
      if (xmlIndex.Columns != null){
        foreach(IndexColumn xmlIndexCol in xmlIndex.Columns){
          DBInfo.Core.Model.IndexColumn ic = new DBInfo.Core.Model.IndexColumn();
          ic.Column = t.FindColumn(xmlIndexCol.Name);
          if (ic.Column == null)
            throw new Exception(String.Format("Couldn't find column {0} on table {1} when creating index {2}", xmlIndexCol.Name, t.TableName, xmlIndex.IndexName));
          ic.Order = xmlIndexCol.Order == SortOrder.Ascending ? DBInfo.Core.Model.IndexColumn.EnumOrder.Ascending : DBInfo.Core.Model.IndexColumn.EnumOrder.Descending;
          i.Columns.Add(ic);
        }
      }
      
      t.Indexes.Add(i);
    }
    
    private void ParseCreateCheckConstraintStatement(CreateCheckConstraint xmlCheck, DBInfo.Core.Model.Database db){
      DBInfo.Core.Model.Table t = db.FindTable(xmlCheck.TableName, true);
      if (t == null)
        throw new Exception(String.Format("Couldn't find table {0} when creating check constraint {1}", xmlCheck.TableName, xmlCheck.CheckConstraintName));
      
      DBInfo.Core.Model.CheckConstraint ck = new DBInfo.Core.Model.CheckConstraint();
      ck.Name = xmlCheck.CheckConstraintName;
      ck.Expression = xmlCheck.SourceCode;
      
      t.CheckConstraints.Add(ck);
    }
    
    private void ParseCreateTriggerStatement(CreateTrigger xmlTrigger, DBInfo.Core.Model.Database db){
      DBInfo.Core.Model.Table t = db.FindTable(xmlTrigger.TableName, true);
      if (t == null)
        throw new Exception(String.Format("Couldn't find table {0} when creating trigger {1}", xmlTrigger.TableName, xmlTrigger.TriggerName));
      
      DBInfo.Core.Model.Trigger tr = new DBInfo.Core.Model.Trigger();
      tr.Body = xmlTrigger.SourceCode;
      tr.Name = xmlTrigger.TriggerName;
      tr.Table = t;
      t.Triggers.Add(tr);
    }
    
    private DBInfo.Core.Model.Procedure ParseCreateProcedureStatement(CreateProcedure xmlProcedure){
      DBInfo.Core.Model.Procedure p = new DBInfo.Core.Model.Procedure();
      p.Body = xmlProcedure.SourceCode;
      p.Name = xmlProcedure.Name;
      return p;
    }
    
    private void ParseCreatePrimaryKeyStatement(CreatePrimaryKey xmlPrimaryKey, DBInfo.Core.Model.Database db){
      DBInfo.Core.Model.Table t = db.FindTable(xmlPrimaryKey.TableName, true);
      if (t == null)
        throw new Exception(String.Format("Couldn't find table {0} when creating primary key {1}", xmlPrimaryKey.TableName, xmlPrimaryKey.PrimaryKeyName));
      
      t.PrimaryKeyName = xmlPrimaryKey.PrimaryKeyName;
      foreach(string c in xmlPrimaryKey.Columns){
        DBInfo.Core.Model.Column col = t.FindColumn(c);
        if (col == null)
          throw new Exception(String.Format("Couldn't find column {0} when creating primary key {1}", c, xmlPrimaryKey.PrimaryKeyName));
        t.PrimaryKeyColumns.Add(col);
      }
    }
    
    private DBInfo.Core.Model.Function ParseCreateFunctionStatement(CreateFunction xmlFunction){
      DBInfo.Core.Model.Function f = new DBInfo.Core.Model.Function();
      f.Body = xmlFunction.SourceCode;
      f.Name = xmlFunction.Name;
      return f;
    }
    
    private DBInfo.Core.Model.Sequence ParseCreateSequenceStatement(CreateSequence xmlSequence){
      DBInfo.Core.Model.Sequence s = new DBInfo.Core.Model.Sequence();
      s.SequenceName = xmlSequence.Name;
      s.Initial = Convert.ToInt32(xmlSequence.Initial);
      s.MinValue = Convert.ToInt32(xmlSequence.MinValue);
      s.MaxValue = Convert.ToInt32(xmlSequence.MaxValue);
      s.Increment = Convert.ToInt32(xmlSequence.Increment);
      s.CycleOnLimit = xmlSequence.CycleOnLimit;
      
      return s;
    }
    
    private DBInfo.Core.Model.View ParseCreateViewStatement(CreateView xmlView){
      DBInfo.Core.Model.View v = new DBInfo.Core.Model.View();
      v.Name = xmlView.Name;
      v.Body = xmlView.SourceCode;
      
      return v;
    }
    
    public DBInfo.Core.Model.Database Extract(List<DBObjectType> dataToExtract){
      return ExtractXMLDatabase(_InputFiles);
    }
    
  }
}
