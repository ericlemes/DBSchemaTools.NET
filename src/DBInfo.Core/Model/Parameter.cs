using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBInfo.Core.Model {
  public enum ParamDirection{
    Input,
    Output,
    InputOutput,
    ReturnValue
  }

  public class Parameter {
    private string _Name;
    public string Name{
      get { return _Name;}
      set { _Name = value;}
    }
    
    private DBColumnType _Type;
    public DBColumnType Type{
      get { return _Type;}
      set { _Type = value;}
    }

    private int _Size;
    public int Size {
      get { return _Size; }
      set { _Size = value; }
    }

    private int _Precision;
    public int Precision {
      get { return _Precision; }
      set { _Precision = value; }
    }

    private int _Scale;
    public int Scale {
      get { return _Scale; }
      set { _Scale = value; }
    }
    
    private ParamDirection _Direction;
    public ParamDirection Direction {
      get { return _Direction;}
      set { _Direction = value;}
    }
    
  }
}
