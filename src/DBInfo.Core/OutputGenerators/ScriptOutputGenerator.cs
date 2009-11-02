using System;
using System.Collections;
using System.IO;
using System.Data;
using DBInfo.Core.Extractor;
using DBInfo.Core.Model;
using System.Collections.Generic;
using DBInfo.Core.Statement;

namespace DBInfo.Core.OutputGenerators {
  public class ScriptOutputGenerator : IStatementCollectionOutputGenerator {
    
    public ExpectedInputType ExpectedInputType{
      get { return ExpectedInputType.StatementCollection;}
    }

    private IScriptOutputHandler _ScriptOutputGen;
    public IScriptOutputHandler ScriptOutputGen {
      get { return _ScriptOutputGen; }
      set { _ScriptOutputGen = value; }
    }        
    
    private string _OutputDir;
    public string OutputDir{
      get { return _OutputDir;}
      set { _OutputDir = value;}
    }
    
    public bool RequiresScriptOutputHandler{
      get { return true; }
    }
    
    private IScriptFileOutputGenerator _ScriptFileOutputGenerator;
    public IScriptFileOutputGenerator ScriptFileOutputGenerator{
      get { return _ScriptFileOutputGenerator;}
      set { _ScriptFileOutputGenerator = value;}
    }
   
    public void SaveScripts(List<BaseStatement> statements) {      
      if (!Directory.Exists(OutputDir))
        Directory.CreateDirectory(OutputDir);        
        
      if (_ScriptFileOutputGenerator == null)
        throw new Exception("ScriptFileOutputGenerator is required");

      _ScriptFileOutputGenerator.GenerateFileOutput(OutputDir, statements, ScriptOutputGen); 
    }

    public void GenerateOutput(List<BaseStatement> statements, List<DBObjectType> dataToGenerateOutput){
      foreach(BaseStatement s in statements){
        if (s is CreateTable)
          s.Script = ScriptOutputGen.GenerateCreateTableScript((CreateTable)s);
        if (s is CreatePrimaryKey)
          s.Script = ScriptOutputGen.GenerateCreatePrimaryKeyScript((CreatePrimaryKey)s); 
        if (s is CreateIndex)
          s.Script = ScriptOutputGen.GenerateCreateIndexScript((CreateIndex)s);
        if (s is CreateForeignKey)
          s.Script = ScriptOutputGen.GenerateCreateForeignKeysScript((CreateForeignKey)s);
        if (s is CreateProcedure)
          s.Script = ScriptOutputGen.GenerateCreateProcedureScript((CreateProcedure)s);
        if (s is CreateFunction)
          s.Script = ScriptOutputGen.GenerateCreateFunctionScript((CreateFunction)s);
        if (s is CreateTrigger)
          s.Script = ScriptOutputGen.GenerateCreateTriggerScript((CreateTrigger)s);
        if (s is CreateView)
          s.Script = ScriptOutputGen.GenerateCreateViewScript((CreateView)s);
      }
      SaveScripts(statements);
    }
    

  }
}
