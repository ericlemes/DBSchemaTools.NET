using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Model;
using DBInfo.Core.Statement;

namespace DBInfo.Core.OutputGenerators {
  public interface IScriptFileOutputGenerator {
    void GenerateFileOutput(string OutputDir, List<BaseStatement> statements, IScriptOutputHandler OutputGenerator);
  }
}
