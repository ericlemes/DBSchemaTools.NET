using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Model;
using System.Data;
using DBInfo.Core.Statement;

namespace DBInfo.Core.OutputGenerators {
  public interface IScriptOutputHandler {
    string ScriptTerminator{
      get;
    }
    string GenerateCreateTableScript(CreateTable CreateTableStatement);
    string GenerateCreatePrimaryKeyScript(CreatePrimaryKey CreatePrimaryKeyStatement);
    string GenerateCreateIndexScript(CreateIndex CreateIndexStatement);
    string GenerateCreateForeignKeysScript(CreateForeignKey CreateForeignKeyStatement);
    string GenerateCreateCheckConstraintScript(CreateCheckConstraint CreateCheckConstraintStatement);
    string GenerateCreateProcedureScript(CreateProcedure CreateProcedureStatement);        
    string GenerateCreateFunctionScript(CreateFunction CreateFunctionStatement);    
    string GenerateCreateTriggerScript(CreateTrigger CreateTriggerStatement);        
    string GenerateCreateViewScript(CreateView CreateViewStatement);        
  }
}
