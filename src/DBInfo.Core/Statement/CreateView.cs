using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Model;

namespace DBInfo.Core.Statement {
  public class CreateView : BaseStatement {
    public View View{
      get; set;
    }

    public override void Apply(Database db) {
      View v = db.FindView(this.View.Name);
      if (v != null)
        db.Views.Remove(v);
      db.Views.Add(v);
    }    
  }
}
