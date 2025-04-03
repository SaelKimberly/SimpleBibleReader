using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace PalmBibleExport
{
    class MyBinaryWriter : BinaryWriter
    {
        public MyBinaryWriter(Stream s)
            : base(s)
        {
        }


        public override void Write(short integer)
        {
            base.Write(IPAddress.NetworkToHostOrder(integer));
        }

        public override void Write(int integer)
        {
            base.Write(IPAddress.NetworkToHostOrder(integer));
        }
    }
}
