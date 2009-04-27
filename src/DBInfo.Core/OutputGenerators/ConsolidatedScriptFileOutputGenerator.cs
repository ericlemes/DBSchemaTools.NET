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
        WriteConstraints(FullConstraintsDir + "\\" + t.TableName + ".constraints.sql", t, OutputGenerator);
        WriteFKs(FullFKDir + "\\" + t.TableName + ".fk.sql", t, OutputGenerator);
        WriteIndexes(FullIndexesDir + "\\" + t.TableName + ".indexes.sql", t, OutputGenerator);
        foreach (Trigger tr in t.Triggers) {
          WriteTrigger(FullTriggersDir + "\\" + t.TableName + "." + tr.Name + ".trigger.sql", tr, OutputGenerator);
        }
      }

      foreach (Function f in db.Functions) {
        WriteFunction(FullFunctionsDir + "\\" + f.Name + ".function.sql", f, OutputGenerator);
      }

      foreach (Procedure p in db.Procedures) {
        WriteProcedure(FullProceduresDir + "\\" + p.Name + ".procedure.sql", p, OutputGenerator);
      }

      foreach (Sequence s in db.Sequences) {
        WriteScript(FullSequencesDir + "\\" + s.SequenceName + ".sequence.sql", s.SequenceScript);
      }

      foreach (View v in db.Views) {
        WriteView(FullViewsDir + "\\" + v.Name + ".view.sql", v, OutputGenerator);
      }
    }

    private void WriteFKs(string FileName, Table t, IScriptOutputGenerator OutputGen) {
      FileStream fs = new FileStream(FileName, FileMode.Create, FileAccess.Write);
      StreamWriter sw = new StreamWriter(fs);
      foreach (ForeignKey fk in t.ForeignKeys) {
        sw.WriteLine(fk.Script);
        sw.WriteLine(OutputGen.ScriptTerminator);
        sw.WriteLine("");
      }
      sw.Flush();
      sw.Close();
      fs.Close();
    }


    #endregion
  }
}
