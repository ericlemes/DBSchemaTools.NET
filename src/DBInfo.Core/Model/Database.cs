using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DBInfo.Core.Model {
  public class Database {
    private List<Table> _Tables = new List<Table>();
    public List<Table> Tables {
      get { return _Tables; }
      set { _Tables = value; }
    }

    private List<string> _TableNames = new List<string>();
    public List<string> TableNames {
      get { return _TableNames; }
      set { _TableNames = value; }
    }

    private List<Procedure> _Procedures = new List<Procedure>();
    public List<Procedure> Procedures {
      get { return _Procedures; }
      set { _Procedures = value; }
    }

    private List<Function> _Functions = new List<Function>();
    public List<Function> Functions {
      get { return _Functions; }
      set { _Functions = value; }
    }

    private List<View> _Views = new List<View>();
    public List<View> Views {
      get { return _Views; }
      set { _Views = value; }
    }

    private List<Sequence> _Sequences = new List<Sequence>();
    public List<Sequence> Sequences {
      get { return _Sequences; }
      set { _Sequences = value; }
    }

    public Table FindTable(string ATableName, bool ACaseInsensitive) {
      Table TmpTable = null;
      foreach (Table Table in Tables) {
        if (ACaseInsensitive) {
          if (Table.TableName.ToUpper() == ATableName.ToUpper()) {
            TmpTable = Table;
            break;
          }
        } else {
          if (Table.TableName == ATableName) {
            TmpTable = Table;
            break;
          }
        }
      }
      return TmpTable;
    }        
    
    public Procedure FindProcedure(string ProcedureName){
      return 
        (from Procedure p in this.Procedures
         where p.Name.ToLower() == ProcedureName.ToLower()
         select p).FirstOrDefault<Procedure>();
    }
    
    public Function FindFunction(string FunctionName){
      return
        (from Function f in this.Functions
         where f.Name.ToLower() == FunctionName.ToLower()
         select f).FirstOrDefault<Function>();
    }
    
    public View FindView(string ViewName){
      return
        (from View v in this.Views
         where v.Name.ToLower() == ViewName.ToLower()
         select v).FirstOrDefault<View>();
    }

  }
}
