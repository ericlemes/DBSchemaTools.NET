using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Extractor;
using System.IO;
using System.Xml.Serialization;
using DBInfo.Core.Statement;

namespace DBInfo.XMLDatabase {
  public class XMLDatabaseExtractor : IScriptExtractor {           
    private string _InputDir;
    public string InputDir{
      get { return _InputDir;}
      set { _InputDir = value;}
    }
    
    private string _InputConnectionString;
    public string InputConnectionString{
      get { return _InputConnectionString;}
      set { _InputConnectionString = value;}
    }    
    
    private IDatabaseExtractor _Extractor;
    public IDatabaseExtractor Extractor{
      get { return _Extractor;}
      set { _Extractor = value;}
    }
    
    private List<string> _InputFiles = new List<string>();
    public List<string> InputFiles{
      get { return _InputFiles;}
    }
    
    public List<BaseStatement> ExtractXMLDatabase(List<string> XMLFiles){
      List<BaseStatement> statementCol = new List<BaseStatement>();      
      
      foreach(string xmlFile in XMLFiles){
        ParseXMLFile(xmlFile, statementCol);
      }      

      return statementCol;
    }
    
    private void ParseXMLFile(string xmlFile, List<BaseStatement> statementCol){
      FileStream fs = new FileStream(xmlFile, FileMode.Open, FileAccess.Read);
      XmlSerializer xs = new XmlSerializer(typeof(StatementCollection));
      StatementCollection sc = (StatementCollection)xs.Deserialize(fs);
      fs.Close();
      
      if (sc.Statement == null)
        return;
      
      foreach(Statement s in sc.Statement){        
        if (s is CreateTable)
          statementCol.Add(ParseCreateTableStatement((CreateTable)s));        
        else if (s is CreateForeignKey)
          statementCol.Add(ParseCreateForeignKeyStatement((CreateForeignKey)s));
        else if (s is CreateIndex)
          statementCol.Add(ParseCreateIndexStatement((CreateIndex)s));
        else if (s is CreateCheckConstraint)
          statementCol.Add(ParseCreateCheckConstraintStatement((CreateCheckConstraint)s));
        else if (s is CreateTrigger)
          statementCol.Add(ParseCreateTriggerStatement((CreateTrigger)s));
        else if (s is CreateProcedure)
          statementCol.Add(ParseCreateProcedureStatement((CreateProcedure)s));
        else if (s is CreatePrimaryKey)
          statementCol.Add(ParseCreatePrimaryKeyStatement((CreatePrimaryKey)s));
        else if (s is CreateFunction)
          statementCol.Add(ParseCreateFunctionStatement((CreateFunction)s));
        else if (s is CreateView)
          statementCol.Add(ParseCreateViewStatement((CreateView)s));                        
      }
    }
    
    private DBInfo.Core.Model.DBColumnType GetDBColumnType(ColumnType ct){
      if (ct == ColumnType.BigInt)
        return DBInfo.Core.Model.DBColumnType.BigInt;
      else if (ct == ColumnType.Binary)
        return DBInfo.Core.Model.DBColumnType.Binary;
      else if (ct == ColumnType.Bit)
        return DBInfo.Core.Model.DBColumnType.Bit;
      else if (ct == ColumnType.Char)
        return DBInfo.Core.Model.DBColumnType.Char;
      else if (ct == ColumnType.DateTime)
        return DBInfo.Core.Model.DBColumnType.DateTime;
      else if (ct == ColumnType.Decimal)
        return DBInfo.Core.Model.DBColumnType.Decimal;
      else if (ct == ColumnType.Float)
        return DBInfo.Core.Model.DBColumnType.Float;
      else if (ct == ColumnType.UniqueIdentifier)
        return DBInfo.Core.Model.DBColumnType.UniqueIdentifier;
      else if (ct == ColumnType.Integer)
        return DBInfo.Core.Model.DBColumnType.Integer;
      else if (ct == ColumnType.Text)
        return DBInfo.Core.Model.DBColumnType.Text;
      else if (ct == ColumnType.Money)
        return DBInfo.Core.Model.DBColumnType.Money;
      else if (ct == ColumnType.Numeric)
        return DBInfo.Core.Model.DBColumnType.Numeric;
      else if (ct == ColumnType.NVarchar)
        return DBInfo.Core.Model.DBColumnType.NVarchar;
      else if (ct == ColumnType.SmallDateTime)
        return DBInfo.Core.Model.DBColumnType.SmallDateTime;
      else if (ct == ColumnType.SmallInt)
        return DBInfo.Core.Model.DBColumnType.SmallInt;
      else if (ct == ColumnType.TimeStamp)
        return DBInfo.Core.Model.DBColumnType.TimeStamp;
      else if (ct == ColumnType.TinyInt)
        return DBInfo.Core.Model.DBColumnType.TinyInt;
      else if (ct == ColumnType.Varchar)
        return DBInfo.Core.Model.DBColumnType.VarChar;
      else if (ct == ColumnType.Real)
        return DBInfo.Core.Model.DBColumnType.Real;
      else if (ct == ColumnType.Image)
        return DBInfo.Core.Model.DBColumnType.Image;
      else if (ct == ColumnType.NChar)
        return DBInfo.Core.Model.DBColumnType.NChar;
      else if (ct == ColumnType.SmallMoney)
        return DBInfo.Core.Model.DBColumnType.SmallMoney;
      else if (ct == ColumnType.VarBinary)
        return DBInfo.Core.Model.DBColumnType.VarBinary;
      else if (ct == ColumnType.Xml)
        return DBInfo.Core.Model.DBColumnType.Xml;        
      else if (ct == ColumnType.NText)
        return DBInfo.Core.Model.DBColumnType.NText;
      else
        throw new Exception(String.Format("Type not supported: {0}", ct.ToString()));      
    }
    
    private DBInfo.Core.Statement.CreateTable ParseCreateTableStatement(CreateTable ct){
      DBInfo.Core.Model.Table t = new DBInfo.Core.Model.Table();
      t.TableName = ct.TableName;
      t.HasIdentity = ct.HasIdentity == YesNo.Yes;
      t.IdentitySeed = Convert.ToInt32(ct.IdentitySeed);
      t.IdentityIncrement = Convert.ToInt32(ct.IdentityIncrement);
      if (ct.Columns != null){
        foreach(Column ctc in ct.Columns){
          DBInfo.Core.Model.Column c = new DBInfo.Core.Model.Column();
          c.Name = ctc.Name;
          c.Type = GetDBColumnType(ctc.Type);
          c.Size = Convert.ToInt32(ctc.Size);
          c.Precision = Convert.ToInt32(ctc.Precision);
          c.Scale = Convert.ToInt32(ctc.Scale);
          c.IsNull = ctc.Nullable == YesNo.Yes; 
          c.IdentityColumn = ctc.IdentityColumn == YesNo.Yes;
          c.DefaultValue = ctc.DefaultValue;
          c.ConstraintDefaultName = ctc.ConstraintDefaultName;
          t.Columns.Add(c);
        }
      }      
      
      DBInfo.Core.Statement.CreateTable createTable = new DBInfo.Core.Statement.CreateTable();
      createTable.Table = t;
      
      return createTable;
    }
    
    private DBInfo.Core.Statement.CreateForeignKey ParseCreateForeignKeyStatement(CreateForeignKey xmlFK){      
      DBInfo.Core.Model.ForeignKey fk = new DBInfo.Core.Model.ForeignKey();
      fk.TableName = xmlFK.TableName;
      fk.ForeignKeyName = xmlFK.ForeignKeyName;
      fk.RefTableName = xmlFK.RefTableName;
      fk.DeleteCascade = xmlFK.DeleteCascade == YesNo.Yes;
      fk.UpdateCascade = xmlFK.UpdateCascade == YesNo.Yes;
      
      if (xmlFK.Columns != null){
        foreach(ForeignKeyColumn xmlFKCol in xmlFK.Columns){
          DBInfo.Core.Model.ForeignKeyColumn fkCol = new DBInfo.Core.Model.ForeignKeyColumn();
          fkCol.Column = xmlFKCol.ColumnName;          
          fkCol.RefTable = xmlFK.RefTableName;          
          fkCol.RefColumn = xmlFKCol.RefColumnName;
          fk.Columns.Add(fkCol);
        }
      }
      
      DBInfo.Core.Statement.CreateForeignKey cfk = new DBInfo.Core.Statement.CreateForeignKey();
      cfk.ForeignKey = fk;
      return cfk;
    }
    
    private DBInfo.Core.Statement.CreateIndex ParseCreateIndexStatement(CreateIndex xmlIndex){     
      DBInfo.Core.Model.Index i = new DBInfo.Core.Model.Index();
      i.TableName = xmlIndex.TableName;
      i.IndexName = xmlIndex.IndexName;
      i.Unique = xmlIndex.Unique == YesNo.Yes;
      i.IsClustered = xmlIndex.Clustered == YesNo.Yes;
      
      if (xmlIndex.Columns != null){
        foreach(IndexColumn xmlIndexCol in xmlIndex.Columns){
          DBInfo.Core.Model.IndexColumn ic = new DBInfo.Core.Model.IndexColumn();
          ic.Column = xmlIndexCol.Name;          
          ic.Order = xmlIndexCol.Order == SortOrder.Ascending ? DBInfo.Core.Model.IndexColumn.EnumOrder.Ascending : DBInfo.Core.Model.IndexColumn.EnumOrder.Descending;
          i.Columns.Add(ic);
        }
      }
      
      DBInfo.Core.Statement.CreateIndex cidx = new DBInfo.Core.Statement.CreateIndex();
      cidx.Index = i;
      return cidx;
    }
    
    private DBInfo.Core.Statement.CreateCheckConstraint ParseCreateCheckConstraintStatement(CreateCheckConstraint xmlCheck){      
      DBInfo.Core.Model.CheckConstraint ck = new DBInfo.Core.Model.CheckConstraint();
      ck.TableName = xmlCheck.TableName;
      ck.CheckConstraintName = xmlCheck.CheckConstraintName;
      ck.Expression = xmlCheck.SourceCode;
      
      DBInfo.Core.Statement.CreateCheckConstraint scc = new DBInfo.Core.Statement.CreateCheckConstraint();
      scc.CheckConstraint = ck;
      
      return scc;           
    }
    
    private DBInfo.Core.Statement.CreateTrigger ParseCreateTriggerStatement(CreateTrigger xmlTrigger){      
      DBInfo.Core.Model.Trigger tr = new DBInfo.Core.Model.Trigger();      
      tr.TableName = xmlTrigger.TableName;
      tr.Body = xmlTrigger.SourceCode;
      tr.TriggerName = xmlTrigger.TriggerName;
      
      DBInfo.Core.Statement.CreateTrigger ctrigger = new DBInfo.Core.Statement.CreateTrigger();
      ctrigger.Trigger = tr;
      return ctrigger;      
    }
    
    private DBInfo.Core.Model.ParamDirection GetParameterDirection(ParameterDirection d){
      if (d == ParameterDirection.Input)
        return DBInfo.Core.Model.ParamDirection.Input;
      else if (d == ParameterDirection.Output)
        return DBInfo.Core.Model.ParamDirection.Output;
      else if (d == ParameterDirection.InputOutput)
        return DBInfo.Core.Model.ParamDirection.InputOutput;
      else if (d == ParameterDirection.ReturnValue)
        return DBInfo.Core.Model.ParamDirection.ReturnValue;
      else
        throw new Exception(String.Format("Invalid parameter direction {0}", d.ToString()));
    }
    
    private DBInfo.Core.Statement.CreateProcedure ParseCreateProcedureStatement(CreateProcedure xmlProcedure){
      DBInfo.Core.Model.Procedure p = new DBInfo.Core.Model.Procedure();
      p.Body = xmlProcedure.SourceCode;
      p.Name = xmlProcedure.Name;
      
      if (xmlProcedure.InputParameters != null){
        foreach(Parameter xmlParam in xmlProcedure.InputParameters){
          DBInfo.Core.Model.Parameter param = new DBInfo.Core.Model.Parameter();
          param.Name = xmlParam.Name;
          param.Type = GetDBColumnType(xmlParam.Type);
          param.Direction = GetParameterDirection(xmlParam.Direction);
          param.Size = Convert.ToInt32(xmlParam.Size);
          param.Scale = Convert.ToInt32(xmlParam.Scale);
          param.Precision = Convert.ToInt32(xmlParam.Precision);
          p.InputParameters.Add(param);          
        }
      }
      if (xmlProcedure.RecordSets != null){
        foreach (RecordSetDef xmlRS in xmlProcedure.RecordSets){
          DBInfo.Core.Model.RecordSet rs = new DBInfo.Core.Model.RecordSet();
          p.RecordSets.Add(rs);
          if (xmlRS.Parameters != null){
            foreach(Parameter xmlParam in xmlRS.Parameters){
              DBInfo.Core.Model.Parameter param = new DBInfo.Core.Model.Parameter();
              param.Name = xmlParam.Name;
              param.Type = GetDBColumnType(xmlParam.Type);
              param.Direction = GetParameterDirection(xmlParam.Direction);
              param.Size = Convert.ToInt32(xmlParam.Size);
              param.Scale = Convert.ToInt32(xmlParam.Scale);
              param.Precision = Convert.ToInt32(xmlParam.Precision);
              rs.Parameters.Add(param);
            }
          }
        }        
      }
      
      DBInfo.Core.Statement.CreateProcedure cp = new DBInfo.Core.Statement.CreateProcedure();
      cp.Procedure = p;
      
      return cp;
    }
    
    private DBInfo.Core.Statement.CreatePrimaryKey ParseCreatePrimaryKeyStatement(CreatePrimaryKey xmlPrimaryKey){      
      DBInfo.Core.Model.Table t = new DBInfo.Core.Model.Table();
      t.TableName = xmlPrimaryKey.TableName;
      t.PrimaryKeyName = xmlPrimaryKey.PrimaryKeyName;
      foreach(string c in xmlPrimaryKey.Columns){        
        t.PrimaryKeyColumns.Add(c);
      }
      DBInfo.Core.Statement.CreatePrimaryKey cpk = new DBInfo.Core.Statement.CreatePrimaryKey();
      cpk.Table = t;
      return cpk;
    }

    private DBInfo.Core.Statement.CreateFunction ParseCreateFunctionStatement(CreateFunction xmlFunction) {
      DBInfo.Core.Model.Function f = new DBInfo.Core.Model.Function();
      f.Body = xmlFunction.SourceCode;
      f.Name = xmlFunction.Name;
      
      DBInfo.Core.Statement.CreateFunction cf = new DBInfo.Core.Statement.CreateFunction();
      cf.Function = f;
      
      return cf;
    }   
    
    private DBInfo.Core.Statement.CreateView ParseCreateViewStatement(CreateView xmlView){
      DBInfo.Core.Model.View v = new DBInfo.Core.Model.View();
      v.Name = xmlView.Name;
      v.Body = xmlView.SourceCode;
      
      DBInfo.Core.Statement.CreateView cv = new DBInfo.Core.Statement.CreateView();
      cv.View = v;
      
      return cv;
    }
        

    public List<BaseStatement> Extract(List<DBObjectType> dataToExtract){
      return ExtractXMLDatabase(_InputFiles);
    }    
    
  }
}
