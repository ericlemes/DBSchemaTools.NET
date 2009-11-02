using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Model;

namespace DBInfo.Core.Statement {
  public class CreateCheckConstraint : BaseStatement {
    public CheckConstraint CheckConstraint{
      get; set;
    }

    public override void Apply(Database db) {
      Table t = db.FindTable(this.CheckConstraint.TableName, false);
      if (t == null)
        throw new DatabaseStatementApplyException("Couldn't find table " + this.CheckConstraint.TableName);
      CheckConstraint cc = t.FindCheckConstraint(this.CheckConstraint.CheckConstraintName);
      if (cc != null)
        throw new DatabaseStatementApplyException("Check constraint " + this.CheckConstraint.CheckConstraintName + " already exists");
      t.CheckConstraints.Add(cc);
    }
  }
}
