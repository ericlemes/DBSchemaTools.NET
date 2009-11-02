using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Model;

namespace DBInfo.Core.Statement {
  public class CreatePrimaryKey : BaseStatement {
    public Table Table{
      get; set;
    }
    
    public override void Apply(Database db){
      Table t = db.FindTable(this.Table.TableName, true);
      if (t == null)
        throw new DatabaseStatementApplyException("Couldn't find table " + this.Table.TableName);
      
      t.PrimaryKeyName = this.Table.PrimaryKeyName;
      t.PrimaryKeyColumns = this.Table.PrimaryKeyColumns;      
    }
  }
}
