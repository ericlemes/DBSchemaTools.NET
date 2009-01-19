using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.IO;
using DBInfo.Core.Extractor;
using DBInfo.Core.OutputGenerators;
using System.Reflection;

namespace DBInfo.MSBuild {
  public class DBInfo:Task {    
    private string _DBExtractorClass;
    [Required]
    public string dbextractorclass{
      get {return _DBExtractorClass;}
      set {_DBExtractorClass = value;}
    }
    
    private string _OutputGeneratorClass;
    [Required]
    public string OutputGeneratorClass{
      get { return _OutputGeneratorClass; }
      set { _OutputGeneratorClass = value; }
    }
    
    private string _InputType;
    public string InputType{
      get { return _InputType;}
      set {_InputType = value;}
    }
    
    private string _InputDir;
    public string InputDir{
      get { return _InputDir;}
      set { _InputDir = value;}
    }
    
    private string _InputConnectionString;
    public string InputConnectionString {
      get { return _InputConnectionString;}
      set { _InputConnectionString = value;}
    }
    
    private string _OutputConnectionString;
    public string OutputConnectionString {
      get { return _OutputConnectionString;}
      set { _OutputConnectionString = value;}
    }
    
    private string _OutputType;
    public string OutputType {
      get { return _OutputType;}
      set { _OutputType = value;}
    }
    
    private string _OutputDir;
    public string OutputDir{
      get { return _OutputDir;}
      set { _OutputDir = value;}
    }

    public override bool Execute() {
      /*if (_InputType == InputOutputType.File && !Directory.Exists(_InputDir))
        throw new Exception(String.Format("The input directory don't exists: {0}", _InputDir));
        
      if (_InputType == InputOutputType.Database && !Directory.Exists(_InputConnectionString))
        throw new Exception(String.Format("The input connection string don't exists: {0}", _InputConnectionString));        
    
      if (_OutputType == InputOutputType.File && !Directory.Exists(_OutputDir))
        throw new Exception(String.Format("The output directory not exists: {0}", _OutputDir));
        
      if (_OutputType == InputOutputType.Database && !Directory.Exists(_OutputConnectionString))
        throw new Exception(String.Format("The output connection string not exists: {0}", _OutputConnectionString));        */
        
      string[] extractorClassInfo = _DBExtractorClass.Split(',');
      if (extractorClassInfo.Length != 2)
        throw new Exception(String.Format("Class name must be in the format 'Namespace.ClassName, Assembly'"));
        
      Assembly.Load(extractorClassInfo[1].Trim());
      
      Type extractorClass = Type.GetType(extractorClassInfo[0].Trim());
      if (extractorClass == null)
        throw new Exception(String.Format("Couldn't create instance for type {0}", _DBExtractorClass));
      IDBInfoExtractor extractor = (IDBInfoExtractor)Activator.CreateInstance(extractorClass);
      IOutputGenerator outputGenerator = (IOutputGenerator)Activator.CreateInstance(Type.GetType(_OutputGeneratorClass));
      
      DBInfoExtractor dbe = new DBInfoExtractor();
      dbe.Extractor = extractor;
      if (InputType == "database")
        dbe.InputType = InputOutputType.Database;
      else if (InputType == "file")
        dbe.InputType = InputOutputType.File;
      else
        throw new Exception(String.Format("Invalid input type: {0}.", InputType));      
      dbe.InputConnectionString = _InputConnectionString;
      dbe.InputDir = _InputDir;
      dbe.Extract();
      
      return true;
    }
  }
}
