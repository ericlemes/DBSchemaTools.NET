using System;
using System.Collections;
using System.IO;
using System.Data;
using DBInfo.Core.Extractor;
using DBInfo.Core.Model;
using System.Collections.Generic;

namespace DBInfo.Core.OutputGenerators {
  public class ScriptOutputGenerator {

    private List<DatabaseScript> _TableScripts = new List<DatabaseScript>();
    public List<DatabaseScript> TableScripts {
      get { return _TableScripts; }
      set { _TableScripts = value; }
    }
    
    private List<DatabaseScript> _CheckConstraintScripts = new List<DatabaseScript>();
    public List<DatabaseScript> CheckConstraintScripts{
      get { return _CheckConstraintScripts;}
      set { _CheckConstraintScripts = value;}
    }

    private List<DatabaseScript> _ForeignKeyScripts = new List<DatabaseScript>();
    public List<DatabaseScript> ForeignKeyScripts {
      get { return _ForeignKeyScripts; }
      set { _ForeignKeyScripts = value; }
    }

    private List<DatabaseScript> _TableDataScripts = new List<DatabaseScript>();
    public List<DatabaseScript> TableDataScripts {
      get { return _TableDataScripts; }
      set { _TableDataScripts = value; }
    }

    private List<DatabaseScript> _ProcedureScripts = new List<DatabaseScript>();
    public List<DatabaseScript> ProcedureScripts {
      get { return _ProcedureScripts; }
      set { _ProcedureScripts = value; }
    }

    private List<DatabaseScript> _FunctionScripts = new List<DatabaseScript>();
    public List<DatabaseScript> FunctionScripts {
      get { return _FunctionScripts; }
      set { _FunctionScripts = value; }
    }

    private List<DatabaseScript> _TriggerScripts = new List<DatabaseScript>();
    public List<DatabaseScript> TriggerScripts {
      get { return _TriggerScripts; }
      set { _TriggerScripts = value; }
    }

    private List<DatabaseScript> _ViewScripts = new List<DatabaseScript>();
    public List<DatabaseScript> ViewScripts {
      get { return _ViewScripts; }
      set { _ViewScripts = value; }
    }

    private List<DatabaseScript> _SequenceScripts = new List<DatabaseScript>();
    public List<DatabaseScript> SequenceScripts {
      get { return _SequenceScripts; }
      set { _SequenceScripts = value; }
    }    

    public delegate void BeforeGenerateScriptHandler(DBObjectType objectType, string objectName);
    public event BeforeGenerateScriptHandler BeforeGenerateScript;

    public delegate void BeforeGenerateTableDataHandler(Table table, DataSet dataset);
    public event BeforeGenerateTableDataHandler BeforeGenerateTableData;
    public delegate void BeforeGenerateDataRowHandler(Table table, DataRow row);
    public event BeforeGenerateDataRowHandler BeforeGenerateDataRow;

    private IScriptsOutputGenerator _OutputGen;
    public IScriptsOutputGenerator OutputGen {
      get { return _OutputGen; }
      set { _OutputGen = value; }
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

    private void GenerateTables(Database db, List<DBObjectType> dataToGenerateOutput) {
      foreach (Table table in db.Tables) {
        if (BeforeGenerateScript != null)
          BeforeGenerateScript(DBObjectType.Tables, table.TableName);

        table.TableScript = OutputGen.GenerateTableScript(table);             
        if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.PrimaryKey)){        
          if (!String.IsNullOrEmpty(table.PrimaryKeyName))
            table.PrimaryKeyScript = OutputGen.GeneratePrimaryKeyScript(table);
        }
          
        if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Indexes))
        foreach (Index index in table.Indexes){
          index.Script = OutputGen.GenerateIndexScript(table, index);
        }        

        if (BeforeGenerateScript != null)
          BeforeGenerateScript(DBObjectType.CheckConstraints, table.TableName);
          
        if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.CheckConstraints)) {
          foreach (CheckConstraint check in table.CheckConstraints){
            check.Script = OutputGen.GenerateCheckConstraintScript(table, check);
          }
        }

        if (BeforeGenerateScript != null)
          BeforeGenerateScript(DBObjectType.ForeignKeys, table.TableName);

        if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.ForeignKeys)) {
          foreach(ForeignKey fk in table.ForeignKeys){
            fk.Script = OutputGen.GenerateForeignKeysScript(table, fk);
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

        DatabaseScript ds = new DatabaseScript();
        ds.ScriptName = t.TableName + ".DadosIniciais.sql";
        ds.ScriptContent = "";

        DataSet DatasetDados = db.TableData[db.TableNames.IndexOf(s)];
        if (DatasetDados.Tables[0].Rows.Count > 0) {
          if (BeforeGenerateTableData != null)
            BeforeGenerateTableData(t, DatasetDados);
          ds.ScriptContent += OutputGen.GenerateTableDataStartScript(t);
          foreach (DataRow r in DatasetDados.Tables[0].Rows) {
            if (BeforeGenerateDataRow != null)
              BeforeGenerateDataRow(t, r);
            ds.ScriptContent += OutputGen.GenerateTableDataRowScript(t, r);
          }
          ds.ScriptContent += OutputGen.GenerateTableDataEndScript(t);
          TableDataScripts.Add(ds);
        }
      }
    }

    private void GenerateProcedures(Database db) {
      foreach (Procedure p in db.Procedures) {
        if (BeforeGenerateScript != null)
          BeforeGenerateScript(DBObjectType.Procedures, p.Name);
        p.DropProcedureScript = OutputGen.GenerateDropProcedureScript(p);                        
        p.CreateProcedureScript = OutputGen.GenerateCreateProcedureScript(p);                        
      }
    }

    private void GenerateFunctions(Database db) {
      foreach (Function f in db.Functions) {
        if (BeforeGenerateScript != null)
          BeforeGenerateScript(DBObjectType.Functions, f.Name);
        DatabaseScript ds = OutputGen.GenerateFunctionScript(f);                
        FunctionScripts.Add(ds);
      }
    }

    private void GenerateTriggers(Database db) {
      foreach (Table table in db.Tables) {
        foreach (Trigger t in table.Triggers) {
          if (BeforeGenerateScript != null)
            BeforeGenerateScript(DBObjectType.Triggers, t.Table.TableName + "." + t.Name);
          DatabaseScript ds = OutputGen.GenerateTriggerScript(table, t);          
          TriggerScripts.Add(ds);
        }
      }
    }

    private void GenerateViews(Database db) {
      foreach (View v in db.Views) {
        if (BeforeGenerateScript != null)
          BeforeGenerateScript(DBObjectType.Views, v.Name);
        DatabaseScript ds = OutputGen.GenerateViewScript(v);               
        ViewScripts.Add(ds);
      }
    }

    private void GenerateSequences(Database db) {
      foreach (Sequence s in db.Sequences) {
        if (BeforeGenerateScript != null)
          BeforeGenerateScript(DBObjectType.Sequences, s.SequenceName);
        DatabaseScript ds = OutputGen.GenerateSequenceScript(s);                
        if (ds!= null)
          SequenceScripts.Add(ds);
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
        SaveScripts();
      else 
        ApplyToDestinationDB(db);
    }
    
    private void ApplyToDestinationDB(Database db){
      if (String.IsNullOrEmpty(_OutputConnectionString))
        throw new Exception("Output connection string is required.");
    
      OutputGen.OpenOutputDatabaseConnection(_OutputConnectionString);
      foreach(Table t in db.Tables){
        if (!String.IsNullOrEmpty(t.TableScript))
          OutputGen.ExecuteOuputDatabaseScript(t.TableScript);
        foreach(Index i in t.Indexes){
          if (!String.IsNullOrEmpty(i.Script)){
            OutputGen.ExecuteOuputDatabaseScript(i.Script);
          }
        }
        if (!String.IsNullOrEmpty(t.PrimaryKeyScript))
          OutputGen.ExecuteOuputDatabaseScript(t.PrimaryKeyScript);                
        foreach(CheckConstraint check in t.CheckConstraints){
          if (!String.IsNullOrEmpty(check.Script))
            OutputGen.ExecuteOuputDatabaseScript(check.Script);
        }
      }
      foreach(Table t in db.Tables){
        foreach(ForeignKey fk in t.ForeignKeys){
          if (!String.IsNullOrEmpty(fk.Script))
            OutputGen.ExecuteOuputDatabaseScript(fk.Script);      
        }
      }          
      foreach(Procedure p in db.Procedures){
        if (!String.IsNullOrEmpty(p.DropProcedureScript))
          OutputGen.ExecuteOuputDatabaseScript(p.DropProcedureScript);
        if (!String.IsNullOrEmpty(p.CreateProcedureScript))
          OutputGen.ExecuteOuputDatabaseScript(p.CreateProcedureScript);
      }  
      
      /*ApplyScriptBatch(FunctionScripts);
      ApplyScriptBatch(TriggerScripts);
      ApplyScriptBatch(ViewScripts);
      ApplyScriptBatch(SequenceScripts);*/
      OutputGen.CloseOutputDatabaseConnection();
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
    
    public void SaveScripts() {
      if (!Directory.Exists(OutputDir))
        throw new Exception(String.Format("Output Dir don't exists: {0}", OutputDir));

      SaveScriptBatch(TableScripts, OutputDir + "\\Tables");
      SaveScriptBatch(ForeignKeyScripts, OutputDir + "\\ForeignKeys");
      SaveScriptBatch(TableDataScripts, OutputDir + "\\DadosIniciais");
      SaveScriptBatch(ProcedureScripts, OutputDir + "\\Procedures");
      SaveScriptBatch(FunctionScripts, OutputDir + "\\Functions");
      SaveScriptBatch(TriggerScripts, OutputDir + "\\Triggers");
      SaveScriptBatch(ViewScripts, OutputDir + "\\Views");
      SaveScriptBatch(SequenceScripts, OutputDir + "\\Sequences");                
    }
    

  }
}
