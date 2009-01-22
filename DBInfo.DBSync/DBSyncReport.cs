using System;
using System.Collections;
using System.IO;
using DBInfo.DBSync;
using DBInfo.Core;
using DBInfo.Core.Extractor;
using DBInfo.Core.Model;
using System.Collections.Generic;

namespace DBInfo.DBSync {
  public class DBSyncReport : DBSync {
    public override void CompararDB(DBInfoExtractor BaseAtual, DBInfoExtractor NovaBase) {
      LimparScripts();

      foreach (object tb in BaseAtual.Database.Tables) {
        Table tbdestino = PegarTabela(((Table)tb).TableName, NovaBase.Database.Tables);

        if (tbdestino != null) {
          //TmpRelatorio += ScriptAlteracaoIdentity( (DBTable) tb, tbdestino);
          ScriptAlteracaoColumns((Table)tb, tbdestino);
          ScriptAlteracaoPrimaryKeys((Table)tb, tbdestino);
          ScriptAlteracaoForeignKeys((Table)tb, tbdestino);
          ScriptAlteracaoIndices((Table)tb, tbdestino);
          ScriptAlteracaoCheckConstraint((Table)tb, tbdestino);
        } else {
          lstScriptRelatorio.Add(ScriptTabelaExcluida((Table)tb));
          AdicionarScriptSegmentado(((Table)tb).TableName, "Exclusão");
        }

      }

      foreach (object tb in NovaBase.Database.Tables) {
        string TmpRelatorio = string.Empty;
        Table tbdorigem = PegarTabela(((Table)tb).TableName, BaseAtual.Database.Tables);

        if (tbdorigem == null) {
          ScriptInclusaoTabela(((Table)tb));
          AdicionarScriptSegmentado(((Table)tb).TableName, "Inclusão");
        }

      }

    }


    #region Tabelas

    protected override void ScriptInclusaoTabela(Table tbNova) {
      string relatorioAlteracao = Separador() + Environment.NewLine;
      relatorioAlteracao += "Inclusão da tabela " + tbNova.TableName + Environment.NewLine;

      foreach (object col in tbNova.Columns)
        relatorioAlteracao += ScriptInclusaoColumns((Column)col, tbNova);

      if (tbNova.PrimaryKeyName != string.Empty)
        relatorioAlteracao += ScriptInclusaoPrimaryKey(tbNova);

      foreach (object fk in tbNova.ForeignKeys)
        relatorioAlteracao += ScriptInclusaoForeignKey((ForeignKey)fk, tbNova.TableName);

      foreach (object chkc in tbNova.CheckConstraints)
        relatorioAlteracao += ScriptInclusaoCheckConstraint((CheckConstraint)chkc, tbNova.TableName);

      relatorioAlteracao += Separador() + Environment.NewLine;

      lstScriptRelatorio.Add(relatorioAlteracao);
    }


    protected override void ScriptAlteracaoColumns(Table tbAtual, Table tbNova) {
      foreach (object col in tbAtual.Columns) {
        Column coldestino = PegarColuna(((Column)col).Name, tbNova.Columns);

        if (coldestino != null) {
          string relatorio = ScriptAlteracaoColumns((Column)col, coldestino, tbNova);
          lstScriptRelatorio.Add(relatorio);
          AdicionarScriptSegmentado(tbAtual.TableName, relatorio);
        } else {
          string relatorio = ScriptColunaExcluida((Column)col, tbAtual.TableName);
          lstScriptRelatorio.Add(relatorio);
          AdicionarScriptSegmentado(tbAtual.TableName, relatorio);
        }
      }

      foreach (object col in tbNova.Columns) {
        Column colorigem = PegarColuna(((Column)col).Name, tbAtual.Columns);

        if (colorigem == null) {
          string relatorio = ScriptInclusaoColumns((Column)col, tbAtual);
          lstScriptRelatorio.Add(relatorio);
          AdicionarScriptSegmentado(tbAtual.TableName, relatorio);
        }
      }
    }


    #region Especializados

    private void ScriptAlteracaoIdentity(Table tbAtual, Table tbNova) {
      string relatorioAlteracao = string.Empty;

      if ((tbAtual.HasIdentity != tbNova.HasIdentity) ||
        (tbAtual.IdentityIncrement != tbNova.IdentityIncrement) ||
        (tbAtual.IdentitySeed != tbNova.IdentitySeed)) {
        relatorioAlteracao += "HasIdentity: " + tbNova.HasIdentity.ToString() + Environment.NewLine;
        relatorioAlteracao += "IdentityIncrement: " + tbNova.IdentityIncrement.ToString() + Environment.NewLine;
        relatorioAlteracao += "IdentitySeed: " + tbNova.IdentitySeed.ToString() + Environment.NewLine;
      }

      lstScriptRelatorio.Add(relatorioAlteracao);
      AdicionarScriptSegmentado(tbAtual.TableName, relatorioAlteracao);
    }


    protected override string ScriptTabelaExcluida(Table Tabela) {
      string relatorioAlteracao = "Excluir Tabela - " + Tabela.TableName + Environment.NewLine;
      return relatorioAlteracao;
    }


    #endregion

    #endregion

    #region Colunas

    #region Especializados

    protected override string ScriptAlteracaoColumns(Column colAtual, Column colNova, Table Tabela) {
      string relatorioAlteracao = string.Empty;

      if (colAtual.Type != colNova.Type)
        relatorioAlteracao += "   Type: " + colNova.Type.ToString() + Environment.NewLine;

      if (colAtual.Size != colNova.Size)
        relatorioAlteracao += "   Size: " + colNova.Size.ToString() + Environment.NewLine;

      if (colAtual.Precision != colNova.Precision)
        relatorioAlteracao += "   Precision: " + colNova.Precision.ToString() + Environment.NewLine;

      if (colAtual.Scale != colNova.Scale)
        relatorioAlteracao += "   Scale: " + colNova.Scale.ToString() + Environment.NewLine;

      if (colAtual.IsPK != colNova.IsPK)
        relatorioAlteracao += "   IsPK: " + colNova.IsPK.ToString() + Environment.NewLine;

      if (colAtual.IsNull != colNova.IsNull)
        relatorioAlteracao += "   IsNull: " + colNova.IsNull.ToString() + Environment.NewLine;

      if (colAtual.IdentityColumn != colNova.IdentityColumn)
        relatorioAlteracao += "   IdentityColumn: " + colNova.IdentityColumn.ToString() + Environment.NewLine;

      if (colAtual.DefaultValue != colNova.DefaultValue)
        relatorioAlteracao += "   DefaultValue: " + colNova.DefaultValue.ToString() + Environment.NewLine;

      if (relatorioAlteracao.Length > 0)
        relatorioAlteracao = "Alteração Coluna: " + colNova.Name + Environment.NewLine + relatorioAlteracao;

      return relatorioAlteracao;
    }


    protected override string ScriptInclusaoColumns(Column colNova, Table Tabela) {
      string relatorioAlteracao = "Inclusão Coluna: " + colNova.Name + Environment.NewLine; ;
      relatorioAlteracao += "   Type: " + colNova.Type.ToString() + Environment.NewLine;
      relatorioAlteracao += "   Size: " + colNova.Size.ToString() + Environment.NewLine;
      relatorioAlteracao += "   Precision: " + colNova.Precision.ToString() + Environment.NewLine;
      relatorioAlteracao += "   Scale: " + colNova.Scale.ToString() + Environment.NewLine;
      relatorioAlteracao += "   IsPK: " + colNova.IsPK.ToString() + Environment.NewLine;
      relatorioAlteracao += "   IsNull: " + colNova.IsNull.ToString() + Environment.NewLine;
      relatorioAlteracao += "   IdentityColumn: " + colNova.IdentityColumn.ToString() + Environment.NewLine;
      relatorioAlteracao += "   DefaultValue: " + colNova.DefaultValue.ToString() + Environment.NewLine;
      return relatorioAlteracao;
    }


    protected override string ScriptColunaExcluida(Column Coluna, string NomeTabela) {
      return "Excluir Coluna - " + Coluna.Name + Environment.NewLine;
    }


    #endregion

    #endregion

    #region PrimaryKeys

    protected override void ScriptAlteracaoPrimaryKeys(Table tbAtual, Table tbNova) {
      if (tbAtual.PrimaryKeyName == tbNova.PrimaryKeyName) {
        string relatorio = ScriptAlteracaoColumnsPrimaryKey(tbAtual.PrimaryKeyColumns, tbNova.PrimaryKeyColumns, tbAtual.TableName);
        lstScriptRelatorio.Add(relatorio);
        AdicionarScriptSegmentado(tbAtual.TableName, relatorio);
      } else {
        if (tbAtual.PrimaryKeyName != string.Empty) {
          string relatorio = ScriptPrimaryKeyExcluida(tbAtual.PrimaryKeyName, tbAtual.TableName);
          lstScriptRelatorio.Add(relatorio);
          AdicionarScriptSegmentado(tbAtual.TableName, relatorio);
        }

        if (tbNova.PrimaryKeyName != string.Empty) {
          string relatorio = ScriptInclusaoPrimaryKey(tbNova);
          lstScriptRelatorio.Add(relatorio);
          AdicionarScriptSegmentado(tbNova.TableName, relatorio);
        }

      }
    }


    #region Especializados

    private string ScriptAlteracaoColumnsPrimaryKey(List<Column> pkAtual, List<Column> pkNova, string NomeTabela) {
      string relatorioAlteracao = string.Empty;

      foreach (object col in pkAtual) {
        Column pk = PegarColuna(((Column)col).Name, pkNova);
        if (pk == null)
          relatorioAlteracao += ScriptColunaPrimaryKeyRemovida((Column)col, NomeTabela);
      }

      foreach (object col in pkNova) {
        Column pk = PegarColuna(((Column)col).Name, pkAtual);

        if (pk == null)
          relatorioAlteracao += ScriptColunaPrimaryKeyAdicionada((Column)col, NomeTabela);
      }

      return relatorioAlteracao;
    }


    private string ScriptInclusaoPrimaryKey(Table tb) {
      string relatorioAlteracao = "Inclusão PrimaryKey - " + tb.PrimaryKeyName + Environment.NewLine;

      foreach (object col in tb.PrimaryKeyColumns)
        relatorioAlteracao += ScriptColunaPrimaryKeyAdicionada((Column)col, tb.TableName);

      return relatorioAlteracao;
    }


    private string ScriptColunaPrimaryKeyAdicionada(Column pk, string NomeTabela) {
      string relatorioAlteracao = "Adicionar Coluna PrimaryKey" + Environment.NewLine;
      relatorioAlteracao += "  Tabela: " + NomeTabela + "Coluna: " + pk.Name + Environment.NewLine;
      return relatorioAlteracao;
    }


    private string ScriptColunaPrimaryKeyRemovida(Column pk, string NomeTabela) {
      string relatorioAlteracao = "Excluir Coluna PrimaryKey" + Environment.NewLine;
      relatorioAlteracao += "  Tabela: " + NomeTabela + "Coluna: " + pk.Name + Environment.NewLine;
      return relatorioAlteracao;
    }


    private string ScriptPrimaryKeyExcluida(string PrimaryKeyName, string TableName) {
      return "Excluir PrimaryKey - " + PrimaryKeyName + Environment.NewLine + Environment.NewLine;
    }


    #endregion

    #endregion

    #region Indexes

    protected override void ScriptAlteracaoIndices(Table tbAtual, Table tbNova) {
      string relatorioAlteracao = string.Empty;

      foreach (object ix in tbAtual.Indexes) {
        Index ixdestino = PegarIndice(((Index)ix).IndexName, tbNova.Indexes);
        if (ixdestino != null) {
          string relatorio = ScriptAlteracaoIndices((Index)ix, ixdestino, tbAtual.TableName);
          lstScriptRelatorio.Add(relatorio);
          AdicionarScriptSegmentado(tbAtual.TableName, relatorio);
        } else {
          string relatorio = ScriptIndiceExcluido((Index)ix, tbAtual.TableName);
          lstScriptRelatorio.Add(relatorio);
          AdicionarScriptSegmentado(tbAtual.TableName, relatorio);
        }
      }


      foreach (object ix in tbNova.Indexes) {
        Index ixorigem = PegarIndice(((Index)ix).IndexName, tbAtual.Indexes);

        if (ixorigem == null) {
          string relatorio = ScriptInclusaoIndice((Index)ix, tbAtual.TableName);
          lstScriptRelatorio.Add(relatorio);
          AdicionarScriptSegmentado(tbAtual.TableName, relatorio);
        }
      }

    }


    #region Especializados

    private string ScriptAlteracaoColumnsIndice(List<IndexColumn> ixAtual, List<IndexColumn> ixNova, string NomeTabela) {
      string relatorioAlteracao = string.Empty;

      foreach (object col in ixAtual) {
        Column ix = PegarColuna(((Column)col).Name, ixNova).Column;
        if (ix == null)
          relatorioAlteracao += ScriptColunaIndiceRemovida((Column)col, NomeTabela);
      }

      foreach (object col in ixNova) {
        Column ix = PegarColuna(((Column)col).Name, ixAtual).Column;

        if (ix == null)
          relatorioAlteracao += ScriptColunaIndiceAdicionada((Column)col, NomeTabela);
      }

      return relatorioAlteracao;
    }


    private string ScriptAlteracaoIndices(Index ixAtual, Index ixNova, string NomeTabela) {
      string relatorioAlteracao = string.Empty;

      if (ixAtual.Unique != ixNova.Unique)
        relatorioAlteracao += "   Unique: " + ixNova.Unique.ToString() + Environment.NewLine;


      relatorioAlteracao += ScriptAlteracaoColumnsIndice(ixAtual.Columns, ixNova.Columns, NomeTabela);

      if (relatorioAlteracao.Length > 0)
        relatorioAlteracao = "Alteração Índice: " + ixAtual.IndexName + Environment.NewLine + relatorioAlteracao;

      return relatorioAlteracao;
    }


    private string ScriptColunaIndiceRemovida(Column cix, string NomeTabelaOrigem) {
      string relatorioAlteracao = "Remover Coluna Índice" + Environment.NewLine;
      relatorioAlteracao += "Coluna: " + cix.Name + Environment.NewLine;
      return relatorioAlteracao;
    }


    private string ScriptColunaIndiceAdicionada(Column cix, string NomeTabela) {
      string relatorioAlteracao = "Adicionar Coluna Índice" + Environment.NewLine;
      relatorioAlteracao += "Coluna: " + cix.Name + Environment.NewLine;
      return relatorioAlteracao;
    }


    private string ScriptInclusaoIndice(Index indice, string NomeTabelaOrigem) {
      string relatorioAlteracao = "Inclusão Índice - " + indice.IndexName + Environment.NewLine;
      relatorioAlteracao += "   Unique: " + indice.Unique.ToString() + Environment.NewLine;

      foreach (object col in indice.Columns)
        relatorioAlteracao += ScriptColunaIndiceAdicionada((Column)col, NomeTabelaOrigem);


      return relatorioAlteracao;
    }


    private string ScriptIndiceExcluido(Index indice, string TableName) {
      return "Excluir Indice - " + indice.IndexName + Environment.NewLine;
    }


    #endregion

    #endregion

    #region ForeignKeys

    protected override void ScriptAlteracaoForeignKeys(Table tbAtual, Table tbNova) {
      foreach (object fk in tbAtual.ForeignKeys) {
        ForeignKey fkdestino = PegarForeignKey(((ForeignKey)fk).ForeignKeyName, tbNova.ForeignKeys);

        if (fkdestino != null) {
          string relatorio = ScriptAlteracaoForeignKeys((ForeignKey)fk, fkdestino, tbAtual.TableName);
          lstScriptRelatorio.Add(relatorio);
          AdicionarScriptSegmentado(tbAtual.TableName, relatorio);
        } else {
          string relatorio = ScriptForeignKeyExcluida((ForeignKey)fk, tbAtual.TableName);
          lstScriptRelatorio.Add(relatorio);
          AdicionarScriptSegmentado(tbAtual.TableName, relatorio);
        }

      }

      foreach (object fk in tbNova.ForeignKeys) {
        ForeignKey fkorigem = PegarForeignKey(((ForeignKey)fk).ForeignKeyName, tbAtual.ForeignKeys);

        if (fkorigem == null) {
          string relatorio = ScriptInclusaoForeignKey((ForeignKey)fk, tbAtual.TableName);
          lstScriptRelatorio.Add(relatorio);
          AdicionarScriptSegmentado(tbAtual.TableName, relatorio);
        }
      }
    }


    private string ScriptAlteracaoColumnsForeignKey(ForeignKey fkAtual, ForeignKey fkNova, string NomeTabela) {
      string relatorioAlteracao = string.Empty;

      foreach (object col in fkAtual.Columns) {
        ForeignKeyColumn fk = PegarColunasFK((ForeignKeyColumn)col, fkNova.Columns);
        if (fk == null)
          relatorioAlteracao += ScriptColunaForeignKeyRemovida((ForeignKeyColumn)col, NomeTabela);
      }


      foreach (object col in fkNova.Columns) {
        ForeignKeyColumn fk = PegarColunasFK((ForeignKeyColumn)col, fkAtual.Columns);

        if (fk == null)
          relatorioAlteracao += ScriptColunaForeignKeyAdicionada((ForeignKeyColumn)col, NomeTabela);
      }

      return relatorioAlteracao;
    }


    #region Especializados

    private string ScriptAlteracaoForeignKeys(ForeignKey fkAtual, ForeignKey fkNova, string NomeTabela) {
      string relatorioAlteracao = string.Empty;

      if (fkAtual.RefTableName != fkNova.RefTableName)
        relatorioAlteracao += "   RefTableName: " + fkNova.RefTableName.ToString() + Environment.NewLine;

      if (fkAtual.DeleteCascade != fkNova.DeleteCascade)
        relatorioAlteracao += "   DeleteCascade: " + fkNova.DeleteCascade.ToString() + Environment.NewLine;

      if (fkAtual.UpdateCascade != fkNova.UpdateCascade)
        relatorioAlteracao += "   UpdateCascade: " + fkNova.UpdateCascade.ToString() + Environment.NewLine;

      relatorioAlteracao += ScriptAlteracaoColumnsForeignKey(fkAtual, fkNova, NomeTabela);

      if (relatorioAlteracao.Length > 0)
        relatorioAlteracao = "Alteração ForeignKey: " + fkAtual.ForeignKeyName + Environment.NewLine + relatorioAlteracao;

      return relatorioAlteracao;
    }


    private string ScriptColunaForeignKeyRemovida(ForeignKeyColumn fk, string NomeTabelaOrigem) {
      string relatorioAlteracao = "Remover Coluna ForeignKey" + Environment.NewLine;
      relatorioAlteracao += "  Tabela Origem: " + NomeTabelaOrigem + " Coluna Origem: " + fk.Column.Name + Environment.NewLine;
      relatorioAlteracao += "  Tabela Referência: " + fk.RefTable.TableName + " Coluna Referência: " + fk.RefColumn.Name + Environment.NewLine;
      return relatorioAlteracao;
    }


    private string ScriptColunaForeignKeyAdicionada(ForeignKeyColumn fk, string NomeTabelaOrigem) {
      string relatorioAlteracao = "Adicionar Coluna ForeignKey" + Environment.NewLine;
      relatorioAlteracao += "  Tabela Origem: " + NomeTabelaOrigem + " Coluna Origem: " + fk.Column.Name + Environment.NewLine;
      relatorioAlteracao += "  Tabela Referência: " + fk.RefTable.TableName + " Coluna Referência: " + fk.RefColumn.Name + Environment.NewLine;
      return relatorioAlteracao;
    }


    private string ScriptInclusaoForeignKey(ForeignKey ForeignKey, string NomeTabelaOrigem) {
      string relatorioAlteracao = "Inclusão ForeignKey - " + ForeignKey.ForeignKeyName + Environment.NewLine;
      relatorioAlteracao += "   RefTableName: " + ForeignKey.RefTableName.ToString() + Environment.NewLine;
      relatorioAlteracao += "   DeleteCascade: " + ForeignKey.DeleteCascade.ToString() + Environment.NewLine;
      relatorioAlteracao += "   UpdateCascade: " + ForeignKey.UpdateCascade.ToString() + Environment.NewLine;

      foreach (object col in ForeignKey.Columns)
        relatorioAlteracao += ScriptColunaForeignKeyAdicionada((ForeignKeyColumn)col, NomeTabelaOrigem);


      return relatorioAlteracao;
    }


    private string ScriptForeignKeyExcluida(ForeignKey ForeignKey, string TableName) {
      return "Excluir ForeignKey - " + ForeignKey.ForeignKeyName + Environment.NewLine;
    }


    #endregion

    #endregion

    #region Constraints

    protected override void ScriptAlteracaoCheckConstraint(Table tbAtual, Table tbNova) {
      foreach (object chkc in tbAtual.CheckConstraints) {
        CheckConstraint ccdestino = PegarCheckConstraint(((CheckConstraint)chkc).Name, tbNova.CheckConstraints);

        if (ccdestino != null) {
          string relatorio = ScriptAlteracaoCheckConstraint((CheckConstraint)chkc, ccdestino, tbAtual.TableName);
          lstScriptRelatorio.Add(relatorio);
          AdicionarScriptSegmentado(tbAtual.TableName, relatorio);
        } else {
          string relatorio = ScriptCheckConstraintExcluida((CheckConstraint)chkc, tbAtual.TableName);
          lstScriptRelatorio.Add(relatorio);
          AdicionarScriptSegmentado(tbAtual.TableName, relatorio);
        }
      }


      foreach (object chkc in tbNova.CheckConstraints) {
        CheckConstraint ccorigem = PegarCheckConstraint(((CheckConstraint)chkc).Name, tbAtual.CheckConstraints);

        if (ccorigem == null) {
          string relatorio = ScriptInclusaoCheckConstraint((CheckConstraint)chkc, tbAtual.TableName);
          lstScriptRelatorio.Add(relatorio);
          AdicionarScriptSegmentado(tbAtual.TableName, relatorio);
        }
      }
    }


    #region Especializados

    private string ScriptAlteracaoCheckConstraint(CheckConstraint ccAtual, CheckConstraint ccNova, string NomeTabela) {
      string relatorioAlteracao = string.Empty;

      if (ccAtual.Expression != ccNova.Expression)
        relatorioAlteracao += "   Expressão: " + ccNova.Expression + Environment.NewLine;

      if (relatorioAlteracao.Length > 0)
        relatorioAlteracao = "Alteração CheckConstraint: " + ccAtual.Name + Environment.NewLine + relatorioAlteracao;

      return relatorioAlteracao;
    }


    private string ScriptInclusaoCheckConstraint(CheckConstraint cc, string NomeTabelaOrigem) {
      string relatorioAlteracao = "Inclusão CheckConstraint - " + cc.Name + Environment.NewLine;
      relatorioAlteracao += "   Expressão: " + cc.Expression + Environment.NewLine;
      return relatorioAlteracao;
    }


    private string ScriptCheckConstraintExcluida(CheckConstraint cc, string TableName) {
      return "Excluir CheckConstraint - " + cc.Name + Environment.NewLine;
    }


    #endregion

    #endregion

    private string Separador() {
      return "------------------------------------------------------------------";
    }


    private string IncluirTabela(string tablename, string script) {
      string relatorio = Separador() + Environment.NewLine;
      relatorio += "Tabela: " + tablename + Environment.NewLine;
      relatorio += script + Environment.NewLine;
      relatorio += Separador() + Environment.NewLine;
      return relatorio;
    }


    public override void GerarScriptOrdenado(string caminho) {
      string ScriptsOrdenados = string.Empty;

      ArrayList[] arrarys = { lstScriptRelatorio };

      foreach (ArrayList array in arrarys) {
        foreach (object script in array)
          ScriptsOrdenados += (string)script;
      }

      StreamWriter sw = new StreamWriter(caminho + "\\Relatorio.txt");
      sw.Write(ScriptsOrdenados);
      sw.Close();

    }


  }
}
