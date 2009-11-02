using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Model;

namespace DBInfo.Core.Statement {
  public abstract class BaseStatement {
    public string Script{
      get; set;
    }
    
    public abstract void Apply(Database db);
  }
  
  public class DatabaseStatementApplyException : Exception{
    public DatabaseStatementApplyException(string Message) : base(Message){
    
    }
  }
}
