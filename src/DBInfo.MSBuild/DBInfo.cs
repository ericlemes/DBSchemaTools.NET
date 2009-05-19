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
    private string _ExtractorClass;
    [Required]    
    public string ExtractorClass{
      get { return _ExtractorClass;}
      set { _ExtractorClass = value;}
    }
  
    private string _DBExtractorClass;    
    public string dbextractorclass{
      get {return _DBExtractorClass;}
      set {_DBExtractorClass = value;}
    }
    
    private string _OutputGeneratorClass = "";
    [Required]
    public string OutputGeneratorClass{
      get { return _OutputGeneratorClass; }
      set { _OutputGeneratorClass = value; }
    }
    
    private string _ScriptOutputGeneratorClass = "";
    public string ScriptOutputGeneratorClass{
      get { return _ScriptOutputGeneratorClass; }
      set { _ScriptOutputGeneratorClass = value; }
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
    
    private string _ScriptFileOutputGenerator;
    public string ScriptFileOutputGenerator{
      get { return _ScriptFileOutputGenerator;}
      set { _ScriptFileOutputGenerator = value;}
    }

    private ITaskItem[] _InputFiles;    
    public ITaskItem[] InputFiles{
      get { return _InputFiles; }
      set { _InputFiles = value; }
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
      
      Type extractorClass = Type.GetType(_ExtractorClass);
      if (extractorClass == null)
        throw new Exception(String.Format("Couldn't create instance for type {0}", _ExtractorClass));
      IExtractor extractor = (IExtractor)Activator.CreateInstance(extractorClass);
      
      if (extractor.Type == ExtractorType.Database){        
        Type dbExtractorClass = Type.GetType(_DBExtractorClass);
        if (dbExtractorClass == null)
          throw new Exception(String.Format("Couldn't create instance for type {0}", _DBExtractorClass));
        IDatabaseExtractor dbExtractor = (IDatabaseExtractor)Activator.CreateInstance(dbExtractorClass);      
        
        //DatabaseExtractor dbe = new DatabaseExtractor();
        
        extractor.Extractor = dbExtractor;
        extractor.InputConnectionString = _InputConnectionString;       
        /*if (!String.IsNullOrEmpty(_TableNames)){
          string[] names = _TableNames.Split(';');
          foreach(string s in names){
            dbe.TableNames.Add(s);
          }
        }*/
      }
      else {
        if (_InputFiles == null)
          throw new Exception("InputFiles must be specified.");
        foreach (ITaskItem file in _InputFiles) {
          extractor.InputFiles.Add(file.ItemSpec);
        }
      }
      
      Type outputGenClass = Type.GetType(_OutputGeneratorClass);
      if (outputGenClass == null)
        throw new Exception(String.Format("Couldn't create instance for type {0}", _OutputGeneratorClass));
      IOutputGenerator gen = (IOutputGenerator)Activator.CreateInstance(outputGenClass);
            
      if (gen.Type == GeneratorType.Script){
        Type scriptOutputGenClass = Type.GetType(_ScriptOutputGeneratorClass);      
      
        if (scriptOutputGenClass == null)
          throw new Exception(String.Format("Couldn't create instance for type {0}", _ScriptOutputGeneratorClass));
        IScriptOutputHandler scriptOutputGen = (IScriptOutputHandler)Activator.CreateInstance(scriptOutputGenClass);
        ((IScriptOutputGenerator)gen).ScriptOutputGen = scriptOutputGen;

        if (String.IsNullOrEmpty(ScriptFileOutputGenerator))
          throw new Exception(String.Format("For output file type you must specify ScriptFileOutputGenerator"));
        Type scriptFileOutputGeneratorType = Type.GetType(ScriptFileOutputGenerator);
        if (scriptFileOutputGeneratorType == null)
          throw new Exception(String.Format("Couldn't create instance for type {0}", ScriptFileOutputGenerator));
        ((IScriptOutputGenerator)gen).ScriptFileOutputGenerator = (IScriptFileOutputGenerator)Activator.CreateInstance(scriptFileOutputGeneratorType);
      }

      Database db = extractor.Extract(dataToExtract);

      gen.OutputDir = OutputDir;      
      gen.GenerateOutput(db, dataToGenerateOutput);
      
      return true;
    }
  }
}
