using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Model;

namespace DBInfo.Core.Statement {
  public class CreateForeignKey : BaseStatement {
    public ForeignKey ForeignKey{
      get; set;
    }

    public override void Apply(Database db) {
      Table t = db.FindTable(this.ForeignKey.TableName, false);
      if (t == null)
        throw new DatabaseStatementApplyException("Couldn't find table " + this.ForeignKey.TableName);
      ForeignKey fk = t.FindForeignKey(this.ForeignKey.ForeignKeyName);
      if (fk != null)
        throw new DatabaseStatementApplyException("Foreign key " + this.ForeignKey.ForeignKeyName + " already exists");
      t.ForeignKeys.Add(fk);
    }
  }
}
