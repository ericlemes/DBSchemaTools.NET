using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Extractor;
using DBInfo.Core.Model;

namespace DBInfo.Core.OutputGenerators {
  public enum GeneratorType{
    Generic,
    Script
  }

  public interface IOutputGenerator {
    GeneratorType Type{
      get;
    }
    
    IScriptOutputGenerator ScriptOutputGen{
      get;
      set;
    }   
    
    IScriptFileOutputGenerator ScriptFileOutputGenerator{
      get;
      set;
    }
    
    string OutputDir{
      get;
      set;
    }
   
    void GenerateOutput(Database db, List<DBObjectType> dataToGenerateOutput);
  }
}
