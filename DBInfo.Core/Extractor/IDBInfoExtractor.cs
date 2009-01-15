using System;
using System.Data;

namespace DBInfo.Core.Extractor {
  public interface IDBInfoExtractor {
    void Abrir();
    void Fechar();
    DataSet PegarDatasetTabelas();
    DataSet PegarDatasetColunas(string ATabela);
    DataSet PegarDatasetForeignKeys(string ATabela);
    DataSet PegarDatasetForeignKeyColumns(string AForeignKey);
    DataSet PegarDatasetPrimaryKey(string ATabela);
    DataSet PegarDatasetPrimaryKeyColumns(string ATabela, string APrimaryKeyName);
    DataSet PegarDatasetIndices(string ATabela);
    DataSet PegarDatasetColunasIndice(string ATabela, string AIndice);
    DataSet PegarDatasetDadosIniciais(string ATabela);
    DataSet PegarDatasetProcedures();
    DataSet PegarDatasetProcedureText(string AProcedure);
    DataSet PegarDatasetFunctions();
    DataSet PegarDatasetFunctionText(string AFunction);
    DataSet PegarDatasetTriggers(string ATabela);
    DataSet PegarDatasetTriggerText(string ATrigger);
    DataSet PegarDatasetViews();
    DataSet PegarDatasetViewText(string AView);
    DataSet PegarDatasetCheckConstraints(string ATabela);
    DataSet PegarDatasetSequences();
    DataSet PegarDatasetTableTriggers(string ATabela);
  }
}
