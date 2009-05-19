using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Model;

namespace DBInfo.Core.OutputGenerators {
  public interface IScriptFileOutputGenerator {
    void GenerateFileOutput(string OutputDir, Database db, IScriptOutputHandler OutputGenerator);
  }
}
