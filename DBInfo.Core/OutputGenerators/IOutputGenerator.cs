using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Model;
using System.Data;

namespace DBInfo.Core.OutputGenerators {
  public interface IOutputGenerator {
    string GenerateTableOutput(Table ATable);
    string GeneratePrimaryKeyOutput(Table ATable);
    string GenerateIndexesOutput(Table ATable);
    string GenerateForeignKeysOutput(Table ATable);
    string GenerateTableDataStartOutput(Table ATable);
    string GenerateTableDataRowOutput(Table ATable, DataRow ARow);
    string GenerateTableDataEndOutput(Table ATable);
    string GenerateProcedureOutput(Procedure AProcedure);
    string GenerateFunctionOutput(Function AFunction);
    string GenerateTriggerOutput(Trigger ATrigger);
    string GenerateViewOutput(View AView);
    string GenerateSequenceOutput(Sequence ASequence);
  }
}
