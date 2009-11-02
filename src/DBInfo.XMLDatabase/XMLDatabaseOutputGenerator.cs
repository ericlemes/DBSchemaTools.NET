using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.OutputGenerators;
using DBInfo.Core.Extractor;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using M=DBInfo.Core.Model;
using DBInfo.Core.Statement;

namespace DBInfo.XMLDatabase {
  public class XMLDatabaseOutputGenerator : IStatementCollectionOutputGenerator {

    public bool RequiresScriptOutputHandler {
      get { return false; }
    }

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

    public ExpectedInputType ExpectedInputType {
      get { return ExpectedInputType.StatementCollection; }
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

    private void GenerateTables(List<DBInfo.Core.Statement.CreateTable> createTableList) {
      if (!Directory.Exists(OutputDir + "\\" + TablesDir))
        Directory.CreateDirectory(OutputDir + "\\" + TablesDir);            
        
      foreach(DBInfo.Core.Statement.CreateTable createTable in createTableList){
        DBInfo.Core.Model.Table t = createTable.Table;
        
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

    private void GenerateConstraints(List<DBInfo.Core.Statement.CreateCheckConstraint> createCheckList,
      List<DBInfo.Core.Statement.CreatePrimaryKey> createPKList) {
      if (!Directory.Exists(OutputDir + "\\" + ConstraintsDir))
        Directory.CreateDirectory(OutputDir + "\\" + ConstraintsDir);

      List<string> tableNames =
        (from DBInfo.Core.Statement.CreateCheckConstraint ccc in createCheckList
         select ccc.CheckConstraint.TableName).Union<string>(
          from DBInfo.Core.Statement.CreatePrimaryKey cpk in createPKList
          select cpk.Table.TableName).Distinct<string>().ToList<string>();  

      foreach (string tableName in tableNames) {
        DBInfo.Core.Statement.CreatePrimaryKey createPK = 
          (from DBInfo.Core.Statement.CreatePrimaryKey cpk in createPKList
           where cpk.Table.TableName == tableName
           select cpk).FirstOrDefault<DBInfo.Core.Statement.CreatePrimaryKey>();
           
        List<DBInfo.Core.Statement.CreateCheckConstraint> createCheckStatements = 
          (from DBInfo.Core.Statement.CreateCheckConstraint ccc in createCheckList
           where ccc.CheckConstraint.TableName == tableName
           select ccc).ToList<DBInfo.Core.Statement.CreateCheckConstraint>();

        int StatementCount = 0;
        if (createPK != null)
          StatementCount++;
        StatementCount += createCheckStatements.Count;          
        StatementCollection stCol = new StatementCollection();
        stCol.Statement = new Statement[StatementCount];
        int count = 0;
           
        if (createPK != null){
          DBInfo.XMLDatabase.CreatePrimaryKey xmlPK = new DBInfo.XMLDatabase.CreatePrimaryKey();
          xmlPK.TableName = createPK.Table.TableName;
          xmlPK.PrimaryKeyName = createPK.Table.PrimaryKeyName;
          xmlPK.Columns = new string[createPK.Table.PrimaryKeyColumns.Count];

          foreach (string c in createPK.Table.PrimaryKeyColumns) {
            xmlPK.Columns[createPK.Table.PrimaryKeyColumns.IndexOf(c)] = c;
          }
          stCol.Statement[0] = xmlPK;  
          count++;
        }                      
        
        foreach (DBInfo.Core.Statement.CreateCheckConstraint ccc in createCheckStatements) {
          CreateCheckConstraint xmlCK = new CreateCheckConstraint();
          xmlCK.TableName = ccc.CheckConstraint.TableName;
          xmlCK.CheckConstraintName = ccc.CheckConstraint.CheckConstraintName;
          xmlCK.SourceCode = ccc.CheckConstraint.Expression;
          stCol.Statement[count] = xmlCK;
          count++;
        }

        generateXMLOutput(stCol, OutputDir + "\\" + ConstraintsDir + "\\" + tableName + ".constraints.xml", true);
      }
    }
    
    private void GenerateForeignKeys(List<DBInfo.Core.Statement.CreateForeignKey> createFKList){
      if (!Directory.Exists(OutputDir + "\\" + ForeignKeysDir))
        Directory.CreateDirectory(OutputDir + "\\" + ForeignKeysDir);

      List<string> tableNames =
        (from DBInfo.Core.Statement.CreateForeignKey cfk in createFKList
         select cfk.ForeignKey.TableName).Distinct<string>().ToList<string>();      
      
      foreach(string tableName in tableNames){
        List<DBInfo.Core.Statement.CreateForeignKey> createFKByTable =
          (from DBInfo.Core.Statement.CreateForeignKey cfk in createFKList
           where cfk.ForeignKey.TableName == tableName
           select cfk).ToList<DBInfo.Core.Statement.CreateForeignKey>();
        
        int count = 0;
        foreach(DBInfo.Core.Statement.CreateForeignKey cfk in createFKByTable){
          StatementCollection xmlDB = new StatementCollection();
          xmlDB.Statement = new Statement[createFKByTable.Count];
                  
          CreateForeignKey xmlFK = new CreateForeignKey();          
          xmlFK.TableName = cfk.ForeignKey.TableName;
          xmlFK.ForeignKeyName = cfk.ForeignKey.ForeignKeyName;
          xmlFK.RefTableName = cfk.ForeignKey.RefTableName;
          xmlFK.DeleteCascade = cfk.ForeignKey.DeleteCascade ? YesNo.Yes : YesNo.No;
          xmlFK.UpdateCascade = cfk.ForeignKey.UpdateCascade ? YesNo.Yes : YesNo.No;
          xmlFK.Columns = new ForeignKeyColumn[cfk.ForeignKey.Columns.Count];
          foreach(DBInfo.Core.Model.ForeignKeyColumn c in cfk.ForeignKey.Columns){
            ForeignKeyColumn xmlFKCol = new ForeignKeyColumn();
            xmlFKCol.ColumnName = c.Column;
            xmlFKCol.RefColumnName = c.RefColumn;
            xmlFK.Columns[cfk.ForeignKey.Columns.IndexOf(c)] = xmlFKCol;
          }
          xmlDB.Statement[count] = xmlFK;          
          count++;
          
          generateXMLOutput(xmlDB, OutputDir + "\\" + ForeignKeysDir + "\\" + tableName + ".fk.xml", true);
        }
      }
    }
    
    private void GenerateIndexes(List<DBInfo.Core.Statement.CreateIndex> indexList){
      if (!Directory.Exists(OutputDir + "\\" + IndexesDir))
        Directory.CreateDirectory(OutputDir + "\\" + IndexesDir);

      List<string> tableNames =
        (from DBInfo.Core.Statement.CreateIndex ci in indexList
         select ci.Index.TableName).Distinct<string>().ToList<string>();              
        
      foreach(string tableName in tableNames){
        List<DBInfo.Core.Statement.CreateIndex> createIndex =
          (from DBInfo.Core.Statement.CreateIndex ci in indexList
           where ci.Index.TableName == tableName
           select ci).ToList<DBInfo.Core.Statement.CreateIndex>();
      
        StatementCollection stCol = new StatementCollection();
        stCol.Statement = new Statement[createIndex.Count];        
      
        int count = 0;
        foreach(DBInfo.Core.Statement.CreateIndex ci in createIndex){
          CreateIndex xmlIdx = new CreateIndex();
          xmlIdx.TableName = ci.Index.TableName;
          xmlIdx.IndexName = ci.Index.IndexName;
          xmlIdx.Unique = ci.Index.Unique ? YesNo.Yes : YesNo.No;
          xmlIdx.Clustered = ci.Index.IsClustered ? YesNo.Yes : YesNo.No;
          xmlIdx.Columns = new IndexColumn[ci.Index.Columns.Count];
          
          foreach(DBInfo.Core.Model.IndexColumn icol in ci.Index.Columns){
            IndexColumn xmlIC = new IndexColumn();
            xmlIC.Order = icol.Order == DBInfo.Core.Model.IndexColumn.EnumOrder.Ascending ? SortOrder.Ascending : SortOrder.Descending;
            xmlIC.Name = icol.Column;
            xmlIdx.Columns[ci.Index.Columns.IndexOf(icol)] = xmlIC;
          }        
          stCol.Statement[count] = xmlIdx;
          count++;
        }
        
        generateXMLOutput(stCol, OutputDir + "\\" + IndexesDir + "\\" + tableName + ".indexes.xml", true);
      }
    }        
    
    private void GenerateTriggers(List<DBInfo.Core.Statement.CreateTrigger> triggerList){
      if (!Directory.Exists(OutputDir + "\\" + TriggersDir))
        Directory.CreateDirectory(OutputDir + "\\" + TriggersDir);

      List<string> tableNames =
        (from DBInfo.Core.Statement.CreateTrigger ct in triggerList
         select ct.Trigger.TableName).Distinct<string>().ToList<string>();             
        
      foreach(string tableName in tableNames){
        List<DBInfo.Core.Statement.CreateTrigger> createTrigger =
          (from DBInfo.Core.Statement.CreateTrigger ct in triggerList
           where ct.Trigger.TableName == tableName
           select ct).ToList<DBInfo.Core.Statement.CreateTrigger>();
      
        foreach(DBInfo.Core.Statement.CreateTrigger tr in createTrigger){
          CreateTrigger xmlTrigger = new CreateTrigger();
          xmlTrigger.TableName = tr.Trigger.TableName;
          xmlTrigger.TriggerName = tr.Trigger.TriggerName;
          xmlTrigger.SourceCode = tr.Trigger.Body;
          
          StatementCollection stCol = new StatementCollection();
          stCol.Statement = new Statement[1];
          stCol.Statement[0] = xmlTrigger;
          
          generateXMLOutput(stCol, OutputDir + "\\" + TriggersDir + "\\" + tableName + "." + tr.Trigger.TriggerName + ".trigger.xml", true);
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
    
    private void GenerateProcedures(List<DBInfo.Core.Statement.CreateProcedure> procList){
      if (!Directory.Exists(OutputDir + "\\" + ProceduresDir))
        Directory.CreateDirectory(OutputDir + "\\" + ProceduresDir);    
    
      foreach(DBInfo.Core.Statement.CreateProcedure p in procList){
        DBInfo.XMLDatabase.CreateProcedure xmlProc = new DBInfo.XMLDatabase.CreateProcedure();
        xmlProc.Name = p.Procedure.Name;
        xmlProc.SourceCode = p.Procedure.Body;
        
        xmlProc.InputParameters = new Parameter[p.Procedure.InputParameters.Count];
        
        foreach(DBInfo.Core.Model.Parameter parm in p.Procedure.InputParameters){
          Parameter xmlParam = new Parameter();
          xmlParam.Name = parm.Name;
          xmlParam.Type = getColumnType(parm.Type);
          xmlParam.Size = parm.Size.ToString();
          xmlParam.Precision = parm.Precision.ToString();
          xmlParam.Scale = parm.Scale.ToString();
          xmlParam.Direction = GetParameterDirection(parm.Direction);
          xmlProc.InputParameters[p.Procedure.InputParameters.IndexOf(parm)] = xmlParam;
        }
        
        xmlProc.RecordSets = new RecordSetDef[p.Procedure.RecordSets.Count];
        foreach (DBInfo.Core.Model.RecordSet r in p.Procedure.RecordSets) {
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
          xmlProc.RecordSets[p.Procedure.RecordSets.IndexOf(r)] = xmlRS;
        }

        StatementCollection stCol = new StatementCollection();
        stCol.Statement = new Statement[1];
        stCol.Statement[0] = xmlProc;
        generateXMLOutput(stCol, OutputDir + "\\" + ProceduresDir + "\\" + p.Procedure.Name + ".proc.xml", true);
      }
    }
    
    private void GenerateFunctions(List<DBInfo.Core.Statement.CreateFunction> functionList){
      if (!Directory.Exists(OutputDir + "\\" + FunctionsDir))
        Directory.CreateDirectory(OutputDir + "\\" + FunctionsDir);
      
      foreach(DBInfo.Core.Statement.CreateFunction f in functionList){
        CreateFunction xmlFunction = new CreateFunction();
        xmlFunction.Name = f.Function.Name;
        xmlFunction.SourceCode = f.Function.Body;
        
        StatementCollection stCol = new StatementCollection();
        stCol.Statement = new Statement[1];
        stCol.Statement[0] = xmlFunction;
        generateXMLOutput(stCol, OutputDir + "\\" + FunctionsDir + "\\" + f.Function.Name + ".function.xml", true); 
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
    
    private void GenerateViews(List<DBInfo.Core.Statement.CreateView> viewList){
      if (!Directory.Exists(OutputDir + "\\" + ViewsDir))
        Directory.CreateDirectory(OutputDir + "\\" + ViewsDir);
        
      foreach ( DBInfo.Core.Statement.CreateView v in viewList){
        CreateView xmlView = new CreateView();
        xmlView.Name = v.View.Name;
        xmlView.SourceCode = v.View.Body;
        
        StatementCollection stCol = new StatementCollection();
        stCol.Statement = new Statement[1];
        stCol.Statement[0] = xmlView;
        generateXMLOutput(stCol, OutputDir + "\\" + ViewsDir + "\\" + v.View.Name + ".view.xml", true);
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

    public void GenerateOutput(List<BaseStatement> statements, List<DBObjectType> dataToGenerateOutput){      
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Tables)) {
        List<DBInfo.Core.Statement.CreateTable> l = 
          (from BaseStatement s in statements
           where s is DBInfo.Core.Statement.CreateTable
           select (DBInfo.Core.Statement.CreateTable)s).ToList<DBInfo.Core.Statement.CreateTable>();
        GenerateTables(l);
      }
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.CheckConstraints)) {
        List<DBInfo.Core.Statement.CreatePrimaryKey> createPKList =
          (from BaseStatement s in statements
           where s is DBInfo.Core.Statement.CreatePrimaryKey
           select (DBInfo.Core.Statement.CreatePrimaryKey)s).ToList<DBInfo.Core.Statement.CreatePrimaryKey>();
        List<DBInfo.Core.Statement.CreateCheckConstraint> createCheckList =
          (from BaseStatement s in statements
           where s is DBInfo.Core.Statement.CreateCheckConstraint
           select (DBInfo.Core.Statement.CreateCheckConstraint)s).ToList<DBInfo.Core.Statement.CreateCheckConstraint>();           
        GenerateConstraints(createCheckList, createPKList);
      }    
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.ForeignKeys)) {
        List<DBInfo.Core.Statement.CreateForeignKey> l =
          (from BaseStatement s in statements
           where s is DBInfo.Core.Statement.CreateForeignKey
           select (DBInfo.Core.Statement.CreateForeignKey)s).ToList<DBInfo.Core.Statement.CreateForeignKey>();
        GenerateForeignKeys(l);
      }
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Indexes)) {
        List<DBInfo.Core.Statement.CreateIndex> l =
          (from BaseStatement s in statements
           where s is DBInfo.Core.Statement.CreateIndex
           select (DBInfo.Core.Statement.CreateIndex)s).ToList<DBInfo.Core.Statement.CreateIndex>();      
        GenerateIndexes(l);
      }
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Triggers)) {
        List<DBInfo.Core.Statement.CreateTrigger> l =
            (from BaseStatement s in statements
             where s is DBInfo.Core.Statement.CreateTrigger
             select (DBInfo.Core.Statement.CreateTrigger)s).ToList<DBInfo.Core.Statement.CreateTrigger>();      
        GenerateTriggers(l);
      }
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Procedures)) {
        List<DBInfo.Core.Statement.CreateProcedure> l =
          (from BaseStatement s in statements
           where s is DBInfo.Core.Statement.CreateProcedure
           select (DBInfo.Core.Statement.CreateProcedure)s).ToList<DBInfo.Core.Statement.CreateProcedure>();            
        GenerateProcedures(l);
      }
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Functions)) {
        List<DBInfo.Core.Statement.CreateFunction> l =
          (from BaseStatement s in statements
           where s is DBInfo.Core.Statement.CreateFunction
           select (DBInfo.Core.Statement.CreateFunction)s).ToList<DBInfo.Core.Statement.CreateFunction>();                  
        GenerateFunctions(l);
      }      
      if (dataToGenerateOutput.Contains(DBObjectType.All) || dataToGenerateOutput.Contains(DBObjectType.Views)) {
        List<DBInfo.Core.Statement.CreateView> l =
          (from BaseStatement s in statements
           where s is DBInfo.Core.Statement.CreateView
           select (DBInfo.Core.Statement.CreateView)s).ToList<DBInfo.Core.Statement.CreateView>();                        
        GenerateViews(l);
      }                                      
    }

    #endregion

    #region IStatementCollectionOutputGenerator Members

    public IScriptOutputHandler ScriptOutputGen {
      get; set;      
    }

    public IScriptFileOutputGenerator ScriptFileOutputGenerator {
      get; set; 
    }

    #endregion

  }
}
