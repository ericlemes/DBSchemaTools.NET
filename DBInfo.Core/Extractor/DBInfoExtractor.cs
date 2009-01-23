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

    public delegate void BeforeExtractDataHandler(DBObjectType objectType, string objectName);
    public event BeforeExtractDataHandler BeforeExtractData;

    private void ReadTables(Database db) {
      db.Tables = _Extractor.GetTables();
      if (db.Tables == null)
        throw new Exception("The IDBExtractor GetTables method mustn't return null");

      if (BeforeExtractData != null)
        BeforeExtractData(DBObjectType.Tables, "");      

      foreach (Table Table in db.Tables) {
        if (BeforeExtractData != null)
          BeforeExtractData(DBObjectType.Columns, Table.TableName);

        Extractor.GetTableColumns(db, Table);                
      }
    }

    private string GetString(object AValue) {
      return AValue == DBNull.Value ? String.Empty : (string)AValue;
    }

    private void ReadPrimaryKeys(Database db) {
      foreach (Table Table in db.Tables) {
        if (BeforeExtractData != null)
          BeforeExtractData(DBObjectType.PrimaryKey, Table.TableName);
        
        Extractor.GetPrimaryKey(db, Table);                
        if (!String.IsNullOrEmpty(Table.PrimaryKeyName))
          Extractor.GetPrimaryKeyColumns(db, Table);
      }
    }

    private void ReadCheckConstraints(Database db) {
      foreach (Table Table in db.Tables) {
        if (BeforeExtractData != null)
          BeforeExtractData(DBObjectType.CheckConstraints, Table.TableName);
        
        Extractor.GetCheckConstraints(db, Table);                
      }
    }

    private void ReadIndexes(Database db) {
      foreach (Table t in db.Tables) {
        if (BeforeExtractData != null)
          BeforeExtractData(DBObjectType.Indexes, t.TableName);
        
        Extractor.GetIndexes(db, t);        

        foreach (Index i in t.Indexes) {
          Extractor.GetIndexColumns(db, t, i);          
        }
      }
    }    

    private void ReadForeignKeys(Database db) {
      foreach (Table Table in db.Tables) {
        if (BeforeExtractData != null)
          BeforeExtractData(DBObjectType.ForeignKeys, Table.TableName);

        Extractor.GetForeignKeys(db, Table);
        
        foreach(ForeignKey fk in Table.ForeignKeys){
          Extractor.GetForeignKeyColumns(db, Table, fk);          
        }
      }
    }

    private void ReadTableData(Database db) {
      foreach (string s in db.TableNames) {
        if (BeforeExtractData != null)
          BeforeExtractData(DBObjectType.TableData, s);
        DataSet dsDados = Extractor.GetTableData(db, s);
        db.TableData.Add(dsDados);
      }
    }

    private void ReadProcedures(Database db) {
      Extractor.GetProcedures(db);
      foreach(Procedure p in db.Procedures){
        if (BeforeExtractData != null)
          BeforeExtractData(DBObjectType.Procedures, p.Name);

        Extractor.GetProcedureText(db, p);        
      }      
    }

    private void ReadFunctions(Database db) {
      _Extractor.GetFunctions(db);
      
      foreach(Function f in db.Functions){      
        if (BeforeExtractData != null)
          BeforeExtractData(DBObjectType.Functions, f.Name);
          
        _Extractor.GetFunctionText(db, f);
      }
      
    }

    private void ReadTriggers(Database db) {
      foreach (Table t in db.Tables) {
        if (BeforeExtractData != null)
          BeforeExtractData(DBObjectType.Triggers, t.TableName);

        _Extractor.GetTriggers(db, t);
        
        foreach(Trigger tr in t.Triggers){
          _Extractor.GetTriggerText(db, t, tr);
        }
      }
    }

    private void ReadViews(Database db) {
      _Extractor.GetViews(db);      

      foreach (View v in db.Views) {
        if (BeforeExtractData != null)
          BeforeExtractData(DBObjectType.Views, v.Name);        
          
        _Extractor.GetViewText(db, v);
      }
    }

    private void ReadSequences(Database db) {
      _Extractor.GetSequences(db);      
    }

    public Database Extract(List<DBObjectType> dataToExtract) {
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
      
      return db;
    }
  }
}
