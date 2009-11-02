using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Model;

namespace DBInfo.Core.Statement {
  public class CreateIndex : BaseStatement {
    public Index Index{
      get; set;
    }
    
    public override void Apply(Database db){
      Table t = db.FindTable(this.Index.TableName, false);
      if (t == null)
        throw new DatabaseStatementApplyException("Couldn't find table " + this.Index.TableName);
      Index idx = t.FindIndex(Index.IndexName);
      if (idx != null)
        throw new DatabaseStatementApplyException("Index " + Index.IndexName + " already exists");
      t.Indexes.Add(Index);
    }
  }
}
