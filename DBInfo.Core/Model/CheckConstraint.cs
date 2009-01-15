using System;

namespace DBInfo.Core.Model {
  public class CheckConstraint {
    public string Nome;
    public string Expressao;

    public CheckConstraint() {
      Nome = "";
      Expressao = "";
    }
  }
}
