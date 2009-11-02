using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Model;

namespace DBInfo.Core.Statement {
  public class CreateTrigger : BaseStatement {
    public Trigger Trigger{
      get; set;
    }

    public override void Apply(Database db) {
      Table t = db.FindTable(this.Trigger.TableName, false);
      if (t == null)
        throw new DatabaseStatementApplyException("Couldn't find table " + this.Trigger.TableName);
      Trigger trg = t.FindTrigger(this.Trigger.TriggerName);
      if (trg != null)
        t.Triggers.Remove(trg);
      t.Triggers.Add(this.Trigger);      
    }
  }
}
