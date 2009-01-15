using System;
using System.Collections;
using System.Data;
using DBInfo.Core.OutputGenerators;
using DBInfo.Core.Model;

namespace DBInfo.OutputGenerators {
  public class OracleScriptGenerator : ScriptGenerator {
    public OracleScriptGenerator() {
    }

    private string RetirarUnderline(string nomeobjeto) {
      // Comentado: unreacheable code return nomeobjeto;
      if (nomeobjeto.Length > 0)
        return nomeobjeto.Substring(0, 1).Replace("_", string.Empty);
      else
        return nomeobjeto;

    }


    private string PegarTipoOracle(DBInfo.Core.Model.Column.DBColumnType AType, int ASize, int ADecimals) {
      switch (AType) {
        //validar os tipos de dados após gerar o script
        case Column.DBColumnType.DBInteger: return "NUMBER(10)"; //ok
        case Column.DBColumnType.DBVarchar: return "VARCHAR2(" + ASize.ToString() + ")"; //ok
        case Column.DBColumnType.DBChar: return "CHAR(" + ASize.ToString() + ")"; // ok
        case Column.DBColumnType.DBBlob: return "BLOB";
        case Column.DBColumnType.DBDecimal: return "NUMBER(" + ASize.ToString() + "," + ADecimals.ToString() + ")"; // ok
        case Column.DBColumnType.DBFloat: return "FLOAT(49)"; // ok
        case Column.DBColumnType.DBMemo: return "CLOB";
        case Column.DBColumnType.DBDateTime: return "Date"; // OK
        case Column.DBColumnType.DBBit: return "NUMBER(1)";
        case Column.DBColumnType.DBSmallDateTime: return "Date"; // OK
        case Column.DBColumnType.DBMoney: return "NUMBER(19,4)";
        case Column.DBColumnType.DBSmallInt: return "NUMBER(5)"; // ok
        case Column.DBColumnType.DBNumeric: return "NUMBER"; // ok
        case Column.DBColumnType.DBGUID: return "ROWID";
        case Column.DBColumnType.DBBigInt: return "NUMBER(19)"; // ok
        case Column.DBColumnType.DBTinyInt: return "NUMBER(3)"; // ok
        case Column.DBColumnType.DBBinary: return "BLOB";
        case Column.DBColumnType.DBNVarchar: return "NVARCHAR2(" + ASize.ToString() + ")"; // ok
        default: throw new Exception("Tipo de dados não suportado " + AType.ToString());
      }
    }

    private string PegarIsNull(bool IsNull) {
      if (IsNull)
        return "";
      else
        return " not null ";
    }

    private string PegarDefault(Table ATable, Column AColumn) {
      if (AColumn.DefaultValue == String.Empty) {
        return "";
      } else {
        // verificar se será feito este tratamento para o getdate
        return " default " + AColumn.DefaultValue.ToLower().Replace("getdate()", "SYSDATE") + " ";
      }
    }

    protected override string GerarScriptTabela(Table ATable) {
      string TmpScript = "";
      TmpScript = "create table " + ATable.TableName + "(" + Environment.NewLine;
      foreach (Column c in ATable.Columns) {
        TmpScript += "  " + c.Name + " " + PegarTipoOracle(c.Type, c.Size, 0) + PegarDefault(ATable, c) + PegarIsNull(c.IsNull);
        if (ATable.Columns.IndexOf(c) == (ATable.Columns.Count - 1))
          TmpScript += Environment.NewLine;
        else
          TmpScript += "," + Environment.NewLine;
      }
      TmpScript += ")" + Environment.NewLine + "go" + Environment.NewLine + Environment.NewLine;

      foreach (CheckConstraint c in ATable.CheckConstraints) {
        TmpScript += "alter table " + ATable.TableName + Environment.NewLine;
        TmpScript += "  add constraint " + c.Nome + Environment.NewLine;

        // validar se irá realmente retirar os [ ]
        TmpScript += "  check " + c.Expressao.Replace("[", string.Empty).Replace("]", string.Empty) + Environment.NewLine + "go" + Environment.NewLine + Environment.NewLine;
      }


      return TmpScript;
    }

    protected override string GerarScriptPrimaryKey(Table ATable) {
      string TmpScript = "";
      if (ATable.PrimaryKeyName != String.Empty) {
        TmpScript += "alter table " + ATable.TableName + Environment.NewLine;
        TmpScript += "  add constraint " + ATable.PrimaryKeyName + Environment.NewLine;
        TmpScript += "  primary key (" + Environment.NewLine;

        foreach (Column c in ATable.PrimaryKeyColumns) {
          TmpScript += "    " + c.Name;
          if (ATable.PrimaryKeyColumns.IndexOf(c) != ATable.PrimaryKeyColumns.Count - 1)
            TmpScript += "," + Environment.NewLine;
          else
            TmpScript += Environment.NewLine + "  )" + Environment.NewLine + "go" + Environment.NewLine + Environment.NewLine;
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

    protected override string GerarScriptIndices(Table ATable) {
      string TmpScript = "";
      foreach (Index i in ATable.Indexes) {
        TmpScript += "create " + PegarUnique(i.Unique) + "index " + RetirarUnderline(i.IndexName) + " on " + ATable.TableName + " (" + Environment.NewLine;
        foreach (IndexColumn c in i.Columns) {
          TmpScript += "  " + c.Column.Name;
          if (i.Columns.IndexOf(c) != (i.Columns.Count - 1))
            TmpScript += "," + Environment.NewLine;
          else
            TmpScript += Environment.NewLine + ")" + Environment.NewLine + "go" + Environment.NewLine + Environment.NewLine;
        }
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

    protected override string GerarScriptForeignKey(Table ATable) {
      string TmpScript = "";
      foreach (ForeignKey fk in ATable.ForeignKeys) {
        TmpScript += "alter table " + ATable.TableName + Environment.NewLine;
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
      }
      return TmpScript;
    }

    private string PegarValorPara(Column.DBColumnType ADBColumnType, object AValue) {
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
        return "'" + Convert.ToDateTime(AValue).ToString("yyyy-MM-dd hh:mm:ss") + "'";
      else if (AValue is Guid)
        return "'" + AValue.ToString() + "'";
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

    public override string GerarScriptDadosIniciaisInicioScript(Table ATable) {
      if (ATable.HasIdentity)
        return "set IDENTITY_INSERT dbo." + ATable.TableName + " on" + Environment.NewLine + Environment.NewLine;
      else
        return "";
    }

    public override string GerarScriptDadosIniciaisFimScript(Table ATable) {
      if (ATable.HasIdentity)
        return "set IDENTITY_INSERT dbo." + ATable.TableName + " off" + Environment.NewLine + Environment.NewLine;
      else
        return "";
    }

    public override string GerarScriptDadosIniciaisLinha(Table ATable, DataRow ARow) {
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

    protected override string GerarScriptProcedure(Procedure AProcedure) {
      string Tmp = "";
      Tmp += "if exists(select 1 from sysobjects where name = '" + AProcedure.Name + "')" + Environment.NewLine;
      Tmp += "  drop procedure dbo." + AProcedure.Name + Environment.NewLine;
      Tmp += "go" + Environment.NewLine + Environment.NewLine;
      Tmp += AProcedure.Body;
      Tmp += Environment.NewLine + "go" + Environment.NewLine + Environment.NewLine;
      return Tmp;
    }

    protected override string GerarScriptFunction(Function AFunction) {
      string Tmp = "";
      Tmp += "if exists(select 1 from sysobjects where name = '" + AFunction.Name + "')" + Environment.NewLine;
      Tmp += "  drop function dbo." + AFunction.Name + Environment.NewLine;
      Tmp += "go" + Environment.NewLine + Environment.NewLine;
      Tmp += AFunction.Body;
      Tmp += Environment.NewLine + "go" + Environment.NewLine + Environment.NewLine;
      return Tmp;
    }

    protected override string GerarScriptTrigger(Trigger ATrigger) {
      string Tmp = "";
      Tmp += "if exists(select 1 from sysobjects where name = '" + ATrigger.Name + "')" + Environment.NewLine;
      Tmp += "  drop trigger dbo." + ATrigger.Name + Environment.NewLine;
      Tmp += "go" + Environment.NewLine + Environment.NewLine;
      Tmp += ATrigger.Body;
      Tmp += Environment.NewLine + "go" + Environment.NewLine + Environment.NewLine;
      return Tmp;
    }

    protected override string GerarScriptView(View AView) {
      string Tmp = "";
      Tmp += "if exists(select 1 from sysobjects where name = '" + AView.Name + "')" + Environment.NewLine;
      Tmp += "  drop view dbo." + AView.Name + Environment.NewLine;
      Tmp += "go" + Environment.NewLine + Environment.NewLine;
      Tmp += AView.Body;
      Tmp += Environment.NewLine + "go" + Environment.NewLine + Environment.NewLine;
      return Tmp;
    }

    protected override string GerarScriptSequence(Sequence ASequence) {
      return String.Empty;
    }


  }
}
