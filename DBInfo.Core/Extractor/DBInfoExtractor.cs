using System;
using System.Data;
using System.Data.Common;
using System.Collections;
using DBInfo.Core;
using DBInfo.Core.Model;
using System.IO;
using System.Collections.Generic;

namespace DBInfo.Core.Extractor {
  public enum InputOutputType{
    File,
    Database
  }

  public enum DBObjectType {
    All,
    Tables,
    CheckConstraints,
    Columns,
    PrimaryKey,
    Indexes,
    ForeignKeys,
    Procedures,
    Functions,
    TableData,
    Triggers,
    Views,
    Sequences    
  };

  public class DBInfoExtractor {
    private InputOutputType _InputType;
    public InputOutputType InputType{
      get { return _InputType;}
      set { _InputType = value;}
    }
    
    private string _InputConnectionString;
    public string InputConnectionString {
      get { return _InputConnectionString;}
      set {_InputConnectionString = value;}
    }
    
    private string _InputDir;
    public string InputDir{
      get { return _InputDir;}
      set { _InputDir = value;}
    }  
  
    private IDBInfoExtractor _Extractor;
    public IDBInfoExtractor Extractor{
      get { return _Extractor;}
      set {_Extractor = value;}
    }
    
    private Database _Database;
    public Database Database {
      get { return _Database;}
      set { _Database = value;}
    }

    public delegate void AntesLerDadosBancoHandler(DBObjectType ADados, string AObjeto);
    public event AntesLerDadosBancoHandler EventoAntesLerDadosBanco;

    private int GetInteger(object AValue) {
      return AValue == DBNull.Value ? 0 : (int)AValue;
    }

    private bool GetBoolean(object AValue) {
      return AValue == DBNull.Value ? false : (bool)AValue;
    }

    private void ReadTables(Database db) {
      db.Tables = _Extractor.GetTables();
      if (db.Tables == null)
        throw new Exception("The IDBExtractor GetTables method mustn't return null");

      if (EventoAntesLerDadosBanco != null)
        EventoAntesLerDadosBanco(DBObjectType.Tables, "");      

      foreach (Table Table in db.Tables) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DBObjectType.Columns, Table.TableName);

        Extractor.GetTableColumns(Table);                
      }
    }

    private string GetString(object AValue) {
      return AValue == DBNull.Value ? String.Empty : (string)AValue;
    }

    private void ReadPrimaryKeys(Database db) {
      foreach (Table Table in db.Tables) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DBObjectType.PrimaryKey, Table.TableName);
        
        Extractor.GetPrimaryKey(Table);                
        Extractor.GetPrimaryKeyColumns(Table);
      }
    }

    private void ReadCheckConstraints(Database db) {
      foreach (Table Table in db.Tables) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DBObjectType.CheckConstraints, Table.TableName);

        DataSet CheckDataset;
        CheckDataset = Extractor.getCheckConstraints(Table.TableName);

        if (CheckDataset == null)
          return;

        if (CheckDataset.Tables[0].Rows.Count > 0) {
          foreach (DataRow r in CheckDataset.Tables[0].Rows) {
            CheckConstraint ch = new CheckConstraint();
            ch.Name = (string)r[0];
            ch.Expression = (string)r[1];
            Table.CheckConstraints.Add(ch);
          }
        }
      }
    }

    private void ReadIndexes(Database db) {
      foreach (Table t in db.Tables) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DBObjectType.Indexes, t.TableName);
        
        Extractor.GetIndexes(t);        

        foreach (Index i in t.Indexes) {
          Extractor.GetIndexColumns(t, i);          
        }
      }
    }    

    private void ReadForeignKeys(Database db) {
      foreach (Table Table in db.Tables) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DBObjectType.ForeignKeys, Table.TableName);

        Extractor.GetForeignKeys(Table);
        
        foreach(ForeignKey fk in Table.ForeignKeys){
          Extractor.GetForeignKeyColumns(db, Table, fk);          
        }
      }
    }

    private void ReadTableData(Database db) {
      foreach (string s in db.TableNames) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DBObjectType.TableData, s);
        DataSet dsDados = Extractor.GetTableData(s);
        db.TableData.Add(dsDados);
      }
    }

    private void ReadProcedures(Database db) {
      Extractor.GetProcedures(db);
      foreach(Procedure p in db.Procedures){
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DBObjectType.Procedures, p.Name);

        Extractor.GetProcedureText(db, p);        
      }      
    }

    private void ReadFunctions(Database db) {
      _Extractor.GetFunctions(db);
      
      foreach(Function f in db.Functions){      
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DBObjectType.Functions, f.Name);
          
        _Extractor.GetFunctionText(db, f);
      }
      
    }

    private void ReadTriggers(Database db) {
      foreach (Table t in db.Tables) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DBObjectType.Triggers, t.TableName);

        _Extractor.GetTriggers(db, t);
        
        foreach(Trigger tr in t.Triggers){
          _Extractor.GetTriggerText(db, t, tr);
        }
      }
    }

    private void ReadViews(Database db) {
      DataSet dsViews = _Extractor.getViews();

      if (dsViews == null)
        return;

      foreach (DataRow dr in dsViews.Tables[0].Rows) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DBObjectType.Views, (string)dr[0]);

        View v = new View();
        v.Name = (string)dr[0];
        v.Body = "";
        DataSet dsViewBody = _Extractor.getViewText(v.Name);
        if (dsViewBody.Tables.Count > 0) {
          foreach (DataRow r2 in dsViewBody.Tables[0].Rows) {
            v.Body += (string)r2[0];
          }
        }
        db.Views.Add(v);
      }
    }

    private void ReadSequences(Database db) {
      DataSet dsSequences = _Extractor.getSequences();

      if (dsSequences == null)
        return;

      foreach (DataRow dr in dsSequences.Tables[0].Rows) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DBObjectType.Sequences, (string)dr[0]);

        Sequence s = new Sequence();
        s.SequenceName = GetString(dr[0]);
        s.Initial = GetInteger(dr[1]);
        s.MinValue = GetInteger(dr[2]);
        s.MaxValue = dr[3] == DBNull.Value ? -1 : (int)dr[3];
        s.Increment = GetInteger(dr[4]);
        s.CycleOnLimit = GetBoolean(dr[5]);
        db.Sequences.Add(s);
      }
    }

    public void Extract(List<DBObjectType> dataToExtract) {
      Database db = new Database();
    
      if (_InputType == InputOutputType.File && !Directory.Exists(_InputDir))
        throw new Exception(String.Format("The input directory don't exists: {0}", _InputDir));

      if (_InputType == InputOutputType.Database && String.IsNullOrEmpty(_InputConnectionString))
        throw new Exception(String.Format("The input connection string don't exists: {0}", _InputConnectionString));        
      
      _Extractor.InputType = _InputType;
      _Extractor.InputConnectionString = _InputConnectionString;
      _Extractor.InputDir = _InputDir;
      
    
      _Extractor.Open();
      try {
        if (dataToExtract.Contains(DBObjectType.All) || dataToExtract.Contains(DBObjectType.Tables)){
          ReadTables(db);
          if (dataToExtract.Contains(DBObjectType.All) || dataToExtract.Contains(DBObjectType.PrimaryKey))
            ReadPrimaryKeys(db);
          if (dataToExtract.Contains(DBObjectType.All) || dataToExtract.Contains(DBObjectType.ForeignKeys))
            ReadForeignKeys(db);
          if (dataToExtract.Contains(DBObjectType.All) || dataToExtract.Contains(DBObjectType.CheckConstraints))
            ReadCheckConstraints(db);
          if (dataToExtract.Contains(DBObjectType.All) || dataToExtract.Contains(DBObjectType.Indexes))
            ReadIndexes(db);
        }        
        if (dataToExtract.Contains(DBObjectType.Functions))
          ReadFunctions(db);
        if (dataToExtract.Contains(DBObjectType.Procedures))
          ReadProcedures(db);
        if (dataToExtract.Contains(DBObjectType.Triggers))
          ReadTriggers(db);
        if (dataToExtract.Contains(DBObjectType.Views))
          ReadViews(db);
        if (dataToExtract.Contains(DBObjectType.Sequences))
          ReadSequences(db);
      } finally {
        _Extractor.Close();
      }
    }
  }
}
