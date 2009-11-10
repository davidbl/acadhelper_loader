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

/*
 Original code credit goes to Kean Walmsley
http://through-the-interface.typepad.com/through_the_interface/2009/04/using-ironruby-inside-autocad.html
AutoCAD version:AutoCAD Civil3D 2009


Autodesk references required for this project are acdbmgd.dll,acmgd.dll, acmddinternal.dll
IronRuby references required for this project are IronRuby.dll, IronRuby.Libraries.dll, IronRuby.Libraries.Yaml
 *                                                Microsoft.Scripting, Microsoft.Scripting.Core
 *                                                
 Changes to this version include addition of RBADD commands and functions that automatically register
 * desired Ruby commands with AutoCAD - See AddRubyCommands function below
 
 * .*/

namespace RubyAcad
{
    
    public class CommandsAndFunctions
    {
        static Document doc = Application.DocumentManager.MdiActiveDocument;
        static Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

        [CommandMethod("RBADD")]
        public static void RubyAddUI()
        {
            RubyAdd(false);
        }
        [CommandMethod("-RBADD")]
        public static void RubyAddCmdLine()
        {
            RubyAdd(true);
        }
        public static void RubyAdd(bool useCmdLine)
        {
            string file_to_load = GetRubyFile(useCmdLine);
            AddRubyCommands(file_to_load);
        }

        public static string GetRubyFile(bool useCmdLine)
        {
 
            string ret_val = string.Empty;
            short fd = (short)Application.GetSystemVariable("FILEDIA");

            // Ask the user to select a .rb file
            PromptOpenFileOptions pfo =
                new PromptOpenFileOptions("Select Ruby script to load");

            pfo.Filter = "Ruby script (*.rb)|*.rb";
            pfo.PreferCommandLine = (useCmdLine || fd == 0);
            PromptFileNameResult pr = ed.GetFileNameForOpen(pfo);

            // if they selected a file, return it
            if (pr.Status == PromptStatus.OK)
            {
                ret_val = pr.StringResult;
            }
            return ret_val;
        }

        [LispFunction("RBLOAD")]
        public ResultBuffer RubyLoadLISP(ResultBuffer rb)
        {
            const int RTSTR = 5005;
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            if (rb == null)
            {
                ed.WriteMessage("\nError: too few arguments\n");
            }
            else
            {
                // We're only really interested in the first argument
                System.Array args = rb.AsArray();
                TypedValue tv = (TypedValue)args.GetValue(0);
                // Which should be the filename of our script
                if (tv != null && tv.TypeCode == RTSTR)
                {
                    // If we manage to execute it, let's return the
                    // filename as the result of the function
                    // (just as (arxload) does)

                    bool success = AddRubyCommands(System.Convert.ToString(tv.Value));
                    return (success ?
                               new ResultBuffer(new TypedValue(RTSTR, tv.Value))
                              : null);
                }
            }
            return null;
        }
  
        private static bool AddRubyCommands(string file)
        {
            // If the file exists, let's load and execute it
            bool ret = System.IO.File.Exists(file);
            if (ret)
            {
                try
                {
                    AcadOutputStream io_stream = new AcadOutputStream();
                   
                   //redirect the stdout so we can output info during loading
                    ScriptEngine engine = Ruby.CreateEngine();
                    engine.Runtime.IO.SetOutput(io_stream, System.Text.Encoding.ASCII);
                    engine.Runtime.IO.SetErrorOutput(io_stream, System.Text.Encoding.ASCII);

                    ScriptScope scope = engine.CreateScope();

                    //read the searh paths
                    engine.SetSearchPaths(read_search_paths());

                    //read the AutoCAD requires
                    List<string> acad_requires = read_autocad_requires();
                    foreach (string acad_require in acad_requires)
                    {
                        engine.RequireRubyFile(acad_require);
                    }
                    
                    //read the entire file - syntax errors will be caught here and
                    //SyntaxErrorException raised
                     engine.ExecuteFile(file,scope);

                    //execute the Ruby code to register the desired commands with AutoCAD
                    var code = @"Ai =  Autodesk::AutoCAD::Internal; Aiu = Autodesk::AutoCAD::Internal::Utils; 
                                  added = []; @exclude ||= []; 
                                  new_commands = private_methods(false) - %w(initialize method_missing) - @exclude; 
                                  new_commands.reject!{|y|  method(y).arity != 0}
                                  new_commands.each{ |x| Aiu.RemoveCommand('rbcmds', x); 
                                        cc= Ai::CommandCallback.new(method(x)); 
                                        Aiu.AddCommand('rbcmds', x, x,  Ar::CommandFlags.Modal, cc);
                                        added << x}
                                  puts added.inspect";
                    var new_result = engine.Execute(code, scope);
                 }
                //catch the SyntaxError so we can show the file name and line number
                catch (Microsoft.Scripting.SyntaxErrorException se)
                {
                    ed.WriteMessage("\nProblem executing script: {0}, on line {1} of {2}", se.Message, se.Line, se.SourcePath);
                }
                catch (System.Exception ex)
                {
                    ed.WriteMessage("\nProblem executing script: {0}", ex.Message);
                }
            }
            return ret;
        }
        public static List<string> read_autocad_requires()
        {
            List<string> acad_requires = new List<string>();
            string home_path = RubyAcadSettings.Default.AcadHome;
            string[] require_names = RubyAcadSettings.Default.AcadRequires.Split(';');
            for (int i = 0; i < require_names.Length; i++)
            {
                string curr_path = home_path + require_names[i];
                acad_requires.Add(curr_path);
            }
            return acad_requires;
        }
        public static List<string> read_search_paths()
        {
            List<string> search_paths = new List<string>();
            string home_path = RubyAcadSettings.Default.IronRubyHome;
            string[] base_paths = RubyAcadSettings.Default.IronRubyPaths.Split(';');
            for (int i = 0; i < base_paths.Length; i++)
            {
                string curr_path = home_path + base_paths[i];
                search_paths.Add(curr_path);
            }
            return search_paths;
        }

    }
}
