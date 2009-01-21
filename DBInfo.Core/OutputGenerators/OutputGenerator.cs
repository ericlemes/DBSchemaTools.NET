using System;
using System.Collections;
using System.IO;
using System.Data;
using DBInfo.Core.Extractor;
using DBInfo.Core.Model;
using System.Collections.Generic;

namespace DBInfo.Core.OutputGenerators {
  public class OutputGenerator {

    private DBInfoExtractor _Extractor;
    public DBInfoExtractor Extractor {
      get { return _Extractor; }
      set { _Extractor = value; }
    }

    private List<DatabaseScript> _ScriptsTabelas = new List<DatabaseScript>();
    public List<DatabaseScript> ScriptsTabelas {
      get { return _ScriptsTabelas; }
      set { _ScriptsTabelas = value; }
    }

    private List<DatabaseScript> _ScriptsForeignKeys = new List<DatabaseScript>();
    public List<DatabaseScript> ScriptsForeignKeys {
      get { return _ScriptsForeignKeys; }
      set { _ScriptsForeignKeys = value; }
    }

    private List<DatabaseScript> _ScriptsDadosIniciais = new List<DatabaseScript>();
    public List<DatabaseScript> ScriptsDadosIniciais {
      get { return _ScriptsDadosIniciais; }
      set { _ScriptsDadosIniciais = value; }
    }

    private List<DatabaseScript> _ScriptsProcedures = new List<DatabaseScript>();
    public List<DatabaseScript> ScriptsProcedures {
      get { return _ScriptsProcedures; }
      set { _ScriptsProcedures = value; }
    }

    private List<DatabaseScript> _ScriptsFunctions = new List<DatabaseScript>();
    public List<DatabaseScript> ScriptsFunctions {
      get { return _ScriptsFunctions; }
      set { _ScriptsFunctions = value; }
    }

    private List<DatabaseScript> _ScriptsTriggers = new List<DatabaseScript>();
    public List<DatabaseScript> ScriptsTriggers {
      get { return _ScriptsTriggers; }
      set { _ScriptsTriggers = value; }
    }

    private List<DatabaseScript> _ScriptsViews = new List<DatabaseScript>();
    public List<DatabaseScript> ScriptsViews {
      get { return _ScriptsViews; }
      set { _ScriptsViews = value; }
    }

    private List<DatabaseScript> _ScriptsSequences = new List<DatabaseScript>();
    public List<DatabaseScript> ScriptsSequences {
      get { return _ScriptsSequences; }
      set { _ScriptsSequences = value; }
    }

    public enum ScriptsAGerar { Tabelas, ForeignKeys, Procedures, Functions, DadosIniciais, Triggers, Views, Sequences };

    public delegate void AntesGerarScriptsHandler(ScriptsAGerar AScripts, string AObjeto);
    public event AntesGerarScriptsHandler EventoAntesGerarScripts;

    public delegate void AntesGerarDadosIniciais(Table ATable, DataSet ADataset);
    public event AntesGerarDadosIniciais EventoAntesGerarDadosIniciais;
    public delegate void AntesGerarLinhaDadoInicial(Table ATAble, DataRow ARow);
    public event AntesGerarLinhaDadoInicial EventoAntesGerarLinhaDadoInicial;

    private IOutputGenerator _OutputGen;
    public IOutputGenerator OutputGen {
      get { return _OutputGen; }
      set { _OutputGen = value; }
    }

    private void GenerateTables(List<DBObjectType> dataToGenerateOutput) {
      foreach (Table table in Extractor.Tables) {
        if (EventoAntesGerarScripts != null)
          EventoAntesGerarScripts(ScriptsAGerar.Tabelas, table.TableName);

        DatabaseScript ds = new DatabaseScript();
        ds.ScriptName = table.TableName + ".Tabela.sql";
        ds.ScriptContent = OutputGen.GenerateTableOutput(table);
        if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.PrimaryKey))
          ds.ScriptContent += OutputGen.GeneratePrimaryKeyOutput(table);
        if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Indexes))
          ds.ScriptContent += OutputGen.GenerateIndexesOutput(table);
        ScriptsTabelas.Add(ds);

        if (EventoAntesGerarScripts != null)
          EventoAntesGerarScripts(ScriptsAGerar.ForeignKeys, table.TableName);

        if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.ForeignKeys)) {
          DatabaseScript dsFK = new DatabaseScript();
          dsFK.ScriptName = table.TableName + ".ForeignKeys.sql";
          dsFK.ScriptContent = OutputGen.GenerateForeignKeysOutput(table);
          ScriptsForeignKeys.Add(dsFK);
        }
      }
    }

    private void GenerateTableData() {
      foreach (string s in Extractor.TableNames) {
        if (EventoAntesGerarScripts != null)
          EventoAntesGerarScripts(ScriptsAGerar.DadosIniciais, s);

        Table t = Extractor.FindTable(s, true);
        if (t == null)
          throw new Exception("Não foi encontrada tabela " + s);

        DatabaseScript ds = new DatabaseScript();
        ds.ScriptName = t.TableName + ".DadosIniciais.sql";
        ds.ScriptContent = "";

        DataSet DatasetDados = (DataSet)Extractor.TableData[Extractor.TableNames.IndexOf(s)];
        if (DatasetDados.Tables[0].Rows.Count > 0) {
          if (EventoAntesGerarDadosIniciais != null)
            EventoAntesGerarDadosIniciais(t, DatasetDados);
          ds.ScriptContent += OutputGen.GenerateTableDataStartOutput(t);
          foreach (DataRow r in DatasetDados.Tables[0].Rows) {
            if (EventoAntesGerarLinhaDadoInicial != null)
              EventoAntesGerarLinhaDadoInicial(t, r);
            ds.ScriptContent += OutputGen.GenerateTableDataRowOutput(t, r);
          }
          ds.ScriptContent += OutputGen.GenerateTableDataEndOutput(t);
          ScriptsDadosIniciais.Add(ds);
        }
      }
    }

    private void GenerateProcedures() {
      foreach (Procedure p in Extractor.Procedures) {
        if (EventoAntesGerarScripts != null)
          EventoAntesGerarScripts(ScriptsAGerar.Procedures, p.Name);
        DatabaseScript ds = new DatabaseScript();
        ds.ScriptName = p.Name + ".prc";
        ds.ScriptContent = OutputGen.GenerateProcedureOutput(p);
        ScriptsProcedures.Add(ds);
      }
    }

    private void GenerateFunctions() {
      foreach (Function f in Extractor.Functions) {
        if (EventoAntesGerarScripts != null)
          EventoAntesGerarScripts(ScriptsAGerar.Functions, f.Name);
        DatabaseScript ds = new DatabaseScript();
        ds.ScriptName = f.Name + ".udf";
        ds.ScriptContent = OutputGen.GenerateFunctionOutput(f);
        ScriptsFunctions.Add(ds);
      }
    }

    private void GenerateTriggers() {
      foreach (Trigger t in Extractor.Triggers) {
        if (EventoAntesGerarScripts != null)
          EventoAntesGerarScripts(ScriptsAGerar.Triggers, t.Table.TableName + "." + t.Name);
        DatabaseScript ds = new DatabaseScript();
        ds.ScriptName = t.Table.TableName + "." + t.Name + ".trg";
        ds.ScriptContent = OutputGen.GenerateTriggerOutput(t);
        ScriptsTriggers.Add(ds);
      }
    }

    private void GenerateViews() {
      foreach (View v in Extractor.Views) {
        if (EventoAntesGerarScripts != null)
          EventoAntesGerarScripts(ScriptsAGerar.Views, v.Name);
        DatabaseScript ds = new DatabaseScript();
        ds.ScriptName = v.Name + ".viw";
        ds.ScriptContent = OutputGen.GenerateViewOutput(v);
        ScriptsViews.Add(ds);
      }
    }

    private void GenerateSequences() {
      foreach (Sequence s in Extractor.Sequences) {
        if (EventoAntesGerarScripts != null)
          EventoAntesGerarScripts(ScriptsAGerar.Sequences, s.SequenceName);
        DatabaseScript ds = new DatabaseScript();
        ds.ScriptName = s.SequenceName + ".seq";
        ds.ScriptContent = OutputGen.GenerateSequenceOutput(s);
        ScriptsSequences.Add(ds);
      }
    }

    public void GenerateOutput(List<DBObjectType> dataToGenerateOutput) {
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Tables))
        GenerateTables(dataToGenerateOutput);
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.TableData))
        GenerateTableData();
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Procedures))
        GenerateProcedures();
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Functions))
        GenerateFunctions();
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Triggers))
        GenerateTriggers();
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Views))
        GenerateViews();
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Sequences))
        GenerateSequences();
    }

    /*
    public void SalvarScripts() {
      if (!Directory.Exists(ScriptsRootDir + "DadosIniciais"))
        Directory.CreateDirectory(ScriptsRootDir + "DadosIniciais");
      foreach (DatabaseScript ds in ScriptsDadosIniciais) {
        if (EventoAntesSalvarScripts != null)
          EventoAntesSalvarScripts(ds.ScriptName);
        if (ds.ScriptContent.Trim() != String.Empty) {
          StreamWriter sw = new StreamWriter(ScriptsRootDir + "DadosIniciais\\" + ds.ScriptName, false, System.Text.Encoding.Default);
          sw.Write(ds.ScriptContent);
          sw.Close();
        }
      }

      if (!Directory.Exists(ScriptsRootDir + "Procedures"))
        Directory.CreateDirectory(ScriptsRootDir + "Procedures");
      foreach (DatabaseScript ds in ScriptsProcedures) {
        if (EventoAntesSalvarScripts != null)
          EventoAntesSalvarScripts(ds.ScriptName);
        if (ds.ScriptContent.Trim() != String.Empty) {
          StreamWriter sw = new StreamWriter(ScriptsRootDir + "Procedures\\" + ds.ScriptName);
          sw.Write(ds.ScriptContent);
          sw.Close();
        }
      }

      if (!Directory.Exists(ScriptsRootDir + "Functions"))
        Directory.CreateDirectory(ScriptsRootDir + "Functions");
      foreach (DatabaseScript ds in ScriptsFunctions) {
        if (EventoAntesSalvarScripts != null)
          EventoAntesSalvarScripts(ds.ScriptName);
        if (ds.ScriptContent.Trim() != String.Empty) {
          StreamWriter sw = new StreamWriter(ScriptsRootDir + "Functions\\" + ds.ScriptName);
          sw.Write(ds.ScriptContent);
          sw.Close();
        }
      }

      if (!Directory.Exists(ScriptsRootDir + "Triggers"))
        Directory.CreateDirectory(ScriptsRootDir + "Triggers");
      foreach (DatabaseScript ds in ScriptsTriggers) {
        if (EventoAntesSalvarScripts != null)
          EventoAntesSalvarScripts(ds.ScriptName);
        if (ds.ScriptContent.Trim() != String.Empty) {
          StreamWriter sw = new StreamWriter(ScriptsRootDir + "Triggers\\" + ds.ScriptName);
          sw.Write(ds.ScriptContent);
          sw.Close();
        }
      }

      if (!Directory.Exists(ScriptsRootDir + "Views"))
        Directory.CreateDirectory(ScriptsRootDir + "Views");
      foreach (DatabaseScript ds in ScriptsViews) {
        if (EventoAntesSalvarScripts != null)
          EventoAntesSalvarScripts(ds.ScriptName);
        if (ds.ScriptContent.Trim() != String.Empty) {
          StreamWriter sw = new StreamWriter(ScriptsRootDir + "Views\\" + ds.ScriptName);
          sw.Write(ds.ScriptContent);
          sw.Close();
        }
      }
    }

    public void SalvarFullScript(System.Text.Encoding AEncoding) {
      if (ScriptsRootDir == String.Empty)
        throw new Exception("ScriptsRootDir não informado");
      if (!Directory.Exists(ScriptsRootDir))
        Directory.CreateDirectory(ScriptsRootDir);

      if (FullScriptName == null || FullScriptName == String.Empty)
        throw new Exception("Informe FullScriptName");

      if (File.Exists(ScriptsRootDir + "\\" + FullScriptName))
        File.Delete(ScriptsRootDir + "\\" + FullScriptName);

      StreamWriter sw = new StreamWriter(ScriptsRootDir + "\\" + FullScriptName, true, AEncoding);
      foreach (DatabaseScript ds in ScriptsSequences) {
        if (EventoAntesSalvarScripts != null)
          EventoAntesSalvarScripts(FullScriptName);
        sw.Write(ds.ScriptContent);
      }
      foreach (DatabaseScript ds in ScriptsTabelas) {
        if (EventoAntesSalvarScripts != null)
          EventoAntesSalvarScripts(FullScriptName);
        sw.Write(ds.ScriptContent);
      }
      sw.Close();
    }
     */

  }
}
