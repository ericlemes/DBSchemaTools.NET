using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Model;

namespace DBInfo.Core.Extractor {
  public enum ExtractorType{
    Generic,
    Database
  }

  public interface IExtractor {
    ExtractorType Type{
      get;
    }
    
    IDatabaseExtractor Extractor{
      get;
      set;
    }       
    
    string InputConnectionString{
      get;
      set;      
    }    
    
    List<string> InputFiles{
      get;
    }
    
    Database Extract(List<DBObjectType> dataToExtract);
  }
}
