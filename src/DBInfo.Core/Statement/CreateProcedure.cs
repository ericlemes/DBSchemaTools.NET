using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Model;

namespace DBInfo.Core.Statement {
  public class CreateProcedure : BaseStatement {
    public Procedure Procedure{
      get; set;
    }
    
    public override void Apply(Database db){
      Procedure p = db.FindProcedure(this.Procedure.Name);
      if (p != null)
        db.Procedures.Remove(p);
      db.Procedures.Add(this.Procedure);
    }
  }
}
