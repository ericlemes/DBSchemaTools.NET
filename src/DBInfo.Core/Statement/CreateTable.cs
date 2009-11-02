using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Model;

namespace DBInfo.Core.Statement {
  public class CreateTable : BaseStatement {
    public Table Table{
      get; set;
    }
    
    public override void Apply(Database db){
      Table t = db.FindTable(this.Table.TableName, true);
      if (t != null)
        throw new DatabaseStatementApplyException("Table " + this.Table.TableName + " already exists.");
        
      db.Tables.Add(this.Table);
    }
  }
}
