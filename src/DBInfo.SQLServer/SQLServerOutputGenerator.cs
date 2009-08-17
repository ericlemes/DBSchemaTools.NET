using System;
using System.Collections;
using System.Data;
using DBInfo.Core.Model;
using DBInfo.Core.OutputGenerators;
using System.Data.SqlClient;

namespace DBInfo.SQLServer {
  public class SQLServerOutputGenerator : IScriptOutputHandler {

    public string ScriptTerminator {
      get { return "go"; }
    }

    private bool IsPK(string colName, Table tb) {
      bool retorno = false;

      foreach (object col in tb.PrimaryKeyColumns) {
        if (((Column)col).Name == colName)
          retorno = true;
      }

      return retorno;
    }
    private string GetSQLType(Column Coluna) {
      switch (Coluna.Type) {
        case DBColumnType.Integer: return "int";
        case DBColumnType.VarChar: return "varchar(" + Coluna.Size.ToString() + ")";
        case DBColumnType.Char: return "char(" + Coluna.Size.ToString() + ")";
        case DBColumnType.Image: return "image";
        case DBColumnType.Decimal: return "decimal(" + Coluna.Precision.ToString() + "," + Coluna.Scale.ToString() + ")";
        case DBColumnType.Float: return "float";
        case DBColumnType.Text: return "text";
        case DBColumnType.DateTime: return "datetime";
        case DBColumnType.Bit: return "bit";
        case DBColumnType.SmallDateTime: return "smalldatetime";
        case DBColumnType.Money: return "money";
        case DBColumnType.SmallInt: return "smallint";
        case DBColumnType.Numeric: return "numeric(" + Coluna.Precision.ToString() + "," + Coluna.Scale.ToString() + ")";
        case DBColumnType.UniqueIdentifier: return "uniqueidentifier";
        case DBColumnType.BigInt: return "bigint";
        case DBColumnType.TinyInt: return "tinyint";
        case DBColumnType.Binary: return "binary";
        case DBColumnType.NVarchar: return "nvarchar(" + Coluna.Size.ToString() + ")";
        case DBColumnType.TimeStamp: return "timestamp";
        case DBColumnType.NChar: return "nchar";
        case DBColumnType.Real: return "real";
        case DBColumnType.SmallMoney: return "smallmoney";
        case DBColumnType.VarBinary: return "varbinary";
        case DBColumnType.Xml: return "xml";
        case DBColumnType.NText: return "ntext";
        default: throw new Exception("Tipo de dados não suportado " + Coluna.Type.ToString());
      }
    }

    private string PegarIsNull(bool IsNull) {
      if (IsNull)
        return "";
      else
        return " not null ";
    }

    private string PegarIdentity(Table ATable, Column AColumn) {
      if (ATable.HasIdentity && AColumn.IdentityColumn) {
        return " identity(" + ATable.IdentitySeed + "," + ATable.IdentityIncrement + ") ";
      } else
        return "";
    }

    private string PegarDefault(Table ATable, Column AColumn) {
      if (AColumn.DefaultValue == String.Empty)
        return "";
      else
        return " default " + AColumn.DefaultValue + " ";
    }

    public string GenerateTableScript(Table table) {
      string TmpScript = "";
      TmpScript = "create table " + table.TableName + "(" + Environment.NewLine;
      foreach (Column c in table.Columns) {
        TmpScript += "  " + c.Name + " " + GetSQLType(c) + PegarIdentity(table, c) + PegarDefault(table, c) + PegarIsNull(c.IsNull);
        if (table.Columns.IndexOf(c) == (table.Columns.Count - 1))
          TmpScript += Environment.NewLine;
        else
          TmpScript += "," + Environment.NewLine;
      }
      TmpScript += ")";
      return TmpScript;
    }

    public string GenerateCheckConstraintScript(Table table, CheckConstraint check) {      
      string ScriptContent = "";

      ScriptContent += "alter table " + table.TableName + Environment.NewLine;
      ScriptContent += "  add constraint " + check.Name + Environment.NewLine;
      ScriptContent += "  check " + check.Expression +
        Environment.NewLine + Environment.NewLine;

      return ScriptContent;
    }

    public string GeneratePrimaryKeyScript(Table table) {
      string TmpScript = "";
      if (table.PrimaryKeyName != String.Empty) {
        TmpScript += "alter table " + table.TableName + Environment.NewLine;
        TmpScript += "  add constraint " + table.PrimaryKeyName + Environment.NewLine;
        TmpScript += "  primary key (" + Environment.NewLine;
        foreach (Column c in table.PrimaryKeyColumns) {
          TmpScript += "    " + c.Name;
          if (table.PrimaryKeyColumns.IndexOf(c) != table.PrimaryKeyColumns.Count - 1)
            TmpScript += "," + Environment.NewLine;
          else
            TmpScript += Environment.NewLine + "  )" + Environment.NewLine;;
        }
      }
      return TmpScript;
    }

    private string PegarUnique(bool AUnique) {
      if (AUnique)
        return "unique ";
      else
        return "";
    }

    public string GenerateIndexScript(Table table, Index index) {
      string TmpScript = "";

      TmpScript += "create " + PegarUnique(index.Unique) + "index " + index.IndexName + " on " + table.TableName + " (" + Environment.NewLine;
      foreach (IndexColumn c in index.Columns) {
        TmpScript += "  " + c.Column.Name;
        if (index.Columns.IndexOf(c) != (index.Columns.Count - 1))
          TmpScript += "," + Environment.NewLine;
        else
          TmpScript += Environment.NewLine + ")" + Environment.NewLine;
      }
      return TmpScript;
    }

    private string PegarUpdateDeleteCascade(ForeignKey ADBForeignKey) {
      string Tmp = "";
      if (ADBForeignKey.UpdateCascade)
        Tmp += " on update cascade ";
      if (ADBForeignKey.DeleteCascade)
        Tmp += " on delete cascade ";
      return Tmp;
    }

    public string GenerateForeignKeysScript(Table table, ForeignKey fk) {
      string TmpScript = "";

      TmpScript += "alter table " + table.TableName + Environment.NewLine;
      TmpScript += "  add constraint \"" + fk.ForeignKeyName + "\" foreign key (" + Environment.NewLine;
      foreach (ForeignKeyColumn fkcol in fk.Columns) {
        TmpScript += "    " + fkcol.Column.Name;
        if (fk.Columns.IndexOf(fkcol) != fk.Columns.Count - 1)
          TmpScript += "," + Environment.NewLine;
        else
          TmpScript += Environment.NewLine + "  )" + Environment.NewLine;
      }
      TmpScript += "  references " + fk.RefTableName + " (" + Environment.NewLine;
      foreach (ForeignKeyColumn fkcol in fk.Columns) {
        TmpScript += "    " + fkcol.RefColumn.Name;
        if (fk.Columns.IndexOf(fkcol) != fk.Columns.Count - 1)
          TmpScript += "," + Environment.NewLine;
        else
          TmpScript += Environment.NewLine + "  ) " + PegarUpdateDeleteCascade(fk) + Environment.NewLine + Environment.NewLine;
      }

      return TmpScript;
    }

    private string PegarValorPara(DBColumnType ADBColumnType, object AValue) {
      System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
      if (AValue is DBNull) {
        return "null";
      } else if (AValue is string) {
        return "'" + Convert.ToString(AValue).Replace("'", "''") + "'";
      } else if (AValue is int || AValue is Int16 || AValue is Byte || AValue is Int64)
        return Convert.ToString(AValue);
      else if (AValue is bool) {
        if ((bool)AValue)
          return "1";
        else
          return "0";
      } else if (AValue is DateTime)
        return "'" + Convert.ToDateTime(AValue).ToString("yyyy-MM-dd HH:mm:ss") + "'";
      else if (AValue is Guid)
        return "'" + AValue.ToString() + "'";
      else if (AValue is decimal)
        return ((decimal)AValue).ToString("0.000000");
      else if (AValue is float)
        return ((float)AValue).ToString("0.000000");
      else if (AValue is Double)
        return ((Double)AValue).ToString("0.000000");
      else
        throw new Exception("Não foi possível converter o valor " + AValue.ToString() + " para " + ADBColumnType.ToString());
    }

    private string MontarWherePK(Table ATable, DataRow ARow) {
      string s = "";
      if (ATable.PrimaryKeyName == String.Empty)
        return "";
      else {
        s += " where ";
        foreach (Column c in ATable.PrimaryKeyColumns) {
          s += c.Name + " = " + PegarValorPara(c.Type, ARow[c.Name]);
          if (ATable.PrimaryKeyColumns.IndexOf(c) != ATable.PrimaryKeyColumns.Count - 1)
            s += " and ";
        }
        s += " ";
      }
      return s;
    }

    public string GenerateTableDataStartScript(Table ATable) {
      if (ATable.HasIdentity)
        return "set IDENTITY_INSERT dbo." + ATable.TableName + " on" + Environment.NewLine + Environment.NewLine;
      else
        return "";
    }

    public string GenerateTableDataEndScript(Table ATable) {
      if (ATable.HasIdentity)
        return "set IDENTITY_INSERT dbo." + ATable.TableName + " off" + Environment.NewLine + Environment.NewLine;
      else
        return "";
    }

    public string GenerateTableDataRowScript(Table ATable, DataRow ARow) {
      string s = "";
      if (ATable.PrimaryKeyName != String.Empty)
        s += "if not exists(select 1 from " + ATable.TableName + MontarWherePK(ATable, ARow) + ")" + Environment.NewLine;
      s += "  insert into dbo." + ATable.TableName + "(" + Environment.NewLine;
      foreach (Column c in ATable.Columns) {
        s += "    " + c.Name;
        if (ATable.Columns.IndexOf(c) != ATable.Columns.Count - 1)
          s += "," + Environment.NewLine;
        else
          s += Environment.NewLine + "  )" + Environment.NewLine;
      }
      s += "  values (" + Environment.NewLine;
      foreach (Column c in ATable.Columns) {
        s += "    " + PegarValorPara(c.Type, ARow[c.Name]);
        if (ATable.Columns.IndexOf(c) != ATable.Columns.Count - 1)
          s += "," + Environment.NewLine;
        else
          s += Environment.NewLine + "  )" + Environment.NewLine + Environment.NewLine;
      }
      return s;
    }

    public string AlterarScriptDadosIniciaisLinha(Table ATable, DataRow ARow) {
      string s = "";
      if (ATable.PrimaryKeyName != String.Empty)
        s += "if exists(select 1 from " + ATable.TableName + MontarWherePK(ATable, ARow) + ")" + Environment.NewLine;
      s += "  update dbo." + ATable.TableName + Environment.NewLine + "SET ";

      foreach (Column c in ATable.Columns) {
        if (!IsPK(c.Name, ATable)) {
          if (ARow[c.Name] == System.DBNull.Value)
            s += "     " + c.Name + " = NULL";
          else
            s += "     " + c.Name + " = " + PegarValorPara(c.Type, ARow[c.Name]);

          if (ATable.Columns.IndexOf(c) != ATable.Columns.Count - 1) {
            s += "," + Environment.NewLine;
          } else {
            s += MontarWherePK(ATable, ARow);
            s += Environment.NewLine + Environment.NewLine;
          }
        }
      }
      return s;
    }
    
    public string GenerateDropProcedureScript(Procedure procedure) {
      string Tmp = "";
      Tmp += "if exists(select 1 from sysobjects where name = '" + procedure.Name + "')" + Environment.NewLine;
      Tmp += "  drop procedure dbo." + procedure.Name + Environment.NewLine;
      Tmp += Environment.NewLine;
      return Tmp;
    }

    public string GenerateCreateProcedureScript(Procedure procedure) {      
      return procedure.Body;
    }
    
    public string GenerateDropFunctionScript(Function function){
      string Tmp =
        "drop function dbo." + function.Name + Environment.NewLine;
      return Tmp;
    }

    public string GenerateCreateFunctionScript(Function function) {      
      string Tmp = "";
      Tmp += function.Body;      

      return Tmp;
    }
    
    public string GenerateDropTriggerScript(Table table, Trigger trigger){
      string tmp =
        "drop trigger dbo." + trigger.Name + Environment.NewLine;
      return tmp;
    }

    public string GenerateCreateTriggerScript(Table table, Trigger trigger) {
      string Tmp = "";      
      Tmp += trigger.Body;      

      return Tmp;
    }
    
    public string GenerateDropViewScript(View view){
      string Tmp =
        "drop view dbo." + view.Name + Environment.NewLine;
      return Tmp;
    }

    public string GenerateCreateViewScript(View view) {      
      string Tmp = "";      
      Tmp += view.Body;      

      return Tmp;
    }

    public string GenerateSequenceScript(Sequence seq) {
      return null;
    }

    private SqlConnection _conn;

    public void OpenOutputDatabaseConnection(string connString) {
      _conn = new SqlConnection();      
      _conn.ConnectionString = connString;
      _conn.Open();
    }

    public void ExecuteOuputDatabaseScript(string script) {
      SqlCommand cmd = new SqlCommand(script, _conn);
      cmd.ExecuteNonQuery();
    }

    public void CloseOutputDatabaseConnection() {
      _conn.Close();
    }

  }
}
