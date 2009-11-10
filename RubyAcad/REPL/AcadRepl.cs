using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Internal;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IronRuby.Hosting;
using IronRuby;

using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

using msh = Microsoft.Scripting.Hosting;

namespace RubyAcad
{
    public class AcadRepl
    {
        /* Ruby IRB on the AutoCAD Command Line
        * Drawing inspiration from this post 
        * http://blog.tomasm.net/2009/04/15/python-says-hello-to-ruby/
        * by  Tomas Matousek, I was able to get an nice little IRB look-alike
        * running on the AutoCAD command line
        */
        [CommandMethod("RB_REPL")]
        public static void RubyRepl()
        {
            AcadOutputStream io_stream = new AcadOutputStream();
            var engine = Ruby.CreateEngine();
            engine.Runtime.IO.SetOutput(io_stream, System.Text.Encoding.ASCII);

            var scope = engine.CreateScope();
            var exception_services = engine.GetService<ExceptionOperations>();

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            PromptStringOptions opts = new PromptStringOptions(" ");
            opts.AllowSpaces = true;
            PromptResult res;
            string line = "";

            while (true)
            {
                try
                {
                    ed.WriteMessage("\nrb> ");
                    res = ed.GetString(opts);
                    if (res.Status == PromptStatus.OK)
                    {
                        line = res.StringResult;
                        line += "\n";
                    }
                    else break;
                    if (line == "\n") break;
                    if (line == "#exit\n") break;
                    ScriptSource source = read_code(line, engine, scope);
                    source.Execute(scope);
                }
                catch (System.Exception e)
                {
                    ed.WriteMessage("\nError: {0}", e.Message);
                }
            }
        }
        private static ScriptSource read_code(string line, ScriptEngine engine, ScriptScope scope)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            PromptStringOptions opts = new PromptStringOptions(" ");
            opts.AllowSpaces = true;
            PromptResult res;

            string code = line;
            while (true)
            {
                try
                {
                    ScriptSource interactive_code = engine.CreateScriptSourceFromString(code, SourceCodeKind.InteractiveCode);
                    ScriptCodeParseResult props = interactive_code.GetCodeProperties();
                    switch (props)
                    {
                        case ScriptCodeParseResult.Complete:
                            return interactive_code;
                        case ScriptCodeParseResult.Invalid:
                            return interactive_code;
                        default:
                            ed.WriteMessage("\nrb| ");
                            res = ed.GetString(opts);
                            if (res.Status == PromptStatus.OK)
                            {
                                code += res.StringResult;
                                code += "\n";
                            }
                            else
                            {
                                return interactive_code;
                            }
                            break;
                    }
                }
                catch (System.Exception e)
                {
                    ed.WriteMessage("\nError: {0}", e.Message);
                }
            }
        }

    }
}
