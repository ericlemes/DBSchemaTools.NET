using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Model;
using System.Data;

namespace DBInfo.Core.OutputGenerators {
  public interface IScriptOutputGenerator {
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
    string GenerateDropFunctionScript(Function function);
    string GenerateCreateFunctionScript(Function function);
    string GenerateDropTriggerScript(Table table, Trigger trigger);
    string GenerateCreateTriggerScript(Table table, Trigger trigger);    
    string GenerateDropViewScript(View v);
    string GenerateCreateViewScript(View v);
    string GenerateSequenceScript(Sequence seq);        
    void OpenOutputDatabaseConnection(string connString);
    void ExecuteOuputDatabaseScript(string script);
    void CloseOutputDatabaseConnection();
  }
}
