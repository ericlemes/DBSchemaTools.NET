using System;
using System.Data;
using DBInfo.Core.Model;
using DBInfo.Core.Extractor;
using DBInfo.Core.OutputGenerators;
using DBInfo.DBExtractors;
using DBInfo.OutputGenerators;
using DBInfo.DBSync;

namespace DBInfo.CommandLine {
  class DBInfoMain {
    static int ProgressoDadoInicial;
    static int TotalLinhas;
    static int ProgressoAtual;

    [STAThread]
    static int Main(string[] args) {

      try {
        if (args.Length < 1)
          throw new Exception("Informe o comando [help, introspectar, introspectartoxml, introspectarfromxml, dadosiniciais, comparardadosiniciais, relatorio, comparacao, comparacaofromxml, introspectarfrompg]");

        if ((args[0].ToLower() == "introspectar") || (args[0].ToLower() == "introspectartoxml")) {
          SelecionarIntrospectar(args);
        } else if (args[0].ToLower() == "introspectarfrompg") {
          SelecionarIntrospectarFromProgress(args);
        } else if (args[0].ToLower() == "introspectarfromxml") {
          SelecionarIntrospectarFromXML(args);
        } else if (args[0].ToLower() == "dadosiniciais") {
          SelecionarDadosIniciais(args);
        } else if (args[0].ToLower() == "comparardadosiniciais") {
          SelecionarComparacaoDadosIniciais(args);
        } else if (args[0].ToLower() == "relatorio") {
          SelecionarRelatorio(args);
        } else if (args[0].ToLower() == "comparacao") {
          SelecionarComparacao(args);
        } else if (args[0].ToLower() == "comparacaofromxml") {
          SelecionarComparacaoFromXML(args);
        } else if (args[0].ToLower() == "help") {
          SelecionarHelp();
        }
        return 0;
      } catch (Exception e) {
        Console.Error.WriteLine(e.Message);
        return -1;
      }
    }


    static void SelecionarHelp() {
      Console.WriteLine("introspectar - Gera os scripts no formato .sql da base informada." + Environment.NewLine);
      Console.WriteLine("introspectarfrompg - Gera os scripts no formato .df com a estrutura da base informada" + Environment.NewLine);
      Console.WriteLine("introspectarfromxml - Gera os arquivos no formato XML da base informada." + Environment.NewLine);
      Console.WriteLine("dadosiniciais - Gera os arquivos de carga inicial no formato .sql e XML da base informada." + Environment.NewLine);
      Console.WriteLine("comparacao - Gera os scripts no formato .sql com as diferenças de estrutura entre as bases informadas" + Environment.NewLine);
      Console.WriteLine("comparacaofromxml - Gera os scripts no formato .sql com as diferenças de estrutura entre as bases informadas" + Environment.NewLine);
      Console.WriteLine("comparardadosiniciais - Gera os scripts no formato .sql com as diferenças de dados iniciais entre as bases informadas" + Environment.NewLine);
    }


    static void SelecionarIntrospectar(string[] args) {
      /*if (args.Length < 2)
        throw new Exception("Informe a connection string: DBInfo introspectar <connectionstring>");

      DBInfoExtractor Introspector = new DBInfoExtractor();
      Introspector.EventoAntesLerDadosBanco += new DBInfoExtractor.AntesLerDadosBancoHandler(AntesLerDadosBanco);
      SQLServerDBExtractor conn = new SQLServerDBExtractor();

      conn.SqlConn.ConnectionString = args[1];
      Introspector. = conn;
      Introspector.IntrospectarBancoDados();

      SQLServerScriptGenerator sg = new SQLServerScriptGenerator();
      sg.EventoAntesGerarScripts += new OutputGenerator.AntesGerarScriptsHandler(AntesGerarScripts);
      sg.EventoAntesSalvarScripts += new OutputGenerator.AntesSalvarScripts(AntesSalvarScripts);
      sg.EventoAntesGerarDadosIniciais += new OutputGenerator.AntesGerarDadosIniciais(AntesGerarDadosIniciais);
      sg.EventoAntesGerarLinhaDadoInicial += new OutputGenerator.AntesGerarLinhaDadoInicial(AntesGerarLinhaDadosIniciais);
      sg.Introspector = Introspector;
      sg.ScriptsRootDir = System.AppDomain.CurrentDomain.BaseDirectory + "Scripts\\";

      sg.GerarScripts();
      if (args[0].ToLower() == "introspectartoxml")
        sg.SalvarScriptsTabelasXML();
      else
        sg.SalvarScriptsTabelasSQL(new System.Text.UTF8Encoding());

      sg.SalvarScripts();*/
    }


    static void SelecionarIntrospectarFromXML(string[] args) {
      if (args.Length < 2)
        throw new Exception("Informe a connection string: DBInfo introspectarfromxml <caminho do XML>");

      //validar de o caminho existe.
      DBInfoExtractor Introspector = new DBInfoExtractor();
      OracleScriptGenerator sg = new OracleScriptGenerator();

      sg.EventoAntesGerarScripts += new OutputGenerator.AntesGerarScriptsHandler(AntesGerarScripts);
      sg.EventoAntesSalvarScripts += new OutputGenerator.AntesSalvarScripts(AntesSalvarScripts);
      sg.EventoAntesGerarDadosIniciais += new OutputGenerator.AntesGerarDadosIniciais(AntesGerarDadosIniciais);
      sg.EventoAntesGerarLinhaDadoInicial += new OutputGenerator.AntesGerarLinhaDadoInicial(AntesGerarLinhaDadosIniciais);

      Introspector.DeserializarXML(args[1]);

      sg.ScriptsRootDir = System.AppDomain.CurrentDomain.BaseDirectory + "Scripts\\Tabelas\\";
      sg.Introspector = Introspector;
      sg.GerarScripts();
      sg.SalvarScriptsTabelasSQL(new System.Text.UTF8Encoding());
    }


    static void SelecionarComparacao(string[] args) {
      /*if (args.Length < 4)
        throw new Exception("Informe a connection string: DBInfo comparacao <connectionstringDBOrigem> <connectionstringDBDestino> <caminho resultado>");

      DBInfoExtractor Introspector1 = new DBInfoExtractor();
      DBInfoExtractor Introspector2 = new DBInfoExtractor();

      SQLServerDBExtractor conn1 = new SQLServerDBExtractor();
      SQLServerDBExtractor conn2 = new SQLServerDBExtractor();

      conn1.SqlConn.ConnectionString = args[1];
      conn2.SqlConn.ConnectionString = args[2];

      Introspector1.Extractor = conn1;
      Introspector2.ConexaoBD = conn2;

      Introspector1.IntrospectarBancoDados();
      Introspector2.IntrospectarBancoDados();

      DBSyncScript dbc = new DBSyncScript();
      DBSyncReport dbc2 = new DBSyncReport();

      dbc.CompararDB(Introspector1, Introspector2);
      dbc2.CompararDB(Introspector1, Introspector2);

      dbc.GerarScriptOrdenado(args[3]);
      dbc2.GerarScriptOrdenado(args[3]);*/
    }


    static void SelecionarComparacaoFromXML(string[] args) {
      if (args.Length < 4)
        throw new Exception("Informe a connection string: DBInfo comparacaofromxml <Caminho XML DBOrigem> <Caminho XML DBDestino> <caminho resultado>");

      DBInfoExtractor Introspector1 = new DBInfoExtractor();
      DBInfoExtractor Introspector2 = new DBInfoExtractor();

      Introspector1.DeserializarXML(args[1]);
      Introspector2.DeserializarXML(args[2]);

      DBSyncScript dbc = new DBSyncScript();
      DBSyncReport dbc2 = new DBSyncReport();

      dbc.CompararDB(Introspector1, Introspector2);
      dbc2.CompararDB(Introspector1, Introspector2);

      dbc.GerarScriptOrdenado(args[3]);
      dbc2.GerarScriptOrdenado(args[3]);


    }


    static void SelecionarComparacaoDadosIniciais(string[] args) {
      if (args.Length < 4)
        throw new Exception("Informe os parâmetros corretamente: DBInfo comparardadosiniciais <connectionstring> <tabelasseparadaspor;> <CaminhoOrigemXMLDadosIniciais> <DestinoComparacao>");

      DBInfoExtractor Introspector = new DBInfoExtractor();
      SQLServerDBExtractor conn = new SQLServerDBExtractor();

      if (args.Length >= 4) {
        string[] DadosIniciais = args[2].Split(';');
        foreach (string s in DadosIniciais)
          Introspector.DataTables.Add(s);

        conn.SqlConn.ConnectionString = args[1];
        Introspector.Extractor = conn;
        Introspector.IntrospectarDadosIniciais();

        DBSyncScript dbc = new DBSyncScript();
        dbc.CompararDadosIniciais(Introspector, DadosIniciais, args[3], args[4]);

      }

    }


    static void SelecionarDadosIniciais(string[] args) {
      /*if (args.Length < 3)
        throw new Exception("Informe os parâmetros corretamente: DBInfo dadosiniciais <connectionstring> <tabelasseparadaspor;>");

      DBInfoExtractor Introspector = new DBInfoExtractor();
      Introspector.EventoAntesLerDadosBanco += new DBInfoExtractor.AntesLerDadosBancoHandler(AntesLerDadosBanco);
      SQLServerDBExtractor conn = new SQLServerDBExtractor();

      if (args.Length >= 2) {
        string[] DadosIniciais = args[2].Split(';');
        foreach (string s in DadosIniciais)
          Introspector.DataTables.Add(s);
      }

      conn.SqlConn.ConnectionString = args[1];
      Introspector.Extractor = conn;
      Introspector.IntrospectarDadosIniciais();

      SQLServerScriptGenerator sg = new SQLServerScriptGenerator();
      sg.EventoAntesGerarScripts += new OutputGenerator.AntesGerarScriptsHandler(AntesGerarScripts);
      sg.EventoAntesSalvarScripts += new OutputGenerator.AntesSalvarScripts(AntesSalvarScripts);
      sg.EventoAntesGerarDadosIniciais += new OutputGenerator.AntesGerarDadosIniciais(AntesGerarDadosIniciais);
      sg.EventoAntesGerarLinhaDadoInicial += new OutputGenerator.AntesGerarLinhaDadoInicial(AntesGerarLinhaDadosIniciais);
      sg.Introspector = Introspector;
      sg.ScriptsRootDir = System.AppDomain.CurrentDomain.BaseDirectory + "Scripts\\";
      sg.GerarScripts();
      sg.SalvarScriptsTabelasSQL(new System.Text.UTF8Encoding());
      sg.SalvarScripts();*/
    }


    static void SelecionarRelatorio(string[] args) {
      if (args.Length < 2)
        throw new Exception("Informe os parâmetros corretamente: DBInfo relatorio <connectionstring> ");

      DBInfoExtractor Introspector = new DBInfoExtractor();
      Introspector.EventoAntesLerDadosBanco += new DBInfoExtractor.AntesLerDadosBancoHandler(AntesLerDadosBanco);
      SQLServerDBExtractor conn = new SQLServerDBExtractor();

      conn.SqlConn.ConnectionString = args[1];
      Introspector.Extractor = conn;
      Introspector.IntrospectarPrimaryKeysEForeignKeys();

      Console.WriteLine("");
      Console.WriteLine("Tabelas sem Primary Key");
      foreach (Table t in Introspector.Tables) {
        if (t.PrimaryKeyName == String.Empty)
          Console.WriteLine(t.TableName);
      }

      Console.WriteLine("");
      Console.WriteLine("Tabelas sem Foreign Key");
      foreach (Table t in Introspector.Tables) {
        if (t.ForeignKeys.Count <= 0)
          Console.WriteLine(t.TableName);
      }
    }


    static void AntesGerarDadosIniciais(Table ATable, DataSet ADataset) {
      ProgressoDadoInicial = 0;
      TotalLinhas = ADataset.Tables[0].Rows.Count;
      ProgressoAtual = 0;
      Console.WriteLine("  Total linhas: " + TotalLinhas.ToString());
    }


    static void AntesGerarLinhaDadosIniciais(Table ATable, DataRow ARow) {
      ProgressoDadoInicial += 1;
      int TmpProg = (ProgressoDadoInicial * 100) / TotalLinhas;
      if ((ProgressoAtual + 500) < ProgressoDadoInicial) {
        ProgressoAtual += 500;
        Console.WriteLine("  Registros lidos: " + ProgressoAtual.ToString());
      }
    }


    static void AntesLerDadosBanco(DBInfoExtractor.DadosALer ADados, string AObjeto) {
      switch (ADados) {
        case DBInfoExtractor.DadosALer.Tabelas: Console.WriteLine("Lendo tabelas"); break;
        case DBInfoExtractor.DadosALer.Colunas: Console.WriteLine("Lendo colunas da tabela " + AObjeto); break;
        case DBInfoExtractor.DadosALer.DadosIniciais: Console.WriteLine("Lendo dados iniciais da tabela " + AObjeto); break;
        case DBInfoExtractor.DadosALer.ForeignKeys: Console.WriteLine("Lendo foreign keys da tabela " + AObjeto); break;
        case DBInfoExtractor.DadosALer.Functions: Console.WriteLine("Lendo function " + AObjeto); break;
        case DBInfoExtractor.DadosALer.Indices: Console.WriteLine("Lendo índices da tabela " + AObjeto); break;
        case DBInfoExtractor.DadosALer.PrimaryKey: Console.WriteLine("Lendo primary key da tabela " + AObjeto); break;
        case DBInfoExtractor.DadosALer.Procedures: Console.WriteLine("Lendo procedure " + AObjeto); break;
        case DBInfoExtractor.DadosALer.Triggers: Console.WriteLine("Lendo triggers da tabela " + AObjeto); break;
        case DBInfoExtractor.DadosALer.Sequences: Console.WriteLine("Lendo sequence " + AObjeto); break;
      }
    }


    static void AntesGerarScripts(OutputGenerator.ScriptsAGerar AScript, string AObjeto) {
      switch (AScript) {
        case OutputGenerator.ScriptsAGerar.DadosIniciais: Console.WriteLine("Gerando dados iniciais: " + AObjeto); break;
        case OutputGenerator.ScriptsAGerar.ForeignKeys: Console.WriteLine("Gerando foreign keys: " + AObjeto); break;
        case OutputGenerator.ScriptsAGerar.Functions: Console.WriteLine("Gerando function: " + AObjeto); break;
        case OutputGenerator.ScriptsAGerar.Procedures: Console.WriteLine("Gerando procedure: " + AObjeto); break;
        case OutputGenerator.ScriptsAGerar.Tabelas: Console.WriteLine("Gerando tabela: " + AObjeto); break;
        case OutputGenerator.ScriptsAGerar.Triggers: Console.WriteLine("Gerando trigger: " + AObjeto); break;
        case OutputGenerator.ScriptsAGerar.Sequences: Console.WriteLine("Gerando sequences: " + AObjeto); break;
      }
    }


    static void AntesSalvarScripts(string AScript) {
      Console.WriteLine("Salvando script " + AScript);
    }

    static void SelecionarIntrospectarFromProgress(string[] args) {
      /*if (args.Length < 2)
        throw new Exception("Informe a connection string: DBInfo introspectarfrompg <connectionstring>");

      DBInfoExtractor Introspector = new DBInfoExtractor();
      Introspector.EventoAntesLerDadosBanco += new DBInfoExtractor.AntesLerDadosBancoHandler(AntesLerDadosBanco);
      ProgressDBExtractor conn = new ProgressDBExtractor();

      conn.Connection.ConnectionString = args[1];
      Introspector.ConexaoBD = conn;
      Introspector.IntrospectarBancoDados();

      ProgressScriptGenerator sg = new ProgressScriptGenerator();
      sg.EventoAntesGerarScripts += new OutputGenerator.AntesGerarScriptsHandler(AntesGerarScripts);
      sg.EventoAntesSalvarScripts += new OutputGenerator.AntesSalvarScripts(AntesSalvarScripts);
      sg.EventoAntesGerarDadosIniciais += new OutputGenerator.AntesGerarDadosIniciais(AntesGerarDadosIniciais);
      sg.EventoAntesGerarLinhaDadoInicial += new OutputGenerator.AntesGerarLinhaDadoInicial(AntesGerarLinhaDadosIniciais);
      sg.Introspector = Introspector;
      sg.ScriptsRootDir = System.AppDomain.CurrentDomain.BaseDirectory + "Scripts\\";

      sg.GerarScripts();
      sg.FullScriptName = "all.df";
      sg.SalvarFullScript(System.Text.Encoding.Default);*/
    }
  }
}
