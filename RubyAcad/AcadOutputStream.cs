using System;
using System.IO;
using System.Text;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Internal;

namespace RubyAcad
{
    class AcadOutputStream : Stream
    {
        Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
        public override void Write(byte[] buffer, int offset, int count)
        {
            string out_message = System.Text.ASCIIEncoding.ASCII.GetString(buffer, offset, count);
            ed.WriteMessage(out_message);
        }
        
        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            
        }

        public override long Length
        {
            get { throw new NotImplementedException("Length Unavailable"); } 
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException("Position Unavailable");
            }
            set
            {
                throw new NotImplementedException("Position Unavailable");
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException("Read Unavailable");
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException("Seek Unavailable");
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException("SetLength Unavailable");
        }
  
       
    }

}
