using System;
using System.Collections;
using System.IO;
using System.Data;
using DBInfo.Core.Extractor;
using DBInfo.Core.Model;
using System.Collections.Generic;

namespace DBInfo.Core.OutputGenerators {
  public class ScriptOutputGenerator : IOutputGenerator {

    public delegate void BeforeGenerateScriptHandler(DBObjectType objectType, string objectName);
    public event BeforeGenerateScriptHandler BeforeGenerateScript;

    public delegate void BeforeGenerateTableDataHandler(Table table, DataTable dataTable);
    public event BeforeGenerateTableDataHandler BeforeGenerateTableData;
    public delegate void BeforeGenerateDataRowHandler(Table table, DataRow row);
    public event BeforeGenerateDataRowHandler BeforeGenerateDataRow;
    
    public GeneratorType Type{
      get { return GeneratorType.Script;}
    }

    private IScriptOutputGenerator _ScriptOutputGen;
    public IScriptOutputGenerator ScriptOutputGen {
      get { return _ScriptOutputGen; }
      set { _ScriptOutputGen = value; }
    }
    
    private InputOutputType _OutputType;
    public InputOutputType OutputType{
      get { return _OutputType;}
      set { _OutputType = value;}
    }
    
    private string _OutputConnectionString;
    public string OutputConnectionString{
      get { return _OutputConnectionString;}
      set { _OutputConnectionString = value;}
    }
    
    private string _OutputDir;
    public string OutputDir{
      get { return _OutputDir;}
      set { _OutputDir = value;}
    }
    
    private IScriptFileOutputGenerator _ScriptFileOutputGenerator;
    public IScriptFileOutputGenerator ScriptFileOutputGenerator{
      get { return _ScriptFileOutputGenerator;}
      set { _ScriptFileOutputGenerator = value;}
    }

    private void GenerateTables(Database db, List<DBObjectType> dataToGenerateOutput) {
      foreach (Table table in db.Tables) {
        if (BeforeGenerateScript != null)
          BeforeGenerateScript(DBObjectType.Tables, table.TableName);

        table.TableScript = ScriptOutputGen.GenerateTableScript(table);             
        if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.PrimaryKey)){        
          if (!String.IsNullOrEmpty(table.PrimaryKeyName))
            table.PrimaryKeyScript = ScriptOutputGen.GeneratePrimaryKeyScript(table);
        }
          
        if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Indexes))
        foreach (Index index in table.Indexes){
          index.Script = ScriptOutputGen.GenerateIndexScript(table, index);
        }        

        if (BeforeGenerateScript != null)
          BeforeGenerateScript(DBObjectType.CheckConstraints, table.TableName);
          
        if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.CheckConstraints)) {
          foreach (CheckConstraint check in table.CheckConstraints){
            check.Script = ScriptOutputGen.GenerateCheckConstraintScript(table, check);
          }
        }

        if (BeforeGenerateScript != null)
          BeforeGenerateScript(DBObjectType.ForeignKeys, table.TableName);

        if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.ForeignKeys)) {
          foreach(ForeignKey fk in table.ForeignKeys){
            fk.Script = ScriptOutputGen.GenerateForeignKeysScript(table, fk);
          }          
        }
      }
    }

    private void GenerateTableData(Database db) {
      foreach (string s in db.TableNames) {
        if (BeforeGenerateScript != null)
          BeforeGenerateScript(DBObjectType.TableData, s);

        Table t = db.FindTable(s, true);
        if (t == null)
          throw new Exception("Não foi encontrada tabela " + s);
          
        if (t.TableData == null)          
          continue;
                          
        if (t.TableData.Rows.Count > 0) {
          if (BeforeGenerateTableData != null)
            BeforeGenerateTableData(t, t.TableData);
          t.TableDataScript = ScriptOutputGen.GenerateTableDataStartScript(t);
          foreach (DataRow r in t.TableData.Rows) {
            if (BeforeGenerateDataRow != null)
              BeforeGenerateDataRow(t, r);
            t.TableDataScript += ScriptOutputGen.GenerateTableDataRowScript(t, r);
          }
          t.TableDataScript += ScriptOutputGen.GenerateTableDataEndScript(t);          
        }
      }
    }

    private void GenerateProcedures(Database db) {
      foreach (Procedure p in db.Procedures) {
        if (BeforeGenerateScript != null)
          BeforeGenerateScript(DBObjectType.Procedures, p.Name);
        p.DropProcedureScript = ScriptOutputGen.GenerateDropProcedureScript(p);
        p.CreateProcedureScript = ScriptOutputGen.GenerateCreateProcedureScript(p);                        
      }
    }

    private void GenerateFunctions(Database db) {
      foreach (Function f in db.Functions) {
        if (BeforeGenerateScript != null)
          BeforeGenerateScript(DBObjectType.Functions, f.Name);
        f.DropFunctionScript = ScriptOutputGen.GenerateDropFunctionScript(f);
        f.CreateFunctionScript = ScriptOutputGen.GenerateCreateFunctionScript(f);                
      }
    }

    private void GenerateTriggers(Database db) {
      foreach (Table table in db.Tables) {
        foreach (Trigger t in table.Triggers) {
          if (BeforeGenerateScript != null)
            BeforeGenerateScript(DBObjectType.Triggers, t.Table.TableName + "." + t.Name);
          t.DropTriggerScript = ScriptOutputGen.GenerateDropTriggerScript(table, t);
          t.CreateTriggerScript = ScriptOutputGen.GenerateCreateTriggerScript(table, t);          
        }
      }
    }

    private void GenerateViews(Database db) {
      foreach (View v in db.Views) {
        if (BeforeGenerateScript != null)
          BeforeGenerateScript(DBObjectType.Views, v.Name);
        v.DropViewScript = ScriptOutputGen.GenerateDropViewScript(v);
        v.CreateViewScript = ScriptOutputGen.GenerateCreateViewScript(v);
      }
    }

    private void GenerateSequences(Database db) {
      foreach (Sequence s in db.Sequences) {
        if (BeforeGenerateScript != null)
          BeforeGenerateScript(DBObjectType.Sequences, s.SequenceName);
        s.SequenceScript = ScriptOutputGen.GenerateSequenceScript(s);        
      }
    }

    public void GenerateOutput(Database db, List<DBObjectType> dataToGenerateOutput) {
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Tables))
        GenerateTables(db, dataToGenerateOutput);
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.TableData))
        GenerateTableData(db);
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Procedures))
        GenerateProcedures(db);
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Functions))
        GenerateFunctions(db);
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Triggers))
        GenerateTriggers(db);
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Views))
        GenerateViews(db);
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Sequences))
        GenerateSequences(db);
        
      if (OutputType == InputOutputType.File)
        SaveScripts(db);
      else 
        ApplyToDestinationDB(db);
    }
    
    private void ApplyToDestinationDB(Database db){
      if (String.IsNullOrEmpty(_OutputConnectionString))
        throw new Exception("Output connection string is required.");

      ScriptOutputGen.OpenOutputDatabaseConnection(_OutputConnectionString);
      foreach(Table t in db.Tables){
        if (!String.IsNullOrEmpty(t.TableScript))
          ScriptOutputGen.ExecuteOuputDatabaseScript(t.TableScript);
        foreach(Index i in t.Indexes){
          if (!String.IsNullOrEmpty(i.Script)){
            ScriptOutputGen.ExecuteOuputDatabaseScript(i.Script);
          }
        }
        if (!String.IsNullOrEmpty(t.PrimaryKeyScript))
          ScriptOutputGen.ExecuteOuputDatabaseScript(t.PrimaryKeyScript);                
        foreach(CheckConstraint check in t.CheckConstraints){
          if (!String.IsNullOrEmpty(check.Script))
            ScriptOutputGen.ExecuteOuputDatabaseScript(check.Script);
        }
      }
      foreach(Table t in db.Tables){
        foreach(ForeignKey fk in t.ForeignKeys){
          if (!String.IsNullOrEmpty(fk.Script))
            ScriptOutputGen.ExecuteOuputDatabaseScript(fk.Script);      
        }
      }          
      foreach(Procedure p in db.Procedures){
        if (!String.IsNullOrEmpty(p.DropProcedureScript))
          ScriptOutputGen.ExecuteOuputDatabaseScript(p.DropProcedureScript);
        if (!String.IsNullOrEmpty(p.CreateProcedureScript))
          ScriptOutputGen.ExecuteOuputDatabaseScript(p.CreateProcedureScript);
      }  
      
      /*ApplyScriptBatch(FunctionScripts);
      ApplyScriptBatch(TriggerScripts);
      ApplyScriptBatch(ViewScripts);
      ApplyScriptBatch(SequenceScripts);*/
      ScriptOutputGen.CloseOutputDatabaseConnection();
    }       
    
    private void SaveScriptBatch(List<DatabaseScript> scripts, string dir){
      if (!Directory.Exists(dir))
        Directory.CreateDirectory(dir);
      foreach (DatabaseScript ds in scripts) {
        if (ds.ScriptContent.Trim() != String.Empty) {
          StreamWriter sw = new StreamWriter(dir + "\\" + ds.ScriptName, false, System.Text.Encoding.Default);
          sw.Write(ds.ScriptContent);
          sw.Close();
        }
      }
    }
    
    public void SaveScripts(Database db) {      
      if (!Directory.Exists(OutputDir))
        Directory.CreateDirectory(OutputDir);        
        
      if (_ScriptFileOutputGenerator == null)
        throw new Exception("ScriptFileOutputGenerator is required");

      _ScriptFileOutputGenerator.GenerateFileOutput(OutputDir, db, ScriptOutputGen); 
    }
    

  }
}
