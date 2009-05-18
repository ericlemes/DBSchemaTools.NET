using System;
using System.Data;
using System.Data.SqlClient;
using DBInfo.Core;
using DBInfo.Core.Extractor;
using DBInfo.Core.Model;
using System.Collections.Generic;

namespace DBInfo.DBExtractors {
  public class SQLServerDBExtractor : IDatabaseExtractor {
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
      if (_InputType == InputOutputType.File)
        throw new Exception(String.Format("The class {0} don't support file extraction", this.GetType().FullName));
      SqlConn.ConnectionString = InputConnectionString;
      SqlConn.Open();
    }

    public void Close() {
      SqlConn.Close();
    }

    private string GetString(object AValue) {
      return AValue == DBNull.Value ? String.Empty : (string)AValue;
    }

    private int GetInteger(object AValue) {
      return AValue == DBNull.Value ? 0 : (int)AValue;
    }

    public List<Table> GetTables() {
      List<Table> result = new List<Table>();

      SqlCommand qry = new SqlCommand("select name, case when ident_seed(name) is null then 0 else 1 end HasIdentity, ident_seed(name) IdentSeed, ident_incr(name) IdentIncr from sysobjects where xtype = 'U' and uid = 1 and status >= 0 order by name", SqlConn);

      SqlDataAdapter dat = new SqlDataAdapter();
      DataSet ds = new DataSet();
      dat.SelectCommand = qry;
      dat.Fill(ds);

      foreach (DataRow row in ds.Tables[0].Rows) {
        Table t = new Table();
        t.TableName = (string)row[0];
        t.HasIdentity = Convert.ToBoolean(row[1]);
        if (t.HasIdentity) {
          t.IdentitySeed = Convert.ToInt32(row[2]);
          t.IdentityIncrement = Convert.ToInt32(row[3]);
        }
        result.Add(t);
      }

      return result;
    }

    public void GetTableColumns(Database db, Table table) {
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
          "    when c.xtype = 189 then 20 " + 
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
          "    when d.object_id is not null then d.definition " +
          "    else '' " +
          "  end DefaultValue, " +
          "  isnull(d.name, '') ConstraintDefaultName " +
          "from " +
          "  syscolumns c " +
          "    inner join sysobjects o on " +
          "      o.id = c.id " +
          "    left outer join sys.default_constraints d on " +
          "      d.object_id = c.cdefault " +
          "where " +
          "  o.name = @Tabela and " +
          "  o.uid = 1 " +
          "order by " +
          "  c.colorder ", SqlConn);

      SqlDataAdapter dat = new SqlDataAdapter();
      DataSet ds = new DataSet();

      qry.Parameters.Add("@Tabela", SqlDbType.VarChar).Value = table.TableName;
      qry.Parameters.Add("@Tabela2", SqlDbType.VarChar).Value = table.TableName;
      dat.SelectCommand = qry;
      dat.Fill(ds);

      foreach (DataRow row in ds.Tables[0].Rows) {
        Column c = new Column();
        c.Table = table;
        c.Name = (string)row[0];
        c.Type = (Column.DBColumnType)Convert.ToInt32(row[1]);
        if (((int)c.Type) == -1)
          throw new Exception("Tipo de dados não suportado para a coluna " + table.TableName + "." + c.Name);
        c.Size = Convert.ToInt32(row[2]);
        c.Precision = Convert.ToInt32(row[3]);
        c.Scale = Convert.ToInt32(row[4]);
        c.IsNull = Convert.ToBoolean(row[5]);
        c.IdentityColumn = Convert.ToBoolean(row[6]);
        c.DefaultValue = (string)row[7];
        c.ConstraintDefaultName = (string)row[8];
        table.Columns.Add(c);

      }      
    }

    public void GetForeignKeys(Database db, Table table) {
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
      qry.Parameters.Add("@tablename", SqlDbType.VarChar).Value = table.TableName;
      SqlDataAdapter dat = new SqlDataAdapter();
      dat.SelectCommand = qry;
      DataSet ds = new DataSet();
      dat.Fill(ds);
                    
      foreach (DataRow fkrow in ds.Tables[0].Rows) {
        ForeignKey fk = new ForeignKey();
        fk.ForeignKeyName = (string)fkrow[0];
        fk.RefTableName = (string)fkrow[1];
        fk.UpdateCascade = Convert.ToBoolean(fkrow[2]);
        fk.DeleteCascade = Convert.ToBoolean(fkrow[3]);
        table.ForeignKeys.Add(fk);
      }

    }

    public void GetForeignKeyColumns(Database db, Table table, ForeignKey fk) {
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
      qry.Parameters.Add("@foreignkeyname", SqlDbType.VarChar).Value = fk.ForeignKeyName;
      dat.Fill(ds);
      
      foreach (DataRow colrow in ds.Tables[0].Rows) {
        ForeignKeyColumn col = new ForeignKeyColumn();                
        col.RefTable = db.FindTable(fk.RefTableName, true);
        col.Column = table.FindColumn((string)colrow[0]);
        col.RefColumn = col.RefTable.FindColumn((string)colrow[1]);
        fk.Columns.Add(col);
      }
    }

    public void GetPrimaryKey(Database db, Table table) {
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
      qry.Parameters.Add("@TableName", SqlDbType.VarChar).Value = table.TableName;
      dat.Fill(ds);      

      if (ds.Tables[0].Rows.Count > 0) {
        table.PrimaryKeyName = Convert.ToString(ds.Tables[0].Rows[0][0]);        
      }
    }

    public void GetPrimaryKeyColumns(Database db, Table table) {
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
      qry.Parameters.Add("@TableName", SqlDbType.VarChar).Value = table.TableName;
      qry.Parameters.Add("@PKName", SqlDbType.VarChar).Value = table.PrimaryKeyName;
      DataSet ds = new DataSet();
      dat.Fill(ds);      
      
      foreach (DataRow r in ds.Tables[0].Rows) {
        Column c = table.FindColumn((string)r[0]);
        if (c == null)
          throw new Exception("Não foi localizada a coluna " + (string)r[0] + " da primary key da tabela " + table.TableName);
        table.PrimaryKeyColumns.Add(c);
      }
    }

    public void GetIndexes(Database db, Table table) {
      SqlCommand qry = new SqlCommand(
        "select " +
        "  i.name, " +
        "  i.is_unique, " +        
        "  i.type_desc " + 
        "from " +
        "  sys.indexes i " +
        "    inner join sysobjects o on " +
        "      i.object_id = o.id " +
        "where " +
        "  o.uid = 1 and " +
        "  o.name = @TableName and " +
        "  i.is_primary_key = 0 and " + //Despreza índice de PK        
        "  i.name is not null " + //HEAP
        "order by i.index_id ", SqlConn);
      SqlDataAdapter dat = new SqlDataAdapter();
      dat.SelectCommand = qry;
      qry.Parameters.Add("@TableName", SqlDbType.VarChar).Value = table.TableName;
      DataSet ds = new DataSet();
      dat.Fill(ds);
      
      foreach(DataRow r in ds.Tables[0].Rows){
        Index i = new Index();
        i.IndexName = (string)r[0];
        i.Unique = Convert.ToBoolean(r[1]);
        i.IsClustered = ((string)r[2]).ToUpper() == "CLUSTERED";
        table.Indexes.Add(i);
      }                  
    }

    public void GetIndexColumns(Database db, Table table, Index index) {
      SqlCommand qry = new SqlCommand(
        "select " +
        "  c.name, " +
        "  k.is_descending_key " +
        "from " +
        "  sys.index_columns k " +
        "    inner join syscolumns c on " +
        "      c.id = k.object_id and " +
        "      c.colid = k.column_id " +
        "    inner join sysobjects o on " +
        "      k.object_id = o.id " +
        "    inner join sysindexes i on " +
        "      i.id = k.object_id and " +
        "      i.indid = k.index_id  " +
        "where " +
        "  o.uid = 1 and " +
        "  o.name = @TableName and " +
        "  i.name = @IndexName " +
        "order by " +
        "  k.index_column_id ", SqlConn);
      SqlDataAdapter dat = new SqlDataAdapter();
      dat.SelectCommand = qry;
      qry.Parameters.Add("@TableName", SqlDbType.VarChar).Value = table.TableName;
      qry.Parameters.Add("@IndexName", SqlDbType.VarChar).Value = index.IndexName;
      DataSet ds = new DataSet();
      dat.Fill(ds);      
      
      foreach (DataRow r2 in ds.Tables[0].Rows) {
        IndexColumn c = new IndexColumn();
        c.Column = table.FindColumn((string)r2[0]);
        c.Order = ((IndexColumn.EnumOrder)Convert.ToInt32(r2[1]));
        if (c == null)
          throw new Exception("Não foi localizada a coluna " + (string)r2[0] + " do índice " + table.TableName + "." + index.IndexName);
        index.Columns.Add(c);
      }
    }

    public DataTable GetTableData(Database db, string ATabela) {
      SqlCommand qry = new SqlCommand(
        "select * from " + ATabela, SqlConn);
      SqlDataAdapter dat = new SqlDataAdapter();
      dat.SelectCommand = qry;
      DataSet ds = new DataSet();
      dat.Fill(ds);
      return ds.Tables[0];
    }

    public void GetProcedures(Database db) {
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

      foreach (DataRow r in ds.Tables[0].Rows) {
        Procedure p = new Procedure();
        p.Name = (string)r[0];        
        db.Procedures.Add(p);
      }
    }

    public void GetProcedureText(Database db, Procedure p) {
      SqlCommand qry = new SqlCommand("sp_helptext " + p.Name, SqlConn);
      SqlDataAdapter dat = new SqlDataAdapter();
      dat.SelectCommand = qry;
      DataSet ds = new DataSet();
      dat.Fill(ds);

      foreach (DataRow r2 in ds.Tables[0].Rows) {
        p.Body += (string)r2[0];
      }
    }

    public void GetFunctions(Database db) {
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
      
      foreach(DataRow r in ds.Tables[0].Rows){
        Function f = new Function();
        f.Name = (string)r[0];
        db.Functions.Add(f);
      }
      
    }

    public void GetFunctionText(Database db, Function f) {
      SqlCommand qry = new SqlCommand("sp_helptext " + f.Name, SqlConn);
      SqlDataAdapter dat = new SqlDataAdapter();
      dat.SelectCommand = qry;
      DataSet ds = new DataSet();
      dat.Fill(ds);
                  
      f.Body = "";
      foreach (DataRow r2 in ds.Tables[0].Rows) {
        f.Body += (string)r2[0];
      }       
    }

    public void GetTriggers(Database db, Table t) {
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
      qry.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = t.TableName;
      SqlDataAdapter dat = new SqlDataAdapter();
      dat.SelectCommand = qry;
      DataSet ds = new DataSet();
      dat.Fill(ds);            

      foreach (DataRow r in ds.Tables[0].Rows) {
        Trigger tr = new Trigger();
        tr.Name = (string)r[0];
        tr.Table = t;        
        t.Triggers.Add(tr);        
      }
    }

    public void GetTriggerText(Database db, Table t, Trigger tr) {
      SqlCommand qry = new SqlCommand("sp_helptext " + tr.Name, SqlConn);
      SqlDataAdapter dat = new SqlDataAdapter();
      dat.SelectCommand = qry;
      DataSet ds = new DataSet();
      dat.Fill(ds);      
      
      tr.Body = "";      
      foreach (DataRow r2 in ds.Tables[0].Rows) {
        tr.Body += r2[0];
      }
    }

    public void GetViews(Database db) {
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
      
      foreach (DataRow dr in ds.Tables[0].Rows){
        View v = new View();
        v.Name = (string)dr[0];
        db.Views.Add(v);
      }                  
    }

    public void GetViewText(Database db, View v) {
      SqlCommand qry = new SqlCommand("sp_helptext " + v.Name, SqlConn);
      SqlDataAdapter dat = new SqlDataAdapter();
      dat.SelectCommand = qry;
      DataSet ds = new DataSet();
      dat.Fill(ds);      

      v.Body = "";
            
      foreach (DataRow r2 in ds.Tables[0].Rows) {
        v.Body += (string)r2[0];
      }      
    }

    public void GetCheckConstraints(Database db, Table t) {
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
      qry.Parameters.Add("@TableName", SqlDbType.VarChar, 200).Value = t.TableName;
      DataSet ds = new DataSet();
      dat.Fill(ds);

      foreach (DataRow r in ds.Tables[0].Rows) {
        CheckConstraint ch = new CheckConstraint();
        ch.Name = (string)r[0];
        ch.Expression = (string)r[1];
        t.CheckConstraints.Add(ch);      
      }
    }

    public void GetSequences(Database db) {
      
    }

    public DataSet getTableTriggers(string ATabela) {
      return null;
    }
  }
}
