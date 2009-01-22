using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Model;
using System.Data;

namespace DBInfo.Core.OutputGenerators {
  public interface IScriptsOutputGenerator {
    DatabaseScript GenerateTableOutput(Table ATable);
    string GeneratePrimaryKeyOutput(Table ATable);
    string GenerateIndexesOutput(Table ATable);
    DatabaseScript GenerateForeignKeysOutput(Table table);
    string GenerateTableDataStartOutput(Table ATable);
    string GenerateTableDataRowOutput(Table ATable, DataRow ARow);
    string GenerateTableDataEndOutput(Table ATable);
    DatabaseScript GenerateProcedureOutput(Procedure procedure);
    DatabaseScript GenerateFunctionOutput(Function function);
    DatabaseScript GenerateTriggerOutput(Table table, Trigger trigger);
    DatabaseScript GenerateViewOutput(View view);
    DatabaseScript GenerateSequenceOutput(Sequence seq);
  }
}
