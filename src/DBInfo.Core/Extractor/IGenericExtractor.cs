using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Model;

namespace DBInfo.Core.Extractor {
  public interface IGenericExtractor {
    List<string> InputFiles {
      get;
    }

    Database Extract(List<DBObjectType> dataToExtract);  
  }
}
