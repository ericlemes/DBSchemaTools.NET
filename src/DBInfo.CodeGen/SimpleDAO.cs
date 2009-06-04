using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.OutputGenerators;
using DBInfo.Core.Model;
using DBInfo.Core.Extractor;
using System.CodeDom;
using System.Xml.Serialization;
using System.CodeDom.Compiler;
using System.IO;
using Microsoft.CSharp;
using System.Data.SqlClient;
using System.Data;

namespace DBInfo.CodeGen {
  public class SimpleDAO : IOutputGenerator {
    #region IOutputGenerator Members

    public GeneratorType Type {
      get { return GeneratorType.Generic; }
    }

    private string _OutputDir;
    public string OutputDir {
      get { return _OutputDir;}         
      set { _OutputDir = value; }        
    }
    
    private string _Namespace = "DAO";
    public string Namespace{
      get { return _Namespace;}
      set { _Namespace = value;}
    }
    
    private string _VOClassSuffix = "DatabaseVO";
    public string VOClassSuffix{
      get { return _VOClassSuffix;}
      set { _VOClassSuffix = value;}
    }
    
    private string _VONamespace = "DatabaseVO";
    public string VONamespace{
      get { return _VONamespace;}
      set { _VONamespace = value;}
    }
    
    private string _DAOClassSuffix = "DAO";
    public string DAOClassSuffix{
      get { return _DAOClassSuffix;}
      set { _DAOClassSuffix = value;}
    }
    
    private string _DAONamespace = "DAO";
    public string DAONamespace{
      get { return _DAONamespace;}
      set { _DAONamespace = value;}
    }
    
    private string _BlobStreamClassSuffix = "BlobStream";
    public string BlobStreamClassSuffix{
      get { return _BlobStreamClassSuffix;}
      set {_BlobStreamClassSuffix = value;}
    }

    public void GenerateOutput(Database db, List<DBObjectType> dataToGenerateOutput) {      
      foreach(Table t in db.Tables){
        GenerateDatabaseVO(t);
        GenerateDatabaseDAO(t);
      }
      foreach(Procedure p in db.Procedures){
        GenerateProcedureInputVO(p);
        GenerateProcedureOutputVOs(p);
      }
    }

    private string _ProcInputVONamespace = "ProcInputVO";
    public string ProcInputVONamespace {
      get { return _ProcInputVONamespace; }
      set { _ProcInputVONamespace = value; }
    }

    private string _ProcInputVOClassSuffix = "ProcInputVO";
    public string ProcInputVOClassSuffix {
      get { return _ProcInputVOClassSuffix; }
      set { _ProcInputVOClassSuffix = value; }
    }

    private string _ProcOutputVONamespace = "ProcOutputVO";
    public string ProcOutputVONamespace {
      get { return _ProcOutputVONamespace; }
      set { _ProcOutputVONamespace = value; }
    }

    private string _ProcOutputVOClassSuffix = "ProcOutputVO";
    public string ProcOutputVOClassSuffix {
      get { return _ProcOutputVOClassSuffix; }
      set { _ProcOutputVOClassSuffix = value; }
    }
    
    private Type GetTypeFromColumnType(DBColumnType t){
      if (t == DBColumnType.DBBigInt)
        return typeof(long?);
      else if (t == DBColumnType.DBBit)
        return typeof(bool?);
      else if (t == DBColumnType.DBChar)
        return typeof(string);
      else if (t == DBColumnType.DBDateTime)
        return typeof(DateTime?);
      else if (t == DBColumnType.DBDecimal)
        return typeof(decimal?);
      else if (t == DBColumnType.DBFloat)
        return typeof(float?);
      else if (t == DBColumnType.DBGUID)
        return typeof(Guid?);
      else if (t == DBColumnType.DBInteger)
        return typeof(int?);
      else if (t == DBColumnType.DBMemo)
        return typeof(string);
      else if (t == DBColumnType.DBMoney)
        return typeof(float?);
      else if (t == DBColumnType.DBNumeric)
        return typeof(decimal?);
      else if (t == DBColumnType.DBNVarchar)
        return typeof(string);
      else if (t == DBColumnType.DBRowID)
        return typeof(string);
      else if (t == DBColumnType.DBSmallDateTime)
        return typeof(DateTime?);
      else if (t == DBColumnType.DBSmallInt)
        return typeof(Int16?);
      else if (t == DBColumnType.DBTimeStamp)
        return typeof(string);
      else if (t == DBColumnType.DBTinyInt)
        return typeof(Int16?);
      else if (t == DBColumnType.DBVarchar)
        return typeof(string);      
      else
        throw new Exception(String.Format("Unsuported type: {0}", t.ToString()));
    }
    
    private bool IsStreamedType(DBColumnType t){
      if (t == DBColumnType.DBBinary)
        return true;
      else if (t == DBColumnType.DBBlob)
        return true;
      else if (t == DBColumnType.DBTimeStamp)
        return true;
      return false;
    }
    
    private void GenerateDatabaseVO(Table t){      
      CodeNamespace codeNS = new CodeNamespace(_Namespace + "." + VONamespace);

      CodeTypeDeclaration voClass = new CodeTypeDeclaration(t.TableName + _VOClassSuffix);
      voClass.IsClass = true;
      foreach(Column c in t.Columns){      
        if (IsStreamedType(c.Type))
          continue;
        CodeMemberField mf = new CodeMemberField(GetTypeFromColumnType(c.Type), "_" + c.Name);
        voClass.Members.Add(mf);               
        
        CodeMemberProperty mp = new CodeMemberProperty();
        mp.Attributes = MemberAttributes.Public;
        mp.HasGet = true;
        mp.HasSet = true;
        mp.Name = c.Name;                
        mp.Type = new CodeTypeReference(GetTypeFromColumnType(c.Type));                
        
        mp.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), mf.Name), new CodePropertySetValueReferenceExpression()));        
        mp.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), mf.Name)));        
        
        voClass.Members.Add(mp);
      }
      codeNS.Types.Add(voClass);
      
      CSharpCodeProvider csharp = new CSharpCodeProvider();

      CodeGeneratorOptions cop = new CodeGeneratorOptions();
      cop.IndentString = "  ";

      if (!Directory.Exists(OutputDir + "\\" + _VONamespace))
        Directory.CreateDirectory(OutputDir + "\\" + _VONamespace);

      FileStream fs = new FileStream(OutputDir + "\\" + _VONamespace + "\\" + t.TableName + _VOClassSuffix + ".cs", FileMode.Create, FileAccess.Write);      
      StreamWriter sw = new StreamWriter(fs);
      csharp.GenerateCodeFromNamespace(codeNS, sw, cop);      
      sw.Flush();
      sw.Close();
      fs.Close();
    }    
    
    private string GetWhereByPk(Table t){
      string tmp = "";
      foreach(Column c in t.PrimaryKeyColumns){
        if (tmp == "")
          tmp += c.Name + " = @" + c.Name;
        else
          tmp += " and " + c.Name + " = @" + c.Name;
      }
      return tmp;
    }
    
    private void GenerateIndexNames(Dictionary<Index, string> IndexNames, List<Index> indexes){
      foreach(Index i in indexes){
        string indexName = "";
        foreach(IndexColumn ic in i.Columns){
          if (indexName == "")
            indexName += ic.Column.Name;
          else
            indexName += "_" + ic.Column.Name;
          if (ic.Order == IndexColumn.EnumOrder.Descending)
            indexName += "Desc";
        }
        IndexNames.Add(i, indexName);
      }
    }

    private void GenerateDatabaseDAO(Table t) {
      CodeNamespace codeNS = new CodeNamespace(_Namespace + "." + DAONamespace);
      codeNS.Imports.Add(new CodeNamespaceImport(_VONamespace));
      codeNS.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
                                    
      CodeTypeDeclaration daoClass = new CodeTypeDeclaration(t.TableName + _DAOClassSuffix);
      codeNS.Types.Add(daoClass);
      daoClass.IsClass = true;
            
      CodeMemberField asterField = new CodeMemberField(new CodeTypeReference(typeof(string)), "Aster");            
      asterField.Attributes = MemberAttributes.Public;
      asterField.Attributes |= MemberAttributes.Static;                        
      asterField.InitExpression = new CodePrimitiveExpression(GetAster(t, "", true, true));
      daoClass.Members.Add(asterField);

      daoClass.Members.Add(GenerateLoadMethod(t));
      daoClass.Members.Add(GenerateInsertMethod(t));
      daoClass.Members.Add(GenerateUpdateMethod(t));
      daoClass.Members.Add(GenerateDeleteMethod(t));
      Dictionary<Index, string> IndexNames = new Dictionary<Index, string>();
      GenerateIndexNames(IndexNames, t.Indexes);
      foreach(Index i in t.Indexes){        
        daoClass.Members.Add(GenerateLoadByIndex(t, i, IndexNames));
      }
      
      foreach(Column c in t.Columns){
        if (!IsStreamedType(c.Type))
          continue;
        codeNS.Types.Add(GenerateBlobStreamClass(t, c));
      }

      CSharpCodeProvider csharp = new CSharpCodeProvider();

      CodeGeneratorOptions cop = new CodeGeneratorOptions();
      cop.IndentString = "  ";
      
      if (!Directory.Exists(OutputDir + "\\" + _DAONamespace))
        Directory.CreateDirectory(OutputDir + "\\" + _DAONamespace);        

      FileStream fs = new FileStream(OutputDir + "\\" + _DAONamespace + "\\" + t.TableName + _DAONamespace + ".cs", FileMode.Create, FileAccess.Write);
      StreamWriter sw = new StreamWriter(fs);
            
      csharp.GenerateCodeFromNamespace(codeNS, sw, cop);
      sw.Flush();
      sw.Close();
      fs.Close();
    }
    
    private SqlDbType GetSqlDbType(DBColumnType t){
      if (t == DBColumnType.DBBigInt)
        return SqlDbType.BigInt;
      else if (t == DBColumnType.DBBinary)
        return SqlDbType.Binary;
      else if (t == DBColumnType.DBBit)
        return SqlDbType.Bit;
      else if (t == DBColumnType.DBBlob)
        return SqlDbType.Image;
      else if (t == DBColumnType.DBChar)
        return SqlDbType.Char;
      else if (t == DBColumnType.DBDateTime)
        return SqlDbType.DateTime;
      else if (t == DBColumnType.DBDecimal)
        return SqlDbType.Decimal;
      else if (t == DBColumnType.DBFloat)
        return SqlDbType.Float;
      else if (t == DBColumnType.DBGUID)
        return SqlDbType.UniqueIdentifier;
      else if (t == DBColumnType.DBInteger)
        return SqlDbType.Int;
      else if (t == DBColumnType.DBMemo)
        return SqlDbType.Text;
      else if (t == DBColumnType.DBMoney)
        return SqlDbType.Money;
      else if (t == DBColumnType.DBNumeric)
        return SqlDbType.Decimal;
      else if (t == DBColumnType.DBNVarchar)
        return SqlDbType.NVarChar;
      else if (t == DBColumnType.DBRowID)
        return SqlDbType.VarChar;
      else if (t == DBColumnType.DBSmallDateTime)
        return SqlDbType.SmallDateTime;
      else if (t == DBColumnType.DBSmallInt)
        return SqlDbType.SmallInt;
      else if (t == DBColumnType.DBTimeStamp)
        return SqlDbType.Timestamp;
      else if (t == DBColumnType.DBTinyInt)
        return SqlDbType.TinyInt;
      else if (t == DBColumnType.DBVarchar)
        return SqlDbType.VarChar;
      else
        throw new Exception(String.Format("Can't find SqlDbType for Type {0}", t.ToString()));
    }
    
    private void AddDefaultParams(CodeMemberMethod method){
      CodeParameterDeclarationExpression connParam = new CodeParameterDeclarationExpression();
      connParam.Type = new CodeTypeReference(typeof(SqlConnection));
      connParam.Name = "Connection";
      method.Parameters.Add(connParam);

      CodeParameterDeclarationExpression transParam = new CodeParameterDeclarationExpression();
      transParam.Type = new CodeTypeReference(typeof(SqlTransaction));
      transParam.Name = "Transaction";
      method.Parameters.Add(transParam);
    }
    
    private CodeExpression CreateSelectAsterFromTableNameExpression(Table t, string WhereClause){
      CodeExpression exp =     
        new CodeBinaryOperatorExpression(
          new CodePrimitiveExpression("select "),
          CodeBinaryOperatorType.Add,
          new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "Aster"));
      exp = new CodeBinaryOperatorExpression(
        exp,
        CodeBinaryOperatorType.Add,
        new CodePrimitiveExpression(" from " + t.TableName + " " + WhereClause));
      return exp;
    }
    
    private CodeVariableDeclarationStatement CreateCommandDeclaration(CodeExpression InitExpression){
      return CreateCommandDeclaration(InitExpression, "Connection", "Transaction");
    }

    private CodeVariableDeclarationStatement CreateCommandDeclaration(CodeExpression InitExpression, string ConnectionVarName, string TransactionVarName) {
      CodeExpression[] commandCreateParams = new CodeExpression[3];
      commandCreateParams[1] = new CodeVariableReferenceExpression(ConnectionVarName);
      commandCreateParams[2] = new CodeVariableReferenceExpression(TransactionVarName);
      commandCreateParams[0] = InitExpression;

      CodeVariableDeclarationStatement command = new CodeVariableDeclarationStatement(
        typeof(SqlCommand), "cmd",
        new CodeObjectCreateExpression(
          new CodeTypeReference(typeof(SqlCommand)),
          commandCreateParams)
        );

      return command;
    }
    
    private void CreateSqlParametersPrimaryKey(CodeStatementCollection statementCollection, Table t, string PrefixForPKVars){
      int ParamCount = 1;
      foreach (Column c in t.PrimaryKeyColumns) {
        CreateSqlParameter(c, "pkParm" + ParamCount.ToString(), statementCollection, new CodeVariableReferenceExpression(PrefixForPKVars + c.Name));
      }
    }

    private void AddPrimaryKeyParams(CodeMemberMethod method, Table t) {
      foreach (Column c in t.PrimaryKeyColumns) {
        CodeParameterDeclarationExpression param = new CodeParameterDeclarationExpression(
          GetTypeFromColumnType(c.Type),
          c.Name);
        method.Parameters.Add(param);
      }
    }    
    
    private CodeMemberMethod GenerateLoadMethod(Table t){
      CodeMemberMethod loadMethod = new CodeMemberMethod();
      loadMethod.Name = "Load";      
      loadMethod.Attributes = MemberAttributes.Public;      
      loadMethod.ReturnType = new CodeTypeReference(t.TableName + _VOClassSuffix);
      
      AddDefaultParams(loadMethod);
      AddPrimaryKeyParams(loadMethod, t);      

      string sql = "select " + GetAster(t, "", false, true) + " from " + t.TableName + " where " + GetWhereByPk(t);
      loadMethod.Statements.Add(CreateCommandDeclaration(new CodePrimitiveExpression(sql)));      
      
      int ParamCount = 1;
      foreach(Column c in t.PrimaryKeyColumns){
        CodeVariableReferenceExpression paramVar = new CodeVariableReferenceExpression("p" + ParamCount.ToString());        
        
        CreateSqlParameter(c, "p" + ParamCount.ToString(), loadMethod, new CodeVariableReferenceExpression(c.Name));      
        ParamCount++;
      }
      
      CreateReaderReturnSingleVO(loadMethod, t);
      
      return loadMethod;
    }
    
    private void CreateReaderReturnSingleVO(CodeMemberMethod method, Table t){
      method.Statements.Add(
        new CodeVariableDeclarationStatement(typeof(SqlDataReader), "rd", new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("cmd"), "ExecuteReader", new CodeExpression[0])));

      CodeStatementCollection col = new CodeStatementCollection();
      AssignReaderOutputToNewVO(t, col);
      col.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(
              new CodeMethodReferenceExpression(
                new CodeVariableReferenceExpression("rd"), "Close"), new CodeExpression[0])));
      col.Add(
        new CodeMethodReturnStatement(new CodeVariableReferenceExpression("vo")));
      CodeStatement[] trueStatements2 = new CodeStatement[col.Count];
      int i = 0;
      for (i = 0; i < col.Count; i++)
        trueStatements2[i] = col[i];

      CodeStatement[] falseStatements2 = new CodeStatement[2];
      falseStatements2[0] = new CodeExpressionStatement(new CodeMethodInvokeExpression(
        new CodeMethodReferenceExpression(
          new CodeVariableReferenceExpression("rd"), "Close"), new CodeExpression[0]));
      falseStatements2[1] = new CodeMethodReturnStatement(new CodePrimitiveExpression(null));

      method.Statements.Add(
        new CodeConditionStatement(
          new CodeMethodInvokeExpression(
            new CodeVariableReferenceExpression("rd"),
            "Read",
            new CodeExpression[0]), trueStatements2, falseStatements2));      
    }
    
    private string GetAster(Table t, string Prefix, bool ReturnStreamed, bool ReturnIdentity){
      string tmp = "";
      foreach(Column c in t.Columns){
        if (c.IdentityColumn && !ReturnIdentity)
          continue;
        if (IsStreamedType(c.Type) && !ReturnStreamed)
          continue;
        if (tmp == "")
          tmp += Prefix + c.Name;
        else 
          tmp += ", " + Prefix + c.Name;
      }
      return tmp;
    }
    
    private CodeMemberMethod GenerateLoadByIndex(Table t, Index i, Dictionary<Index, string> IndexNames){
      CodeMemberMethod loadMethod = new CodeMemberMethod();
      loadMethod.Attributes = MemberAttributes.Public;
      if (i.Unique){
        loadMethod.Name = "LoadByAlternateKey_" + IndexNames[i];
        loadMethod.ReturnType = new CodeTypeReference(t.TableName + _VOClassSuffix);
      }
      else {
        loadMethod.Name = "LoadByIndex_" + IndexNames[i];
        loadMethod.ReturnType = new CodeTypeReference("List<" + t.TableName + _VOClassSuffix + ">");
      }              

      AddDefaultParams(loadMethod);
      
      foreach(IndexColumn ic in i.Columns){
        loadMethod.Parameters.Add(
          new CodeParameterDeclarationExpression(
            new CodeTypeReference(GetTypeFromColumnType(ic.Column.Type)), 
            ic.Column.Name));
      }
                 
      string sql =
        "select " + GetAster(t, "", false, true) + " from " + t.TableName + " where ";
        
      bool first = true;
      foreach(IndexColumn ic in i.Columns){
        if (first)
          sql += ic.Column.Name + " = @" + ic.Column.Name;
        else
          sql += " and " + ic.Column.Name + " = @" + ic.Column.Name;
        first = false;
      }
      
      loadMethod.Statements.Add(CreateCommandDeclaration(new CodePrimitiveExpression(sql)));
      
      int ParamCount = 1;
      foreach(IndexColumn ic in i.Columns){
        CreateSqlParameter(ic.Column, "p" + ParamCount.ToString(), loadMethod, new CodeVariableReferenceExpression(ic.Column.Name));
        ParamCount++;
      }      
      
      if (i.Unique)      
        CreateReaderReturnSingleVO(loadMethod, t);
      else
        CreateReaderReturnVOList(loadMethod, t);
      
      return loadMethod;
    }    
    
    private void CreateReaderReturnVOList(CodeMemberMethod method, Table t){
      method.Statements.Add(
          new CodeVariableDeclarationStatement(typeof(SqlDataReader), "rd", new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("cmd"), "ExecuteReader", new CodeExpression[0])));
    
      method.Statements.Add(
          new CodeVariableDeclarationStatement("List<" + t.TableName + _VOClassSuffix + ">", "result",
          new CodeObjectCreateExpression("List<" + t.TableName + _VOClassSuffix + ">", new CodeExpression[0])));

      CodeIterationStatement whileReaderRead = new CodeIterationStatement(
        new CodeSnippetStatement(""),
        new CodeMethodInvokeExpression(
          new CodeMethodReferenceExpression(
            new CodeVariableReferenceExpression("rd"),
            "Read"),
            new CodeExpression[0]),
        new CodeSnippetStatement(""));
      method.Statements.Add(whileReaderRead);

      AssignReaderOutputToNewVO(t, whileReaderRead.Statements);
      CodeExpression[] addParms = new CodeExpression[1];
      addParms[0] = new CodeVariableReferenceExpression("vo");
      whileReaderRead.Statements.Add(
        new CodeMethodInvokeExpression(
          new CodeMethodReferenceExpression(
            new CodeVariableReferenceExpression("result"),
            "Add"),
            addParms));
            
      method.Statements.Add(
        new CodeMethodInvokeExpression(
          new CodeMethodReferenceExpression(
            new CodeVariableReferenceExpression("rd"),
            "Close")));

      method.Statements.Add(
        new CodeMethodReturnStatement(new CodeVariableReferenceExpression("result")));      
    }
    
    private CodeMemberMethod GenerateInsertMethod(Table t){
      CodeMemberMethod insertMethod = new CodeMemberMethod();
      insertMethod.Attributes = MemberAttributes.Public;
      insertMethod.Name = "Insert";
      
      AddDefaultParams(insertMethod);
      
      CodeParameterDeclarationExpression voParam = new CodeParameterDeclarationExpression(
        t.TableName + _VOClassSuffix, "vo");
      insertMethod.Parameters.Add(voParam);

      string sql = 
        "insert into " + t.TableName + " ( " + GetAster(t, "", true, false) + ") values (";
      bool first = true;
      foreach(Column c in t.Columns){
        if (c.IdentityColumn)
          continue;
        if (!first)
          sql += ", ";
        if (IsStreamedType(c.Type))
          sql += "null";
        else
          sql += "@" + c.Name;
        first = false;
      }
      sql += ")";
      if (!String.IsNullOrEmpty(t.GetIdentityColumn())){
        sql += Environment.NewLine + 
          "select SCOPE_IDENTITY()";
      }
      
      CodeExpression insertExpression = new CodePrimitiveExpression(sql);      
      insertMethod.Statements.Add(CreateCommandDeclaration(insertExpression));
      
      insertMethod.Statements.Add(
        new CodeVariableDeclarationStatement(typeof(object), "val", new CodePrimitiveExpression(null)));
      
      int ParamCount = 1;
      foreach(Column c in t.Columns){
        if (IsStreamedType(c.Type))
          continue;
        if (c.IdentityColumn)
          continue;
        string paramName = "p" + ParamCount.ToString();        
        
        CodeStatement[] trueStatements = new CodeStatement[1];
        trueStatements[0] = 
          new CodeAssignStatement(
            new CodeVariableReferenceExpression("val"),
            new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(DBNull)), "Value"));
        CodeStatement[] falseStatements = new CodeStatement[1];
         falseStatements[0] = 
          new CodeAssignStatement(
            new CodeVariableReferenceExpression("val"),
            new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("vo"), c.Name));
        CodeConditionStatement ifValueNull = new CodeConditionStatement(
          new CodeBinaryOperatorExpression(
            new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("vo"), c.Name),
            CodeBinaryOperatorType.ValueEquality,
            new CodePrimitiveExpression(null)),
            trueStatements,
            falseStatements);
        insertMethod.Statements.Add(ifValueNull);
        
        CreateSqlParameter(c, paramName, insertMethod, new CodeVariableReferenceExpression("val"));
        
        ParamCount++;
      }      
      
      string IdentityCol = t.GetIdentityColumn();
      if (!String.IsNullOrEmpty(IdentityCol)){        
        CodeExpression[] toInt32Parms = new CodeExpression[1];
        toInt32Parms[0] = 
          new CodeCastExpression(
            new CodeTypeReference(typeof(decimal)),
            new CodeMethodInvokeExpression(
              new CodeMethodReferenceExpression(
                new CodeVariableReferenceExpression("cmd"),
                "ExecuteScalar"),
                new CodeExpression[0]));      
        insertMethod.Statements.Add(
          new CodeAssignStatement(
            new CodeFieldReferenceExpression(
              new CodeVariableReferenceExpression("vo"),
              IdentityCol),
            new CodeMethodInvokeExpression(
              new CodeMethodReferenceExpression(
                new CodeTypeReferenceExpression(typeof(decimal)),
                "ToInt32"),
                toInt32Parms)));
      }
      else {
        insertMethod.Statements.Add(
          new CodeMethodInvokeExpression(
            new CodeVariableReferenceExpression("cmd"),
            "ExecuteNonQuery",
            new CodeExpression[0]));
      }            
           
      return insertMethod;
    }

    private CodeMemberMethod GenerateUpdateMethod(Table t) {
      CodeMemberMethod updateMethod = new CodeMemberMethod();
      updateMethod.Attributes = MemberAttributes.Public;
      updateMethod.Name = "Update";
      updateMethod.ReturnType = new CodeTypeReference(typeof(int));

      AddDefaultParams(updateMethod);

      CodeParameterDeclarationExpression voParam = new CodeParameterDeclarationExpression(
        t.TableName + _VOClassSuffix, "vo");
      updateMethod.Parameters.Add(voParam);

      string sql =
        "update " + t.TableName + " " +
        "set ";
      bool first = true;
      foreach (Column c in t.Columns){
        if (IsStreamedType(c.Type))
          continue;
        if (c.IsPK)
          continue;
        if (first)
          sql += " " + c.Name + " = @" + c.Name;
        else
          sql += ", " + c.Name + " = @" + c.Name;
        first = false;
      }
      sql += " where " +
      GetWhereByPk(t);                
      
      updateMethod.Statements.Add(CreateCommandDeclaration(new CodePrimitiveExpression(sql)));
      
      
      updateMethod.Statements.Add(
        new CodeVariableDeclarationStatement(typeof(object), "val", new CodePrimitiveExpression(null)));
      int ParamCount = 1;            
      foreach(Column c in t.Columns){
        if (IsStreamedType(c.Type))
          continue;
        CodeStatement[] trueStatements = new CodeStatement[1];
        trueStatements[0] = 
          new CodeAssignStatement(
            new CodeVariableReferenceExpression("val"),
            new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(DBNull)), "Value"));
        CodeStatement[] falseStatements = new CodeStatement[1];
         falseStatements[0] = 
          new CodeAssignStatement(
            new CodeVariableReferenceExpression("val"),
            new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("vo"), c.Name));
        CodeConditionStatement ifValueNull = new CodeConditionStatement(
          new CodeBinaryOperatorExpression(
            new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("vo"), c.Name),
            CodeBinaryOperatorType.ValueEquality,
            new CodePrimitiveExpression(null)),
            trueStatements,
            falseStatements);
        updateMethod.Statements.Add(ifValueNull);
      
        CreateSqlParameter(c, "p" + ParamCount.ToString(), updateMethod, new CodeVariableReferenceExpression("val"));
        
        ParamCount++;
      }
      
      updateMethod.Statements.Add(        
          new CodeMethodReturnStatement(
            new CodeMethodInvokeExpression(
              new CodeMethodReferenceExpression(
                new CodeVariableReferenceExpression("cmd"), 
                "ExecuteNonQuery"), 
                new CodeExpression[0])));
      
      return updateMethod;
    }
    
    private CodeMemberMethod GenerateDeleteMethod(Table t) {
      CodeMemberMethod deleteMethod = new CodeMemberMethod();
      deleteMethod.Attributes = MemberAttributes.Public;
      deleteMethod.Name = "Delete";
      deleteMethod.ReturnType = new CodeTypeReference(typeof(int));

      AddDefaultParams(deleteMethod);

      CodeParameterDeclarationExpression voParam = new CodeParameterDeclarationExpression(
        t.TableName + _VOClassSuffix, "vo");
      deleteMethod.Parameters.Add(voParam);

      string sql =
        "delete from " + t.TableName + " " +
        " where " +
        GetWhereByPk(t);                
      
      deleteMethod.Statements.Add(CreateCommandDeclaration(new CodePrimitiveExpression(sql)));
      
      int ParamCount = 1;
      foreach(Column c in t.PrimaryKeyColumns){
        CreateSqlParameter(c, "p" + ParamCount.ToString(), deleteMethod, new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("vo"), c.Name));
        ParamCount++;
      }

      deleteMethod.Statements.Add(
        new CodeMethodReturnStatement(
          new CodeMethodInvokeExpression(
            new CodeMethodReferenceExpression(
              new CodeVariableReferenceExpression("cmd"),
              "ExecuteNonQuery"),
              new CodeExpression[0])));

      return deleteMethod;      
    }

    private void CreateSqlParameter(Column c, string paramName, CodeStatementCollection statementCollection, CodeExpression valueExpression) {
      CodeVariableReferenceExpression paramVar = new CodeVariableReferenceExpression(paramName);

      CodeVariableDeclarationStatement param = new CodeVariableDeclarationStatement(typeof(SqlParameter), paramName, new CodeObjectCreateExpression(typeof(SqlParameter), new CodeExpression[0]));
      statementCollection.Add(param);
      statementCollection.Add(
        new CodeAssignStatement(
          new CodeFieldReferenceExpression(paramVar, "ParameterName"),
          new CodePrimitiveExpression("@" + c.Name)));
      statementCollection.Add(
        new CodeAssignStatement(
          new CodeFieldReferenceExpression(paramVar, "SqlDbType"),
          new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SqlDbType)), GetSqlDbType(c.Type).ToString())));
      if (c.Size > 0) {
        statementCollection.Add(
          new CodeAssignStatement(
            new CodeFieldReferenceExpression(paramVar, "Size"),
            new CodePrimitiveExpression(c.Size)));
      }
      statementCollection.Add(new CodeAssignStatement(
        new CodeFieldReferenceExpression(paramVar, "Value"),
        valueExpression));

      statementCollection.Add(
        new CodeMethodInvokeExpression(
          new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("cmd"), "Parameters"),
          "Add",
          new CodeVariableReferenceExpression(paramName)));
    }
    
    private void CreateSqlParameter(Column c, string paramName, CodeMemberMethod method, CodeExpression valueExpression){
      CreateSqlParameter(c, paramName, method.Statements, valueExpression);
    }

    private void CreateSqlParameter(string ParamVarName, string ParamName, SqlDbType ParamType, int Size, CodeStatementCollection statementCollection, CodeExpression valueExpression, ParameterDirection Direction) {
      CodeVariableReferenceExpression paramVar = new CodeVariableReferenceExpression(ParamVarName);

      CodeVariableDeclarationStatement param = new CodeVariableDeclarationStatement(
        typeof(SqlParameter), 
        ParamVarName, 
        new CodeObjectCreateExpression(typeof(SqlParameter), new CodeExpression[0]));
      statementCollection.Add(param);
      statementCollection.Add(
        new CodeAssignStatement(
          new CodeFieldReferenceExpression(paramVar, "ParameterName"),
          new CodePrimitiveExpression("@" + ParamName)));
      statementCollection.Add(
        new CodeAssignStatement(
          new CodeFieldReferenceExpression(paramVar, "SqlDbType"),
          new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SqlDbType)), ParamType.ToString())));
      if (Size > 0) {
        statementCollection.Add(
          new CodeAssignStatement(
            new CodeFieldReferenceExpression(paramVar, "Size"),
            new CodePrimitiveExpression(Size)));
      }
      if (valueExpression != null){
        statementCollection.Add(new CodeAssignStatement(
          new CodeFieldReferenceExpression(paramVar, "Value"),
          valueExpression));
      }
      statementCollection.Add(
        new CodeAssignStatement(
          new CodeFieldReferenceExpression(paramVar, "Direction"),
          new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(ParameterDirection)), Direction.ToString())));

      statementCollection.Add(
        new CodeMethodInvokeExpression(
          new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("cmd"), "Parameters"),
          "Add",
          paramVar));
    }

    private void CreateSqlParameter(string ParamVarName, string ParamName, SqlDbType ParamType, CodeExpression Size, CodeStatementCollection statementCollection, CodeExpression valueExpression, ParameterDirection Direction) {
      CodeVariableReferenceExpression paramVar = new CodeVariableReferenceExpression(ParamVarName);

      CodeVariableDeclarationStatement param = new CodeVariableDeclarationStatement(
        typeof(SqlParameter),
        ParamVarName,
        new CodeObjectCreateExpression(typeof(SqlParameter), new CodeExpression[0]));
      statementCollection.Add(param);
      statementCollection.Add(
        new CodeAssignStatement(
          new CodeFieldReferenceExpression(paramVar, "ParameterName"),
          new CodePrimitiveExpression("@" + ParamName)));
      statementCollection.Add(
        new CodeAssignStatement(
          new CodeFieldReferenceExpression(paramVar, "SqlDbType"),
          new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SqlDbType)), ParamType.ToString())));      
      statementCollection.Add(
        new CodeAssignStatement(
          new CodeFieldReferenceExpression(paramVar, "Size"),
          Size));
      if (valueExpression != null) {
        statementCollection.Add(new CodeAssignStatement(
          new CodeFieldReferenceExpression(paramVar, "Value"),
          valueExpression));
      }
      statementCollection.Add(
        new CodeAssignStatement(
          new CodeFieldReferenceExpression(paramVar, "Direction"),
          new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(ParameterDirection)), Direction.ToString())));

      statementCollection.Add(
        new CodeMethodInvokeExpression(
          new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("cmd"), "Parameters"),
          "Add",
          paramVar));
    }        
    
    
    private void AssignReaderOutputToNewVO(Table t, CodeStatementCollection statements){
      CodeVariableDeclarationStatement newVO = new CodeVariableDeclarationStatement(t.TableName + _VOClassSuffix, "vo", new CodeObjectCreateExpression(t.TableName + _VOClassSuffix, new CodeExpression[0]));
      statements.Add(newVO);
      
      CodeVariableReferenceExpression vo = new CodeVariableReferenceExpression("vo");
      CodeVariableReferenceExpression rd = new CodeVariableReferenceExpression("rd");
      
      int colIndex = 0;
      foreach(Column c in t.Columns){
        if (IsStreamedType(c.Type))
          continue;
          
        CodeExpression[] readerParms = new CodeExpression[1];
          readerParms[0] = new CodePrimitiveExpression(colIndex);

        CodeStatement[] trueStatements1 = new CodeStatement[1];
        trueStatements1[0] = new CodeAssignStatement(
          new CodeFieldReferenceExpression(vo, c.Name),
          new CodePrimitiveExpression(null));
                
        CodeStatement[] falseStatements1 = new CodeStatement[1];
        falseStatements1[0] = new CodeAssignStatement(
          new CodeFieldReferenceExpression(vo, c.Name), 
          new CodeMethodInvokeExpression(rd, GetReaderMethodByType(c.Type), readerParms));          

        CodeConditionStatement ifStatement = new CodeConditionStatement(
          new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("rd"), "IsDBNull", readerParms),
          trueStatements1,
          falseStatements1);
        statements.Add(ifStatement);
        colIndex++;        
      }      
    }
    
    private string GetReaderMethodByType(DBColumnType t){
      if (t == DBColumnType.DBBigInt)
        return "GetInt64";
      else if (t == DBColumnType.DBBinary)
        return "GetBytes";
      else if (t == DBColumnType.DBBit)
        return "GetBoolean";
      else if (t == DBColumnType.DBBlob)
        return "GetBytes";
      else if (t == DBColumnType.DBChar)
        return "GetString";
      else if (t == DBColumnType.DBDateTime)
        return "GetDateTime";
      else if (t == DBColumnType.DBDecimal)
        return "GetDecimal";
      else if (t == DBColumnType.DBFloat)
        return "GetFloat";
      else if (t == DBColumnType.DBGUID)
        return "GetGuid";
      else if (t == DBColumnType.DBInteger)
        return "GetInt32";
      else if (t == DBColumnType.DBMemo)
        return "GetString";
      else if (t == DBColumnType.DBMoney)
        return "GetFloat";        
      else if (t == DBColumnType.DBNumeric)
        return "GetDecimal";
      else if (t == DBColumnType.DBNVarchar)
        return "GetString";
      else if (t == DBColumnType.DBRowID)
        return "GetString";
      else if (t == DBColumnType.DBSmallDateTime)
        return "GetDateTime";
      else if (t == DBColumnType.DBSmallInt)
        return "GetInt16";
      else if (t == DBColumnType.DBTimeStamp)
        return "GetString";
      else if (t == DBColumnType.DBTinyInt)
        return "GetInt16";
      else if (t == DBColumnType.DBVarchar)
        return "GetString";
      else 
        throw new Exception(String.Format("Type not supported by SqlDataReader: {0}", t.ToString()));
    }
    
    private CodeTypeDeclaration GenerateBlobStreamClass(Table t, Column c){
      CodeTypeDeclaration blobClass = new CodeTypeDeclaration();
      blobClass.Name = t.TableName + c.Name + _BlobStreamClassSuffix;
      blobClass.IsClass = true;
      blobClass.Attributes = MemberAttributes.Public;
      blobClass.BaseTypes.Add(new CodeTypeReference(typeof(Stream)));
      
      blobClass.Members.Add(
        new CodeMemberField(typeof(int), "_ReadPosition"));
      
      blobClass.Members.Add(
        new CodeMemberField(typeof(SqlConnection), "_Connection"));
      blobClass.Members.Add(
        new CodeMemberField(typeof(SqlTransaction), "_Transaction"));
        
      foreach(Column col in t.PrimaryKeyColumns){
        blobClass.Members.Add(
          new CodeMemberField(GetTypeFromColumnType(col.Type), "_" + col.Name));          
      }
      
      blobClass.Members.Add(
        new CodeMemberField(
          new CodeTypeReference(typeof(object)), 
          "_TextPtrOut"));

      blobClass.Members.Add(
              new CodeMemberField(
                new CodeTypeReference(typeof(object)),
                "_LengthOut"));          
      
      blobClass.Members.Add(CreateBlobStreamConstructor(t));

      blobClass.Members.Add(CreateGetOnlyFixedReturnValueBooleanProperty("CanRead", true));
      blobClass.Members.Add(CreateGetOnlyFixedReturnValueBooleanProperty("CanWrite", true));
      blobClass.Members.Add(CreateGetOnlyFixedReturnValueBooleanProperty("CanSeek", false));            
      
      blobClass.Members.Add(GenerateUpdateTextPointerMethod(t, c));
      blobClass.Members.Add(GenerateBlobStreamWriteMethod(t, c));
      blobClass.Members.Add(GenerateBlobStreamReadMethod(t, c));
      CodeMemberMethod setLengthMethod = CreatePublicOverrideNotImplementedMethod("SetLength", null);  
      setLengthMethod.Parameters.Add(
        new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(long)), "value"));
      blobClass.Members.Add(setLengthMethod);
      
      CodeMemberMethod seekMethod = CreatePublicOverrideNotImplementedMethod("Seek", new CodeTypeReference(typeof(long)));
      seekMethod.Parameters.Add(
        new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(long)), "offset"));
      seekMethod.Parameters.Add(
        new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(SeekOrigin)), "origin"));
      blobClass.Members.Add(seekMethod);
      
      blobClass.Members.Add(CreatePublicOverrideMethod("Flush", null));
      
      CodeMemberProperty position = new CodeMemberProperty();
      position.Name = "Position";
      position.Type = new CodeTypeReference(typeof(long));
      position.Attributes = (MemberAttributes.Public | MemberAttributes.Override);

      CodeExpression[] convertParams = new CodeExpression[1];
      convertParams[0] = new CodeVariableReferenceExpression("_LengthOut");      
      position.GetStatements.Add(
        new CodeMethodReturnStatement(
          new CodeMethodInvokeExpression(
            new CodeMethodReferenceExpression(
              new CodeTypeReferenceExpression(typeof(Convert)), "ToInt64"),
              convertParams)));
      position.SetStatements.Add(
        new CodeThrowExceptionStatement(new CodeObjectCreateExpression(typeof(NotImplementedException))));
      blobClass.Members.Add(position);
      
      CodeMemberProperty length = new CodeMemberProperty();
      length.Name = "Length";
      length.Type = new CodeTypeReference(typeof(long));
      length.Attributes = (MemberAttributes.Public | MemberAttributes.Override);
      
      length.GetStatements.Add(
        new CodeMethodReturnStatement(
          new CodeCastExpression(
            new CodeTypeReference(typeof(long)), 
            new CodeVariableReferenceExpression("_LengthOut"))));
      blobClass.Members.Add(length);
      
      return blobClass;      
    }

    private CodeMemberMethod CreatePublicOverrideMethod(string Name, CodeTypeReference Type) {
      CodeMemberMethod method = new CodeMemberMethod();
      method.Name = Name;
      method.Attributes = (MemberAttributes.Public | MemberAttributes.Override);
      method.ReturnType = Type;      

      return method;
    }
    
    private CodeMemberMethod CreatePublicOverrideNotImplementedMethod(string Name, CodeTypeReference Type){
      CodeMemberMethod method = CreatePublicOverrideMethod(Name, Type);      
      
      method.Statements.Add(
        new CodeThrowExceptionStatement(
          new CodeObjectCreateExpression(typeof(NotImplementedException), new CodeExpression[0])));                
          
      return method;
    }
    
    private CodeMemberMethod GenerateUpdateTextPointerMethod(Table t, Column c){
      CodeMemberMethod updateTextPointerMethod = new CodeMemberMethod();
      updateTextPointerMethod.Name = "UpdateTextPointer";
      
      string sql = 
        "select " +
        "  @Ptr=TEXTPTR(" + c.Name + "), " +
        "  @Length=DATALENGTH(" + c.Name + ") " +
        "from " + t.TableName + " " +        
        "where " + 
        GetWhereByPk(t);
        
      updateTextPointerMethod.Statements.Add(CreateCommandDeclaration(new CodePrimitiveExpression(sql), "_Connection", "_Transaction"));
      CreateSqlParameter("ptrParam", "Ptr", SqlDbType.VarBinary, 100, updateTextPointerMethod.Statements, null, ParameterDirection.Output);
      CreateSqlParameter("lengthParam", "Length", SqlDbType.Int, sizeof(int), updateTextPointerMethod.Statements, null, ParameterDirection.Output);
      CreateSqlParametersPrimaryKey(updateTextPointerMethod.Statements, t, "_");      
      
      updateTextPointerMethod.Statements.Add(
        new CodeMethodInvokeExpression(
          new CodeMethodReferenceExpression(
            new CodeVariableReferenceExpression("cmd"), 
            "ExecuteNonQuery"), 
            new CodeExpression[0]));
      
      updateTextPointerMethod.Statements.Add(
        new CodeAssignStatement(
          new CodeVariableReferenceExpression("_TextPtrOut"),
          new CodeFieldReferenceExpression(
            new CodeVariableReferenceExpression("ptrParam"), "Value")));
      
      updateTextPointerMethod.Statements.Add(
        new CodeAssignStatement(
          new CodeVariableReferenceExpression("_LengthOut"),
          new CodeFieldReferenceExpression(
            new CodeVariableReferenceExpression("lengthParam"), "Value")));
            
      return updateTextPointerMethod;
    }
    
    private CodeMemberMethod GenerateBlobStreamWriteMethod(Table t, Column c){
      CodeMemberMethod writeMethod = new CodeMemberMethod();
      
      writeMethod.Name = "Write";
      writeMethod.Attributes = (MemberAttributes.Public | MemberAttributes.Override);
      
      writeMethod.Parameters.Add(
        new CodeParameterDeclarationExpression(
          new CodeTypeReference(typeof(byte[])),
          "buffer"));
      
      writeMethod.Parameters.Add(
        new CodeParameterDeclarationExpression(
          new CodeTypeReference(typeof(int)),
          "offset"));

      writeMethod.Parameters.Add(
        new CodeParameterDeclarationExpression(
          new CodeTypeReference(typeof(int)),
          "count"));          
      
      CodeStatementCollection firstChunkStatements = new CodeStatementCollection();
      string sqlFirst = 
        "update " + t.TableName + " set " + c.Name + " = @Chunk where " + GetWhereByPk(t);
      firstChunkStatements.Add(CreateCommandDeclaration(new CodePrimitiveExpression(sqlFirst), "_Connection", "_Transaction"));
      CreateSqlParameter("chunkParam", "Chunk", 
        SqlDbType.Binary, 
        new CodeVariableReferenceExpression("count"), 
        firstChunkStatements, 
        new CodeVariableReferenceExpression("buffer"), 
        ParameterDirection.Input);
      CreateSqlParametersPrimaryKey(firstChunkStatements, t, "_");
      firstChunkStatements.Add(
        new CodeMethodInvokeExpression(
          new CodeMethodReferenceExpression(
            new CodeVariableReferenceExpression("cmd"), "ExecuteNonQuery"), 
            new CodeExpression[0]));
            
      CodeStatementCollection nextChunksStatements = new CodeStatementCollection();
      string sqlNext = 
        "updatetext " + t.TableName + "." + c.Name + " @Ptr @Offset 0 WITH LOG @Chunk";
      nextChunksStatements.Add(CreateCommandDeclaration(new CodePrimitiveExpression(sqlNext), "_Connection", "_Transaction"));
      CreateSqlParameter("ptrParam", "Ptr", SqlDbType.VarBinary, 16, nextChunksStatements, new CodeVariableReferenceExpression("_TextPtrOut"), ParameterDirection.Input);
      CreateSqlParameter("offsetParam", "Offset", SqlDbType.Int, sizeof(int), nextChunksStatements, new CodeVariableReferenceExpression("_LengthOut"), ParameterDirection.Input);
      CreateSqlParameter("chunkParam", "Chunk", SqlDbType.Binary, new CodeVariableReferenceExpression("count"), nextChunksStatements, new CodeVariableReferenceExpression("buffer"), ParameterDirection.Input);
      nextChunksStatements.Add(
        new CodeMethodInvokeExpression(
          new CodeMethodReferenceExpression(
            new CodeVariableReferenceExpression("cmd"), "ExecuteNonQuery"),
            new CodeExpression[0]));        

      writeMethod.Statements.Add(
        new CodeConditionStatement(
          new CodeBinaryOperatorExpression(
            new CodeVariableReferenceExpression("_TextPtrOut"),
              CodeBinaryOperatorType.ValueEquality,            
              new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(DBNull)), "Value")),                                              
            CodeStatementCollectionToArray(firstChunkStatements),
            CodeStatementCollectionToArray(nextChunksStatements)));
      
      writeMethod.Statements.Add(
        new CodeMethodInvokeExpression(
          new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "UpdateTextPointer"), new CodeExpression[0]));
      
      return writeMethod;
    }
    
    private CodeStatement[] CodeStatementCollectionToArray(CodeStatementCollection col){
      CodeStatement[] arr = new CodeStatement[col.Count];
      foreach(CodeStatement st in col){
        arr[col.IndexOf(st)] = st;
      }
      return arr;
    }
    
    private CodeMemberMethod GenerateBlobStreamReadMethod(Table t, Column c){
      CodeMemberMethod readMethod = new CodeMemberMethod();
      readMethod.Name = "Read";
      readMethod.ReturnType = new CodeTypeReference(typeof(int));

      readMethod.Attributes = (MemberAttributes.Public | MemberAttributes.Override);

      readMethod.Parameters.Add(
        new CodeParameterDeclarationExpression(
          new CodeTypeReference(typeof(byte[])),
          "buffer"));

      readMethod.Parameters.Add(
        new CodeParameterDeclarationExpression(
          new CodeTypeReference(typeof(int)),
          "offset"));

      readMethod.Parameters.Add(
        new CodeParameterDeclarationExpression(
          new CodeTypeReference(typeof(int)),
          "count"));        
          
      CodeStatementCollection readPosGtLength = new CodeStatementCollection();
      readPosGtLength.Add(
        new CodeMethodReturnStatement(new CodePrimitiveExpression(0)));
      readMethod.Statements.Add(
        new CodeConditionStatement(
          new CodeBinaryOperatorExpression(
            new CodeVariableReferenceExpression("_ReadPosition"),
            CodeBinaryOperatorType.GreaterThanOrEqual,
            new CodeCastExpression(
              new CodeTypeReference(typeof(int)),
              new CodeVariableReferenceExpression("_LengthOut"))), CodeStatementCollectionToArray(readPosGtLength)));              
    
      
      readMethod.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(int)), "bytesToRead"));
      CodeStatementCollection lessThanLengthStatements = new CodeStatementCollection();
      lessThanLengthStatements.Add(
        new CodeAssignStatement(
          new CodeVariableReferenceExpression("bytesToRead"),
          new CodeCastExpression(typeof(int),
            new CodeBinaryOperatorExpression(
              new CodeCastExpression(typeof(int), new CodeVariableReferenceExpression("_LengthOut")),
              CodeBinaryOperatorType.Subtract,
              new CodeVariableReferenceExpression("_ReadPosition")))));
              
      CodeStatementCollection readPosLessOrEqLength = new CodeStatementCollection();
      readPosLessOrEqLength.Add(
        new CodeAssignStatement(
          new CodeVariableReferenceExpression("bytesToRead"),
          new CodeVariableReferenceExpression("count")));
      
      readMethod.Statements.Add(
        new CodeConditionStatement(
          new CodeBinaryOperatorExpression(
            new CodeBinaryOperatorExpression(
              new CodeVariableReferenceExpression("_ReadPosition"),
              CodeBinaryOperatorType.Add,
              new CodeVariableReferenceExpression("count")),
            CodeBinaryOperatorType.GreaterThan,
              new CodeCastExpression(typeof(int), new CodeVariableReferenceExpression("_LengthOut"))), 
          CodeStatementCollectionToArray(readPosGtLength),
          CodeStatementCollectionToArray(readPosLessOrEqLength)));
      
      string sql = 
        "readtext " + t.TableName + "." + c.Name + " @Ptr @Offset @Size HOLDLOCK";
      
      readMethod.Statements.Add(
        CreateCommandDeclaration(new CodePrimitiveExpression(sql), "_Connection", "_Transaction"));

      CreateSqlParameter("ptrParam", "Ptr", SqlDbType.VarBinary, 16, readMethod.Statements, new CodeVariableReferenceExpression("_TextPtrOut"), ParameterDirection.Input);
      CreateSqlParameter("offsetParam", "Offset", SqlDbType.Int, sizeof(int), readMethod.Statements, new CodeVariableReferenceExpression("_ReadPosition"), ParameterDirection.Input);
      CreateSqlParameter("sizeParam", "Size", SqlDbType.Int, sizeof(int), readMethod.Statements, new CodeVariableReferenceExpression("bytesToRead"), ParameterDirection.Input);      
      
      CodeExpression[] readerParms = new CodeExpression[1];
      readerParms[0] = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(CommandBehavior)), "SingleResult");
      
      readMethod.Statements.Add(
        new CodeVariableDeclarationStatement(
          new CodeTypeReference(typeof(SqlDataReader)), "rd",
          new CodeMethodInvokeExpression(
            new CodeMethodReferenceExpression(
              new CodeVariableReferenceExpression("cmd"), "ExecuteReader"),
              readerParms)));
      readMethod.Statements.Add(
        new CodeMethodInvokeExpression(
          new CodeMethodReferenceExpression(
            new CodeVariableReferenceExpression("rd"),
            "Read")));
            
      CodeExpression[] getBytesParms = new CodeExpression[5];
      getBytesParms[0] = new CodePrimitiveExpression(0);
      getBytesParms[1] = new CodePrimitiveExpression(0);
      getBytesParms[2] = new CodeVariableReferenceExpression("buffer");
      getBytesParms[3] = new CodeVariableReferenceExpression("offset");
      getBytesParms[4] = new CodeVariableReferenceExpression("bytesToRead");
      readMethod.Statements.Add(
        new CodeMethodInvokeExpression(
          new CodeMethodReferenceExpression(
            new CodeVariableReferenceExpression("rd"), "GetBytes"),
            getBytesParms));
      readMethod.Statements.Add(
        new CodeMethodInvokeExpression(
          new CodeMethodReferenceExpression(
            new CodeVariableReferenceExpression("rd"), "Close"),
            new CodeExpression[0]));
            
      readMethod.Statements.Add(
        new CodeAssignStatement(
          new CodeVariableReferenceExpression("_ReadPosition"),
          new CodeBinaryOperatorExpression(
            new CodeVariableReferenceExpression("_ReadPosition"),
            CodeBinaryOperatorType.Add,
            new CodeVariableReferenceExpression("bytesToRead"))));         
      
      readMethod.Statements.Add(
        new CodeMethodReturnStatement(
          new CodeVariableReferenceExpression("bytesToRead")));
      
      return readMethod;
    }
    
    private CodeConstructor CreateBlobStreamConstructor(Table t){
      CodeConstructor constructor = new CodeConstructor();      
      constructor.Attributes = MemberAttributes.Public;

      AddDefaultParams(constructor);
      AddPrimaryKeyParams(constructor, t);

      constructor.Statements.Add(
        new CodeAssignStatement(
          new CodeVariableReferenceExpression("_Connection"),
          new CodeVariableReferenceExpression("Connection")));

      constructor.Statements.Add(
        new CodeAssignStatement(
          new CodeVariableReferenceExpression("_Transaction"),
          new CodeVariableReferenceExpression("Transaction")));

      foreach (Column col in t.PrimaryKeyColumns) {
        constructor.Statements.Add(
          new CodeAssignStatement(
            new CodeVariableReferenceExpression("_" + col.Name),
            new CodeVariableReferenceExpression(col.Name)));
      }
      
      constructor.Statements.Add(
        new CodeMethodInvokeExpression(
          new CodeMethodReferenceExpression(
            new CodeThisReferenceExpression(), "UpdateTextPointer"), new CodeExpression[0]));
       
      return constructor;   
    }
    
    private CodeMemberProperty CreateGetOnlyFixedReturnValueBooleanProperty(string Name, bool value){
      CodeMemberProperty prop = new CodeMemberProperty();
      prop.Name = Name;
      prop.Attributes = (MemberAttributes.Public | MemberAttributes.Override);
      prop.Type = new CodeTypeReference(typeof(bool));
      
      prop.GetStatements.Add(
        new CodeMethodReturnStatement(new CodePrimitiveExpression(value)));
        
      return prop;
    }

    private void GenerateProcedureInputVO(Procedure p) {
      CodeNamespace codeNS = new CodeNamespace(_Namespace + "." + ProcInputVONamespace);

      CodeTypeDeclaration voClass = new CodeTypeDeclaration(p.Name + ProcInputVOClassSuffix);
      voClass.IsClass = true;
      foreach (Parameter param in p.InputParameters) {
        if (IsStreamedType(param.Type))
          continue;
        CodeMemberField mf = new CodeMemberField(GetTypeFromColumnType(param.Type), "_" + param.Name);
        voClass.Members.Add(mf);

        CodeMemberProperty mp = new CodeMemberProperty();
        mp.Attributes = MemberAttributes.Public;
        mp.HasGet = true;
        mp.HasSet = true;
        mp.Name = param.Name;
        mp.Type = new CodeTypeReference(GetTypeFromColumnType(param.Type));

        mp.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), mf.Name), new CodePropertySetValueReferenceExpression()));
        mp.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), mf.Name)));

        voClass.Members.Add(mp);
      }
      codeNS.Types.Add(voClass);

      CSharpCodeProvider csharp = new CSharpCodeProvider();

      CodeGeneratorOptions cop = new CodeGeneratorOptions();
      cop.IndentString = "  ";

      if (!Directory.Exists(OutputDir + "\\" + _ProcInputVONamespace))
        Directory.CreateDirectory(OutputDir + "\\" + _ProcInputVONamespace);

      FileStream fs = new FileStream(OutputDir + "\\" + _ProcInputVONamespace + "\\" + p.Name + ProcInputVOClassSuffix + ".cs", FileMode.Create, FileAccess.Write);
      StreamWriter sw = new StreamWriter(fs);
      csharp.GenerateCodeFromNamespace(codeNS, sw, cop);
      sw.Flush();
      sw.Close();
      fs.Close();
    }

    private void GenerateProcedureOutputVOs(Procedure p) {          
      
      foreach(RecordSet rs in p.RecordSets){
        string VOClassName;
        if (p.RecordSets.Count > 1)
          VOClassName = p.Name + "RS" + (p.RecordSets.IndexOf(rs) + 1).ToString() + ProcOutputVOClassSuffix;
        else
          VOClassName = p.Name + ProcOutputVOClassSuffix;
      
        CodeNamespace codeNS = new CodeNamespace(_Namespace + "." + ProcOutputVONamespace);

        CodeTypeDeclaration voClass = new CodeTypeDeclaration(VOClassName);
        voClass.IsClass = true;
        foreach (Parameter param in rs.Parameters) {
          if (IsStreamedType(param.Type))
            continue;
          CodeMemberField mf = new CodeMemberField(GetTypeFromColumnType(param.Type), "_" + param.Name);
          voClass.Members.Add(mf);

          CodeMemberProperty mp = new CodeMemberProperty();
          mp.Attributes = MemberAttributes.Public;
          mp.HasGet = true;
          mp.HasSet = true;
          mp.Name = param.Name;
          mp.Type = new CodeTypeReference(GetTypeFromColumnType(param.Type));

          mp.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), mf.Name), new CodePropertySetValueReferenceExpression()));
          mp.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), mf.Name)));

          voClass.Members.Add(mp);
        }
        codeNS.Types.Add(voClass);

        CSharpCodeProvider csharp = new CSharpCodeProvider();

        CodeGeneratorOptions cop = new CodeGeneratorOptions();
        cop.IndentString = "  ";

        if (!Directory.Exists(OutputDir + "\\" + ProcOutputVONamespace))
          Directory.CreateDirectory(OutputDir + "\\" + ProcOutputVONamespace);

        FileStream fs = new FileStream(OutputDir + "\\" + ProcOutputVONamespace + "\\" + VOClassName + ".cs", FileMode.Create, FileAccess.Write);
        StreamWriter sw = new StreamWriter(fs);
        csharp.GenerateCodeFromNamespace(codeNS, sw, cop);
        sw.Flush();
        sw.Close();
        fs.Close();
      }
    }        

    #endregion
  }
}
