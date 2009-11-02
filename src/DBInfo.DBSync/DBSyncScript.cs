using System;
using System.Collections;
using System.IO;
using System.Data;
using DBInfo.DBSync;
using DBInfo.Core;
using DBInfo.Core.Model;
using DBInfo.Core.Extractor;
using System.Collections.Generic;
using DBInfo.SQLServer;

namespace DBInfo.DBSync {
  public class DBSyncScript : DBSync {

    ArrayList lstScriptExclusaoIndice_tmp = new ArrayList(), lstScriptExclusaoFK_tmp = new ArrayList(), lstScriptExclusaoPK_tmp = new ArrayList(),
      lstScriptCriacaoPK_tmp = new ArrayList(), lstScriptCriacaoIndice_tmp = new ArrayList(), lstScriptCriacaoFK_tmp = new ArrayList(),
      lstScriptAlteracaoColuna_tmp = new ArrayList(), IndicesDependentes = new ArrayList(), ForeignKeysDependentes = new ArrayList(), PrimaryKeyDependentes = new ArrayList();


    bool InsertIdentity = false;

    private string CriarCriterio(Table tabela, DataRow drDado) {

      string criterio = string.Empty;
      string separador = string.Empty;

      foreach (object obj in tabela.PrimaryKeyColumns) {
        Column cpk = (Column)obj;

        if ((cpk.Type == DBColumnType.Char) ||
          (cpk.Type == DBColumnType.NVarchar) ||
          (cpk.Type == DBColumnType.VarChar)) {
          criterio += separador + cpk.Name + " = '" + drDado[cpk.Name] + "'";
          separador = " and ";
        } else {
          criterio += separador + cpk.Name + " = " + drDado[cpk.Name];
          separador = " and ";
        }
      }
      return criterio;
    }


    private bool CompararDiferencaDados(DataRow drOriginal, DataRow drAtualizada) {
      int TotalColunas = drOriginal.Table.Columns.Count;

      for (int i = 0; i < TotalColunas; i++) {
        if (drOriginal[i].ToString().Trim() != drAtualizada[i].ToString().Trim())
          return true;
      }

      return false;
    }


    public override void CompararDB(Database BaseAtual, Database NovaBase) {
      LimparScripts();

      foreach (object tb in BaseAtual.Tables) {

        Table tbdestino = PegarTabela(((Table)tb).TableName, NovaBase.Tables);

        if (tbdestino != null) {
          ScriptAlteracaoColumns((Table)tb, tbdestino);
          ScriptAlteracaoPrimaryKeys((Table)tb, tbdestino);
          ScriptAlteracaoForeignKeys((Table)tb, tbdestino);
          ScriptAlteracaoIndices((Table)tb, tbdestino);
          ScriptAlteracaoCheckConstraint((Table)tb, tbdestino);
        } else {
          string segmentacao = ScriptTabelaExcluida((Table)tb);
          lstScriptExclusaoTabela.Add(segmentacao);
          AdicionarScriptSegmentado(((Table)tb).TableName, segmentacao);
        }

      }

      foreach (object tb in NovaBase.Tables) {
        Table tbdorigem = PegarTabela(((Table)tb).TableName, BaseAtual.Tables);

        if (tbdorigem == null) {
          ScriptInclusaoTabela(((Table)tb));
        }
      }

    }


    public override void GerarScriptOrdenado(string caminho) {
      string ScriptsOrdenados = string.Empty;

      ArrayList[] arrarys = { lstScriptNovaTabela, lstScriptExclusaoFK, lstScriptExclusaoPK,
									  lstScriptExclusaoCheckConstraint, lstScriptExclusaoIndice, 
									  lstScriptExclusaoTabela, lstScriptNovaColuna, lstScriptAlteracaoColuna, 
									  lstScriptExclusaoColuna, lstScriptCriacaoPK, lstScriptCriacaoIndice, 
									  lstScriptCriacaoFK, lstScriptCriacaoCheckConstraint};

      foreach (ArrayList array in arrarys) {
        foreach (object script in array)
          ScriptsOrdenados += (string)script;
      }

      StreamWriter sw = new StreamWriter(caminho + "\\Script.txt");
      sw.Write(ScriptsOrdenados);
      sw.Close();

    }


    #region Tabelas

    protected override void ScriptInclusaoTabela(Table tbNova) {
      string script = Environment.NewLine + Environment.NewLine;
      string SeparadorColunas = "  ";
      string segmentacao = string.Empty;

      script += "CREATE TABLE [dbo].[" + tbNova.TableName + "] (" + Environment.NewLine;

      foreach (object col in tbNova.Columns) {
        script += SeparadorColunas + ScriptInclusaoInicialColumns((Column)col, tbNova, true);
        ScriptValorDefault(tbNova, (Column)col, null);
        SeparadorColunas = ", ";
      }

      script += " )" + Environment.NewLine;
      script += "GO" + Environment.NewLine + Environment.NewLine;

      lstScriptNovaTabela.Add(script);
      segmentacao = script;

      string scriptpk = ScriptInclusaoPrimaryKey(tbNova);
      lstScriptCriacaoPK.Add(scriptpk);
      segmentacao += scriptpk;

      foreach (object fk in tbNova.ForeignKeys) {
        string temp = ScriptInclusaoForeignKey((ForeignKey)fk, tbNova.TableName);
        lstScriptCriacaoFK.Add(temp);
        segmentacao += temp;
      }

      foreach (object ix in tbNova.Indexes) {
        string temp = ScriptInclusaoIndice((Index)ix, tbNova.TableName);
        lstScriptCriacaoIndice.Add(temp);
        segmentacao += temp;
      }

      foreach (object chkc in tbNova.CheckConstraints) {
        string temp = ScriptInclusaoCheckConstraint((CheckConstraint)chkc, tbNova.TableName);
        lstScriptCriacaoCheckConstraint.Add(ScriptInclusaoCheckConstraint((CheckConstraint)chkc, tbNova.TableName));
        segmentacao += temp;
      }

      AdicionarScriptSegmentado(tbNova.TableName, segmentacao);
    }


    #region Especializados

    protected override string ScriptTabelaExcluida(Table Tabela) {
      string script = "drop table [dbo].[";
      script += Tabela.TableName + "]" + Environment.NewLine;
      script += "GO" + Environment.NewLine + Environment.NewLine;
      return script;
    }


    #endregion

    #endregion

    #region Colunas

    #region Especializados

    private void IndexDependencies(Table Tabela, string NomeColuna, ArrayList IndicesDependentes) {
      foreach (object ix in Tabela.Indexes) {
        bool colunadependente = false;
        foreach (object col in ((Index)ix).Columns) {
          if (((Column)col).Name == NomeColuna)
            colunadependente = true;
        }

        if ((colunadependente) && (!IndiceDependente(IndicesDependentes, (Index)ix))) {
          string segmentacao = string.Empty, temp = string.Empty;
          IndicesDependentes.Add(ix);

          temp = ScriptIndiceExcluido((Index)ix, Tabela.TableName);
          lstScriptExclusaoIndice_tmp.Add(temp);
          segmentacao = temp;
          temp = ScriptInclusaoIndice((Index)ix, Tabela.TableName);
          lstScriptCriacaoIndice_tmp.Add(temp);
          segmentacao += temp;
          AdicionarScriptSegmentado(Tabela.TableName, segmentacao);
        }
      }
    }


    private bool IndiceDependente(ArrayList IndicesDependentes, Index ix) {
      foreach (object idx in IndicesDependentes) {
        if (((Index)idx).IndexName == ix.IndexName)
          return true;
      }
      return false;
    }


    private void ForeignKeyDependencies(Table Tabela, string NomeColuna, ArrayList ForeignKeysDependentes) {
      foreach (object fk in Tabela.ForeignKeys) {
        bool colunadependente = false;
        foreach (object fkc in ((ForeignKey)fk).Columns) {
          if (((ForeignKeyColumn)fkc).Column == NomeColuna)
            colunadependente = true;
        }

        if ((colunadependente) && (!ForeignKeyDependente(ForeignKeysDependentes, (ForeignKey)fk))) {
          string segmentacao = string.Empty, temp = string.Empty;
          ForeignKeysDependentes.Add(fk);

          temp = ScriptConstraintExcluida(((ForeignKey)fk).ForeignKeyName, Tabela.TableName);
          lstScriptExclusaoFK_tmp.Add(temp);
          segmentacao = temp;
          temp = ScriptInclusaoForeignKey((ForeignKey)fk, Tabela.TableName);
          lstScriptCriacaoFK_tmp.Add(temp);
          segmentacao += temp;
          AdicionarScriptSegmentado(Tabela.TableName, segmentacao);
        }
      }
    }


    private bool ForeignKeyDependente(ArrayList ForeignKeysDependentes, ForeignKey fk) {
      foreach (object afk in ForeignKeysDependentes) {
        if (((ForeignKey)afk).ForeignKeyName == fk.ForeignKeyName)
          return true;
      }
      return false;
    }


    private void PrimaryKeyDependencies(Table Tabela, string NomeColuna, ArrayList PrimaryKeyDependentes) {
      foreach (object pkc in Tabela.PrimaryKeyColumns) {
        bool colunadependente = false;

        if (((Column)pkc).Name == NomeColuna)
          colunadependente = true;

        if ((colunadependente) && (!PrimaryKeyDependente(PrimaryKeyDependentes, Tabela))) {
          string segmentacao = string.Empty, temp = string.Empty;
          PrimaryKeyDependentes.Add(Tabela);
          temp = ScriptConstraintExcluida(Tabela.PrimaryKeyName, Tabela.TableName);
          lstScriptExclusaoPK_tmp.Add(temp);
          segmentacao = temp;
          temp = ScriptInclusaoPrimaryKey(Tabela);
          lstScriptCriacaoPK_tmp.Add(temp);
          segmentacao += temp;
          AdicionarScriptSegmentado(Tabela.TableName, segmentacao);
        }
      }

    }


    private bool PrimaryKeyDependente(ArrayList PrimaryKeyDependentes, Table Tabela) {
      foreach (object pk in PrimaryKeyDependentes) {
        if (((Table)pk).PrimaryKeyName == Tabela.PrimaryKeyName)
          return true;
      }
      return false;
    }


    protected override string ScriptAlteracaoColumns(Column colAtual, Column colNova, Table Tabela) {
      string script = string.Empty;
      string segmentacao = string.Empty, temp = string.Empty;

      if (colAtual.DefaultValue != colNova.DefaultValue)
        ScriptValorDefault(Tabela, colNova, colAtual);

      if ((colAtual.Type != colNova.Type) ||
        (colAtual.Size != colNova.Size) ||
        (colAtual.Precision != colNova.Precision) ||
        (colAtual.Scale != colNova.Scale) ||
        (colAtual.IsPK != colNova.IsPK) ||
        (colAtual.IsNull != colNova.IsNull) ||
        (colAtual.IdentityColumn != colNova.IdentityColumn)) {
        IndexDependencies(Tabela, colNova.Name, IndicesDependentes);
        ForeignKeyDependencies(Tabela, colNova.Name, ForeignKeysDependentes);
        PrimaryKeyDependencies(Tabela, colNova.Name, PrimaryKeyDependentes);

        if (colAtual.IdentityColumn == colNova.IdentityColumn) {
          script += "ALTER TABLE [dbo].[" + Tabela.TableName + "] ALTER COLUMN " + Environment.NewLine;
          script += ScriptInclusaoInicialColumns(colNova, Tabela, false);
          script += "GO" + Environment.NewLine + Environment.NewLine;
          AdicionarScriptSegmentado(Tabela.TableName, script);
        } else {
          if (colNova.IdentityColumn) {
            script += ScriptInclusaoIdentity(colNova, Tabela);

            temp = ScriptInclusaoPrimaryKey(Tabela);
            lstScriptCriacaoPK.Add(temp);
            segmentacao += temp;


            foreach (object fk in Tabela.ForeignKeys) {
              temp = ScriptInclusaoForeignKey((ForeignKey)fk, Tabela.TableName);
              lstScriptCriacaoFK.Add(temp);
              segmentacao += temp;
            }

            foreach (object ix in Tabela.Indexes) {
              temp = ScriptInclusaoIndice((Index)ix, Tabela.TableName);
              lstScriptCriacaoIndice.Add(temp);
              segmentacao += temp;
            }

            foreach (object chkc in Tabela.CheckConstraints) {
              temp = ScriptInclusaoCheckConstraint((CheckConstraint)chkc, Tabela.TableName);
              lstScriptCriacaoCheckConstraint.Add(temp);
              segmentacao += temp;
            }

            InsertIdentity = true;

            lstScriptAlteracaoColuna.Add(script);
            segmentacao = script + segmentacao;
            AdicionarScriptSegmentado(Tabela.TableName, segmentacao);
          } else {
            script += ScriptRemocaoIdentity(colNova, Tabela);
            AdicionarScriptSegmentado(Tabela.TableName, script);
          }
        }

      }

      return script;
    }


    private string ScriptRemocaoIdentity(Column colNova, Table Tabela) {
      string script = string.Empty;

      string NomeOriginalColuna = colNova.Name;
      bool ColunaOriginalNula = colNova.IsNull;
      colNova.Name = "new_" + colNova.Name;
      colNova.IsNull = true;

      script += "ALTER TABLE [dbo].[" + Tabela.TableName + "] ADD " + Environment.NewLine;
      script += ScriptInclusaoInicialColumns(colNova, Tabela, true);
      script += "GO" + Environment.NewLine + Environment.NewLine;

      script += "UPDATE [dbo].[" + Tabela.TableName + "] SET " + Environment.NewLine;
      script += colNova.Name + " = " + NomeOriginalColuna + Environment.NewLine;
      script += "GO" + Environment.NewLine + Environment.NewLine;

      script += "ALTER TABLE [dbo].[" + Tabela.TableName + "] DROP COLUMN " + NomeOriginalColuna + Environment.NewLine;
      script += "GO" + Environment.NewLine + Environment.NewLine;

      if (!ColunaOriginalNula) {
        colNova.IsNull = ColunaOriginalNula;
        script += "ALTER TABLE [dbo].[" + Tabela.TableName + "] ALTER COLUMN " + Environment.NewLine;
        script += ScriptInclusaoInicialColumns(colNova, Tabela, false);
        script += "GO" + Environment.NewLine + Environment.NewLine;
      }

      script += "EXEC sp_rename '" + Tabela.TableName + "." + colNova.Name + "', '" +
        NomeOriginalColuna + "' , 'COLUMN'" + Environment.NewLine;
      script += "GO" + Environment.NewLine + Environment.NewLine;

      colNova.Name = NomeOriginalColuna;

      return script;
    }


    private string ScriptInclusaoIdentity(Column colNova, Table Tabela) {
      string script = string.Empty;
      string SeparadorColunas = "  ";
      string NomeColunasTabela = string.Empty;
      //TODO implementar inclusao identity
      script += "CREATE TABLE [dbo].[tmp_" + Tabela.TableName + "] (" + Environment.NewLine;

      foreach (object col in Tabela.Columns) {
        script += SeparadorColunas + ScriptInclusaoInicialColumns((Column)col, Tabela, true);
        ScriptValorDefault(Tabela, (Column)col, null);
        NomeColunasTabela += SeparadorColunas + ((Column)col).Name;
        SeparadorColunas = ", ";
      }

      script += " )" + Environment.NewLine;
      script += "GO" + Environment.NewLine + Environment.NewLine;

      script += "SET IDENTITY_INSERT dbo.[tmp_" + Tabela.TableName + "] ON " + Environment.NewLine;
      script += "GO" + Environment.NewLine + Environment.NewLine;

      script += "IF EXISTS(SELECT TOP 1 * FROM dbo.[" + Tabela.TableName + "] )" + Environment.NewLine;
      script += "   EXEC('INSERT INTO dbo.[tmp_" + Tabela.TableName + "] (";
      script += NomeColunasTabela + ") " + Environment.NewLine;
      script += "    SELECT " + NomeColunasTabela + " FROM dbo.[" + Tabela.TableName + "] (HOLDLOCK TABLOCKX) ') " + Environment.NewLine;

      script += "GO" + Environment.NewLine + Environment.NewLine;

      script += "SET IDENTITY_INSERT dbo.[tmp_" + Tabela.TableName + "] OFF " + Environment.NewLine;
      script += "GO" + Environment.NewLine + Environment.NewLine;

      script += "DROP TABLE dbo.[" + Tabela.TableName + "] " + Environment.NewLine;
      script += "GO" + Environment.NewLine + Environment.NewLine;

      script += "EXECUTE sp_rename 'dbo.[tmp_" + Tabela.TableName + "]', '" + Tabela.TableName + "', 'OBJECT'" + Environment.NewLine;
      script += "GO" + Environment.NewLine + Environment.NewLine;

      return script;
    }


    protected override string ScriptInclusaoColumns(Column colNova, Table NovaTabela) {
      string script;
      bool IsColunaNula = colNova.IsNull;

      if (!IsColunaNula)
        colNova.IsNull = true;

      script = "ALTER TABLE [dbo].[" + NovaTabela.TableName + "] ADD " + Environment.NewLine;
      script += ScriptInclusaoInicialColumns(colNova, NovaTabela, true);
      ScriptValorDefault(NovaTabela, colNova, null);
      script += "GO" + Environment.NewLine + Environment.NewLine;


      if (!IsColunaNula) {
        colNova.IsNull = IsColunaNula;
        script += "ALTER TABLE [dbo]." + NovaTabela.TableName + " ALTER COLUMN " + Environment.NewLine;
        script += ScriptInclusaoInicialColumns(colNova, NovaTabela, false);
        script += "GO" + Environment.NewLine + Environment.NewLine;
      }

      return script;
    }


    private void LimpaDependencias() {
      InsertIdentity = false;
      ArrayList[] arrays = {lstScriptExclusaoIndice_tmp, lstScriptExclusaoFK_tmp, lstScriptExclusaoPK_tmp,
									 lstScriptCriacaoPK_tmp, lstScriptCriacaoIndice_tmp, lstScriptCriacaoFK_tmp, 
									 IndicesDependentes, ForeignKeysDependentes, PrimaryKeyDependentes, lstScriptAlteracaoColuna_tmp};
      foreach (ArrayList array in arrays)
        array.Clear();

    }


    protected override void ScriptAlteracaoColumns(Table tbAtual, Table tbNova) {
      LimpaDependencias();

      foreach (object col in tbAtual.Columns) {
        Column coldestino = PegarColuna(((Column)col).Name, tbNova.Columns);

        if (coldestino != null) {
          lstScriptAlteracaoColuna_tmp.Add(ScriptAlteracaoColumns((Column)col, coldestino, tbNova));
        } else {
          string temp = ScriptColunaExcluida((Column)col, tbAtual.TableName);
          lstScriptExclusaoColuna.Add(temp);
          AdicionarScriptSegmentado(tbAtual.TableName, temp);
        }
      }

      if (!InsertIdentity)
        ResgataDependencias();

      foreach (object col in tbNova.Columns) {
        Column colorigem = PegarColuna(((Column)col).Name, tbAtual.Columns);

        if (colorigem == null) {
          string temp = ScriptInclusaoColumns((Column)col, tbNova);
          lstScriptAlteracaoColuna.Add(temp);
          AdicionarScriptSegmentado(tbNova.TableName, temp);
        }
      }
    }


    private void ResgataDependencias() {
      string scriptAlteracao = string.Empty;

      foreach (string script in lstScriptExclusaoIndice_tmp)
        scriptAlteracao += script;

      foreach (string script in lstScriptExclusaoFK_tmp)
        scriptAlteracao += script;

      foreach (string script in lstScriptExclusaoPK_tmp)
        scriptAlteracao += script;

      foreach (string script in lstScriptAlteracaoColuna_tmp)
        scriptAlteracao += script;

      foreach (string script in lstScriptCriacaoPK_tmp)
        scriptAlteracao += script;

      foreach (string script in lstScriptCriacaoIndice_tmp)
        scriptAlteracao += script;

      foreach (string script in lstScriptCriacaoFK_tmp)
        scriptAlteracao += script;

      lstScriptAlteracaoColuna.Add(scriptAlteracao);
    }


    private string ScriptInclusaoInicialColumns(Column colNova, Table NovaTabela, bool SetIdentity) {
      string script = string.Empty;
      script += "  [" + colNova.Name + "] ";
      script += PegarTipoSQL(colNova);
      if (SetIdentity)
        script += PegarIdentity(NovaTabela, colNova);
      script += PegarIsNull(colNova.IsNull);
      return script + Environment.NewLine;
    }


    private void ScriptValorDefault(Table NovaTabela, Column colNova, Column colAtual) {

      string script = string.Empty;

      if ((colAtual == null) || (colAtual.DefaultValue == string.Empty)) {
        if (colNova.DefaultValue != string.Empty) {
          script = "IF OBJECT_ID ('" + colNova.ConstraintDefaultName + "') IS NOT NULL " + Environment.NewLine;
          script += "BEGIN " + Environment.NewLine;
          script += "ALTER TABLE [dbo].[" + NovaTabela.TableName + "] " + Environment.NewLine;
          script += "DROP CONSTRAINT [" + colNova.ConstraintDefaultName + "] " + Environment.NewLine;
          script += "END " + Environment.NewLine;
          script += "GO" + Environment.NewLine + Environment.NewLine;

          script += "ALTER TABLE [dbo].[" + NovaTabela.TableName + "] ADD ";
          script += "CONSTRAINT [" + colNova.ConstraintDefaultName + "] DEFAULT " + colNova.DefaultValue + " FOR " + colNova.Name;
          script += Environment.NewLine + "GO" + Environment.NewLine + Environment.NewLine;
        }
      } else {
        script = "IF OBJECT_ID ('" + colAtual.ConstraintDefaultName + "') IS NOT NULL " + Environment.NewLine;
        script += "BEGIN " + Environment.NewLine;
        script += "ALTER TABLE [dbo].[" + NovaTabela.TableName + "] " + Environment.NewLine;
        script += "DROP CONSTRAINT [" + colAtual.ConstraintDefaultName + "] " + Environment.NewLine;
        script += "END " + Environment.NewLine;
        script += "GO" + Environment.NewLine + Environment.NewLine;

        if (colNova.DefaultValue != string.Empty) {
          script += "ALTER TABLE [dbo].[" + NovaTabela.TableName + "] ADD ";
          script += "CONSTRAINT [" + colAtual.ConstraintDefaultName + "] DEFAULT " + colNova.DefaultValue + " FOR " + colNova.Name;
          script += Environment.NewLine + "GO" + Environment.NewLine + Environment.NewLine;
        }
      }
      lstScriptCriacaoCheckConstraint.Add(script);

      AdicionarScriptSegmentado(NovaTabela.TableName, script);

    }


    private string PegarTipoSQL(Column coluna) {
      switch (coluna.Type) {
        case DBColumnType.Integer: return "int";
        case DBColumnType.VarChar: return "varchar(" + coluna.Size.ToString() + ")";
        case DBColumnType.Char: return "char(" + coluna.Size.ToString() + ")";
        case DBColumnType.Image: return "image";
        case DBColumnType.Decimal: return "decimal(" + coluna.Precision.ToString() + "," + coluna.Scale.ToString() + ")";
        case DBColumnType.Float: return "float";
        case DBColumnType.Text: return "text";
        case DBColumnType.DateTime: return "datetime";
        case DBColumnType.Bit: return "bit";
        case DBColumnType.SmallDateTime: return "smalldatetime";
        case DBColumnType.Money: return "money";
        case DBColumnType.SmallInt: return "smallint";
        case DBColumnType.Numeric: return "numeric(" + coluna.Precision.ToString() + "," + coluna.Scale.ToString() + ")";
        case DBColumnType.UniqueIdentifier: return "uniqueidentifier";
        case DBColumnType.BigInt: return "bigint";
        case DBColumnType.TinyInt: return "tinyint";
        case DBColumnType.Binary: return "binary(" + coluna.Size.ToString() + ")";
        case DBColumnType.NVarchar: return "nvarchar(" + coluna.Size.ToString() + ")";
        default: throw new Exception("Tipo de dados não suportado " + coluna.Type.ToString());
      }
    }


    private string PegarIsNull(bool IsNull) {
      if (IsNull)
        return "";
      else
        return " not null ";
    }


    private string PegarIdentity(Table ATable, Column AColumn) {
      if (AColumn.IdentityColumn) {
        return " identity(" + ATable.IdentitySeed + "," + ATable.IdentityIncrement + ") ";
      } else
        return "";
    }


    protected override string ScriptColunaExcluida(Column Coluna, string NomeTabela) {
      string script = string.Empty;
      if (Coluna.ConstraintDefaultName != string.Empty)
        script = ScriptConstraintExcluida(Coluna.ConstraintDefaultName, NomeTabela);

      script += "ALTER TABLE [dbo].[" + NomeTabela + "] DROP COLUMN [" + Coluna.Name + "] " + Environment.NewLine;
      script += "GO" + Environment.NewLine + Environment.NewLine;
      return script;
    }


    #endregion

    #endregion

    #region PrimaryKeys

    protected override void ScriptAlteracaoPrimaryKeys(Table tbAtual, Table tbNova) {
      // validação para verificar se ja foi recriada durante a alteração da coluna.
      if (PrimaryKeyDependente(PrimaryKeyDependentes, tbNova))
        return;

      if (tbAtual.PrimaryKeyName == tbNova.PrimaryKeyName) {
        /*if (!ValidaAlteracaoColumnsPrimaryKey(tbAtual.PrimaryKeyColumns, tbNova.PrimaryKeyColumns)) {

          string segmentacao = ScriptConstraintExcluida(tbAtual.PrimaryKeyName, tbAtual.TableName);
          lstScriptExclusaoPK.Add(segmentacao);
          AdicionarScriptSegmentado(tbAtual.TableName, segmentacao);

          segmentacao = ScriptInclusaoPrimaryKey(tbNova);
          lstScriptCriacaoPK.Add(segmentacao);
          AdicionarScriptSegmentado(tbNova.TableName, segmentacao);

        }*/
      } else {
        if (tbAtual.PrimaryKeyName != string.Empty) {
          string segmentacao = ScriptConstraintExcluida(tbAtual.PrimaryKeyName, tbAtual.TableName);
          lstScriptExclusaoPK.Add(segmentacao);
          AdicionarScriptSegmentado(tbAtual.TableName, segmentacao);
        }

        if (tbNova.PrimaryKeyName != string.Empty) {
          string segmentacao = ScriptInclusaoPrimaryKey(tbNova);
          lstScriptCriacaoPK.Add(segmentacao);
          AdicionarScriptSegmentado(tbNova.TableName, segmentacao);
        }
      }
    }


    #region Especializados

    private bool ValidaAlteracaoColumnsPrimaryKey(List<Column> pkAtual, List<Column> pkNova) {
      foreach (object col in pkAtual) {
        Column pk = PegarColuna(((Column)col).Name, pkNova);
        if (pk == null)
          return false;
      }

      foreach (object col in pkNova) {
        Column pk = PegarColuna(((Column)col).Name, pkAtual);

        if (pk == null)
          return false;
      }

      return true;
    }


    private string ScriptInclusaoPrimaryKey(Table tb) {
      if (tb.PrimaryKeyName == string.Empty)
        return string.Empty;

      string SeparadorColuna = "  ";
      string script = "ALTER TABLE [dbo].[" + tb.TableName + "] WITH NOCHECK ADD " + Environment.NewLine;
      script += "CONSTRAINT [" + tb.PrimaryKeyName + "] PRIMARY KEY  CLUSTERED " + Environment.NewLine;
      script += "( " + Environment.NewLine;

      foreach (object col in tb.PrimaryKeyColumns) {
        script += SeparadorColuna + ScriptColunaPrimaryKeyAdicionada((Column)col, tb.TableName);
        SeparadorColuna = ", ";
      }

      script += ") " + Environment.NewLine;
      script += "GO " + Environment.NewLine + Environment.NewLine;
      return script;
    }


    private string ScriptColunaPrimaryKeyAdicionada(Column pk, string NomeTabela) {
      return "[" + pk.Name + "]" + Environment.NewLine;
    }


    #endregion

    #endregion

    #region Indexes

    protected override void ScriptAlteracaoIndices(Table tbAtual, Table tbNova) {
      string relatorioAlteracao = string.Empty;

      foreach (object ix in tbAtual.Indexes) {
        Index ixdestino = PegarIndice(((Index)ix).IndexName, tbNova.Indexes);

        if (ixdestino != null) {
          if (!ValidaAlteracaoIndices((Index)ix, ixdestino, tbAtual.TableName)) {
            string segmentacao = ScriptIndiceExcluido((Index)ixdestino, tbAtual.TableName);
            lstScriptExclusaoIndice.Add(segmentacao);
            AdicionarScriptSegmentado(tbAtual.TableName, segmentacao);

            segmentacao = ScriptInclusaoIndice((Index)ixdestino, tbNova.TableName);
            lstScriptCriacaoIndice.Add(segmentacao);
            AdicionarScriptSegmentado(tbNova.TableName, segmentacao);

          }
        } else {
          string segmentacao = ScriptIndiceExcluido((Index)ix, tbAtual.TableName);
          lstScriptExclusaoIndice.Add(segmentacao);
          AdicionarScriptSegmentado(tbAtual.TableName, segmentacao);
        }
      }


      foreach (object ix in tbNova.Indexes) {
        Index ixorigem = PegarIndice(((Index)ix).IndexName, tbAtual.Indexes);

        if (ixorigem == null) {
          string segmentacao = ScriptInclusaoIndice((Index)ix, tbAtual.TableName);
          lstScriptCriacaoIndice.Add(segmentacao);
          AdicionarScriptSegmentado(tbAtual.TableName, segmentacao);
        }
      }

    }


    #region Especializados

    private bool ValidaAlteracaoIndices(Index ixAtual, Index ixNova, string NomeTabela) {
      string relatorioAlteracao = string.Empty;
      return (ixAtual.Unique == ixNova.Unique) && (ValidaAlteracaoColumnsIndice(ixAtual.Columns, ixNova.Columns, NomeTabela));
    }


    private bool ValidaAlteracaoColumnsIndice(List<IndexColumn> ixAtual, List<IndexColumn> ixNova, string NomeTabela) {
      foreach (object col in ixAtual) {
        /*Column ix = PegarColuna(((Column)col).Name, ixNova).Column;
        if (ix == null)
          return false;*/
      }

      foreach (object col in ixNova) {
        /*Column ix = PegarColuna(((Column)col).Name, ixAtual).Column;

        if (ix == null)
          return false;*/
      }

      return true;
    }


    private string ScriptColunaIndiceAdicionada(Column cix, string NomeTabela) {
      return "[" + cix.Name + "]" + Environment.NewLine;
    }


    private string ScriptInclusaoIndice(Index indice, string NomeTabelaOrigem) {
      string Unique = string.Empty;
      string SeparadorColuna = "  ";

      if (indice.Unique)
        Unique = "UNIQUE ";

      string script = "CREATE " + Unique + "INDEX [" + indice.IndexName + "] ON [dbo].[" + NomeTabelaOrigem + "] ( " + Environment.NewLine;

      foreach (object col in indice.Columns) {
        script += SeparadorColuna + ScriptColunaIndiceAdicionada((Column)col, NomeTabelaOrigem);
        SeparadorColuna = ", ";
      }

      script += " )" + Environment.NewLine + "GO " + Environment.NewLine + Environment.NewLine;

      return script;
    }


    private string ScriptIndiceExcluido(Index indice, string TableName) {
      string script = "DROP INDEX [" + TableName + "].[" + indice.IndexName + "] " + Environment.NewLine +
        "GO " + Environment.NewLine + Environment.NewLine;
      return script;
    }


    #endregion

    #endregion

    #region ForeignKeys

    protected override void ScriptAlteracaoForeignKeys(Table tbAtual, Table tbNova) {
      foreach (object fk in tbAtual.ForeignKeys) {
        ForeignKey fkdestino = PegarForeignKey(((ForeignKey)fk).ForeignKeyName, tbNova.ForeignKeys);

        if (fkdestino != null) {
          if (!ValidaAlteracaoForeignKeys((ForeignKey)fk, fkdestino)) {
            string segmentacao = ScriptConstraintExcluida(fkdestino.ForeignKeyName, tbAtual.TableName);
            lstScriptExclusaoFK.Add(segmentacao);
            AdicionarScriptSegmentado(tbAtual.TableName, segmentacao);

            segmentacao = ScriptInclusaoForeignKey(fkdestino, tbAtual.TableName);
            lstScriptCriacaoFK.Add(segmentacao);
            AdicionarScriptSegmentado(tbAtual.TableName, segmentacao);

          }
        } else {
          string segmentacao = ScriptConstraintExcluida(((ForeignKey)fk).ForeignKeyName, tbAtual.TableName);
          lstScriptExclusaoFK.Add(segmentacao);
          AdicionarScriptSegmentado(tbAtual.TableName, segmentacao);
        }

      }

      foreach (object fk in tbNova.ForeignKeys) {
        ForeignKey fkorigem = PegarForeignKey(((ForeignKey)fk).ForeignKeyName, tbAtual.ForeignKeys);

        if (fkorigem == null) {
          string segmentacao = ScriptInclusaoForeignKey((ForeignKey)fk, tbAtual.TableName);
          lstScriptCriacaoFK.Add(segmentacao);
          AdicionarScriptSegmentado(tbAtual.TableName, segmentacao);
        }
      }
    }


    private bool ValidaAlteracaoColumnsForeignKey(ForeignKey fkAtual, ForeignKey fkNova) {
      foreach (object col in fkAtual.Columns) {
        ForeignKeyColumn fk = PegarColunasFK((ForeignKeyColumn)col, fkNova.Columns);
        if (fk == null)
          return false;
      }


      foreach (object col in fkNova.Columns) {
        ForeignKeyColumn fk = PegarColunasFK((ForeignKeyColumn)col, fkAtual.Columns);

        if (fk == null)
          return false;
      }

      return true;
    }


    #region Especializados

    private bool ValidaAlteracaoForeignKeys(ForeignKey fkAtual, ForeignKey fkNova) {
      return (fkAtual.RefTableName == fkNova.RefTableName) &&
           (fkAtual.DeleteCascade == fkNova.DeleteCascade) &&
           (fkAtual.UpdateCascade == fkNova.UpdateCascade) &&
           (ValidaAlteracaoColumnsForeignKey(fkAtual, fkNova));
    }


    private string ScriptInclusaoForeignKey(ForeignKey ForeignKey, string NomeTabelaOrigem) {
      string SeparadorColuna = "  ";
      string ColunaOrigem = string.Empty, ColunaReferencia = string.Empty;
      string script = "ALTER TABLE [dbo].[" + NomeTabelaOrigem + "] ADD " + Environment.NewLine;
      script += "CONSTRAINT [" + ForeignKey.ForeignKeyName + "] FOREIGN KEY " + Environment.NewLine;
      script += "( " + Environment.NewLine;

      foreach (object col in ForeignKey.Columns) {
        ForeignKeyColumn coluna = (ForeignKeyColumn)col;
        ColunaOrigem += SeparadorColuna + "[" + coluna.Column + "]";
        ColunaReferencia += SeparadorColuna + "[" + coluna.RefColumn + "]";
        SeparadorColuna = ", ";
      }

      script += ColunaOrigem + Environment.NewLine;
      script += " ) REFERENCES [dbo].[" + ForeignKey.RefTableName + "] ( " + Environment.NewLine;
      script += ColunaReferencia + Environment.NewLine;

      script += " ) ";

      if (ForeignKey.DeleteCascade)
        script += " ON DELETE CASCADE ";

      if (ForeignKey.UpdateCascade)
        script += " ON UPDATE CASCADE ";

      script += Environment.NewLine;
      script += "GO " + Environment.NewLine + Environment.NewLine;

      return script;
    }


    #endregion

    #endregion

    #region Constraints

    protected override void ScriptAlteracaoCheckConstraint(Table tbAtual, Table tbNova) {
      foreach (object chkc in tbAtual.CheckConstraints) {
        CheckConstraint ccdestino = PegarCheckConstraint(((CheckConstraint)chkc).CheckConstraintName, tbNova.CheckConstraints);

        if (ccdestino != null) {
          if (!ValidaAlteracaoCheckConstraint((CheckConstraint)chkc, ccdestino, tbAtual.TableName)) {
            string segmentacao = ScriptConstraintExcluida(((CheckConstraint)chkc).CheckConstraintName, tbAtual.TableName);
            lstScriptExclusaoCheckConstraint.Add(segmentacao);
            AdicionarScriptSegmentado(tbAtual.TableName, segmentacao);

            segmentacao = ScriptInclusaoCheckConstraint(ccdestino, tbAtual.TableName);
            lstScriptCriacaoCheckConstraint.Add(segmentacao);
            AdicionarScriptSegmentado(tbAtual.TableName, segmentacao);

          }
        } else {
          string segmentacao = ScriptConstraintExcluida(((CheckConstraint)chkc).CheckConstraintName, tbAtual.TableName);
          lstScriptExclusaoCheckConstraint.Add(segmentacao);
          AdicionarScriptSegmentado(tbAtual.TableName, segmentacao);
        }
      }


      foreach (object chkc in tbNova.CheckConstraints) {
        CheckConstraint ccorigem = PegarCheckConstraint(((CheckConstraint)chkc).CheckConstraintName, tbAtual.CheckConstraints);

        if (ccorigem == null) {
          string segmentacao = ScriptInclusaoCheckConstraint((CheckConstraint)chkc, tbAtual.TableName);
          lstScriptCriacaoCheckConstraint.Add(segmentacao);
          AdicionarScriptSegmentado(tbAtual.TableName, segmentacao);
        }
      }
    }


    #region Especializados

    private bool ValidaAlteracaoCheckConstraint(CheckConstraint ccAtual, CheckConstraint ccNova, string NomeTabela) {
      return (ccAtual.Expression == ccNova.Expression);
    }


    private string ScriptInclusaoCheckConstraint(CheckConstraint cc, string NomeTabelaOrigem) {
      string script = "ALTER TABLE [dbo].[" + NomeTabelaOrigem + "] ADD " + Environment.NewLine;
      script += "CONSTRAINT [" + cc.CheckConstraintName + "] CHECK " + cc.Expression + Environment.NewLine;
      script += "GO " + Environment.NewLine + Environment.NewLine;
      return script;
    }


    #endregion

    private string ScriptConstraintExcluida(string ContraintName, string TableName) {
      string script = "ALTER TABLE [dbo].[" + TableName + "] DROP CONSTRAINT " + "[" + ContraintName + "] " + Environment.NewLine +
        "GO " + Environment.NewLine + Environment.NewLine;
      return script;
    }


    #endregion

  }
}
