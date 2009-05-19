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
    
    private string _ClassSuffix = "DatabaseVO";
    public string ClassSuffix{
      get { return _ClassSuffix;}
      set { _ClassSuffix = value;}
    }
    
    private string _VONamespace = "DatabaseVO";
    public string VONamespace{
      get { return _VONamespace;}
      set { _VONamespace = value;}
    }

    public void GenerateOutput(Database db, List<DBObjectType> dataToGenerateOutput) {      
      foreach(Table t in db.Tables){
        GenerateDatabaseVO(t);
      }
    }
    
    private Type GetTypeFromColumnType(DBColumnType t){
      if (t == DBColumnType.DBBigInt)
        return typeof(long?);
      else if (t == DBColumnType.DBBit)
        return typeof(bool?);
      else if (t == DBColumnType.DBChar)
        return typeof(char?);
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
        return typeof(float?);
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
      return false;
    }
    
    private void GenerateDatabaseVO(Table t){      
      CodeNamespace codeNS = new CodeNamespace(_Namespace + "." + VONamespace);

      CodeTypeDeclaration voClass = new CodeTypeDeclaration(t.TableName + _ClassSuffix);
      voClass.IsClass = true;
      foreach(Column c in t.Columns){      
        if (IsStreamedType(c.Type))
          continue;
        CodeMemberField mf = new CodeMemberField(GetTypeFromColumnType(c.Type), "_" + c.Name);
        voClass.Members.Add(mf);               
        
        CodeMemberProperty mp = new CodeMemberProperty();
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

      FileStream fs = new FileStream(OutputDir + "\\" + _VONamespace + "\\" + t.TableName + _ClassSuffix + ".cs", FileMode.Create, FileAccess.Write);      
      StreamWriter sw = new StreamWriter(fs);
      csharp.GenerateCodeFromNamespace(codeNS, sw, cop);      
      sw.Flush();
      sw.Close();
      fs.Close();
    }

    #endregion
  }
}
