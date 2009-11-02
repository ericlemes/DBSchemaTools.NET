using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Statement;

namespace DBInfo.Core.Extractor {
  public interface IScriptExtractor {    
    List<string> InputFiles {
      get;
    }  
  
    List<BaseStatement> Extract(List<DBObjectType> dataToExtract);
  }
}
