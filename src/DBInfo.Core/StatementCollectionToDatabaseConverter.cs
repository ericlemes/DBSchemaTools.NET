using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Model;
using DBInfo.Core.Statement;

namespace DBInfo.Core {
  public class StatementCollectionToDatabaseConverter {
    public Database Convert(List<BaseStatement> statementCollection){
      Database db = new Database();
      foreach(BaseStatement s in statementCollection){
        s.Apply(db);
      }
      return db;
    }
  }
}
