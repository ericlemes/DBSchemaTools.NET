using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Model;
using DBInfo.Core.Statement;

namespace DBInfo.Core {
  public class DatabaseToStatementCollectionConverter {
    public List<BaseStatement> Convert(Database db){
      List<BaseStatement> l = new List<BaseStatement>();
      
      foreach(Table t in db.Tables){
        l.Add(GenerateCreateTableStatement(t));
      }
      foreach(Table t in db.Tables){
        l.Add(GenerateCreatePrimaryKeyStatement(t));
      }
      foreach(Table t in db.Tables){
        foreach(ForeignKey fk in t.ForeignKeys)
          l.Add(GenerateCreateForeignKeyStatement(fk));
      }
      foreach(Table t in db.Tables){
        foreach(Index i in t.Indexes)
          l.Add(GenerateCreateIndexStatement(i));
      }
      foreach(Procedure p in db.Procedures)
        l.Add(GenerateCreateProcedureStatement(p));       
      foreach(Function f in db.Functions)
        l.Add(GenerateCreateFunctionStatement(f));
      foreach(Table t in db.Tables){
        foreach(Trigger tr in t.Triggers)          
          l.Add(GenerateCreateTriggerStatement(tr));
      }
      foreach(View v in db.Views)
        l.Add(GenerateCreateViewStatement(v));
            
      return l;
    }
    
    public CreateTable GenerateCreateTableStatement(Table t){
      CreateTable ct = new CreateTable();
      ct.Table = t;
      return ct;
    }
    
    public CreatePrimaryKey GenerateCreatePrimaryKeyStatement(Table t){
      CreatePrimaryKey cpk = new CreatePrimaryKey();
      cpk.Table = t;
      return cpk;
    }
    
    public CreateForeignKey GenerateCreateForeignKeyStatement(ForeignKey fk){
      CreateForeignKey cfk = new CreateForeignKey();
      cfk.ForeignKey = fk;
      return cfk;
    }
    
    public CreateIndex GenerateCreateIndexStatement(Index i){
      CreateIndex ci = new CreateIndex();
      ci.Index = i;
      return ci;
    }
    
    public CreateProcedure GenerateCreateProcedureStatement(Procedure p){
      CreateProcedure cp = new CreateProcedure();
      cp.Procedure = p;
      return cp;
    }
    
    public CreateFunction GenerateCreateFunctionStatement(Function f){
      CreateFunction cf = new CreateFunction();
      cf.Function = f;
      return cf;
    }
    
    public CreateTrigger GenerateCreateTriggerStatement(Trigger t){
      CreateTrigger ct = new CreateTrigger();
      ct.Trigger = t;
      return ct;
    }
    
    public CreateView GenerateCreateViewStatement(View v){
      CreateView cv = new CreateView();
      cv.View = v;
      return cv;
    }
  }
}
