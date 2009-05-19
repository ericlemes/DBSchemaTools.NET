using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBInfo.Core.OutputGenerators {
  public interface IScriptOutputGenerator : IOutputGenerator {
    IScriptOutputHandler ScriptOutputGen {
      get;
      set;
    }

    IScriptFileOutputGenerator ScriptFileOutputGenerator {
      get;
      set;
    }
  }
}
