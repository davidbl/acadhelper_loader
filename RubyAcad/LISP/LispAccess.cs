using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace RubyAcad
{
    class Lisp
    {
        //This code is taken from a post here http://discussion.autodesk.com/forums/thread.jspa?threadID=522825
        //by  Alexander Rivilis - Thanks Alexander

        //NOTE:  The path below will need to be edited before compiling
        [SuppressUnmanagedCodeSecurity]
        [DllImport(@"C:\Program Files (x86)\AutoCAD Civil 3D 2009\acad.exe", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode,
               EntryPoint = "?acedEvaluateLisp@@YAHPB_WAAPAUresbuf@@@Z")]
        extern private static int acedEvaluateLisp(string lispString, out IntPtr result);

        static public Object evalLisp(string lispString)
        {
            IntPtr out_data = IntPtr.Zero;
            acedEvaluateLisp(lispString, out out_data);
            if (out_data != IntPtr.Zero)
            {
                try
                {
                    ResultBuffer ret_data = DisposableWrapper.Create(typeof(ResultBuffer), out_data, true) as ResultBuffer;
                    return ret_data;
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

    }
}
