using System;
using System.Data;
using DBInfo.Core.Model;
using System.Collections.Generic;

namespace DBInfo.Core.Extractor {
  public interface IDBInfoExtractor {
    InputOutputType InputType{
      get;
      set;
    }
    string InputConnectionString{
      get;
      set;
    }
    string InputDir{
      get;
      set;
    }
    void Open();
    void Close();
    List<Table> GetTables();
    void GetTableColumns(Table table);
    DataSet getForeignKeys(string ATabela);
    DataSet getForeignKeyColumns(string AForeignKey);
    DataSet getPrimaryKey(string ATabela);
    DataSet getPrimaryKeyColumns(string ATabela, string APrimaryKeyName);
    DataSet getIndexes(string ATabela);
    DataSet getIndexColumns(string ATabela, string AIndice);
    DataSet getTableData(string ATabela);
    DataSet getProcedures();
    DataSet getProcedureText(string AProcedure);
    DataSet getFunctions();
    DataSet getFunctionText(string AFunction);
    DataSet getTriggers(string ATabela);
    DataSet getTriggerText(string ATrigger);
    DataSet getViews();
    DataSet getViewText(string AView);
    DataSet getCheckConstraints(string ATabela);
    DataSet getSequences();
    DataSet getTableTriggers(string ATabela);
  }
}
