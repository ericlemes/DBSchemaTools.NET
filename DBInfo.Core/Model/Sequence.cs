using System;

namespace DBInfo.Core.Model {
  public class Sequence {
    private string _SequenceName;
    public string SequenceName{
      get { return _SequenceName;}
      set {_SequenceName = value;}
    }
    
    private int _Initial;
    public int Initial{
      get { return _Initial;}
      set { _Initial = value;}
    }
    
    private int _MinValue;
    public int MinValue{
      get { return _MinValue;}
      set { _MinValue = value;}
    }
    
    private int? _MaxValue;
    public int? MaxValue{
      get { return _MaxValue;}
      set {_MaxValue = value;}
    }
    
    private int _Increment;
    public int Increment{
      get { return _Increment;}
      set { _Increment = value;}
    }
        
    private bool _CycleOnLimit;
    public bool CycleOnLimit{
      get { return _CycleOnLimit;}
      set {_CycleOnLimit = value;}
    }

  }
}
