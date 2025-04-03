using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Simple_Bible_Reader
{

    // SCOP Stream Cipher was converted from C to C# from the following source code
    // https://www.geocities.ws/smaltchev/scopdemo.c


    class SCOP
    {
        const string key = Passwords.theWord_Password;
        
        struct st_key {
          public uint[] v;
          public byte i;
          public byte j;
          public byte t3;
        };

        struct st_gp8 {
            public byte[,] coef;
            public uint[] x;
        };

        st_key kt;
        st_gp8 int_state;

        public SCOP()
        {
            kt.v = new uint[384];
            int_state.coef = new byte[8, 4];
            int_state.x = new uint[4];
        }

        public void expand_key (ref byte[] bytes_in,int in_size)
        {
          byte[] p=getBytes(int_state);

          for (int i = 0; i < in_size; i++)
                p[i] = bytes_in[i];

          for (int i = in_size; i < 48; i++)
              p[i] = (byte)(p[i - in_size] + p[i - in_size + 1]);

          int counter = 1;
          for (int i = 0; i < 32; i++)
            {
              if (p[i] == 0)
                p[i] = (byte)counter++;
            }
          int_state = fromBytes(p);
        }

        byte[] getBytes(st_gp8 str)
        {
            int size = 48;
            byte[] arr = new byte[size];
            int c=0;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 4; j++)
                    arr[c++] = str.coef[i, j];
            byte[] bytes=ConvertInt32ToBytes(str.x);
            foreach (byte b in bytes)
                arr[c++] = b;
            return arr;
        }

        st_gp8 fromBytes(byte[] arr)
        {
            st_gp8 str = new st_gp8();
            str.coef = new byte[8, 4];
            str.x = new uint[4];
            int c = 0;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 4; j++)
                    str.coef[i, j]=arr[c++];
            byte[] bytes=new byte[16];
            for(int i=32;i<48;i++)
                bytes[i-32]=arr[c++];
            str.x=ConvertBytesToInt32(bytes);
            return str;
        }

        public void  init_key (byte[] in_bytes, int in_size)
        {
          uint odd;
          uint[] t=new uint[4];
          int i, j;

          expand_key(ref in_bytes, in_size);

          for (i = 0; i < 8; i++)
            gp8 (ref t,0);

          for (i = 0; i < 12; i++)
            {
                for (j = 0; j < 8; j++)
                    gp8 (ref kt.v, i * 32 + j * 4);
                gp8(ref t, 0);
            }

          gp8 (ref t,0);
          kt.i  = (byte) (t[3] >> 24);
          kt.j  = (byte) (t[3] >> 16);
          kt.t3 = (byte) (t[3] >> 8);          
          odd = t[3] & 0x7f;
          kt.v[odd] |= 1;
        }
      
        public void gp8 (ref uint[] out_bytes, int offset)
        {
          uint y1, y2, x_1, x_2, x_3, x_4;
          uint[] newx=new uint[4];
          int i, i2;

          for (i = 0; i < 8; i += 2)
            {
              i2 = i >> 1;

              x_1 = int_state.x[i2] >> 16;
              x_2 = x_1 * x_1;
              x_3 = x_2 * x_1;
              x_4 = x_3 * x_1;

              y1 = int_state.coef[i,0] * x_4 +
                   int_state.coef[i,1] * x_3 +
                   int_state.coef[i,2] * x_2 +
                   int_state.coef[i,3] * x_1 + 1;

              x_1 = int_state.x[i2] & 0xffff;
              x_2 = x_1 * x_1;
              x_3 = x_2 * x_1;
              x_4 = x_3 * x_1;

              y2 = int_state.coef[i + 1,0] * x_4 +
                   int_state.coef[i + 1,1] * x_3 +
                   int_state.coef[i + 1,2] * x_2 +
                   int_state.coef[i + 1,3] * x_1 + 1;

              out_bytes[i2 + offset] = (y1 << 16) | (y2 & 0xffff);
              newx[i2] = (y1 & 0xffff0000) | (y2 >> 16);
            }

          int_state.x[0] = (newx[0] >> 16) | (newx[3] << 16);
          int_state.x[1] = (newx[0] << 16) | (newx[1] >> 16);
          int_state.x[2] = (newx[1] << 16) | (newx[2] >> 16);
          int_state.x[3] = (newx[2] << 16) | (newx[3] >> 16);
        }

        public void decrypt(ref uint[] buf, int buflen)
        {
            byte i, j;
            uint t1, t2, t3;
            uint k, t;
            uint[] v;

            i = kt.i;
            j = kt.j;
            t3 = kt.t3;
            v = kt.v;

            //Console.WriteLine("i = " + String.Format("{0:X2}", i) + ", j = " + String.Format("{0:X2}", j) + ", t3 = " + String.Format("{0:X2}", t3));

            int idx = 0;
            while (idx < buflen)
            {
                t1 = v[128 + j];
                j += (byte)t3;
                t = v[i];
                t2 = v[128 + j];

                i++;

                t3 = t2 + t;
                v[128 + j] = t3;
                j += (byte)t2;
                k = t1 + t2;

                buf[idx++] -= k;
            }
        }

        public void encrypt(ref uint[] buf, int buflen)
        {
            byte i, j;
            uint t1, t2, t3;
            uint k, t;
            uint[] v;

            i = kt.i;
            j = kt.j;
            t3 = kt.t3;
            v = kt.v;

            //Console.WriteLine("i = " + String.Format("{0:X2}", i) + ", j = " + String.Format("{0:X2}", j) + ", t3 = " + String.Format("{0:X2}", t3));

            int idx = 0;
            while (idx < buflen)
            {
                t1 = v[128 + j];
                j += (byte)t3;
                t = v[i];
                t2 = v[128 + j];

                i++;

                t3 = t2 + t;
                v[128 + j] = t3;
                j += (byte)t2;
                k = t1 + t2;

                buf[idx++] += k;
            }
        }

        private static uint[] ConvertBytesToInt32(byte[] buffer_bytes)
        {
            uint[] intvals = new uint[buffer_bytes.Length / 4];
            using (MemoryStream memory = new MemoryStream(buffer_bytes))
            {
                using (BinaryReader reader = new BinaryReader(memory))
                {
                    for (int i = 0; i < intvals.Length; i++)
                    {
                        intvals[i] = reader.ReadUInt32();
                    }
                }
            }
            return intvals;
        }

        private static byte[] ConvertInt32ToBytes(uint[] buffer_uint)
        {
            byte[] byte_vals = new byte[buffer_uint.Length * 4];
            using (MemoryStream memory = new MemoryStream(byte_vals))
            {
                using (BinaryWriter writer = new BinaryWriter(memory))
                {
                    for (int i = 0; i < buffer_uint.Length; i++)
                    {
                        writer.Write(buffer_uint[i]);
                    }
                }
            }
            return byte_vals;
        }  

        /*
        static void Main(string[] args)
        {
              byte[] key = {  };
              byte[] buffer_bytes = { 0x6A, 0x66, 0x31, 0x13, 0xD8, 0x81, 0x31, 0x5F, 0xF4, 0x7B, 0x1A, 0xF0, 0x24, 0x57, 0x48, 0x47 };
                
            
              SCOP p = new SCOP();
              p.init_key(key, key.Length);
              uint[] ilong = ConvertBytesToInt32(buffer_bytes);
              p.crypt(ref ilong, ilong.Length);
              byte[] bytes_out = ConvertInt32ToBytes(ilong);
              Console.Write("Program Output  :");
              foreach (byte b in bytes_out)
              {
                    Console.Write(b.ToString("X") + " ");
              }             
              Console.ReadKey();
        }
        */

        public static byte[] decryptSCOPBytes(byte[] buffer_bytes)
        {
            return decryptSCOPBytes(buffer_bytes, 0);
        }

        public static byte[] decryptSCOPBytes(byte[] buffer_bytes,byte xorKey)
        {
            SCOP scop = new SCOP();
            uint[] buffer = ConvertBytesToInt32(buffer_bytes);
            byte[] newKey = Convert.FromBase64String(SCOP.key);
            for (int i = 0; i < newKey.Length; i++)
                newKey[i] = (byte)(newKey[i] ^ xorKey);
            scop.init_key(newKey, newKey.Length);
            uint[] ilong = ConvertBytesToInt32(buffer_bytes);
            scop.decrypt(ref ilong, ilong.Length);
            byte[] bytes_out = ConvertInt32ToBytes(ilong);
            return bytes_out;
        }  
    }
}
