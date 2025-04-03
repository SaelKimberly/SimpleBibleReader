using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HBB
{
    public class LightningHeader
    {
        public ushort usMaxRecords;
        public uint uiISBN;
        public uint uiSubSectionPointerOffset;
        public uint[] auiSubSectionPointer = new uint[20];
        public byte[] baBitHeader = new byte[8];
        public string[] saBasicInfo = new string[4];
    }
}
