using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.IO;
using DBInfo.Core.Extractor;

namespace DBInfo.MSBuild {
  public class DBInfo:Task {
  
    public enum OutputTypeEnum{
      File,
      DirectToDB
    }
  
    private string _DBExtractorClass;
    [Required]
    public string DBExtractorClass{
      get {return _DBExtractorClass;}
      set {_DBExtractorClass = value;}
    }
    
    private string _OutputGeneratorClass;
    [Required]
    public string OutputGeneratorClass{
      get { return _DBExtractorClass;}
      set {_DBExtractorClass = value;}
    }
    
    private string _ExtractorConnectionString;
    public string ExtractorConnectionString {
      get { return _ExtractorConnectionString;}
      set { _ExtractorConnectionString = value;}
    }
    
    private string _OutputConnectionString;
    public string OutputConnectionString {
      get { return _OutputConnectionString;}
      set { _OutputConnectionString = value;}
    }
    
    private OutputTypeEnum _OutputType;
    public OutputTypeEnum OutputType{
      get { return _OutputType;}
      set { _OutputType = value;}
    }
    
    private string _OutputDir;
    public string OutputDir{
      get { return _OutputDir;}
      set { _OutputDir = value;}
    }

    public override bool Execute() {
      if (_OutputType == OutputTypeEnum.File && !Directory.Exists(_OutputDir))
        throw new Exception(String.Format("The output directory not exists: {0}", _OutputDir));
      
      IDBInfoExtractor extractor = (IDBInfoExtractor)Activator.CreateInstance(Type.GetType(_DBExtractorClass));
      
      
      return true;
    }
  }
}
