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
    void GetForeignKeys(Table table);
    void GetForeignKeyColumns(Database database, Table table, ForeignKey fk);
    void GetPrimaryKey(Table table);
    void GetPrimaryKeyColumns(Table table);
    void GetIndexes(Table table);
    void GetIndexColumns(Table table, Index index);
    DataSet GetTableData(string ATabela);
    void GetProcedures(Database db);
    void GetProcedureText(Database db, Procedure p);
    void GetFunctions(Database db);
    void GetFunctionText(Database db, Function f);
    void GetTriggers(Database db, Table table);
    void GetTriggerText(Database db, Table t, Trigger tr);
    DataSet getViews();
    DataSet getViewText(string AView);
    DataSet getCheckConstraints(string ATabela);
    DataSet getSequences();    
  }
}
