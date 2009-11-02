using System;
using System.Collections;
using System.Data;
using DBInfo.Core.Model;
using DBInfo.Core.OutputGenerators;
using System.Data.SqlClient;
using DBInfo.Core.Statement;

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

    public string GenerateCreateTableScript(CreateTable CreateTableStatement){
      string TmpScript = "";
      TmpScript = "create table " + CreateTableStatement.Table.TableName + "(" + Environment.NewLine;
      foreach (Column c in CreateTableStatement.Table.Columns) {
        TmpScript += "  " + c.Name + " " + GetSQLType(c) + PegarIdentity(CreateTableStatement.Table, c) + PegarDefault(CreateTableStatement.Table, c) + PegarIsNull(c.IsNull);
        if (CreateTableStatement.Table.Columns.IndexOf(c) == (CreateTableStatement.Table.Columns.Count - 1))
          TmpScript += Environment.NewLine;
        else
          TmpScript += "," + Environment.NewLine;
      }
      TmpScript += ")";
      return TmpScript;
    }

    public string GenerateCreateCheckConstraintScript(CreateCheckConstraint CreateCheckConstraintStatement){
      string ScriptContent = "";

      ScriptContent += "alter table " + CreateCheckConstraintStatement.CheckConstraint.TableName + Environment.NewLine;
      ScriptContent += "  add constraint " + CreateCheckConstraintStatement.CheckConstraint.CheckConstraintName + Environment.NewLine;
      ScriptContent += "  check " + CreateCheckConstraintStatement.CheckConstraint.Expression +
        Environment.NewLine + Environment.NewLine;

      return ScriptContent;
    }

    public string GenerateCreatePrimaryKeyScript(CreatePrimaryKey CreatePrimaryKeyStatement){
      string TmpScript = "";
      if (CreatePrimaryKeyStatement.Table.PrimaryKeyName != String.Empty) {
        TmpScript += "alter table " + CreatePrimaryKeyStatement.Table.TableName + Environment.NewLine;
        TmpScript += "  add constraint " + CreatePrimaryKeyStatement.Table.PrimaryKeyName + Environment.NewLine;
        TmpScript += "  primary key (" + Environment.NewLine;
        foreach (string colName in CreatePrimaryKeyStatement.Table.PrimaryKeyColumns) {
          //Column c = CreatePrimaryKeyStatement.Table.FindColumn(colName);
          TmpScript += "    " + colName;
          if (CreatePrimaryKeyStatement.Table.PrimaryKeyColumns.IndexOf(colName) != CreatePrimaryKeyStatement.Table.PrimaryKeyColumns.Count - 1)
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

    public string GenerateCreateIndexScript(CreateIndex CreateIndexStatement){
      string TmpScript = "";

      TmpScript += "create " + PegarUnique(CreateIndexStatement.Index.Unique) + "index " + CreateIndexStatement.Index.IndexName + " on " + CreateIndexStatement.Index.TableName + " (" + Environment.NewLine;
      foreach (IndexColumn c in CreateIndexStatement.Index.Columns) {
        TmpScript += "  " + c.Column;
        if (CreateIndexStatement.Index.Columns.IndexOf(c) != (CreateIndexStatement.Index.Columns.Count - 1))
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

    public string GenerateCreateForeignKeysScript(CreateForeignKey CreateForeignKeyStatement){
      string TmpScript = "";

      TmpScript += "alter table " + CreateForeignKeyStatement.ForeignKey.TableName + Environment.NewLine;
      TmpScript += "  add constraint \"" + CreateForeignKeyStatement.ForeignKey.ForeignKeyName + "\" foreign key (" + Environment.NewLine;
      foreach (ForeignKeyColumn fkcol in CreateForeignKeyStatement.ForeignKey.Columns) {
        TmpScript += "    " + fkcol.Column;
        if (CreateForeignKeyStatement.ForeignKey.Columns.IndexOf(fkcol) != CreateForeignKeyStatement.ForeignKey.Columns.Count - 1)
          TmpScript += "," + Environment.NewLine;
        else
          TmpScript += Environment.NewLine + "  )" + Environment.NewLine;
      }
      TmpScript += "  references " + CreateForeignKeyStatement.ForeignKey.RefTableName + " (" + Environment.NewLine;
      foreach (ForeignKeyColumn fkcol in CreateForeignKeyStatement.ForeignKey.Columns) {
        TmpScript += "    " + fkcol.RefColumn;
        if (CreateForeignKeyStatement.ForeignKey.Columns.IndexOf(fkcol) != CreateForeignKeyStatement.ForeignKey.Columns.Count - 1)
          TmpScript += "," + Environment.NewLine;
        else
          TmpScript += Environment.NewLine + "  ) " + PegarUpdateDeleteCascade(CreateForeignKeyStatement.ForeignKey) + Environment.NewLine + Environment.NewLine;
      }

      return TmpScript;
    }
    
    public string GenerateCreateProcedureScript(CreateProcedure CreateProcedureStatement){
      string Tmp = "";
      Tmp += "if exists(select 1 from sysobjects where name = '" + CreateProcedureStatement.Procedure.Name + "')" + Environment.NewLine;
      Tmp += "begin " + Environment.NewLine;
      Tmp += "  drop procedure dbo." + CreateProcedureStatement.Procedure.Name + Environment.NewLine;
      Tmp += "end" + Environment.NewLine;
      Tmp += "GO " + Environment.NewLine;      
      Tmp += Environment.NewLine;

      return Tmp + CreateProcedureStatement.Procedure.Body;
    }
    
    public string GenerateCreateFunctionScript(CreateFunction CreateFunction){
      string Tmp =
        "drop function dbo." + CreateFunction.Function.Name + Environment.NewLine;

      Tmp += CreateFunction.Function.Body;      

      return Tmp;
    }
    
    public string GenerateCreateTriggerScript(CreateTrigger CreateTriggerStatement){
      string Tmp =
        "drop trigger dbo." + CreateTriggerStatement.Trigger.TriggerName + Environment.NewLine;

      Tmp += CreateTriggerStatement.Trigger.Body;      

      return Tmp;
    }
    
    public string GenerateCreateViewScript(CreateView CreateViewStatement){
      string Tmp =
        "drop view dbo." + CreateViewStatement.View.Name + Environment.NewLine;

      Tmp += CreateViewStatement.View.Body;      

      return Tmp;
    }

  }
}
