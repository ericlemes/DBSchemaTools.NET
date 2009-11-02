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
using DBInfo.Core.Statement;
using DBInfo.Core;

namespace DBInfo.MSBuild {

  public enum ExtractionType{
    Database,
    Script
  }

  public class DBInfo:Task {        
    [Required]    
    public string extractiontype{
      get; set;      
    }
  
    private string _DBExtractorClass;    
    public string dbextractorclass{
      get {return _DBExtractorClass;}
      set {_DBExtractorClass = value;}
    }

    public string ScriptExtractorClass{
      get; set;
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
      
      ExtractionType exType;
      if(this.extractiontype.ToLower() == "database")
        exType = ExtractionType.Database;
      else if (this.extractiontype == "script")
        exType = ExtractionType.Script;
      else
        throw new Exception(String.Format("Invalid extraction type: {0}", this.extractiontype));
                        
      Database db = null;
      List<BaseStatement> statementCol = null;
      if (exType == ExtractionType.Database){  
        if (String.IsNullOrEmpty(_DBExtractorClass))
          throw new Exception("For database extraction type, dbextractorclass is required");
                  
        Type dbExtractorClass = Type.GetType(_DBExtractorClass);
        if (dbExtractorClass == null)
          throw new Exception(String.Format("Couldn't create instance for type {0}", _DBExtractorClass));
        IDatabaseExtractor dbExtractor = (IDatabaseExtractor)Activator.CreateInstance(dbExtractorClass);

        DatabaseExtractor extractor = new DatabaseExtractor();        
        extractor.Extractor = dbExtractor;
        extractor.InputConnectionString = _InputConnectionString;
        
        db = extractor.Extract(dataToExtract);
      }
      else {
        if (String.IsNullOrEmpty(ScriptExtractorClass))
          throw new Exception("For script extraction type, scriptextractorclass is required");      
      
        Type scriptExtractorClass = Type.GetType(ScriptExtractorClass);
        if (scriptExtractorClass == null)
          throw new Exception(String.Format("Couldn't create instance for type {0}", scriptExtractorClass));

        IScriptExtractor scriptExtractor = (IScriptExtractor)Activator.CreateInstance(scriptExtractorClass);      
      
        if (_InputFiles == null)
          throw new Exception("InputFiles must be specified.");
                
        foreach (ITaskItem file in _InputFiles) {
          scriptExtractor.InputFiles.Add(file.ItemSpec);
        }
        statementCol = scriptExtractor.Extract(dataToExtract);
      }           
      
      Type outputGenClass = Type.GetType(_OutputGeneratorClass);
      if (outputGenClass == null)
        throw new Exception(String.Format("Couldn't create instance for type {0}", _OutputGeneratorClass));
      IOutputGenerator gen = (IOutputGenerator)Activator.CreateInstance(outputGenClass);
            
      if (gen.ExpectedInputType == ExpectedInputType.StatementCollection){
        if (statementCol == null){
          DatabaseToStatementCollectionConverter conv = new DatabaseToStatementCollectionConverter();
          statementCol = conv.Convert(db);
        }
      
        Type scriptOutputGenClass = Type.GetType(_ScriptOutputGeneratorClass);      
      
        if (((IStatementCollectionOutputGenerator)gen).RequiresScriptOutputHandler){
          if (scriptOutputGenClass == null)
            throw new Exception(String.Format("Couldn't create instance for type {0}", _ScriptOutputGeneratorClass));
          IScriptOutputHandler scriptOutputGen = (IScriptOutputHandler)Activator.CreateInstance(scriptOutputGenClass);
          ((IStatementCollectionOutputGenerator)gen).ScriptOutputGen = scriptOutputGen;
        }

        if (String.IsNullOrEmpty(ScriptFileOutputGenerator))
          throw new Exception(String.Format("For output file type you must specify ScriptFileOutputGenerator"));
        Type scriptFileOutputGeneratorType = Type.GetType(ScriptFileOutputGenerator);
        if (scriptFileOutputGeneratorType == null)
          throw new Exception(String.Format("Couldn't create instance for type {0}", ScriptFileOutputGenerator));
        ((IStatementCollectionOutputGenerator)gen).ScriptFileOutputGenerator = (IScriptFileOutputGenerator)Activator.CreateInstance(scriptFileOutputGeneratorType);

        gen.OutputDir = OutputDir;
        ((IStatementCollectionOutputGenerator)gen).GenerateOutput(statementCol, dataToGenerateOutput);
      }
      else {
        StatementCollectionToDatabaseConverter conv = new StatementCollectionToDatabaseConverter();
        db = conv.Convert(statementCol);
      
        gen.OutputDir = OutputDir;
        ((IDBSchemaOutputGenerator)gen).GenerateOutput(db, dataToGenerateOutput);
      }
                  
      return true;
    }
  }
}
