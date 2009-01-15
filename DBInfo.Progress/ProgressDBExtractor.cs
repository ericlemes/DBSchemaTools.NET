using System;
using System.Data;
using System.Data.Odbc;
using DBInfo.Core.Extractor;

namespace DBInfo.DBExtractors {
  public class ProgressDBExtractor : IDBInfoExtractor {
    public OdbcConnection Connection;

    public ProgressDBExtractor() {
      Connection = new OdbcConnection();
    }

    public void Abrir() {
      Connection.Open();
    }

    public void Fechar() {

    }

    public DataSet getCheckConstraints(string ATabela) {
      return null;
    }

    private string QuotedString(string APlainString) {
      return "'" + APlainString.Replace("'", "''") + "'";
    }

    private int GetFieldType(string ATabela, string AFieldName, string AFieldType) {
      switch (AFieldType) {
        case "bit": return 9;
        case "date": return 10;
        case "integer": return 1;
        case "numeric": return 13;
        case "varchar": return 2;
        case "character": return 3;
        case "logical": return 9;
        case "decimal": return 13;
        case "recid": return 19;
        default: throw new Exception("Tipo de dados não suportado: " + ATabela + "." + AFieldName + "." + AFieldType);
      }
    }

    public DataSet getTableColumns(string ATabela) {
      OdbcCommand qry = new OdbcCommand(
        "select " +
        "  fi.\"_Field-Name\", " +
        "  fi.\"_Data-Type\", " +
        "  fi.\"_Width\", " +
        "  fi.\"_Decimals\", " +
        "  fi.\"_Mandatory\", " +
        "  fi.\"_Initial\", " +
        "  fi.\"_Desc\", " +
        "  fi.\"_Format\", " +
        "  fi.\"_Label\", " +
        "  fi.\"_Field-Physpos\", " +
        "  fi.\"_Help\", " +
        "  fi.\"_Order\", " +
        "  fi.\"_Valexp\", " +
        "  fi.\"_Valmsg\", " +
        "  fi.\"_Decimals\" " +
        "from " +
        "  pub.\"_Field\" fi " +
        "where " +
        "  fi.\"_File-recid\" = (select rowid from pub.\"_File\" where \"_File-Name\" = " + QuotedString(ATabela) + ")" +
        "order by " +
        "  fi.\"_Field-Physpos\"", Connection);
      OdbcDataAdapter dat = new OdbcDataAdapter();
      DataSet ds = new DataSet();
      dat.SelectCommand = qry;
      dat.Fill(ds);

      DataSet dsretorno = new DataSet();
      DataTable dt = new DataTable();
      dt.Columns.Add(new DataColumn("FieldName", typeof(string)));
      dt.Columns.Add(new DataColumn("Type", typeof(int)));
      dt.Columns.Add(new DataColumn("Precision", typeof(int)));
      dt.Columns.Add(new DataColumn("Scale", typeof(int)));
      dt.Columns.Add(new DataColumn("IsNull", typeof(int)));
      dt.Columns.Add(new DataColumn("Identity", typeof(int)));
      dt.Columns.Add(new DataColumn("IdentityCol", typeof(int)));
      dt.Columns.Add(new DataColumn("Default", typeof(string)));
      dt.Columns.Add(new DataColumn("DefaultConstraintName", typeof(string)));
      dt.Columns.Add(new DataColumn("Description", typeof(string)));
      dt.Columns.Add(new DataColumn("Format", typeof(string)));
      dt.Columns.Add(new DataColumn("Label", typeof(string)));
      dt.Columns.Add(new DataColumn("Position", typeof(int)));
      dt.Columns.Add(new DataColumn("Help", typeof(string)));
      dt.Columns.Add(new DataColumn("Ordem", typeof(int)));
      dt.Columns.Add(new DataColumn("Valexp", typeof(string)));
      dt.Columns.Add(new DataColumn("Valmsg", typeof(string)));
      dt.Columns.Add(new DataColumn("Decimals", typeof(int)));
      dsretorno.Tables.Add(dt);
      foreach (DataRow r in ds.Tables[0].Rows) {
        DataRow newrow = dt.NewRow();
        newrow["FieldName"] = r[0];
        newrow["Type"] = GetFieldType(ATabela, (string)r[0], (string)r[1]);
        newrow["Precision"] = r[2];
        newrow["Scale"] = r[3];
        if (newrow["Scale"] == DBNull.Value)
          newrow["Scale"] = 0;
        if ((bool)r[4])
          newrow["IsNull"] = 0;
        else
          newrow["IsNull"] = 1;
        newrow["Default"] = r[5];
        if (newrow["Default"] == DBNull.Value)
          newrow["Default"] = String.Empty;
        newrow["Identity"] = 0;
        newrow["IdentityCol"] = 0;
        newrow["DefaultConstraintName"] = String.Empty;
        newrow["Description"] = r[6];
        newrow["Format"] = r[7];
        newrow["Label"] = r[8];
        newrow["Position"] = r[9];
        newrow["Help"] = r[10];
        newrow["Ordem"] = r[11];
        newrow["Valexp"] = r[12];
        newrow["Valmsg"] = r[13];
        newrow["Decimals"] = r[14];
        dt.Rows.Add(newrow);
      }

      return dsretorno;
    }

    public DataSet getIndexColumns(string ATabela, string AIndice) {
      OdbcCommand qry = new OdbcCommand(
        "select colname, idxorder from sysindexes where tbl = " + QuotedString(ATabela) + " and idxname = " + QuotedString(AIndice) + " order by idxseq", Connection);
      OdbcDataAdapter dat = new OdbcDataAdapter();
      DataSet ds = new DataSet();
      dat.SelectCommand = qry;
      dat.Fill(ds);

      DataSet dsretorno = new DataSet();
      DataTable dt = new DataTable();
      dt.Columns.Add(new DataColumn("ColName", typeof(string)));
      dt.Columns.Add(new DataColumn("ColOrder", typeof(int)));
      dsretorno.Tables.Add(dt);
      foreach (DataRow r in ds.Tables[0].Rows) {
        DataRow newrow = dt.NewRow();
        newrow[0] = r[0];
        if ((string)r[1] == "A")
          newrow[1] = 0;
        else
          newrow[1] = 1;
        dt.Rows.Add(newrow);
      }
      return dsretorno;
    }

    public DataSet getTableData(string ATabela) {
      return null;
    }

    public DataSet getForeignKeys(string ATabela) {
      return null;
    }

    public DataSet getForeignKeyColumns(string AForeignKey) {
      return null;
    }

    public DataSet getFunctions() {
      return null;
    }

    public DataSet getFunctionText(string AFunction) {
      return null;
    }

    private bool GetPrimaryIndex(string ATabela, string AIndex) {
      OdbcCommand qry = new OdbcCommand(
        "select 1 " +
        "from " +
        "  pub.\"_File\" f " +
        "where " +
        "  f.\"_File-Name\" = " + QuotedString(ATabela) + " and " +
        "  f.\"_Prime-index\" = (" +
        "    select rowid " +
        "    from pub.\"_Index\" " +
        "    where " +
        "      \"_Index-Name\" = " + QuotedString(AIndex) + " and " +
        "      \"_File-recid\" = f.rowid )",
        Connection);
      OdbcDataAdapter dat = new OdbcDataAdapter();
      DataSet ds = new DataSet();
      dat.SelectCommand = qry;
      dat.Fill(ds);
      return ds.Tables[0].Rows.Count > 0;
    }

    public DataSet getIndexes(string ATabela) {
      OdbcCommand qry = new OdbcCommand(
        "select " +
        "  i.\"_Index-Name\", " +
        "  i.\"_Unique\", " +
        "  a.\"_Area-name\" " +
        "from " +
        "  pub.\"_Index\" i " +
        "    inner join pub.\"_Area\" a on" +
        "      a.\"_Area-number\" = i.\"_ianum\" " +
        "where " +
        "  i.\"_Index-Name\" <> 'default' and " +
        "  i.\"_File-recid\" = (select rowid from pub.\"_File\" where \"_File-Name\" = " + QuotedString(ATabela) + ")",
        Connection);
      OdbcDataAdapter dat = new OdbcDataAdapter();
      DataSet ds = new DataSet();
      dat.SelectCommand = qry;
      dat.Fill(ds);

      DataSet dsretorno = new DataSet();
      DataTable dt = new DataTable();
      dt.Columns.Add(new DataColumn("IndexName", typeof(string)));
      dt.Columns.Add(new DataColumn("Unique", typeof(int)));
      dt.Columns.Add(new DataColumn("Area", typeof(string)));
      dt.Columns.Add(new DataColumn("Primary", typeof(bool)));
      dsretorno.Tables.Add(dt);
      foreach (DataRow r in ds.Tables[0].Rows) {
        DataRow newrow = dt.NewRow();
        newrow["IndexName"] = r[0];
        if ((bool)r[1])
          newrow["Unique"] = 1;
        else
          newrow["Unique"] = 0;
        newrow["Area"] = r[2];
        newrow["Primary"] = GetPrimaryIndex(ATabela, (string)r[0]);
        dt.Rows.Add(newrow);
      }

      return dsretorno;
    }

    public DataSet getPrimaryKey(string ATabela) {
      return null;
    }

    public DataSet getPrimaryKeyColumns(string ATabela, string APrimaryKeyName) {
      return null;
    }

    public DataSet getProcedures() {
      return null;
    }

    public DataSet getProcedureText(string AProcedure) {
      return null;
    }

    public DataSet getTables() {
      OdbcCommand qry = new OdbcCommand(
        "select " +
        "  f.\"_File-Name\", " +
        "  0, " +
        "  0, " +
        "  0, " +
        "  a.\"_Area-Name\", " +
        "  f.\"_Desc\", " +
        "  f.\"_Dump-name\", " +
        "  f.\"_File-label\", " +
        "  f.\"_Valexp\", " +
        "  f.\"_Valmsg\", " +
        "  f.\"_For-Name\" " +
        "from " +
        "  pub.\"_File\" f " +
        "    inner join pub.\"_Area\" a on " +
        "      a.\"_Area-number\" = f.\"_ianum\" " +
        "where " +
        "  f.\"_Owner\" = 'pub' and " +
        "  \"_file-number\" > 0"
        , Connection);
      OdbcDataAdapter dat = new OdbcDataAdapter();
      DataSet ds = new DataSet();
      dat.SelectCommand = qry;
      dat.Fill(ds);
      return ds;
    }

    public DataSet getTriggers(string ATabela) {
      return null;
    }

    public DataSet getTriggerText(string ATrigger) {
      return null;
    }

    public DataSet getViews() {
      return null;
    }

    public DataSet getViewText(string AView) {
      return null;
    }

    public DataSet getSequences() {
      OdbcCommand qry = new OdbcCommand(
        "select " +
        "  s.\"_Seq-Name\", " +
        "  s.\"_Seq-Init\", " +
        "  s.\"_Seq-Min\", " +
        "  s.\"_Seq-Max\", " +
        "  s.\"_Seq-Incr\", " +
        "  s.\"_Cycle-Ok\" " +
        "from " +
        "  pub.\"_Sequence\" s " +
        "order by " +
        "  s.\"_Seq-Num\" ", Connection);
      OdbcDataAdapter dat = new OdbcDataAdapter();
      DataSet ds = new DataSet();
      dat.SelectCommand = qry;
      dat.Fill(ds);
      return ds;
    }

    public DataSet getTableTriggers(string ATabela) {
      OdbcCommand qry = new OdbcCommand(
        "select " +
        "  t.\"_Event\", " +
        "  t.\"_Proc-Name\", " +
        "  t.\"_Override\", " +
        "  t.\"_Trig-Crc\" " +
        "from " +
        "  pub.\"_File-trig\" t " +
        "where " +
        "  t.\"_File-Recid\" = (select rowid from pub.\"_File\" where \"_File-Name\" = " + QuotedString(ATabela) + ")"
        , Connection);
      OdbcDataAdapter dat = new OdbcDataAdapter();
      DataSet ds = new DataSet();
      dat.SelectCommand = qry;
      dat.Fill(ds);
      return ds;
    }


  }
}
