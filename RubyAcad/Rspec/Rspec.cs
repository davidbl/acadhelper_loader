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
    public class Rspec
    {
        [CommandMethod("RBSPEC")]
        public static void RubySpecUI()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            short fd = (short)Application.GetSystemVariable("FILEDIA");

            // Ask the user to select a .rb file
            PromptOpenFileOptions pfo =
                new PromptOpenFileOptions("Select Ruby script to load");

            pfo.Filter = "Ruby script (*.rb)|*.rb";
            PromptFileNameResult pr = ed.GetFileNameForOpen(pfo);

            // And then try to load and execute it
            if (pr.Status == PromptStatus.OK)
                runRspec(pr.StringResult);
        }
        private static bool runRspec(string file)
        {
            // If the file exists, let's load and execute it
            bool ret = System.IO.File.Exists(file);

            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            if (ret)
            {
                try
                {
                    AcadOutputStream io_stream = new AcadOutputStream();
                    var engine = Ruby.CreateEngine();

                    //redirect the stdout 
                    engine.Runtime.IO.SetOutput(io_stream, System.Text.Encoding.ASCII);
                    engine.Runtime.IO.SetErrorOutput(io_stream, System.Text.Encoding.ASCII);

                    List<string> search_paths = CommandsAndFunctions.read_search_paths();

                    engine.SetSearchPaths(search_paths);

                    //read the AutoCAD requires
                    List<string> acad_requires = CommandsAndFunctions.read_autocad_requires();
                    foreach (string acad_require in acad_requires)
                    {
                        engine.RequireRubyFile(acad_require);
                    }

                    string code = @"require 'rubygems';
                                    $0='c:/ironruby-0.9.0/lib/IronRuby/gems/1.8/bin/spec'
                                    ARGV << '" + file + @"' << '-c' << '-f' << 'nested' ;
                                    gem 'rspec', '>=0';  
                                    load 'spec'; ";
                    var scope = engine.CreateScope();
                    ed.WriteMessage("\n...Beginning Tests\n");

                    var exception_services = engine.GetService<ExceptionOperations>();

                    ScriptSource interactive_code = engine.CreateScriptSourceFromString(code, SourceCodeKind.Statements);
                    ScriptCodeParseResult props = interactive_code.GetCodeProperties();
                    if (props == ScriptCodeParseResult.Complete)
                        interactive_code.Execute(scope);
                }
                //catch the SyntaxError so we can show the file name and line number
                catch (Microsoft.Scripting.SyntaxErrorException se)
                {
                    ed.WriteMessage("\nProblem executing script: {0}:{1}\n::{2}", se.Message, se.Line, se.SourcePath);
                }
                catch (System.Exception ex)
                {
                    if (ex.Message == "exit")
                    {
                        //don't do anyting because, for some reason, rspec raise an Exit exception
                        // when it finishes cleanly
                        ed.WriteMessage("\n");
                        
                    }
                    else
                    {
                        ed.WriteMessage("\nThere was a problem executing script: {0}", ex.Message);
                    }
                }
            }
            return ret;

        }
    }
}
