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
using DBInfo.Core.Model;

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
    [Required]
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
    [Required]
    public string OutputType {
      get { return _OutputType;}
      set { _OutputType = value;}
    }
    
    private string _OutputDir;
    public string OutputDir{
      get { return _OutputDir;}
      set { _OutputDir = value;}
    }
    
    private string _DataToExtract;
    [Required]
    public string DataToExtract{
      get { return _DataToExtract;}
      set { _DataToExtract = value;}
    }
    
    private string _DataToGenerateOutput;
    [Required]
    public string DataToGenerateOutput{
      get { return _DataToGenerateOutput;}
      set { _DataToGenerateOutput = value;}
    }
    
    private string _TableNames;
    public string TableNames{
      get { return _TableNames;}
      set { _TableNames = value;}
    }

    private EnumType DescriptionToEnum<EnumType>(string description) where EnumType : new() {      
      //Case-insensitive search.
      string allTypes = "";
      foreach (FieldInfo fi in typeof(EnumType).GetFields()) {                
        if (allTypes != "")
          allTypes += ", ";
        allTypes += fi.Name;
        if (description.ToLower() == fi.Name.ToLower()) {          
          return (EnumType)fi.GetValue(fi);          
        }
      }
      throw new Exception(String.Format("Invalid database object type: {0}. The following types are valid: {1}", description, allTypes));
    }
    
    private List<DBObjectType> GetDataToExtractEnum(){
      List<DBObjectType> l = new List<DBObjectType>();
      string[] splited = _DataToExtract.Split(';');
      foreach(string s in splited){
        l.Add(DescriptionToEnum<DBObjectType>(s));
      }
      return l;
    }

    private List<DBObjectType> GetDataToGenerateOutputEnum() {
      List<DBObjectType> l = new List<DBObjectType>();
      string[] splited = _DataToGenerateOutput.Split(';');
      foreach (string s in splited) {
        l.Add(DescriptionToEnum<DBObjectType>(s));
      }
      return l;
    }

    public override bool Execute() {              
      List<DBObjectType> dataToExtract = GetDataToExtractEnum();
      List<DBObjectType> dataToGenerateOutput = GetDataToGenerateOutputEnum();
        
      Type extractorClass = Type.GetType(_DBExtractorClass);
      if (extractorClass == null)
        throw new Exception(String.Format("Couldn't create instance for type {0}", _DBExtractorClass));
      IDBInfoExtractor extractor = (IDBInfoExtractor)Activator.CreateInstance(extractorClass);
      IScriptsOutputGenerator outputGenerator = (IScriptsOutputGenerator)Activator.CreateInstance(Type.GetType(_OutputGeneratorClass));
      
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
      if (!String.IsNullOrEmpty(_TableNames)){
        string[] names = _TableNames.Split(';');
        foreach(string s in names){
          dbe.TableNames.Add(s);
        }
      }      
      Database db = dbe.Extract(dataToExtract);
      
      Type generatorClass = Type.GetType(_OutputGeneratorClass);
      if (generatorClass == null)
        throw new Exception(String.Format("Couldn't create instance for type {0}", _OutputGeneratorClass));
      IScriptsOutputGenerator generator = (IScriptsOutputGenerator)Activator.CreateInstance(generatorClass);           
      
      ScriptOutputGenerator gen = new ScriptOutputGenerator();
      gen.OutputGen = generator;
      if (OutputType == "database")
        gen.OutputType = InputOutputType.Database;
      else if (OutputType == "file")
        gen.OutputType = InputOutputType.File;
      else
        throw new Exception(String.Format("Invalid output type: {0}.", OutputType));            
      gen.OutputDir = OutputDir;
      gen.OutputConnectionString = OutputConnectionString;
      gen.GenerateOutput(db, dataToGenerateOutput);
      
      return true;
    }
  }
}
