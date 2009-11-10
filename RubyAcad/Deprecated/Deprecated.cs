using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Internal;

using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Runtime.InteropServices;

using IronRuby.Hosting;
using IronRuby;

using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

using msh = Microsoft.Scripting.Hosting;

namespace RubyAcad
{
    public class Deprecated
    {
        static Document doc = Application.DocumentManager.MdiActiveDocument;
        static Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

        [CommandMethod("-RBLOAD")]
        public static void RubyLoadCmdLine()
        {
            RubyLoad(true);
        }
        [CommandMethod("RBLOAD")]
        public static void RubyLoadUI()
        {
            RubyLoad(false);
        }
        public static void RubyLoad(bool useCmdLine)
        {
            short fd = (short)Application.GetSystemVariable("FILEDIA");

            // Ask the user to select a .rb file
            PromptOpenFileOptions pfo =
                new PromptOpenFileOptions("Select Ruby script to load");

            pfo.Filter = "Ruby script (*.rb)|*.rb";
            pfo.PreferCommandLine = (useCmdLine || fd == 0);
            PromptFileNameResult pr = ed.GetFileNameForOpen(pfo);

            // And then try to load and execute it
            if (pr.Status == PromptStatus.OK)
                ExecuteRubyScript(pr.StringResult);
        }
        private static bool ExecuteRubyScript(string file)
        {
            // If the file exists, let's load and execute it
            bool ret = System.IO.File.Exists(file);
            if (ret)
            {
                try
                {
                    ScriptEngine engine = Ruby.CreateEngine();
                    engine.ExecuteFile(file);
                }
                //catch the SyntaxError so we can show the file name and line number
                catch (Microsoft.Scripting.SyntaxErrorException se)
                {

                    ed.WriteMessage("\nProblem executing script: {0}, on line {1} of {2}", se.Message, se.Line, se.SourcePath);
                }
                catch (System.Exception ex)
                {
                    Document doc = Application.DocumentManager.MdiActiveDocument;
                    Editor ed = doc.Editor;
                    ed.WriteMessage("\nProblem executing script: {0}", ex.Source);
                }
            }
            return ret;
        }
    }
}
