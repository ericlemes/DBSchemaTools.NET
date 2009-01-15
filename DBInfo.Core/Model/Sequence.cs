using System;

namespace DBInfo.Core.Model {
  public class Sequence {
    public string SequenceName;
    public int Initial;
    public int MinValue;
    public int MaxValue; //< 0 Caso nao possua valor maior
    public int Increment;
    public bool CycleOnLimit; //Progress

  }
}
