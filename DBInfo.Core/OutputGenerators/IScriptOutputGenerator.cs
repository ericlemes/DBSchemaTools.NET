using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Model;
using System.Data;

namespace DBInfo.Core.OutputGenerators {
  public interface IScriptsOutputGenerator {
    string ScriptTerminator{
      get;
    }
    string GenerateTableScript(Table ATable);
    string GeneratePrimaryKeyScript(Table ATable);
    string GenerateIndexScript(Table table, Index index);
    string GenerateForeignKeysScript(Table table, ForeignKey fk);
    string GenerateCheckConstraintScript(Table table, CheckConstraint check);
    string GenerateTableDataStartScript(Table ATable);
    string GenerateTableDataRowScript(Table ATable, DataRow ARow);
    string GenerateTableDataEndScript(Table ATable);
    string GenerateDropProcedureScript(Procedure procedure);
    string GenerateCreateProcedureScript(Procedure procedure);
    DatabaseScript GenerateFunctionScript(Function function);
    DatabaseScript GenerateTriggerScript(Table table, Trigger trigger);
    DatabaseScript GenerateViewScript(View view);
    DatabaseScript GenerateSequenceScript(Sequence seq);
    void OpenOutputDatabaseConnection(string connString);
    void ExecuteOuputDatabaseScript(string script);
    void CloseOutputDatabaseConnection();
  }
}
