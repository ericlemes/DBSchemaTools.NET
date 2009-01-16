using System;
using System.Data;
using System.Data.Common;
using System.Collections;
using DBInfo.Core;
using DBInfo.Core.Model;

namespace DBInfo.Core.Extractor {
  public class DBInfoExtractor {
    public IDBInfoExtractor ConexaoBD;
    public ArrayList Tables;
    public ArrayList DataTables; //Tabelas para extrair dados iniciais
    public ArrayList DadosIniciais;
    public ArrayList Procedures;
    public ArrayList Functions;
    public ArrayList Triggers;
    public ArrayList Views;
    public ArrayList Sequences;
    public string ScriptDadosIniciais;


    public enum DadosALer { Tabelas, CheckConstraints, Colunas, PrimaryKey, Indices, ForeignKeys, Procedures, Functions, DadosIniciais, Triggers, Views, Sequences, TableTriggers };

    public delegate void AntesLerDadosBancoHandler(DadosALer ADados, string AObjeto);
    public event AntesLerDadosBancoHandler EventoAntesLerDadosBanco;

    public DBInfoExtractor() {
      Tables = new ArrayList();
      DataTables = new ArrayList();
      DadosIniciais = new ArrayList();
      Procedures = new ArrayList();
      Functions = new ArrayList();
      Triggers = new ArrayList();
      Views = new ArrayList();
      Sequences = new ArrayList();
    }


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

    private string GetString(object AValue) {
      return AValue == DBNull.Value ? String.Empty : (string)AValue;
    }

    private int GetInteger(object AValue) {
      return AValue == DBNull.Value ? 0 : (int)AValue;
    }

    private bool GetBoolean(object AValue) {
      return AValue == DBNull.Value ? false : (bool)AValue;
    }

    private void LerTabelas() {
      DataSet TablesDataset;
      TablesDataset = ConexaoBD.getTables();
      if (TablesDataset == null)
        return;

      if (EventoAntesLerDadosBanco != null)
        EventoAntesLerDadosBanco(DadosALer.Tabelas, "");

      foreach (DataRow row in TablesDataset.Tables[0].Rows) {        
        Table t = new Table();
        t.TableName = (string)row[0];
        t.HasIdentity = Convert.ToBoolean(row[1]);
        if (t.HasIdentity) {
          t.IdentitySeed = Convert.ToInt32(row[2]);
          t.IdentityIncrement = Convert.ToInt32(row[3]);
        }
        t.Area = GetString(row[4]);
        t.Description = GetString(row[5]);
        t.DumpName = GetString(row[6]);
        t.Label = GetString(row[7]);
        t.ValExp = GetString(row[8]);
        t.ValMsg = GetString(row[9]);
        t.ForeignName = GetString(row[10]);
        Tables.Add(t);
      }

      foreach (Table Table in Tables) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DadosALer.Colunas, Table.TableName);

        DataSet ColumnsDataset = ConexaoBD.getTableColumns(Table.TableName);
        foreach (DataRow row in ColumnsDataset.Tables[0].Rows) {
          Column c = new Column();
          c.Table = Table;
          c.Name = (string)row[0];
          c.Type = (Column.DBColumnType)Convert.ToInt32(row[1]);
          if (((int)c.Type) == -1)
            throw new Exception("Tipo de dados não suportado para a coluna " + Table.TableName + "." + c.Name);
          c.Size = Convert.ToInt32(row[2]);
          c.SqlWidth = Convert.ToInt32(row[2]);
          c.Precision = Convert.ToInt32(row[3]);
          c.Scale = Convert.ToInt32(row[4]);
          c.IsNull = Convert.ToBoolean(row[5]);
          c.IdentityColumn = Convert.ToBoolean(row[6]);
          c.DefaultValue = (string)row[7];
          c.ConstraintDefaultName = (string)row[8];
          c.Description = GetString(row[9]);
          c.Format = GetString(row[10]);
          c.Label = GetString(row[11]);
          c.Position = GetInteger(row[12]);
          c.Help = GetString(row[13]);
          c.Order = GetInteger(row[14]);
          c.ValExp = GetString(row[15]);
          c.ValMsg = GetString(row[16]);
          c.Decimals = row[17] == DBNull.Value ? -1 : (int)row[17];
          Table.Columns.Add(c);
        }

        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DadosALer.TableTriggers, Table.TableName);

        DataSet trg = ConexaoBD.getTableTriggers(Table.TableName);
        if (trg == null)
          continue;

        foreach (DataRow row in trg.Tables[0].Rows) {
          TableTrigger tt = new TableTrigger();
          tt.Event = GetString(row[0]);
          tt.Procedure = GetString(row[1]);
          tt.Override = GetBoolean(row[2]);
          tt.CRC = GetString(row[3]);
          Table.TableTriggers.Add(tt);
        }
      }
    }

    private void LerPrimaryKeys() {
      foreach (Table Table in Tables) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DadosALer.PrimaryKey, Table.TableName);

        DataSet PKDataset;
        PKDataset = ConexaoBD.getPrimaryKey(Table.TableName);

        if (PKDataset == null)
          return;

        if (PKDataset.Tables[0].Rows.Count > 0) {
          Table.PrimaryKeyName = Convert.ToString(PKDataset.Tables[0].Rows[0][0]);
          DataSet PKColsDataset = ConexaoBD.getPrimaryKeyColumns(Table.TableName, Table.PrimaryKeyName);
          foreach (DataRow r in PKColsDataset.Tables[0].Rows) {
            Column c = Table.FindColumn((string)r[0]);
            if (c == null)
              throw new Exception("Não foi localizada a coluna " + (string)r[0] + " da primary key da tabela " + Table.TableName);
            Table.PrimaryKeyColumns.Add(c);
          }
        }
      }
    }

    private void LerCheckConstraints() {
      foreach (Table Table in Tables) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DadosALer.CheckConstraints, Table.TableName);

        DataSet CheckDataset;
        CheckDataset = ConexaoBD.getCheckConstraints(Table.TableName);

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

    private void LerIndices() {
      foreach (Table t in Tables) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DadosALer.Indices, t.TableName);

        DataSet IndexesDataset;
        IndexesDataset = ConexaoBD.getIndexes(t.TableName);

        if (IndexesDataset == null)
          return;

        foreach (DataRow r in IndexesDataset.Tables[0].Rows) {
          Index i = new Index();
          i.IndexName = (string)r[0];
          i.Unique = Convert.ToBoolean(r[1]);
          i.Area = GetString(r[2]);
          i.Primary = GetBoolean(r[3]);
          DataSet IndexColsDataset = ConexaoBD.getIndexColumns(t.TableName, i.IndexName);
          foreach (DataRow r2 in IndexColsDataset.Tables[0].Rows) {
            IndexColumn c = new IndexColumn();
            c.Column = t.FindColumn((string)r2[0]);
            c.Order = ((IndexColumn.EnumOrder)r2[1]);
            if (c == null)
              throw new Exception("Não foi localizada a coluna " + (string)r2[0] + " do índice " + t.TableName + "." + i.IndexName);
            i.Columns.Add(c);
          }
          t.Indexes.Add(i);
        }
      }
    }

    private void AtribuiValoresTabelaFK(Table tborigem, Table tbdestino) {
      tbdestino.TableName = tborigem.TableName;
      tbdestino.HasIdentity = tborigem.HasIdentity;
      tbdestino.IdentitySeed = tborigem.IdentitySeed;
      tbdestino.IdentityIncrement = tborigem.IdentityIncrement;
    }

    private void LerForeignKeys() {
      foreach (Table Table in Tables) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DadosALer.ForeignKeys, Table.TableName);

        DataSet FKsDataset = ConexaoBD.getForeignKeys(Table.TableName);

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


          FKColumnsDataset = ConexaoBD.getForeignKeyColumns(fk.ForeignKeyName);
          foreach (DataRow colrow in FKColumnsDataset.Tables[0].Rows) {
            ForeignKeyColumn col = new ForeignKeyColumn();
            col.RefTable = new Table();
            col.Column = FindColumn(Table.TableName, (string)colrow[0]);
            col.RefColumn = FindColumn(fk.RefTableName, (string)colrow[1]);
            AtribuiValoresTabelaFK(FindTable(fk.RefTableName, false), col.RefTable);
            fk.Columns.Add(col);
          }
        }
      }
    }

    private void LerDadosIniciais() {
      foreach (string s in DataTables) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DadosALer.DadosIniciais, s);
        DataSet dsDados = ConexaoBD.getTableData(s);
        DadosIniciais.Add(dsDados);
      }
    }

    private void LerProcedures() {
      DataSet dsProcedures = ConexaoBD.getProcedures();

      if (dsProcedures == null)
        return;

      foreach (DataRow r in dsProcedures.Tables[0].Rows) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DadosALer.Procedures, (string)r[0]);

        DataSet dsProc = ConexaoBD.getProcedureText((string)r[0]);

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

    private void LerFunctions() {
      DataSet dsFunctions = ConexaoBD.getFunctions();

      if (dsFunctions == null)
        return;

      foreach (DataRow r in dsFunctions.Tables[0].Rows) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DadosALer.Functions, (string)r[0]);

        DataSet dsFunction = ConexaoBD.getFunctionText((string)r[0]);

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

    private void LerTriggers() {
      foreach (Table t in Tables) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DadosALer.Triggers, t.TableName);

        DataSet dsTriggers = ConexaoBD.getTriggers(t.TableName);

        if (dsTriggers == null)
          return;

        foreach (DataRow r in dsTriggers.Tables[0].Rows) {
          Trigger tr = new Trigger();
          tr.Name = (string)r[0];
          tr.Table = t;
          tr.Body = "";
          DataSet dsTriggerBody = ConexaoBD.getTriggerText(tr.Name);
          if (dsTriggerBody.Tables.Count > 0) {
            foreach (DataRow r2 in dsTriggerBody.Tables[0].Rows) {
              tr.Body += r2[0];
            }
            Triggers.Add(tr);
          }
        }
      }
    }

    private void LerViews() {
      DataSet dsViews = ConexaoBD.getViews();

      if (dsViews == null)
        return;

      foreach (DataRow dr in dsViews.Tables[0].Rows) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DadosALer.Views, (string)dr[0]);

        View v = new View();
        v.Name = (string)dr[0];
        v.Body = "";
        DataSet dsViewBody = ConexaoBD.getViewText(v.Name);
        if (dsViewBody.Tables.Count > 0) {
          foreach (DataRow r2 in dsViewBody.Tables[0].Rows) {
            v.Body += (string)r2[0];
          }
        }
        Views.Add(v);
      }
    }

    private void LerSequences() {
      DataSet dsSequences = ConexaoBD.getSequences();

      if (dsSequences == null)
        return;

      foreach (DataRow dr in dsSequences.Tables[0].Rows) {
        if (EventoAntesLerDadosBanco != null)
          EventoAntesLerDadosBanco(DadosALer.Sequences, (string)dr[0]);

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

    public void DeserializarXML(string caminho) {
      System.Xml.XmlDocument xm = new System.Xml.XmlDocument();
      System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(Table));
      string[] arquivos = System.IO.Directory.GetFiles(caminho);
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

    public void IntrospectarBancoDados() {
      ConexaoBD.Abrir();
      try {
        LerTabelas();
        LerPrimaryKeys();
        LerForeignKeys();
        LerCheckConstraints();
        LerIndices();

        LerFunctions();
        LerProcedures();
        LerTriggers();
        LerViews();
        LerSequences();
      } finally {
        ConexaoBD.Fechar();
      }
    }

    public void IntrospectarDadosIniciais() {
      ConexaoBD.Abrir();
      try {
        LerTabelas();
        LerPrimaryKeys();
        LerDadosIniciais();
      } finally {
        ConexaoBD.Fechar();
      }
    }

    public void IntrospectarPrimaryKeysEForeignKeys() {
      ConexaoBD.Abrir();
      try {
        LerTabelas();
        LerPrimaryKeys();
        LerForeignKeys();
      } finally {
        ConexaoBD.Fechar();
      }
    }

  }
}
