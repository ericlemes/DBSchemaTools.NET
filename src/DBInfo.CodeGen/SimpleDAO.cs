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

    public void GenerateOutput(Database db, List<DBObjectType> dataToGenerateOutput) {      
      foreach(Table t in db.Tables){
        GenerateDatabaseVO(t);
        GenerateDatabaseDAO(t);
      }
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

    private void GenerateDatabaseDAO(Table t) {
      CodeNamespace codeNS = new CodeNamespace(_Namespace + "." + DAONamespace);
      codeNS.Imports.Add(new CodeNamespaceImport(_VONamespace));
                                    
      CodeTypeDeclaration daoClass = new CodeTypeDeclaration(t.TableName + _DAOClassSuffix);
      codeNS.Types.Add(daoClass);
      daoClass.IsClass = true;
            
      CodeMemberField asterField = new CodeMemberField(new CodeTypeReference(typeof(string)), "Aster");            
      asterField.Attributes = MemberAttributes.Public;
      asterField.Attributes &= MemberAttributes.Static;                  
      asterField.InitExpression = new CodePrimitiveExpression(GetAster(t, "", true, true));
      daoClass.Members.Add(asterField);

      daoClass.Members.Add(GenerateLoadMethod(t));
      daoClass.Members.Add(GenerateInsertMethod(t));

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
      CodeExpression[] commandCreateParams = new CodeExpression[3];
      commandCreateParams[1] = new CodeVariableReferenceExpression("Connection");
      commandCreateParams[2] = new CodeVariableReferenceExpression("Transaction");
      commandCreateParams[0] = InitExpression;
                
      CodeVariableDeclarationStatement command = new CodeVariableDeclarationStatement(
        typeof(SqlCommand), "cmd",
        new CodeObjectCreateExpression(
          new CodeTypeReference(typeof(SqlCommand)),
          commandCreateParams)
        );
        
      return command;
    }
    
    
    private CodeMemberMethod GenerateLoadMethod(Table t){
      CodeMemberMethod loadMethod = new CodeMemberMethod();
      loadMethod.Name = "Load";      
      loadMethod.Attributes = MemberAttributes.Public;      
      loadMethod.ReturnType = new CodeTypeReference(t.TableName + _VOClassSuffix);
      
      AddDefaultParams(loadMethod);

      foreach (Column c in t.PrimaryKeyColumns) {
        CodeParameterDeclarationExpression param = new CodeParameterDeclarationExpression(
          GetTypeFromColumnType(c.Type),
          c.Name);
        loadMethod.Parameters.Add(param);
      }

      CodeExpression selectAsterFromTableWhereByPk = CreateSelectAsterFromTableNameExpression(t, 
        " where " + GetWhereByPk(t));
      loadMethod.Statements.Add(CreateCommandDeclaration(selectAsterFromTableWhereByPk));      
      
      int ParamCount = 1;
      foreach(Column c in t.PrimaryKeyColumns){
        CodeVariableReferenceExpression paramVar = new CodeVariableReferenceExpression("p" + ParamCount.ToString());        
        
        CreateSqlParameter(c, "p" + ParamCount.ToString(), loadMethod, new CodeVariableReferenceExpression(c.Name));      
        ParamCount++;
      }
      
      loadMethod.Statements.Add(
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
        
      loadMethod.Statements.Add(
        new CodeConditionStatement(
          new CodeMethodInvokeExpression(
            new CodeVariableReferenceExpression("rd"), 
            "Read", 
            new CodeExpression[0]), trueStatements2, falseStatements2));      
      
      return loadMethod;
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
    
    private CodeMemberMethod GenerateInsertMethod(Table t){
      CodeMemberMethod insertMethod = new CodeMemberMethod();
      insertMethod.Attributes = MemberAttributes.Public;
      insertMethod.Name = "Insert";
      
      AddDefaultParams(insertMethod);
      
      CodeParameterDeclarationExpression voParam = new CodeParameterDeclarationExpression(
        t.TableName + _VOClassSuffix, "vo");
      insertMethod.Parameters.Add(voParam);

      string sql = 
        "insert into " + t.TableName + " ( " + GetAster(t, "", false, false) + ") values (" + GetAster(t, "@", false, false) + ")" +
        Environment.NewLine + 
        "select SCOPE_IDENTITY()";
      
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
    
    private void CreateSqlParameter(Column c, string paramName, CodeMemberMethod method, CodeExpression valueExpression){
      CodeVariableReferenceExpression paramVar = new CodeVariableReferenceExpression(paramName);

      CodeVariableDeclarationStatement param = new CodeVariableDeclarationStatement(typeof(SqlParameter), paramName, new CodeObjectCreateExpression(typeof(SqlParameter), new CodeExpression[0]));
      method.Statements.Add(param);
      method.Statements.Add(
        new CodeAssignStatement(
          new CodeFieldReferenceExpression(paramVar, "ParameterName"),
          new CodePrimitiveExpression("@" + c.Name)));
      method.Statements.Add(
        new CodeAssignStatement(
          new CodeFieldReferenceExpression(paramVar, "SqlDbType"),
          new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SqlDbType)), GetSqlDbType(c.Type).ToString())));
      if (c.Size > 0) {
        method.Statements.Add(
          new CodeAssignStatement(
            new CodeFieldReferenceExpression(paramVar, "Size"),
            new CodePrimitiveExpression(c.Size)));
      }
      method.Statements.Add(new CodeAssignStatement(
        new CodeFieldReferenceExpression(paramVar, "Value"),
        valueExpression));

      method.Statements.Add(
        new CodeMethodInvokeExpression(
          new CodeFieldReferenceExpression(new CodeVariableReferenceExpression("cmd"), "Parameters"),
          "Add",
          new CodeVariableReferenceExpression(paramName)));
    }
    
    
    private void AssignReaderOutputToNewVO(Table t, CodeStatementCollection statements){
      CodeVariableDeclarationStatement newVO = new CodeVariableDeclarationStatement(t.TableName + _VOClassSuffix, "vo", new CodeObjectCreateExpression(t.TableName + _VOClassSuffix, new CodeExpression[0]));
      statements.Add(newVO);
      
      CodeVariableReferenceExpression vo = new CodeVariableReferenceExpression("vo");
      CodeVariableReferenceExpression rd = new CodeVariableReferenceExpression("rd");
      
      int colIndex = 0;
      foreach(Column c in t.Columns){
        if (!IsStreamedType(c.Type)){
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
        }
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

    #endregion
  }
}
