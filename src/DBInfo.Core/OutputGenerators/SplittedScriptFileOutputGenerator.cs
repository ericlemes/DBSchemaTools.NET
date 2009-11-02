using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Model;
using System.IO;
using DBInfo.Core.Statement;

namespace DBInfo.Core.OutputGenerators {
  public class SplittedScriptFileOutputGenerator : IScriptFileOutputGenerator {
    private string _TablesDir = "Tables";
    public string TablesDir{
      get { return _TablesDir;}
      set { _TablesDir = value;}
    }
    
    private string _ForeignKeysDir = "ForeignKeys";
    public string ForeignKeysDir{
      get { return _ForeignKeysDir;}
      set { _ForeignKeysDir = value;}
    }
    
    private string _ConstraintsDir = "Constraints";
    public string ConstraintsDir {
      get { return _ConstraintsDir;}
      set { _ConstraintsDir = value;}
    }
    
    private string _FunctionsDir = "Functions";
    public string FunctionsDir {
      get { return _FunctionsDir;}
      set { _FunctionsDir = value;}
    }
    
    private string _IndexesDir = "Indexes";
    public string IndexesDir{
      get { return _IndexesDir;}
      set { _IndexesDir = value;}
    }
    
    private string _ProceduresDir = "Procedures";
    public string ProceduresDir{
      get { return _ProceduresDir;}
      set { _ProceduresDir = value;}
    }
    
    private string _SequencesDir = "Sequences";
    public string SequencesDir{
      get { return _SequencesDir;}
      set { _SequencesDir = value;}
    }
    
    private string _TriggersDir = "Triggers";
    public string TriggersDir{
      get { return _TriggersDir;}
      set { _TriggersDir = value;}
    }
    
    private string _ViewsDir = "Views";
    public string ViewsDir{
      get { return _ViewsDir;}
      set { _ViewsDir = value;}
    }
  
    #region IScriptFileOutputGenerator Members
    
    private void WriteScript(string File, string Script){
      FileStream fs = new FileStream(File, FileMode.Create, FileAccess.Write);
      StreamWriter sw = new StreamWriter(fs);
      sw.WriteLine(Script);      
      sw.Flush();
      sw.Close();
      fs.Close();    
    }

    public void GenerateFileOutput(string OutputDir, List<BaseStatement> statements, IScriptOutputHandler OutputGenerator){
      string FullTableDir = OutputDir + "\\" + TablesDir;
      Directory.CreateDirectory(FullTableDir);
      
      string FullFKDir = OutputDir + "\\" + ForeignKeysDir;
      Directory.CreateDirectory(FullFKDir);
      
      string FullConstraintsDir = OutputDir + "\\" + ConstraintsDir;
      Directory.CreateDirectory(FullConstraintsDir);
      
      string FullFunctionsDir = OutputDir + "\\" + FunctionsDir;
      Directory.CreateDirectory(FullFunctionsDir);
      
      string FullIndexesDir = OutputDir + "\\" + IndexesDir;
      Directory.CreateDirectory(FullIndexesDir);
      
      string FullProceduresDir = OutputDir + "\\" + ProceduresDir;
      Directory.CreateDirectory(FullProceduresDir);
      
      string FullSequencesDir = OutputDir + "\\" + SequencesDir;
      Directory.CreateDirectory(FullSequencesDir);
      
      string FullTriggersDir = OutputDir + "\\" + TriggersDir;
      Directory.CreateDirectory(FullTriggersDir);
      
      string FullViewsDir = OutputDir + "\\" + ViewsDir;
      Directory.CreateDirectory(FullViewsDir);            
      
      foreach(BaseStatement s in statements){
        if (s is CreateTable)
          WriteScript(FullTableDir + "\\" + ((CreateTable)s).Table.TableName + ".table.sql", s.Script);                
        if (s is CreateTrigger)
          WriteScript(FullTriggersDir + "\\" + ((CreateTrigger)s).Trigger.TableName + "." + ((CreateTrigger)s).Trigger.TriggerName + ".trigger.sql", s.Script);
        if (s is CreateFunction)
          WriteScript(FullFunctionsDir + "\\" + ((CreateFunction)s).Function.Name + ".function.sql", s.Script);
        if (s is CreateProcedure)
          WriteScript(FullProceduresDir + "\\" + ((CreateProcedure)s).Procedure.Name + ".procedure.sql", s.Script);
        if (s is CreateView)
          WriteScript(FullViewsDir + "\\" + ((CreateView)s).View.Name + ".view.sql", s.Script);
      }
      
      WriteConstraints(FullConstraintsDir, statements, OutputGenerator);      
      WriteFKs(FullFKDir, statements, OutputGenerator);
      //WriteIndexes(FullIndexesDir + "\\" + t.TableName + ".indexes.sql", t, OutputGenerator);      
    }

    private void WriteFKs(string FullFKsDir, List<BaseStatement> statements, IScriptOutputHandler OutputGen) {
      List<string> tableNames =
        (from DBInfo.Core.Statement.BaseStatement s in statements
         where s is CreateForeignKey
         select (s as CreateForeignKey).ForeignKey.TableName).ToList<string>();
      foreach (string tableName in tableNames) {
        FileStream fs = new FileStream(FullFKsDir + "\\" + tableName + ".fk.sql", FileMode.Create, FileAccess.Write);
        StreamWriter sw = new StreamWriter(fs);

        List<BaseStatement> tableStatements =
          (from BaseStatement s in statements
           where s is CreateForeignKey && ((s as CreateForeignKey).ForeignKey.TableName == tableName)
           select s).ToList<BaseStatement>();

        foreach (BaseStatement s in tableStatements) {
          sw.WriteLine(s.Script);
          sw.WriteLine(OutputGen.ScriptTerminator);
          sw.WriteLine("");
        }

        sw.Flush();
        sw.Close();
        fs.Close();
      }      
    }
    
    private void WriteConstraints(string FullConstraintsDir, List<BaseStatement> statements, IScriptOutputHandler OutputGen){      
      List<string> tableNames =
        (from DBInfo.Core.Statement.BaseStatement s in statements
         where s is CreatePrimaryKey
         select (s as CreatePrimaryKey).Table.TableName).Union<string>(
          from BaseStatement s2 in statements
          where s2 is CreateCheckConstraint
          select (s2 as CreateCheckConstraint).CheckConstraint.TableName).Distinct<string>().ToList<string>();
      foreach (string tableName in tableNames) {
        FileStream fs = new FileStream(FullConstraintsDir + "\\" + tableName + ".constraints.sql", FileMode.Create, FileAccess.Write);
        StreamWriter sw = new StreamWriter(fs);
        
        List<BaseStatement> tableStatements = 
          (from BaseStatement s in statements
           where s is CreatePrimaryKey && ((s as CreatePrimaryKey).Table.TableName == tableName)
           select s).Union<BaseStatement>(
            from BaseStatement s2 in statements
            where s2 is CreateCheckConstraint && ((s2 as CreateCheckConstraint).CheckConstraint.TableName == tableName)
            select s2).ToList<BaseStatement>();
        
        foreach(BaseStatement s in tableStatements){
          sw.WriteLine(s.Script);
          sw.WriteLine(OutputGen.ScriptTerminator);
          sw.WriteLine("");
        }        

        sw.Flush();
        sw.Close();
        fs.Close();      
      }            
    }

    private void WriteIndexes(string FullIndexesDir, List<BaseStatement> statements, IScriptOutputHandler OutputGen) {     
      List<string> tableNames =
        (from DBInfo.Core.Statement.BaseStatement s in statements
         where s is CreateIndex
         select (s as CreateIndex).Index.TableName).ToList<string>();
      foreach (string tableName in tableNames) {
        FileStream fs = new FileStream(FullIndexesDir + "\\" + tableName + ".fk.sql", FileMode.Create, FileAccess.Write);
        StreamWriter sw = new StreamWriter(fs);

        List<BaseStatement> tableStatements =
          (from BaseStatement s in statements
           where s is CreateIndex && ((s as CreateIndex).Index.TableName == tableName)
           select s).ToList<BaseStatement>();

        foreach (BaseStatement s in tableStatements) {
          sw.WriteLine(s.Script);
          sw.WriteLine(OutputGen.ScriptTerminator);
          sw.WriteLine("");
        }

        sw.Flush();
        sw.Close();
        fs.Close();
      }      
    }    
    
    private void WriteView(string FileName, View v, IScriptOutputHandler OutputGen){
      FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.Write);
      StreamWriter sw = new StreamWriter(fs);      
      sw.WriteLine(v.CreateViewScript);
      sw.WriteLine(OutputGen.ScriptTerminator);
      sw.Flush();
      sw.Close();
      fs.Close();
    }
    
    #endregion
  }
}
