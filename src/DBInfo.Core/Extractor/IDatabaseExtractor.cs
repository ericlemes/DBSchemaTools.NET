using System;
using System.Data;
using DBInfo.Core.Model;
using System.Collections.Generic;

namespace DBInfo.Core.Extractor {
  public interface IDatabaseExtractor {
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
    void GetTableColumns(Database db, Table table);
    void GetForeignKeys(Database db, Table table);
    void GetForeignKeyColumns(Database database, Table table, ForeignKey fk);
    void GetPrimaryKey(Database db, Table table);
    void GetPrimaryKeyColumns(Database db, Table table);
    void GetIndexes(Database db, Table table);
    void GetIndexColumns(Database db, Table table, Index index);
    DataTable GetTableData(Database db, string tableName);
    void GetProcedures(Database db);
    void GetProcedureText(Database db, Procedure p);
    void GetFunctions(Database db);
    void GetFunctionText(Database db, Function f);
    void GetTriggers(Database db, Table table);
    void GetTriggerText(Database db, Table t, Trigger tr);
    void GetViews(Database db);
    void GetViewText(Database db, View v);
    void GetCheckConstraints(Database db, Table t);
    void GetSequences(Database db);    
  }
}
