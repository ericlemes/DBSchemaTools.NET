using System;

namespace DBInfo.Core.Model {
  // Eric Lemes: Toda a classe � espec�fica para Progress. N�o se aplica a nenhum dos demais bancos.
  // Usado para gerar o trecho "TABLE-TRIGGER" no .df
  public class TableTrigger {
    public string Event;
    public bool Override;
    public string Procedure;
    public string CRC;


  }
}
