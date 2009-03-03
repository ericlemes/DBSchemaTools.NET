using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DBInfo.Core.Model {
  public class Database {
    private List<Table> _Tables = new List<Table>();
    public List<Table> Tables {
      get { return _Tables; }
      set { _Tables = value; }
    }

    private List<string> _TableNames = new List<string>();
    public List<string> TableNames {
      get { return _TableNames; }
      set { _TableNames = value; }
    }

    private List<DataSet> _TableData = new List<DataSet>();
    public List<DataSet> TableData {
      get { return _TableData; }
      set { _TableData = value; }
    }

    private List<Procedure> _Procedures = new List<Procedure>();
    public List<Procedure> Procedures {
      get { return _Procedures; }
      set { _Procedures = value; }
    }

    private List<Function> _Functions = new List<Function>();
    public List<Function> Functions {
      get { return _Functions; }
      set { _Functions = value; }
    }

    private List<View> _Views = new List<View>();
    public List<View> Views {
      get { return _Views; }
      set { _Views = value; }
    }

    private List<Sequence> _Sequences = new List<Sequence>();
    public List<Sequence> Sequences {
      get { return _Sequences; }
      set { _Sequences = value; }
    }

    public Table FindTable(string ATableName, bool ACaseInsensitive) {
      Table TmpTable = null;
      foreach (Table Table in Tables) {
        if (ACaseInsensitive) {
          if (Table.TableName.ToUpper() == ATableName.ToUpper()) {
            TmpTable = Table;
            break;
          }
        } else {
          if (Table.TableName == ATableName) {
            TmpTable = Table;
            break;
          }
        }
      }
      return TmpTable;
    }

  }
}
