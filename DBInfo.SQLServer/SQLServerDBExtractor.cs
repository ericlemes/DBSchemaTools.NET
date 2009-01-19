using System;
using System.Data;
using System.Data.SqlClient;
using DBInfo.Core;
using DBInfo.Core.Extractor;

namespace DBInfo.DBExtractors {
  public class SQLServerDBExtractor : IDBInfoExtractor {
    private InputOutputType _InputType;
    public InputOutputType InputType {
      get { return _InputType; }
      set { _InputType = value; }
    }

    private string _InputConnectionString;
    public string InputConnectionString {
      get { return _InputConnectionString; }
      set { _InputConnectionString = value; }
    }

    private string _InputDir;
    public string InputDir {
      get { return _InputDir; }
      set { _InputDir = value; }
    }  
  
    public SqlConnection SqlConn;

    public SQLServerDBExtractor() {
      SqlConn = new SqlConnection();
    }

    public void Open() {
      SqlConn.Open();
    }

    public void Close() {
      SqlConn.Close();
    }

    public DataSet getTables() {
      SqlCommand qry = new SqlCommand("select name, case when ident_seed(name) is null then 0 else 1 end HasIdentity, ident_seed(name) IdentSeed, ident_incr(name) IdentIncr from sysobjects where xtype = 'U' and uid = 1 and status >= 0 order by name", SqlConn);
      SqlDataAdapter dat = new SqlDataAdapter();
      DataSet ds = new DataSet();
      dat.SelectCommand = qry;
      dat.Fill(ds);
      return ds;
    }

    public DataSet getTableColumns(string ATabela) {
      SqlCommand qry = new SqlCommand(
          "select " +
          "  c.name, " +
          "  case " +
          "    when c.xtype = 56 then 1 " +
          "    when c.xtype = 231 then 18 " +
          "    when c.xtype = 167 then 2 " +
          "    when c.xtype = 175 then 3 " +
          "    when c.xtype = 35 then 6 " +
          "    when c.xtype = 99 then 6 " +
          "    when c.xtype = 34 then 7 " +
          "    when c.xtype = 61 then 8 " +
          "    when c.xtype = 104 then 9 " +
          "    when c.xtype = 106 then 4 " +
          "    when c.xtype = 58 then 10 " +
          "    when c.xtype = 60 then 11 " +
          "    when c.xtype = 62 then 5 " +
          "    when c.xtype = 52 then 12 " +
          "    when c.xtype = 108 then 13 " +
          "    when c.xtype = 36 then 14 " +
          "    when c.xtype = 127 then 15 " +
          "    when c.xtype = 48 then 16 " +
          "    when c.xtype = 173 then 17 " +
          "    else -1 " +
          "  end type, " +
          "  length, " +
      "  c.xprec prec, " +
          "  c.xscale scale, " +
          "  c.isnullable isnull, " +
          "  case " +
          "    when c.status & 128 = 128 then 1 " +
          "    else 0 " +
          "  end IdentityColumn, " +
          "  case " +
          "    when c.cdefault > 0 then d.text " +
          "    else '' " +
          "  end DefaultValue, " +
      "  isnull(df.name, '') ConstraintDefaultName " +
          "from " +
          "  syscolumns c " +
          "    inner join sysobjects o on " +
          "      o.id = c.id " +
          "    left outer join syscomments d on " +
          "      d.id = c.cdefault " +
      "    left join sysobjects df on " +
      "      df.id = c.cdefault " +
          "where " +
          "  o.name = @Tabela and " +
          "  o.uid = 1 " +
          "order by " +
          "  c.colorder ", SqlConn);

      SqlDataAdapter dat = new SqlDataAdapter();
      DataSet ds = new DataSet();

      qry.Parameters.Add("@Tabela", SqlDbType.VarChar).Value = ATabela;
      qry.Parameters.Add("@Tabela2", SqlDbType.VarChar).Value = ATabela;
      dat.SelectCommand = qry;
      dat.Fill(ds);
      return ds;
    }

    public DataSet getForeignKeys(string ATabela) {
      SqlCommand qry = new SqlCommand(
        "select " +
        "  const.name, " +
        "  constraintreference.name, " +
        "  case " +
        "    when rc.update_rule = 'CASCADE' then 1 " +
        "    else 0 " +
        "  end UpdateCascade, " +
        "  case " +
        "    when rc.delete_rule = 'CASCADE' then 1 " +
        "    else 0 " +
        "  end DeleteCascade " +
        "from " +
        "  sysconstraints c " +
        "    inner join sysobjects const on " +
        "      const.id = c.constid " +
        "    inner join sysobjects ownertable on " +
        "      ownertable.id = const.parent_obj " +
        "    inner join sysreferences r on " +
        "        r.constid = c.constid " +
        "    inner join sysobjects constraintreference on " +
        "        constraintreference.id = r.rkeyid " +
        "    inner join information_schema.referential_constraints rc on " +
        "      rc.constraint_schema = 'dbo' and " +
        "      rc.constraint_name = const.name " +
        "where " +
        "  const.xtype = 'F' and " +
        "  const.uid = 1 and " +
        "  ownertable.name = @tablename", SqlConn);
      qry.Parameters.Add("@tablename", SqlDbType.VarChar).Value = ATabela;
      SqlDataAdapter dat = new SqlDataAdapter();
      dat.SelectCommand = qry;
      DataSet ds = new DataSet();
      dat.Fill(ds);
      return ds;
    }

    public DataSet getForeignKeyColumns(string AForeignKey) {
      SqlCommand qry = new SqlCommand(
        "select " +
        "  origincolumn.name, " +
        "  destinationcolumn.name " +
        "from " +
        "  sysforeignkeys fks " +
        "    inner join sysobjects fk on " +
        "      fk.id = fks.constid and " +
        "      fk.xtype = 'F' " +
        "    inner join syscolumns origincolumn on " +
        "      origincolumn.id = fks.fkeyid and " +
        "      origincolumn.colid = fks.fkey " +
        "    inner join syscolumns destinationcolumn on " +
        "      destinationcolumn.id = fks.rkeyid and " +
        "      destinationcolumn.colid = fks.rkey " +
        "where " +
        "  fk.uid = 1 and " +
        "  fk.name = @foreignkeyname " +
    "order by keyno", SqlConn);
      SqlDataAdapter dat = new SqlDataAdapter();
      DataSet ds = new DataSet();
      dat.SelectCommand = qry;
      qry.Parameters.Add("@foreignkeyname", SqlDbType.VarChar).Value = AForeignKey;
      dat.Fill(ds);
      return ds;
    }

    public DataSet getPrimaryKey(string ATabela) {
      SqlCommand qry = new SqlCommand(
        "select " +
        "  constraint_name " +
        "from " +
        "  information_schema.table_constraints " +
        "where " +
        "  table_name = @TableName and " +
        "  constraint_type = 'PRIMARY KEY' ", SqlConn);
      SqlDataAdapter dat = new SqlDataAdapter();
      DataSet ds = new DataSet();
      dat.SelectCommand = qry;
      qry.Parameters.Add("@TableName", SqlDbType.VarChar).Value = ATabela;
      dat.Fill(ds);
      return ds;
    }

    public DataSet getPrimaryKeyColumns(string ATabela, string APrimaryKeyName) {
      SqlCommand qry = new SqlCommand(
        "select " +
        "  column_name " +
        "from " +
        "  information_schema.key_column_usage " +
        "where " +
        "  table_name = @TableName and " +
        "  constraint_name = @PKName and " +
        "  constraint_schema = 'dbo' " +
        "order by " +
        "  ordinal_position ", SqlConn);
      SqlDataAdapter dat = new SqlDataAdapter();
      dat.SelectCommand = qry;
      qry.Parameters.Add("@TableName", SqlDbType.VarChar).Value = ATabela;
      qry.Parameters.Add("@PKName", SqlDbType.VarChar).Value = APrimaryKeyName;
      DataSet ds = new DataSet();
      dat.Fill(ds);
      return ds;
    }

    public DataSet getIndexes(string ATabela) {
      SqlCommand qry = new SqlCommand(
        "select " +
        "  i.name, " +
        "  case " +
        "    when i.status & 2 = 2 then 1 " +
        "    else 0 " +
        "  end IsUnique " +
        "from " +
        "  sysindexes i " +
        "    inner join sysobjects o on " +
        "      i.id = o.id " +
        "where " +
        "  o.uid = 1 and " +
        "  o.name = @TableName and " +
        "  (i.status & 2048) = 0 and " + //Despreza índice de PK
        "  i.indid between 2 and 254 " +
        "order by indid ", SqlConn);
      SqlDataAdapter dat = new SqlDataAdapter();
      dat.SelectCommand = qry;
      qry.Parameters.Add("@TableName", SqlDbType.VarChar).Value = ATabela;
      DataSet ds = new DataSet();
      dat.Fill(ds);
      return ds;
    }

    public DataSet getIndexColumns(string ATabela, string AIndice) {
      SqlCommand qry = new SqlCommand(
        "select " +
        "  c.name " +
        "from " +
        "  sysindexkeys k " +
        "    inner join syscolumns c on " +
        "      c.id = k.id and " +
        "      c.colid = k.colid " +
        "    inner join sysobjects o on " +
        "      k.id = o.id " +
        "    inner join sysindexes i on " +
        "      i.id = k.id and " +
        "      i.indid = k.indid  " +
        "where " +
        "  o.uid = 1 and " +
        "  o.name = @TableName and " +
        "  i.name = @IndexName " +
        "order by " +
        "  k.keyno ", SqlConn);
      SqlDataAdapter dat = new SqlDataAdapter();
      dat.SelectCommand = qry;
      qry.Parameters.Add("@TableName", SqlDbType.VarChar).Value = ATabela;
      qry.Parameters.Add("@IndexName", SqlDbType.VarChar).Value = AIndice;
      DataSet ds = new DataSet();
      dat.Fill(ds);
      return ds;
    }

    public DataSet getTableData(string ATabela) {
      SqlCommand qry = new SqlCommand(
        "select * from " + ATabela, SqlConn);
      SqlDataAdapter dat = new SqlDataAdapter();
      dat.SelectCommand = qry;
      DataSet ds = new DataSet();
      dat.Fill(ds);
      return ds;
    }

    public DataSet getProcedures() {
      SqlCommand qry = new SqlCommand(
        "select  " +
        "  routine_name  " +
        "from  " +
        "  information_schema.routines  " +
        "where  " +
        "  routine_type = 'PROCEDURE' and  " +
        "  specific_schema = 'dbo'  " +
        "order by  " +
        "  routine_name ", SqlConn);
      SqlDataAdapter dat = new SqlDataAdapter();
      dat.SelectCommand = qry;
      DataSet ds = new DataSet();
      dat.Fill(ds);
      return ds;
    }

    public DataSet getProcedureText(string AProcedure) {
      SqlCommand qry = new SqlCommand("sp_helptext " + AProcedure, SqlConn);
      SqlDataAdapter dat = new SqlDataAdapter();
      dat.SelectCommand = qry;
      DataSet ds = new DataSet();
      dat.Fill(ds);
      return ds;
    }

    public DataSet getFunctions() {
      SqlCommand qry = new SqlCommand(
        "select  " +
        "  routine_name  " +
        "from  " +
        "  information_schema.routines  " +
        "where  " +
        "  routine_type = 'FUNCTION' and  " +
        "  specific_schema = 'dbo'  " +
        "order by  " +
        "  routine_name ", SqlConn);
      SqlDataAdapter dat = new SqlDataAdapter();
      dat.SelectCommand = qry;
      DataSet ds = new DataSet();
      dat.Fill(ds);
      return ds;
    }

    public DataSet getFunctionText(string AFunction) {
      SqlCommand qry = new SqlCommand("sp_helptext " + AFunction, SqlConn);
      SqlDataAdapter dat = new SqlDataAdapter();
      dat.SelectCommand = qry;
      DataSet ds = new DataSet();
      dat.Fill(ds);
      return ds;
    }

    public DataSet getTriggers(string ATabela) {
      SqlCommand qry = new SqlCommand(
        "select " +
        "  name " +
        "from " +
        "  sysobjects  " +
        "where " +
        "  xtype = 'TR' and " +
        "  uid = 1 and " +
        "  parent_obj = (select id from sysobjects where name = @TableName and uid = 1) " +
        "order by name ", SqlConn);
      qry.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = ATabela;
      SqlDataAdapter dat = new SqlDataAdapter();
      dat.SelectCommand = qry;
      DataSet ds = new DataSet();
      dat.Fill(ds);
      return ds;
    }

    public DataSet getTriggerText(string ATrigger) {
      SqlCommand qry = new SqlCommand("sp_helptext " + ATrigger, SqlConn);
      SqlDataAdapter dat = new SqlDataAdapter();
      dat.SelectCommand = qry;
      DataSet ds = new DataSet();
      dat.Fill(ds);
      return ds;
    }

    public DataSet getViews() {
      SqlCommand qry = new SqlCommand(
        "select " +
        "  table_name " +
        "from " +
        "  information_schema.views " +
        "where " +
        "  table_schema = 'dbo' " +
        "order by " +
        "  table_name ", SqlConn);
      SqlDataAdapter dat = new SqlDataAdapter();
      dat.SelectCommand = qry;
      DataSet ds = new DataSet();
      dat.Fill(ds);
      return ds;
    }

    public DataSet getViewText(string AView) {
      SqlCommand qry = new SqlCommand("sp_helptext " + AView, SqlConn);
      SqlDataAdapter dat = new SqlDataAdapter();
      dat.SelectCommand = qry;
      DataSet ds = new DataSet();
      dat.Fill(ds);
      return ds;
    }

    public DataSet getCheckConstraints(string ATabela) {
      SqlCommand qry = new SqlCommand(
        "select " +
        "  o.name, " +
        "  c.text " +
        "from " +
        "  sysobjects o " +
        "    inner join syscomments c on " +
        "      c.id = o.id " +
        "where " +
        "  o.parent_obj = object_id(@TableName) and " +
        "  o.uid = 1 and " +
        "  o.xtype = 'C' " +
        "order by " +
        "  o.name ", SqlConn);
      SqlDataAdapter dat = new SqlDataAdapter();
      dat.SelectCommand = qry;
      qry.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = ATabela;
      DataSet ds = new DataSet();
      dat.Fill(ds);
      return ds;
    }

    public DataSet getSequences() {
      return null;
    }

    public DataSet getTableTriggers(string ATabela) {
      return null;
    }
  }
}
