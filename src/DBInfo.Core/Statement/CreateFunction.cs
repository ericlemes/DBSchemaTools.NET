using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Model;

namespace DBInfo.Core.Statement {
  public class CreateFunction : BaseStatement {
    public Function Function{
      get; set;
    }

    public override void Apply(Database db) {
      Function f = db.FindFunction(this.Function.Name);
      if (f != null)
        db.Functions.Remove(f);
      db.Functions.Add(f);
    }
  }
}
