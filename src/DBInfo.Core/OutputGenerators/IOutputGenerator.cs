using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Extractor;
using DBInfo.Core.Model;
using DBInfo.Core.Statement;

namespace DBInfo.Core.OutputGenerators {
  public enum ExpectedInputType{
    DatabaseSchema,
    StatementCollection
  }

  public interface IOutputGenerator {
    ExpectedInputType ExpectedInputType {
      get;
    }   
    
    string OutputDir{
      get;
      set;
    }        
  }  
  
  public interface IDBSchemaOutputGenerator : IOutputGenerator{
    void GenerateOutput(Database db, List<DBObjectType> dataToGenerateOutput);
  }
  
  public interface IStatementCollectionOutputGenerator : IOutputGenerator{
    IScriptOutputHandler ScriptOutputGen {
      get;
      set;
    }

    IScriptFileOutputGenerator ScriptFileOutputGenerator {
      get;
      set;
    }  
    
    bool RequiresScriptOutputHandler{
      get;
    }
    
    void GenerateOutput(List<BaseStatement> statements, List<DBObjectType> dataToGenerateOutput);
  }
}
