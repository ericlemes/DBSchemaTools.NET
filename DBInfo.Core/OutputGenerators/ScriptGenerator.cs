using System;
using System.Collections;
using System.IO;
using System.Data;
using DBInfo.Core.Extractor;
using DBInfo.Core.Model;

namespace DBInfo.Core.OutputGenerators {
  public abstract class ScriptGenerator {
    public DBInfoExtractor Introspector;
    public ArrayList ScriptsTabelas;
    public ArrayList ScriptsForeignKeys;
    public ArrayList ScriptsDadosIniciais;
    public ArrayList ScriptsProcedures;
    public ArrayList ScriptsFunctions;
    public ArrayList ScriptsTriggers;
    public ArrayList ScriptsViews;
    public ArrayList ScriptsSequences;
    public string ScriptsRootDir;
    public string FullScriptName;

    public enum ScriptsAGerar { Tabelas, ForeignKeys, Procedures, Functions, DadosIniciais, Triggers, Views, Sequences };

    public delegate void AntesGerarScriptsHandler(ScriptsAGerar AScripts, string AObjeto);
    public event AntesGerarScriptsHandler EventoAntesGerarScripts;

    public delegate void AntesSalvarScripts(string AScript);
    public event AntesSalvarScripts EventoAntesSalvarScripts;

    public delegate void AntesGerarDadosIniciais(Table ATable, DataSet ADataset);
    public event AntesGerarDadosIniciais EventoAntesGerarDadosIniciais;
    public delegate void AntesGerarLinhaDadoInicial(Table ATAble, DataRow ARow);
    public event AntesGerarLinhaDadoInicial EventoAntesGerarLinhaDadoInicial;

    public ScriptGenerator() {
      ScriptsRootDir = String.Empty;
      ScriptsTabelas = new ArrayList();
      ScriptsForeignKeys = new ArrayList();
      ScriptsDadosIniciais = new ArrayList();
      ScriptsProcedures = new ArrayList();
      ScriptsFunctions = new ArrayList();
      ScriptsTriggers = new ArrayList();
      ScriptsViews = new ArrayList();
      ScriptsSequences = new ArrayList();
    }

    protected abstract string GenerateTableOutput(Table ATable);
    protected abstract string GerarScriptPrimaryKey(Table ATable);
    protected abstract string GerarScriptIndices(Table ATable);
    protected abstract string GerarScriptForeignKey(Table ATable);
    public abstract string GerarScriptDadosIniciaisInicioScript(Table ATable);
    public abstract string GerarScriptDadosIniciaisLinha(Table ATable, DataRow ARow);
    public abstract string GerarScriptDadosIniciaisFimScript(Table ATable);
    protected abstract string GerarScriptProcedure(Procedure AProcedure);
    protected abstract string GerarScriptFunction(Function AFunction);
    protected abstract string GerarScriptTrigger(Trigger ATrigger);
    protected abstract string GerarScriptView(View AView);
    protected abstract string GerarScriptSequence(Sequence ASequence);

    public void SalvarScriptsTabelasXML() {
      if (ScriptsRootDir == String.Empty)
        throw new Exception("ScriptsRootDir não informado");
      if (!Directory.Exists(ScriptsRootDir))
        Directory.CreateDirectory(ScriptsRootDir);

      if (!Directory.Exists(ScriptsRootDir + "Tabelas"))
        Directory.CreateDirectory(ScriptsRootDir + "Tabelas");

      System.Xml.Serialization.XmlSerializer mySerializer =
        new System.Xml.Serialization.XmlSerializer(typeof(Table));

      foreach (Table table in Introspector.Tables) {
        StreamWriter myWriter = new StreamWriter(ScriptsRootDir + "Tabelas\\" + table.TableName + ".xml");
        mySerializer.Serialize(myWriter, table);
      }

    }


    public void GerarScripts() {
      ScriptsTabelas.Clear();
      ScriptsForeignKeys.Clear();
      foreach (Table table in Introspector.Tables) {
        if (EventoAntesGerarScripts != null)
          EventoAntesGerarScripts(ScriptsAGerar.Tabelas, table.TableName);
        DatabaseScript ds = new DatabaseScript();
        ds.ScriptName = table.TableName + ".Tabela.sql";
        ds.ScriptContent = GenerateTableOutput(table);
        //ds.ScriptContent += "\n\n";
        ds.ScriptContent += GerarScriptPrimaryKey(table);
        //ds.ScriptContent += "\n\n";
        ds.ScriptContent += GerarScriptIndices(table);
        ScriptsTabelas.Add(ds);

        EventoAntesGerarScripts(ScriptsAGerar.ForeignKeys, table.TableName);
        DatabaseScript dsFK = new DatabaseScript();
        dsFK.ScriptName = table.TableName + ".ForeignKeys.sql";
        dsFK.ScriptContent = GerarScriptForeignKey(table);
        ScriptsForeignKeys.Add(dsFK);
      }

      foreach (string s in Introspector.DataTables) {
        if (EventoAntesGerarScripts != null)
          EventoAntesGerarScripts(ScriptsAGerar.DadosIniciais, s);

        if (!Directory.Exists(ScriptsRootDir + "DadosIniciais"))
          Directory.CreateDirectory(ScriptsRootDir + "DadosIniciais");

        if (!Directory.Exists(ScriptsRootDir + "DadosIniciais\\XML"))
          Directory.CreateDirectory(ScriptsRootDir + "DadosIniciais\\XML");

        Table t = Introspector.FindTable(s, true);
        if (t == null)
          throw new Exception("Não foi encontrada tabela " + s);
        DatabaseScript ds = new DatabaseScript();
        ds.ScriptName = t.TableName + ".DadosIniciais.sql";
        ds.ScriptContent = "";
        DataSet DatasetDados = (DataSet)Introspector.DadosIniciais[Introspector.DataTables.IndexOf(s)];
        if (DatasetDados.Tables[0].Rows.Count > 0) {
          if (EventoAntesGerarDadosIniciais != null)
            EventoAntesGerarDadosIniciais(t, DatasetDados);
          ds.ScriptContent += GerarScriptDadosIniciaisInicioScript(t);
          foreach (DataRow r in DatasetDados.Tables[0].Rows) {
            if (EventoAntesGerarLinhaDadoInicial != null)
              EventoAntesGerarLinhaDadoInicial(t, r);
            ds.ScriptContent += GerarScriptDadosIniciaisLinha(t, r);
          }
          ds.ScriptContent += GerarScriptDadosIniciaisFimScript(t);
          ScriptsDadosIniciais.Add(ds);
        }

        FileStream fs = new FileStream(ScriptsRootDir + "DadosIniciais\\XML\\" + s + ".xml", FileMode.CreateNew, FileAccess.Write, FileShare.None);
        DatasetDados.WriteXml(fs, System.Data.XmlWriteMode.WriteSchema);

      }

      foreach (Procedure p in Introspector.Procedures) {
        if (EventoAntesGerarScripts != null)
          EventoAntesGerarScripts(ScriptsAGerar.Procedures, p.Name);
        DatabaseScript ds = new DatabaseScript();
        ds.ScriptName = p.Name + ".prc";
        ds.ScriptContent = GerarScriptProcedure(p);
        ScriptsProcedures.Add(ds);
      }

      foreach (Function f in Introspector.Functions) {
        if (EventoAntesGerarScripts != null)
          EventoAntesGerarScripts(ScriptsAGerar.Functions, f.Name);
        DatabaseScript ds = new DatabaseScript();
        ds.ScriptName = f.Name + ".udf";
        ds.ScriptContent = GerarScriptFunction(f);
        ScriptsFunctions.Add(ds);
      }

      foreach (Trigger t in Introspector.Triggers) {
        if (EventoAntesGerarScripts != null)
          EventoAntesGerarScripts(ScriptsAGerar.Triggers, t.Table.TableName + "." + t.Name);
        DatabaseScript ds = new DatabaseScript();
        ds.ScriptName = t.Table.TableName + "." + t.Name + ".trg";
        ds.ScriptContent = GerarScriptTrigger(t);
        ScriptsTriggers.Add(ds);
      }

      foreach (View v in Introspector.Views) {
        if (EventoAntesGerarScripts != null)
          EventoAntesGerarScripts(ScriptsAGerar.Views, v.Name);
        DatabaseScript ds = new DatabaseScript();
        ds.ScriptName = v.Name + ".viw";
        ds.ScriptContent = GerarScriptView(v);
        ScriptsViews.Add(ds);
      }

      foreach (Sequence s in Introspector.Sequences) {
        if (EventoAntesGerarScripts != null)
          EventoAntesGerarScripts(ScriptsAGerar.Sequences, s.SequenceName);
        DatabaseScript ds = new DatabaseScript();
        ds.ScriptName = s.SequenceName + ".seq";
        ds.ScriptContent = GerarScriptSequence(s);
        ScriptsSequences.Add(ds);
      }
    }


    public void SalvarScriptsTabelasSQL(System.Text.Encoding AEncoding) {
      if (ScriptsRootDir == String.Empty)
        throw new Exception("ScriptsRootDir não informado");
      if (!Directory.Exists(ScriptsRootDir))
        Directory.CreateDirectory(ScriptsRootDir);

      if (!Directory.Exists(ScriptsRootDir + "Tabelas"))
        Directory.CreateDirectory(ScriptsRootDir + "Tabelas");
      foreach (DatabaseScript ds in ScriptsTabelas) {
        if (EventoAntesSalvarScripts != null)
          EventoAntesSalvarScripts(ds.ScriptName);
        StreamWriter sw = new StreamWriter(ScriptsRootDir + "Tabelas\\" + ds.ScriptName, false, AEncoding);
        sw.Write(ds.ScriptContent);
        sw.Close();
      }

      if (!Directory.Exists(ScriptsRootDir + "ForeignKeys"))
        Directory.CreateDirectory(ScriptsRootDir + "ForeignKeys");
      foreach (DatabaseScript ds in ScriptsForeignKeys) {
        if (EventoAntesSalvarScripts != null)
          EventoAntesSalvarScripts(ds.ScriptName);
        if (ds.ScriptContent.Trim() != String.Empty) {
          StreamWriter sw = new StreamWriter(ScriptsRootDir + "ForeignKeys\\" + ds.ScriptName);
          sw.Write(ds.ScriptContent);
          sw.Close();
        }
      }
    }


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

  }
}
