using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace PalmBiblePlusCSharp.bibleplus
{
    class MyBinaryReader : BinaryReader
    {
        public MyBinaryReader(Stream s)
            : base(s)
        {
        }

        public override short ReadInt16()
        {
            return IPAddress.HostToNetworkOrder(base.ReadInt16());
        }

        public override int ReadInt32()
        {
            return IPAddress.HostToNetworkOrder(base.ReadInt32());
        }
    }
}
