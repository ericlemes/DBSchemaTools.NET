﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.OutputGenerators;
using DBInfo.Core.Extractor;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using M=DBInfo.Core.Model;

namespace DBInfo.XMLDatabase {
  public class XMLDatabaseOutputGenerator : IOutputGenerator {

    private string _TablesDir = "Tables";
    public string TablesDir {
      get { return _TablesDir; }
      set { _TablesDir = value; }
    }

    private string _ForeignKeysDir = "ForeignKeys";
    public string ForeignKeysDir {
      get { return _ForeignKeysDir; }
      set { _ForeignKeysDir = value; }
    }

    private string _ConstraintsDir = "Constraints";
    public string ConstraintsDir {
      get { return _ConstraintsDir; }
      set { _ConstraintsDir = value; }
    }

    private string _FunctionsDir = "Functions";
    public string FunctionsDir {
      get { return _FunctionsDir; }
      set { _FunctionsDir = value; }
    }

    private string _IndexesDir = "Indexes";
    public string IndexesDir {
      get { return _IndexesDir; }
      set { _IndexesDir = value; }
    }

    private string _ProceduresDir = "Procedures";
    public string ProceduresDir {
      get { return _ProceduresDir; }
      set { _ProceduresDir = value; }
    }

    private string _SequencesDir = "Sequences";
    public string SequencesDir {
      get { return _SequencesDir; }
      set { _SequencesDir = value; }
    }

    private string _TriggersDir = "Triggers";
    public string TriggersDir {
      get { return _TriggersDir; }
      set { _TriggersDir = value; }
    }

    private string _ViewsDir = "Views";
    public string ViewsDir {
      get { return _ViewsDir; }
      set { _ViewsDir = value; }
    }  
  
    #region IOutputGenerator Members

    public GeneratorType Type {
      get { return GeneratorType.Generic; }
    }
    
    private string _OutputDir;

    public string OutputDir {
      get { return _OutputDir;}
      set { _OutputDir = value;}        
    }

    private string _OutputConnectionString;
    public string OutputConnectionString {
      get { return _OutputConnectionString; }
      set { _OutputConnectionString = value; }
    }

    public void GenerateOutput(DBInfo.Core.Model.Database db, List<DBObjectType> dataToGenerateOutput) {
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Tables)) {
        GenerateTables(db);
      }
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.CheckConstraints)) {
        GenerateConstraints(db);
      }
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.ForeignKeys)) {
        GenerateForeignKeys(db);
      }
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Indexes)) {
        GenerateIndexes(db);
      }
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Triggers)) {
        GenerateTriggers(db);
      }                        
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Procedures)) {
        GenerateProcedures(db);
      }
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Functions)) {
        GenerateFunctions(db);
      }
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Sequences)) {
        GenerateSequences(db);
      }
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Views)) {
        GenerateViews(db);
      }                        
    }
    
    private ColumnType getColumnType(M.DBColumnType type){
      if (type == M.DBColumnType.BigInt)
        return ColumnType.BigInt;
      else if (type == M.DBColumnType.Binary)
        return ColumnType.Binary;
      else if (type == M.DBColumnType.Bit)
        return ColumnType.Bit;
      else if (type == M.DBColumnType.Image)
        return ColumnType.Image;
      else if (type == M.DBColumnType.DateTime)
        return ColumnType.DateTime;
      else if (type == M.DBColumnType.Decimal)
        return ColumnType.Decimal;
      else if (type == M.DBColumnType.Float)
        return ColumnType.Float;
      else if (type == M.DBColumnType.UniqueIdentifier)
        return ColumnType.UniqueIdentifier;
      else if (type == M.DBColumnType.Integer)
        return ColumnType.Integer;
      else if (type == M.DBColumnType.Text)
        return ColumnType.Text;
      else if (type == M.DBColumnType.Money)
        return ColumnType.Money;
      else if (type == M.DBColumnType.Numeric)
        return ColumnType.Numeric;
      else if (type == M.DBColumnType.NVarchar)
        return ColumnType.NVarchar;      
      else if (type == M.DBColumnType.SmallDateTime)
        return ColumnType.SmallDateTime;
      else if (type == M.DBColumnType.SmallInt)
        return ColumnType.SmallInt;
      else if (type == M.DBColumnType.TimeStamp)
        return ColumnType.TimeStamp;
      else if (type == M.DBColumnType.TinyInt)
        return ColumnType.TinyInt;
      else if (type == M.DBColumnType.VarChar)
        return ColumnType.Varchar;
      else if (type == M.DBColumnType.Char)
        return ColumnType.Char;
      else if (type == M.DBColumnType.NChar)
        return ColumnType.NChar;
      else if (type == M.DBColumnType.Real)
        return ColumnType.Real;
      else if (type == M.DBColumnType.SmallMoney)
        return ColumnType.SmallMoney;
      else if (type == M.DBColumnType.VarBinary)
        return ColumnType.VarBinary;
      else if (type == M.DBColumnType.Xml)
        return ColumnType.Xml;
      else if (type == M.DBColumnType.NText)
        return ColumnType.NText;                
      else 
        throw new Exception(String.Format("Type not supported by XMLDatabase: {0}", type.ToString()));
    }

    private void GenerateTables(DBInfo.Core.Model.Database db) {
      if (!Directory.Exists(OutputDir + "\\" + TablesDir))
        Directory.CreateDirectory(OutputDir + "\\" + TablesDir);

      foreach (DBInfo.Core.Model.Table t in db.Tables) {
        DBInfo.XMLDatabase.CreateTable xmlTable = new DBInfo.XMLDatabase.CreateTable();
        xmlTable.TableName = t.TableName;
        xmlTable.HasIdentity = t.HasIdentity ? YesNo.Yes : YesNo.No;
        xmlTable.IdentitySeed = t.IdentitySeed.ToString();
        xmlTable.IdentityIncrement = t.IdentityIncrement.ToString();
        xmlTable.Columns = new Column[t.Columns.Count];

        foreach (DBInfo.Core.Model.Column c in t.Columns) {
          DBInfo.XMLDatabase.Column xmlCol = new DBInfo.XMLDatabase.Column();
          xmlCol.Name = c.Name;
          xmlCol.Type = getColumnType(c.Type);
          xmlCol.Size = c.Size.ToString();
          xmlCol.Precision = c.Precision.ToString();
          xmlCol.Scale = c.Scale.ToString();
          xmlCol.Nullable = c.IsNull ? YesNo.Yes : YesNo.No;
          xmlCol.IdentityColumn = c.IdentityColumn ? YesNo.Yes : YesNo.No;
          xmlCol.DefaultValue = c.DefaultValue;
          xmlCol.ConstraintDefaultName = c.ConstraintDefaultName;

          xmlTable.Columns[t.Columns.IndexOf(c)] = xmlCol;
        }

        StatementCollection stCol = new StatementCollection();
        stCol.Statement = new Statement[1];
        stCol.Statement[0] = xmlTable;
        generateXMLOutput(stCol, OutputDir + "\\" + TablesDir + "\\" + t.TableName + ".table.xml", true);
      }
    }

    private void GenerateConstraints(DBInfo.Core.Model.Database db) {
      if (!Directory.Exists(OutputDir + "\\" + ConstraintsDir))
        Directory.CreateDirectory(OutputDir + "\\" + ConstraintsDir);

      foreach (DBInfo.Core.Model.Table t in db.Tables) {
        DBInfo.XMLDatabase.CreatePrimaryKey xmlPK = new DBInfo.XMLDatabase.CreatePrimaryKey();
        xmlPK.TableName = t.TableName;
        xmlPK.PrimaryKeyName = t.PrimaryKeyName;
        xmlPK.Columns = new string[t.PrimaryKeyColumns.Count];

        foreach (DBInfo.Core.Model.Column c in t.PrimaryKeyColumns) {
          xmlPK.Columns[t.PrimaryKeyColumns.IndexOf(c)] = c.Name;
        }

        StatementCollection stCol = new StatementCollection();
        stCol.Statement = new Statement[t.CheckConstraints.Count + 1];                
        stCol.Statement[0] = xmlPK;        

        foreach (DBInfo.Core.Model.CheckConstraint ck in t.CheckConstraints) {
          CreateCheckConstraint xmlCK = new CreateCheckConstraint();
          xmlCK.TableName = t.TableName;
          xmlCK.CheckConstraintName = ck.Name;
          xmlCK.SourceCode = ck.Expression;
          stCol.Statement[t.CheckConstraints.IndexOf(ck) + 1] = xmlCK;
        }

        generateXMLOutput(stCol, OutputDir + "\\" + ConstraintsDir + "\\" + t.TableName + ".constraints.xml", true);
      }
    }
    
    private void GenerateForeignKeys(DBInfo.Core.Model.Database db){
      if (!Directory.Exists(OutputDir + "\\" + ForeignKeysDir))
        Directory.CreateDirectory(OutputDir + "\\" + ForeignKeysDir);
      
      foreach(DBInfo.Core.Model.Table t in db.Tables){
        StatementCollection xmlDB = new StatementCollection();
        xmlDB.Statement = new Statement[t.ForeignKeys.Count];
      
        foreach(DBInfo.Core.Model.ForeignKey fk in t.ForeignKeys){
          CreateForeignKey xmlFK = new CreateForeignKey();          
          xmlFK.TableName = t.TableName;
          xmlFK.ForeignKeyName = fk.ForeignKeyName;
          xmlFK.RefTableName = fk.RefTableName;
          xmlFK.DeleteCascade = fk.DeleteCascade ? YesNo.Yes : YesNo.No;
          xmlFK.UpdateCascade = fk.UpdateCascade ? YesNo.Yes : YesNo.No;
          xmlFK.Columns = new ForeignKeyColumn[fk.Columns.Count];
          foreach(DBInfo.Core.Model.ForeignKeyColumn c in fk.Columns){
            ForeignKeyColumn xmlFKCol = new ForeignKeyColumn();
            xmlFKCol.ColumnName = c.Column.Name;
            xmlFKCol.RefColumnName = c.RefColumn.Name;
            xmlFK.Columns[fk.Columns.IndexOf(c)] = xmlFKCol;
          }
          xmlDB.Statement[t.ForeignKeys.IndexOf(fk)] = xmlFK;
        }        
        
        generateXMLOutput(xmlDB, OutputDir + "\\" + ForeignKeysDir + "\\" + t.TableName + ".fk.xml", true);
      }
    }
    
    private void GenerateIndexes(DBInfo.Core.Model.Database db){
      if (!Directory.Exists(OutputDir + "\\" + IndexesDir))
        Directory.CreateDirectory(OutputDir + "\\" + IndexesDir);
        
      foreach(DBInfo.Core.Model.Table t in db.Tables){
        StatementCollection stCol = new StatementCollection();
        stCol.Statement = new Statement[t.Indexes.Count];        
      
        foreach(DBInfo.Core.Model.Index i in t.Indexes){
          CreateIndex xmlIdx = new CreateIndex();
          xmlIdx.TableName = t.TableName;
          xmlIdx.IndexName = i.IndexName;
          xmlIdx.Unique = i.Unique ? YesNo.Yes : YesNo.No;
          xmlIdx.Clustered = i.IsClustered ? YesNo.Yes : YesNo.No;
          xmlIdx.Columns = new IndexColumn[i.Columns.Count];
          
          foreach(DBInfo.Core.Model.IndexColumn icol in i.Columns){
            IndexColumn xmlIC = new IndexColumn();
            xmlIC.Order = icol.Order == DBInfo.Core.Model.IndexColumn.EnumOrder.Ascending ? SortOrder.Ascending : SortOrder.Descending;
            xmlIC.Name = icol.Column.Name;
            xmlIdx.Columns[i.Columns.IndexOf(icol)] = xmlIC;
          }        
          stCol.Statement[t.Indexes.IndexOf(i)] = xmlIdx;
        }
        
        generateXMLOutput(stCol, OutputDir + "\\" + IndexesDir + "\\" + t.TableName + ".indexes.xml", true);
      }
    }        
    
    private void GenerateTriggers(DBInfo.Core.Model.Database db){
      if (!Directory.Exists(OutputDir + "\\" + TriggersDir))
        Directory.CreateDirectory(OutputDir + "\\" + TriggersDir);
        
      foreach(DBInfo.Core.Model.Table t in db.Tables){
        foreach(DBInfo.Core.Model.Trigger tr in t.Triggers){
          CreateTrigger xmlTrigger = new CreateTrigger();
          xmlTrigger.TableName = t.TableName;
          xmlTrigger.TriggerName = tr.Name;
          xmlTrigger.SourceCode = tr.Body;
          
          StatementCollection stCol = new StatementCollection();
          stCol.Statement = new Statement[1];
          stCol.Statement[0] = xmlTrigger;
          
          generateXMLOutput(stCol, OutputDir + "\\" + TriggersDir + "\\" + t.TableName + "." + tr.Name + ".trigger.xml", true);
        }
      }
    }
    
    private ParameterDirection GetParameterDirection(DBInfo.Core.Model.ParamDirection d){
      if (d == DBInfo.Core.Model.ParamDirection.Input)
        return ParameterDirection.Input;
      else if (d == DBInfo.Core.Model.ParamDirection.Output)
        return ParameterDirection.Output;
      else if (d == DBInfo.Core.Model.ParamDirection.InputOutput)
        return ParameterDirection.InputOutput;
      else if (d == DBInfo.Core.Model.ParamDirection.ReturnValue)
        return ParameterDirection.ReturnValue;
      else
        throw new Exception(String.Format("Invalid parameter direction: {0}", d.ToString()));
    }
    
    private void GenerateProcedures(DBInfo.Core.Model.Database db){
      if (!Directory.Exists(OutputDir + "\\" + ProceduresDir))
        Directory.CreateDirectory(OutputDir + "\\" + ProceduresDir);    
    
      foreach(DBInfo.Core.Model.Procedure p in db.Procedures){
        DBInfo.XMLDatabase.CreateProcedure xmlProc = new DBInfo.XMLDatabase.CreateProcedure();
        xmlProc.Name = p.Name;
        xmlProc.SourceCode = p.Body;
        
        xmlProc.InputParameters = new Parameter[p.InputParameters.Count];
        
        foreach(DBInfo.Core.Model.Parameter parm in p.InputParameters){
          Parameter xmlParam = new Parameter();
          xmlParam.Name = parm.Name;
          xmlParam.Type = getColumnType(parm.Type);
          xmlParam.Size = parm.Size.ToString();
          xmlParam.Precision = parm.Precision.ToString();
          xmlParam.Scale = parm.Scale.ToString();
          xmlParam.Direction = GetParameterDirection(parm.Direction);
          xmlProc.InputParameters[p.InputParameters.IndexOf(parm)] = xmlParam;
        }
        
        xmlProc.RecordSets = new RecordSetDef[p.RecordSets.Count];
        foreach(DBInfo.Core.Model.RecordSet r in p.RecordSets){
          RecordSetDef xmlRS = new RecordSetDef();
          xmlRS.Parameters = new Parameter[r.Parameters.Count];
          foreach(DBInfo.Core.Model.Parameter parm in r.Parameters){
            Parameter xmlParam = new Parameter();
            xmlParam.Name = parm.Name;
            xmlParam.Type = getColumnType(parm.Type);
            xmlParam.Size = parm.Size.ToString();
            xmlParam.Precision = parm.Precision.ToString();
            xmlParam.Scale = parm.Scale.ToString();
            xmlParam.Direction = GetParameterDirection(parm.Direction);
            xmlRS.Parameters[r.Parameters.IndexOf(parm)] = xmlParam;
          }
          xmlProc.RecordSets[p.RecordSets.IndexOf(r)] = xmlRS;
        }

        StatementCollection stCol = new StatementCollection();
        stCol.Statement = new Statement[1];
        stCol.Statement[0] = xmlProc;
        generateXMLOutput(stCol, OutputDir + "\\" + ProceduresDir + "\\" + p.Name + ".proc.xml", true);
      }
    }
    
    private void GenerateFunctions(DBInfo.Core.Model.Database db){
      if (!Directory.Exists(OutputDir + "\\" + FunctionsDir))
        Directory.CreateDirectory(OutputDir + "\\" + FunctionsDir);
      
      foreach(DBInfo.Core.Model.Function f in db.Functions){
        CreateFunction xmlFunction = new CreateFunction();
        xmlFunction.Name = f.Name;
        xmlFunction.SourceCode = f.Body;
        
        StatementCollection stCol = new StatementCollection();
        stCol.Statement = new Statement[1];
        stCol.Statement[0] = xmlFunction;
        generateXMLOutput(stCol, OutputDir + "\\" + FunctionsDir + "\\" + f.Name + ".function.xml", true); 
      }
    }
    
    private void GenerateSequences(DBInfo.Core.Model.Database db){
      if (!Directory.Exists(OutputDir + "\\" + SequencesDir))
        Directory.CreateDirectory(OutputDir + "\\" + SequencesDir);
        
      foreach(DBInfo.Core.Model.Sequence s in db.Sequences){
        CreateSequence xmlSequence = new CreateSequence();
        xmlSequence.Name = s.SequenceName;
        xmlSequence.Initial = s.Initial.ToString();
        xmlSequence.MinValue = s.MinValue.ToString();
        xmlSequence.MaxValue = s.MaxValue.ToString();
        xmlSequence.Increment = s.Increment.ToString();
        xmlSequence.CycleOnLimit = s.CycleOnLimit ? YesNo.Yes : YesNo.No;
        
        StatementCollection stCol = new StatementCollection();
        stCol.Statement = new Statement[1];
        stCol.Statement[0] = xmlSequence;
        generateXMLOutput(stCol, OutputDir + "\\" + SequencesDir + "\\" + s.SequenceName + ".sequence.xml", false);
      }
    }
    
    private void GenerateViews(DBInfo.Core.Model.Database db){
      if (!Directory.Exists(OutputDir + "\\" + ViewsDir))
        Directory.CreateDirectory(OutputDir + "\\" + ViewsDir);
        
      foreach ( DBInfo.Core.Model.View v in db.Views){
        CreateView xmlView = new CreateView();
        xmlView.Name = v.Name;
        xmlView.SourceCode = v.Body;
        
        StatementCollection stCol = new StatementCollection();
        stCol.Statement = new Statement[1];
        stCol.Statement[0] = xmlView;
        generateXMLOutput(stCol, OutputDir + "\\" + ViewsDir + "\\" + v.Name + ".view.xml", true);
      }
    }

    private void generateXMLOutput(StatementCollection col, string FileName, bool generateCDATAForSourceCode) {
      MemoryStream ms = new MemoryStream();
      
      if (generateCDATAForSourceCode){      
        //Change the contents of all nodes named "SourceCode" to CDATA. There's no option to do this automatticaly in XMLSerializer
        XmlSerializer ser = new XmlSerializer(typeof(StatementCollection), "http://dbinfo.sourceforge.net/Schemas/DBInfo.xsd");
        ser.Serialize(ms, col);

        FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.ReadWrite);
        fixSourceCodeNode(ms, fs);

        ms.Close();
        fs.Close();        
      }else {
        FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.ReadWrite);
        XmlSerializer ser = new XmlSerializer(typeof(StatementCollection), "http://dbinfo.sourceforge.net/Schemas/DBInfo.xsd");
        ser.Serialize(fs, col);
        fs.Close();              
      }
    }
    
    private void findSourceCodeNodes(List<XmlNode> SourceCodeNodes, XmlNode searchNode){
      if (searchNode.Name == "SourceCode")
        SourceCodeNodes.Add(searchNode);
      foreach(XmlNode childNode in searchNode.ChildNodes){
        findSourceCodeNodes(SourceCodeNodes, childNode);
      }
    }
    
    private void fixSourceCodeNode(Stream inputStream, Stream outputStream){
      inputStream.Seek(0, SeekOrigin.Begin);
      XmlDocument xml = new XmlDocument();
      xml.Load(inputStream); 
      List<XmlNode> SourceCodeNodes = new List<XmlNode>();
      findSourceCodeNodes(SourceCodeNodes, xml.DocumentElement);      
      
      foreach(XmlNode SourceCodeNode in SourceCodeNodes){            
        XmlNode newNode = xml.CreateCDataSection(SourceCodeNode.ChildNodes[0].Value);
        SourceCodeNode.RemoveAll();
        SourceCodeNode.AppendChild(newNode);            
      }
      xml.Save(outputStream);      
    }

    #endregion
  }
}
