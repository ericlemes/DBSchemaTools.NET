using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Model;
using System.IO;

namespace DBInfo.Core.OutputGenerators {
  public class ConsolidatedScriptFileOutputGenerator : IScriptFileOutputGenerator {
    #region IScriptFileOutputGenerator Members
    
    private string _ScriptName = "database_script.sql";
    public string ScriptName{
      get { return _ScriptName;}
      set { _ScriptName = value;}
    }

    public void GenerateFileOutput(string OutputDir, Database db, IScriptOutputGenerator OutputGenerator) {
      FileStream fs = new FileStream(OutputDir + "\\" + ScriptName, FileMode.Create, FileAccess.Write);
      StreamWriter sw = new StreamWriter(fs);
      foreach (Table t in db.Tables) {
        sw.WriteLine(t.TableScript);
        sw.WriteLine(OutputGenerator.ScriptTerminator);        
        WriteConstraints(sw, t, OutputGenerator);
        WriteIndexes(sw, t, OutputGenerator);
      }
      
      foreach(Table t in db.Tables){
        WriteFKs(sw, t, OutputGenerator);        
      }

      foreach (Function f in db.Functions) {
        WriteFunction(sw, f, OutputGenerator);
      }

      foreach (Procedure p in db.Procedures) {
        WriteProcedure(sw, p, OutputGenerator);
      }

      foreach (Sequence s in db.Sequences) {
        sw.WriteLine(s.SequenceScript);
        sw.WriteLine(OutputGenerator.ScriptTerminator);        
      }

      foreach (View v in db.Views) {
        WriteView(sw, v, OutputGenerator);
      }

      foreach (Table t in db.Tables){
        foreach (Trigger tr in t.Triggers) {
          WriteTrigger(sw, tr, OutputGenerator);
        }      
      }
      sw.Flush();
      sw.Close();
      fs.Close();
    }

    private void WriteConstraints(StreamWriter sw, Table t, IScriptOutputGenerator OutputGen) {
      sw.WriteLine(t.PrimaryKeyScript);
      sw.WriteLine(OutputGen.ScriptTerminator);
      sw.WriteLine("");
      foreach (CheckConstraint cc in t.CheckConstraints) {
        sw.WriteLine(cc.Script);
        sw.WriteLine(OutputGen.ScriptTerminator);
        sw.WriteLine("");
      }      
    }    

    private void WriteFKs(StreamWriter sw, Table t, IScriptOutputGenerator OutputGen) {
      foreach (ForeignKey fk in t.ForeignKeys) {
        sw.WriteLine(fk.Script);
        sw.WriteLine(OutputGen.ScriptTerminator);
        sw.WriteLine("");
      }   
    }

    private void WriteIndexes(StreamWriter sw, Table t, IScriptOutputGenerator OutputGen) {
      foreach (Index i in t.Indexes) {
        sw.WriteLine(i.Script);
        sw.WriteLine(OutputGen.ScriptTerminator);
        sw.WriteLine("");
      }
    }

    private void WriteFunction(StreamWriter sw, Function f, IScriptOutputGenerator OutputGen) {
      sw.WriteLine(f.CreateFunctionScript);
      sw.WriteLine(OutputGen.ScriptTerminator);
      sw.WriteLine("");      
    }

    private void WriteProcedure(StreamWriter sw, Procedure p, IScriptOutputGenerator OutputGen) {
      sw.WriteLine(p.CreateProcedureScript);
      sw.WriteLine(OutputGen.ScriptTerminator);
      sw.WriteLine("");
    }

    private void WriteView(StreamWriter sw, View v, IScriptOutputGenerator OutputGen) {
      sw.WriteLine(v.CreateViewScript);
      sw.WriteLine(OutputGen.ScriptTerminator);      
    }

    private void WriteTrigger(StreamWriter sw, Trigger t, IScriptOutputGenerator OutputGen) {
      sw.WriteLine(t.CreateTriggerScript);
      sw.WriteLine(OutputGen.ScriptTerminator);      
    }
    

    #endregion
  }
}
