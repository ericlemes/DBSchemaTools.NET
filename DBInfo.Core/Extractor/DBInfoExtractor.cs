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
    
    private List<Table> _Tables = new List<Table>();
    public List<Table> Tables{
      get { return _Tables;}
      set { _Tables = value;}
    }
    
    private List<string> _TableNames = new List<string>();
    public List<string> TableNames{
      get { return _TableNames;}
      set { _TableNames = value;}
    }
        
    private List<DataSet> _TableData = new List<DataSet>();
    public List<DataSet> TableData{
      get { return _TableData;}
      set { _TableData = value;}
    }
    
    private List<Procedure> _Procedures = new List<Procedure>();
    public List<Procedure> Procedures{
      get { return _Procedures;}
      set { _Procedures = value;}
    }
    
    private List<Function> _Functions = new List<Function>();    
    public List<Function> Functions{
      get { return _Functions;}
      set { _Functions = value;}
    }
    
    private List<Trigger> _Triggers = new List<Trigger>();
    public List<Trigger> Triggers{
      get { return _Triggers;}
      set { _Triggers = value;}
    }
    
    private List<View> _Views;    
    public List<View> Views{
      get { return _Views;}
      set { _Views= value;}
    }
    
    private List<Sequence> _Sequences;
    public List<Sequence> Sequences{
      get { return _Sequences;}
      set { _Sequences = value;}
    }

    public enum DataToRead { Tables, CheckConstraints, Columns, PrimaryKey, Indexes, ForeignKeys, Procedures, Functions, TableData, Triggers, Views, Sequences, TableTriggers };

    public delegate void AntesLerDadosBancoHandler(DataToRead ADados, string AObjeto);
    public event AntesLerDadosBancoHandler EventoAntesLerDadosBanco;

    public Table FindTable(string ATableName, bool ACaseInsensitive) {
      Table TmpTable = null;
      foreach (Table Table in Tables) {
        if (ACaseInsensitive) {
          if (Table.TableName.ToUpper() == ATableName.ToUpper()) {
            TmpTable = Table;
            break;
          }
        } else {
          if (Table.TableName == ATableName) {
            TmpTable = Table;
            break;
          }
        }
      }
      return TmpTable;
    }

    public Column FindColumn(string ATableName, string AColumnName) {
      Column TmpColumn = null;
      Table TmpTable = FindTable(ATableName, false);
      if (TmpTable == null)
        return null;
      foreach (Column col in TmpTable.Columns) {
        if (col.Name == AColumnName) {
          TmpColumn = col;
          break;
        }
      }
      return TmpColumn;
    }    

    private int GetInteger(object AValue) {
      return AValue == DBNull.Value ? 0 : (int)AValue;
    }

    private bool GetBoolean(object AValue) {
      return AValue == DBNull.Value ? false : (bool)AValue;
    }

    private void ReadTables() {
      Tables = _Extractor.GetTables();
      if (Tables == null)
        throw new Exception("The IDBExtractor GetTables method mustn't return null");

      if (EventoAntesLerDadosBanco != null)
        EventoAntesLerDadosBanco(DataToRead.Tables, "");      

      foreach (Table Table in Tables) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DataToRead.Columns, Table.TableName);

        Extractor.GetTableColumns(Table);

        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DataToRead.TableTriggers, Table.TableName);
        
      }
    }

    private string GetString(object AValue) {
      return AValue == DBNull.Value ? String.Empty : (string)AValue;
    }

    private void ReadPrimaryKeys() {
      foreach (Table Table in Tables) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DataToRead.PrimaryKey, Table.TableName);

        DataSet PKDataset;
        PKDataset = Extractor.getPrimaryKey(Table.TableName);

        if (PKDataset == null)
          return;

        if (PKDataset.Tables[0].Rows.Count > 0) {
          Table.PrimaryKeyName = Convert.ToString(PKDataset.Tables[0].Rows[0][0]);
          DataSet PKColsDataset = Extractor.getPrimaryKeyColumns(Table.TableName, Table.PrimaryKeyName);
          foreach (DataRow r in PKColsDataset.Tables[0].Rows) {
            Column c = Table.FindColumn((string)r[0]);
            if (c == null)
              throw new Exception("Não foi localizada a coluna " + (string)r[0] + " da primary key da tabela " + Table.TableName);
            Table.PrimaryKeyColumns.Add(c);
          }
        }
      }
    }

    private void ReadCheckConstraints() {
      foreach (Table Table in Tables) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DataToRead.CheckConstraints, Table.TableName);

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

    private void ReadIndexes() {
      foreach (Table t in Tables) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DataToRead.Indexes, t.TableName);
        
        Extractor.GetIndexes(t);        

        foreach (Index i in t.Indexes) {
          Extractor.getIndexColumns(t, i);          
        }
      }
    }

    private void SetValuesTableForeignKey(Table tborigem, Table tbdestino) {
      tbdestino.TableName = tborigem.TableName;
      tbdestino.HasIdentity = tborigem.HasIdentity;
      tbdestino.IdentitySeed = tborigem.IdentitySeed;
      tbdestino.IdentityIncrement = tborigem.IdentityIncrement;
    }

    private void ReadForeignKeys() {
      foreach (Table Table in Tables) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DataToRead.ForeignKeys, Table.TableName);

        DataSet FKsDataset = Extractor.getForeignKeys(Table.TableName);

        if (FKsDataset == null)
          return;

        DataSet FKColumnsDataset;
        foreach (DataRow fkrow in FKsDataset.Tables[0].Rows) {
          ForeignKey fk = new ForeignKey();
          fk.ForeignKeyName = (string)fkrow[0];
          fk.RefTableName = (string)fkrow[1];
          fk.UpdateCascade = Convert.ToBoolean(fkrow[2]);
          fk.DeleteCascade = Convert.ToBoolean(fkrow[3]);
          Table.ForeignKeys.Add(fk);


          FKColumnsDataset = Extractor.getForeignKeyColumns(fk.ForeignKeyName);
          foreach (DataRow colrow in FKColumnsDataset.Tables[0].Rows) {
            ForeignKeyColumn col = new ForeignKeyColumn();
            col.RefTable = new Table();
            col.Column = FindColumn(Table.TableName, (string)colrow[0]);
            col.RefColumn = FindColumn(fk.RefTableName, (string)colrow[1]);
            SetValuesTableForeignKey(FindTable(fk.RefTableName, false), col.RefTable);
            fk.Columns.Add(col);
          }
        }
      }
    }

    private void ReadTableData() {
      foreach (string s in TableNames) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DataToRead.TableData, s);
        DataSet dsDados = Extractor.getTableData(s);
        TableData.Add(dsDados);
      }
    }

    private void ReadProcedures() {
      DataSet dsProcedures = Extractor.getProcedures();

      if (dsProcedures == null)
        return;

      foreach (DataRow r in dsProcedures.Tables[0].Rows) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DataToRead.Procedures, (string)r[0]);

        DataSet dsProc = Extractor.getProcedureText((string)r[0]);

        if (dsProc == null)
          return;

        if (dsProc.Tables.Count > 0) {
          if (dsProc.Tables[0].Rows.Count <= 0)
            throw new Exception("Não foi possível localizar o texto da procedure " + (string)r[0]);
          Procedure p = new Procedure();
          p.Name = (string)r[0];
          p.Body = "";
          foreach (DataRow r2 in dsProc.Tables[0].Rows) {
            p.Body += (string)r2[0];
          }
          Procedures.Add(p);
        }
      }
    }

    private void ReadFunctions() {
      DataSet dsFunctions = _Extractor.getFunctions();

      if (dsFunctions == null)
        return;

      foreach (DataRow r in dsFunctions.Tables[0].Rows) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DataToRead.Functions, (string)r[0]);

        DataSet dsFunction = _Extractor.getFunctionText((string)r[0]);

        if (dsFunction == null)
          return;

        if (dsFunction.Tables.Count > 0) {
          if (dsFunction.Tables[0].Rows.Count <= 0)
            throw new Exception("Não foi possível localizar o texto da procedure " + (string)r[0]);
          Function f = new Function();
          f.Name = (string)r[0];
          f.Body = "";
          foreach (DataRow r2 in dsFunction.Tables[0].Rows) {
            f.Body += (string)r2[0];
          }
          Functions.Add(f);
        }
      }
    }

    private void ReadTriggers() {
      foreach (Table t in Tables) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DataToRead.Triggers, t.TableName);

        DataSet dsTriggers = _Extractor.getTriggers(t.TableName);

        if (dsTriggers == null)
          return;

        foreach (DataRow r in dsTriggers.Tables[0].Rows) {
          Trigger tr = new Trigger();
          tr.Name = (string)r[0];
          tr.Table = t;
          tr.Body = "";
          DataSet dsTriggerBody = _Extractor.getTriggerText(tr.Name);
          if (dsTriggerBody.Tables.Count > 0) {
            foreach (DataRow r2 in dsTriggerBody.Tables[0].Rows) {
              tr.Body += r2[0];
            }
            Triggers.Add(tr);
          }
        }
      }
    }

    private void ReadViews() {
      DataSet dsViews = _Extractor.getViews();

      if (dsViews == null)
        return;

      foreach (DataRow dr in dsViews.Tables[0].Rows) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DataToRead.Views, (string)dr[0]);

        View v = new View();
        v.Name = (string)dr[0];
        v.Body = "";
        DataSet dsViewBody = _Extractor.getViewText(v.Name);
        if (dsViewBody.Tables.Count > 0) {
          foreach (DataRow r2 in dsViewBody.Tables[0].Rows) {
            v.Body += (string)r2[0];
          }
        }
        Views.Add(v);
      }
    }

    private void ReadSequences() {
      DataSet dsSequences = _Extractor.getSequences();

      if (dsSequences == null)
        return;

      foreach (DataRow dr in dsSequences.Tables[0].Rows) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DataToRead.Sequences, (string)dr[0]);

        Sequence s = new Sequence();
        s.SequenceName = GetString(dr[0]);
        s.Initial = GetInteger(dr[1]);
        s.MinValue = GetInteger(dr[2]);
        s.MaxValue = dr[3] == DBNull.Value ? -1 : (int)dr[3];
        s.Increment = GetInteger(dr[4]);
        s.CycleOnLimit = GetBoolean(dr[5]);
        Sequences.Add(s);
      }
    }

    public void DeserializeXML(string path) {
      System.Xml.XmlDocument xm = new System.Xml.XmlDocument();
      System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(Table));
      string[] arquivos = System.IO.Directory.GetFiles(path);
      Tables.Clear();

      foreach (string arquivo in arquivos) {
        string[] strs = arquivo.Split('.');
        if ((strs.Length > 0) && (strs[strs.Length - 1].ToLower() == "xml")) {
          System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(arquivo);
          Table tbl = (Table)ser.Deserialize(reader);
          reader.Close();
          Tables.Add(tbl);
        }
      }

    }

    public void Extract() {
      if (_InputType == InputOutputType.File && !Directory.Exists(_InputDir))
        throw new Exception(String.Format("The input directory don't exists: {0}", _InputDir));

      if (_InputType == InputOutputType.Database && String.IsNullOrEmpty(_InputConnectionString))
        throw new Exception(String.Format("The input connection string don't exists: {0}", _InputConnectionString));        
      
      _Extractor.InputType = _InputType;
      _Extractor.InputConnectionString = _InputConnectionString;
      _Extractor.InputDir = _InputDir;
      
    
      _Extractor.Open();
      try {
        ReadTables();
        ReadPrimaryKeys();
        ReadForeignKeys();
        ReadCheckConstraints();
        ReadIndexes();

        ReadFunctions();
        ReadProcedures();
        ReadTriggers();
        ReadViews();
        ReadSequences();
      } finally {
        _Extractor.Close();
      }
    }
  }
}
