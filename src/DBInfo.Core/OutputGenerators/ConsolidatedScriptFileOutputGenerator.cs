using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBInfo.Core.Model;
using System.IO;
using DBInfo.Core.Statement;

namespace DBInfo.Core.OutputGenerators {
  public class ConsolidatedScriptFileOutputGenerator : IScriptFileOutputGenerator {
    #region IScriptFileOutputGenerator Members
    
    private string _ScriptName = "database_script.sql";
    public string ScriptName{
      get { return _ScriptName;}
      set { _ScriptName = value;}
    }

    public void GenerateFileOutput(string OutputDir, List<BaseStatement> statements, IScriptOutputHandler OutputGenerator){
      FileStream fs = new FileStream(OutputDir + "\\" + ScriptName, FileMode.Create, FileAccess.Write);
      StreamWriter sw = new StreamWriter(fs);
      foreach(BaseStatement s in statements){
        sw.WriteLine(s.Script);
        sw.WriteLine(OutputGenerator.ScriptTerminator);              
        sw.WriteLine("");
      }      
      sw.Flush();
      sw.Close();
      fs.Close();
    }

    private void WriteConstraints(StreamWriter sw, Table t, IScriptOutputHandler OutputGen) {
      sw.WriteLine(t.PrimaryKeyScript);
      sw.WriteLine(OutputGen.ScriptTerminator);
      sw.WriteLine("");
      foreach (CheckConstraint cc in t.CheckConstraints) {
        sw.WriteLine(cc.Script);
        sw.WriteLine(OutputGen.ScriptTerminator);
        sw.WriteLine("");
      }      
    }    

    private void WriteFKs(StreamWriter sw, Table t, IScriptOutputHandler OutputGen) {
      foreach (ForeignKey fk in t.ForeignKeys) {
        sw.WriteLine(fk.Script);
        sw.WriteLine(OutputGen.ScriptTerminator);
        sw.WriteLine("");
      }   
    }

    private void WriteIndexes(StreamWriter sw, Table t, IScriptOutputHandler OutputGen) {
      foreach (Index i in t.Indexes) {
        sw.WriteLine(i.Script);
        sw.WriteLine(OutputGen.ScriptTerminator);
        sw.WriteLine("");
      }
    }

    private void WriteFunction(StreamWriter sw, Function f, IScriptOutputHandler OutputGen) {
      sw.WriteLine(f.CreateFunctionScript);
      sw.WriteLine(OutputGen.ScriptTerminator);
      sw.WriteLine("");      
    }

    private void WriteProcedure(StreamWriter sw, Procedure p, IScriptOutputHandler OutputGen) {
      sw.WriteLine(p.CreateProcedureScript);
      sw.WriteLine(OutputGen.ScriptTerminator);
      sw.WriteLine("");
    }

    private void WriteView(StreamWriter sw, View v, IScriptOutputHandler OutputGen) {
      sw.WriteLine(v.CreateViewScript);
      sw.WriteLine(OutputGen.ScriptTerminator);      
    }

    private void WriteTrigger(StreamWriter sw, Trigger t, IScriptOutputHandler OutputGen) {
      sw.WriteLine(t.CreateTriggerScript);
      sw.WriteLine(OutputGen.ScriptTerminator);      
    }
    

    #endregion
  }
}
