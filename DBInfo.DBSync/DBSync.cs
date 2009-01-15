using System;
using System.Collections;
using DBInfo.Core.Model;
using DBInfo.Core.Extractor;

namespace DBInfo.DBSync {
  public class DBSync {

    protected ArrayList lstScriptNovaTabela = new ArrayList();
    protected ArrayList lstScriptExclusaoTabela = new ArrayList();
    protected ArrayList lstScriptNovaColuna = new ArrayList();
    protected ArrayList lstScriptAlteracaoColuna = new ArrayList();
    protected ArrayList lstScriptCriacaoPK = new ArrayList();
    protected ArrayList lstScriptCriacaoIndice = new ArrayList();
    protected ArrayList lstScriptCriacaoFK = new ArrayList();
    protected ArrayList lstScriptCriacaoCheckConstraint = new ArrayList();
    protected ArrayList lstScriptExclusaoColuna = new ArrayList();
    protected ArrayList lstScriptExclusaoIndice = new ArrayList();
    protected ArrayList lstScriptExclusaoCheckConstraint = new ArrayList();
    protected ArrayList lstScriptExclusaoPK = new ArrayList();
    protected ArrayList lstScriptExclusaoFK = new ArrayList();
    protected ArrayList lstScriptRelatorio = new ArrayList();

    public virtual void CompararDB(DBInfoExtractor BaseAtual, DBInfoExtractor NovaBase) {
    }

    public virtual void GerarScriptOrdenado(string caminho) {
    }

    protected Hashtable ScriptSegmentado = new Hashtable();

    protected void AdicionarScriptSegmentado(string chave, string conteudo) {
      if (ScriptSegmentado.ContainsKey(chave))
        ScriptSegmentado[chave] = ScriptSegmentado[chave].ToString() + conteudo;
      else
        ScriptSegmentado.Add(chave, conteudo);
    }


    public string GerarScriptSegmentado(string chave) {
      if (ScriptSegmentado.ContainsKey(chave))
        return ScriptSegmentado[chave].ToString();
      else
        return string.Empty;
    }


    public Hashtable RetornarTabelas() {
      return ScriptSegmentado;
    }


    protected void LimparScripts() {
      ArrayList[] arrarys = { lstScriptNovaTabela, lstScriptExclusaoTabela, lstScriptNovaColuna, 
									  lstScriptAlteracaoColuna, lstScriptExclusaoCheckConstraint,
									  lstScriptExclusaoFK, lstScriptExclusaoPK, lstScriptExclusaoIndice, 
									  lstScriptExclusaoColuna, lstScriptCriacaoPK, lstScriptCriacaoIndice,
									  lstScriptCriacaoFK, lstScriptCriacaoCheckConstraint, lstScriptRelatorio};

      foreach (ArrayList array in arrarys)
        array.Clear();

      ScriptSegmentado.Clear();
    }


    #region Tabelas

    protected Table PegarTabela(string NomeTabela, System.Collections.ArrayList tabelas) {
      foreach (object tb in tabelas) {
        if (((Table)tb).TableName == NomeTabela) {
          return (Table)tb;
        }
      }
      return null;
    }


    protected virtual void ScriptInclusaoTabela(Table tbNova) {
    }

    #region Especializados

    protected virtual string ScriptTabelaExcluida(Table Tabela) {
      return null;
    }

    #endregion

    #endregion

    #region Colunas

    protected Column PegarColuna(string NomeColuna, System.Collections.ArrayList colunas) {
      foreach (object col in colunas) {
        if (((Column)col).Name == NomeColuna) {
          return (Column)col;
        }
      }
      return null;
    }


    protected virtual void ScriptAlteracaoColumns(Table tbAtual, Table tbNova) {
    }

    #region Especializados

    protected virtual string ScriptAlteracaoColumns(Column colAtual, Column colNova, Table Tabela) {
      return null;
    }

    protected virtual string ScriptInclusaoColumns(Column colNova, Table Tabela) {
      return null;
    }

    protected virtual string ScriptColunaExcluida(Column Coluna, string NomeTabela) {
      return null;
    }

    #endregion

    #endregion

    #region PrimaryKeys

    protected virtual void ScriptAlteracaoPrimaryKeys(Table tbAtual, Table tbNova) {
    }

    #endregion

    #region Indexes

    protected Index PegarIndice(string NomeIndice, System.Collections.ArrayList Indexes) {
      foreach (object ix in Indexes) {
        if (((Index)ix).IndexName == NomeIndice) {
          return (Index)ix;
        }
      }
      return null;
    }


    protected virtual void ScriptAlteracaoIndices(Table tbAtual, Table tbNova) {
    }

    #endregion

    #region ForeignKeys

    protected ForeignKey PegarForeignKey(string NomeForeignKey, System.Collections.ArrayList ForeignKeys) {
      foreach (object fk in ForeignKeys) {
        if (((ForeignKey)fk).ForeignKeyName == NomeForeignKey) {
          return (ForeignKey)fk;
        }
      }
      return null;
    }


    protected ForeignKeyColumn PegarColunasFK(ForeignKeyColumn fkAtual, System.Collections.ArrayList ColunasFK) {
      foreach (object col in ColunasFK) {
        ForeignKeyColumn fk = (ForeignKeyColumn)col;

        if ((fkAtual.RefTable.TableName == fk.RefTable.TableName) &&
          (fkAtual.Column.Name == fk.Column.Name) &&
          (fkAtual.RefColumn.Name == fk.RefColumn.Name))
          return (ForeignKeyColumn)col;
      }
      return null;
    }


    protected virtual void ScriptAlteracaoForeignKeys(Table tbAtual, Table tbNova) {
    }

    #endregion

    #region Constraints

    protected CheckConstraint PegarCheckConstraint(string Nome, System.Collections.ArrayList CheckConstraints) {
      foreach (object constraint in CheckConstraints) {
        if (((CheckConstraint)constraint).Nome == Nome) {
          return (CheckConstraint)constraint;
        }
      }
      return null;
    }


    protected virtual void ScriptAlteracaoCheckConstraint(Table tbAtual, Table tbNova) {
    }

    #endregion



  }
}
