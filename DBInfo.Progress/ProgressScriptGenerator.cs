using System;
using DBInfo.Core.OutputGenerators;
using DBInfo.Core.Model;

namespace DBInfo.OutputGenerators {
  public class ProgressScriptGenerator : ScriptGenerator {
    public ProgressScriptGenerator() {
    }

    public override string GerarScriptDadosIniciaisFimScript(Table ATable) {
      return String.Empty;
    }

    public override string GerarScriptDadosIniciaisInicioScript(Table ATable) {
      return String.Empty;
    }

    public override string GerarScriptDadosIniciaisLinha(Table ATable, System.Data.DataRow ARow) {
      return String.Empty;
    }

    protected override string GerarScriptForeignKey(Table ATable) {
      return String.Empty;
    }

    protected override string GerarScriptFunction(Function AFunction) {
      return String.Empty;
    }

    private string GetOrder(IndexColumn.EnumOrder AOrder) {
      if (AOrder == IndexColumn.EnumOrder.Descending)
        return "DESCENDING";
      else
        return "ASCENDING";
    }

    protected override string GerarScriptIndices(Table ATable) {
      string script = "";
      foreach (Index idx in ATable.Indexes) {
        script +=
          "ADD INDEX \"" + idx.IndexName + "\" ON \"" + ATable.TableName + "\"\n" +
          "  AREA \"" + idx.Area + "\"\n";
        if (idx.Unique)
          script += "  UNIQUE\n";
        if (idx.Primary)
          script += "  PRIMARY\n";
        foreach (IndexColumn col in idx.Columns)
          script += "  INDEX-FIELD \"" + col.Column.Name + "\" " + GetOrder(col.Order) + "\n";
        script += "\n";
      }
      return script;
    }

    protected override string GerarScriptPrimaryKey(Table ATable) {
      return String.Empty;
    }

    protected override string GerarScriptProcedure(Procedure AProcedure) {
      return String.Empty;
    }

    private string GetFieldType(Column AColumn) {
      switch (AColumn.Type) {
        case Column.DBColumnType.DBInteger: return "integer";
        case Column.DBColumnType.DBVarchar: return "varchar";
        case Column.DBColumnType.DBChar: return "character";
        case Column.DBColumnType.DBBit: return "logical";
        case Column.DBColumnType.DBSmallDateTime: return "date";
        case Column.DBColumnType.DBNumeric: return "decimal";
        case Column.DBColumnType.DBRowID: return "recid";
        default: throw new Exception("Tipo de dados não suportado " + AColumn.Table.TableName + "." + AColumn.Name + "." + AColumn.Type.ToString());
      }
    }

    private string GetOverride(bool AOverride) {
      if (AOverride)
        return "OVERRIDE";
      else
        return "NO-OVERRIDE";
    }

    private string GetCRC(string CRC) {
      if (CRC == null || CRC == String.Empty)
        return "\"?\"";
      else
        return "\"" + CRC + "\"";
    }

    protected override string GerarScriptTabela(Table ATable) {
      string script;
      script =
        "ADD TABLE \"" + ATable.TableName + "\"\n" +
        "  AREA \"" + ATable.Area + "\"\n";
      if (ATable.Label != null && ATable.Label != String.Empty)
        script += "  LABEL \"" + ATable.Label + "\"\n";
      script += "  DESCRIPTION \"" + ATable.Description + "\"\n";
      if (ATable.ValExp != null && ATable.ValExp != String.Empty)
        script += "  VALEXP \"" + ATable.ValExp + "\"\n";
      if (ATable.ValMsg != null && ATable.ValMsg != String.Empty)
        script += "  VALMSG \"" + ATable.ValMsg + "\"\n";
      script += "  DUMP-NAME \"" + ATable.DumpName + "\"\n";
      if (ATable.ForeignName != null && ATable.ForeignName != String.Empty)
        script += "  FOREIGN-NAME \"" + ATable.ForeignName + "\"\n";
      foreach (TableTrigger trg in ATable.TableTriggers) {
        script +=
          "  TABLE-TRIGGER \"" + trg.Event + "\" " + GetOverride(trg.Override) + " PROCEDURE \"" + trg.Procedure + "\" CRC " + GetCRC(trg.CRC) + "\n";
      }
      script += "\n";
      foreach (Column col in ATable.Columns) {
        script +=
          "ADD FIELD \"" + col.Name + "\" OF \"" + ATable.TableName + "\" AS " + GetFieldType(col) + "\n" +
          "  DESCRIPTION \"" + col.Description + "\"\n" +
          "  FORMAT \"" + col.Format + "\"\n" +
          "  INITIAL \"" + col.DefaultValue + "\"\n" +
          "  LABEL \"" + col.Label + "\"\n" +
          "  POSITION " + col.Position + "\n" +
          "  SQL-WIDTH " + col.SqlWidth + "\n";
        if (col.ValExp != null && col.ValExp != String.Empty)
          script += "  VALEXP \"" + col.ValExp + "\"\n";
        if (col.ValMsg != null && col.ValMsg != String.Empty)
          script += "  VALMSG \"" + col.ValMsg + "\"\n";
        script +=
          //"  COLUMN-LABEL \"" + col.Label + "\"\n" + 
          "  HELP \"" + col.Help + "\"\n";
        if (col.Decimals > 0)
          script += "  DECIMALS " + col.Decimals.ToString() + "\n";
        script +=
          "  ORDER " + col.Order + "\n" +
          "\n";
      }
      return script;
    }

    protected override string GerarScriptTrigger(Trigger ATrigger) {
      return String.Empty;
    }

    protected override string GerarScriptView(View AView) {
      return String.Empty;
    }

    private string GetCycleOnLimit(bool ACycle) {
      if (ACycle)
        return "yes";
      else
        return "no";
    }

    protected override string GerarScriptSequence(Sequence ASequence) {
      string script =
        "ADD SEQUENCE \"" + ASequence.SequenceName + "\"\n" +
        "  INITIAL " + ASequence.Initial.ToString() + "\n" +
        "  INCREMENT " + ASequence.Increment.ToString() + "\n" +
        "  CYCLE-ON-LIMIT " + GetCycleOnLimit(ASequence.CycleOnLimit) + "\n" +
        "  MIN-VAL " + ASequence.MinValue.ToString() + "\n";
      if (ASequence.MaxValue > 0)
        script += "  MAX-VAL " + ASequence.MaxValue + "\n";
      script += "\n";

      return script;
    }

  }
}
