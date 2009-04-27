using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Model;
using System.IO;

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

    public void GenerateFileOutput(string OutputDir, Database db, IScriptOutputGenerator OutputGenerator) {
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
      
      foreach(Table t in db.Tables){
        WriteScript(FullTableDir + "\\" + t.TableName + ".table.sql", t.TableScript);
        WriteConstraints(FullConstraintsDir + "\\" + t.TableName + ".constraints.sql", t, OutputGenerator);
        WriteFKs(FullFKDir + "\\" + t.TableName + ".fk.sql", t, OutputGenerator);
        WriteIndexes(FullIndexesDir + "\\" + t.TableName + ".indexes.sql", t, OutputGenerator);
        foreach(Trigger tr in t.Triggers){
          WriteTrigger(FullTriggersDir + "\\" + t.TableName + "." + tr.Name + ".trigger.sql", tr, OutputGenerator);
        }
      }
      
      foreach(Function f in db.Functions){
        WriteFunction(FullFunctionsDir + "\\" + f.Name + ".function.sql", f, OutputGenerator);
      }
      
      foreach(Procedure p in db.Procedures){
        WriteProcedure(FullProceduresDir + "\\" + p.Name + ".procedure.sql", p, OutputGenerator);
      }
      
      foreach(Sequence s in db.Sequences){
        WriteScript(FullSequencesDir + "\\" + s.SequenceName + ".sequence.sql", s.SequenceScript);
      }
      
      foreach(View v in db.Views){
        WriteView(FullViewsDir + "\\" + v.Name + ".view.sql", v, OutputGenerator);
      }
    }
    
    private void WriteFKs(string FileName, Table t, IScriptOutputGenerator OutputGen){
      FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.Write);
      StreamWriter sw = new StreamWriter(fs);
      foreach(ForeignKey fk in t.ForeignKeys){
        sw.WriteLine(fk.Script);
        sw.WriteLine(OutputGen.ScriptTerminator);
        sw.WriteLine("");
      }
      sw.Flush();
      sw.Close();
      fs.Close();
    }
    
    private void WriteConstraints(string FileName, Table t, IScriptOutputGenerator OutputGen){
      FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.Write);
      StreamWriter sw = new StreamWriter(fs);
      sw.WriteLine(t.PrimaryKeyScript);
      sw.WriteLine(OutputGen.ScriptTerminator);
      sw.WriteLine("");
      foreach(CheckConstraint cc in t.CheckConstraints){
        sw.WriteLine(cc.Script);
        sw.WriteLine(OutputGen.ScriptTerminator);
        sw.WriteLine("");
      }
      sw.Flush();
      sw.Close();
      fs.Close();      
    }
    
    private void WriteFunction(string FileName, Function f, IScriptOutputGenerator OutputGen){
      FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.Write);
      StreamWriter sw = new StreamWriter(fs);     
      sw.WriteLine(f.CreateFunctionScript);
      sw.WriteLine(OutputGen.ScriptTerminator);
      sw.WriteLine("");      
      sw.Flush();
      sw.Close();
      fs.Close();
    }
    
    private void WriteProcedure(string FileName, Procedure p, IScriptOutputGenerator OutputGen){
      FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.Write);
      StreamWriter sw = new StreamWriter(fs);
      sw.WriteLine(p.CreateProcedureScript);
      sw.WriteLine(OutputGen.ScriptTerminator);
      sw.WriteLine("");
      sw.Flush();
      sw.Close();
      fs.Close();
    }
    
    private void WriteIndexes(string FileName, Table t, IScriptOutputGenerator OutputGen){
      FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.Write);
      StreamWriter sw = new StreamWriter(fs);
      foreach (Index i in t.Indexes){
        sw.WriteLine(i.Script);
        sw.WriteLine(OutputGen.ScriptTerminator);
        sw.WriteLine("");
      }
      sw.Flush();
      sw.Close();
      fs.Close();      
    }
    
    private void WriteTrigger(string FileName, Trigger t, IScriptOutputGenerator OutputGen){
      FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.Write);
      StreamWriter sw = new StreamWriter(fs);
      sw.WriteLine(t.CreateTriggerScript);
      sw.WriteLine(OutputGen.ScriptTerminator);
      sw.Flush();
      sw.Close();
      fs.Close();
    }
    
    private void WriteView(string FileName, View v, IScriptOutputGenerator OutputGen){
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
