using System;
using System.Collections.Generic;
using System.IO;
using System.Text;



namespace Simple_Bible_Reader
{
    public class LZSS
    {
        public struct LZSS_UNPACK_DATA
        {
            public long src_total;
            public int src_offset;
            public int i;
            public int j;
            public int k;
            public int r;
            public int c;
            public int exit_point;	// what step to continue unpacking
            public uint flags;
            public byte[] text_buf;	// ring buffer of size N, with extra F-1 bytes to aid string comparison
        }

        private const int LZSS_N = 4096;		// 4k buffers for LZ compression 
        private const int LZSS_F = 18;			// upper limit for LZ match length 
        private const int LZSS_THRESHOLD = 2;	// LZ encode string into pos and length

        public LZSS_UNPACK_DATA unpack;

        public LZSS()
        {
            unpack.text_buf = new byte[LZSS_N + LZSS_F - 1];
            unpack.exit_point = 0;
        }

        private void decompress_init()
        {
            unpack.exit_point = 0;
            unpack.src_total = 0;
            unpack.src_offset = 0;

            for (int i = 0; i < LZSS_N - LZSS_F; i++)
                unpack.text_buf[i] = 0;

            unpack.r = LZSS_N - LZSS_F;
            unpack.flags = 0;
        }

        public long decompress(byte[] dst, byte[] src, long size)
        {
            int i, j, k, r, c, src_size, dst_size, src_offset, dst_offset, k_start;
            uint flags;

            if (unpack.src_total == 0)
                decompress_init();

            i = unpack.i;
            j = unpack.j;
            r = unpack.r;
            c = unpack.c;
            k = k_start = unpack.k;
            flags = unpack.flags;
            src_offset = unpack.src_offset;

            src_size = src.Length;
            dst_size = dst.Length;
            dst_offset = 0;

            for (; ; )
            {
                if (unpack.exit_point == 0 || unpack.exit_point == 1)
                {
                    if ((((flags >>= 1) & 256) == 0) || unpack.exit_point == 1)
                    {
                        if (unpack.exit_point == 0)
                        {
                            //is file totally readed?
                            if (unpack.src_total++ >= size)
                                break;

                            //is src must be refreshed?
                            if (src_offset >= src_size)
                            {
                                unpack.exit_point = 1;
                                goto BUFFER_FULL;
                            }
                        }

                        unpack.exit_point = 0;
                        c = src[src_offset++];
                        flags = (uint)(c | 0xff00);	 // uses higher byte cleverly to count eight
                    }
                }

                if (((flags & 1) != 0) || (unpack.exit_point == 2 || unpack.exit_point == 3))
                {
                    if (unpack.exit_point == 0)
                    {
                        //is file totally readed?
                        if (unpack.src_total++ >= size)
                            break;
                    }

                    //is src must be refreshed?
                    if (src_offset >= src_size)
                    {
                        unpack.exit_point = 2;
                        goto BUFFER_FULL;
                    }

                    if (unpack.exit_point == 0 || unpack.exit_point == 2)
                        c = src[src_offset++];

                    //is dst must be flushed?
                    if (dst_offset >= dst_size)
                    {
                        unpack.exit_point = 3;
                        goto BUFFER_FULL;
                    }

                    unpack.exit_point = 0;
                    dst[dst_offset++] = (byte)c;
                    unpack.text_buf[r++] = (byte)c;
                    r &= (LZSS_N - 1);
                }
                else
                {
                    if (unpack.exit_point == 0)
                    {
                        //is file totally readed?
                        if (unpack.src_total++ >= size)
                            break;
                    }

                    //is src must be refreshed?
                    if (src_offset >= src_size)
                    {
                        unpack.exit_point = 4;
                        goto BUFFER_FULL;
                    }

                    if (unpack.exit_point == 0 || unpack.exit_point == 4)
                    {
                        i = src[src_offset++];

                        //is file totally readed?
                        if (unpack.src_total++ >= size)
                            break;
                    }

                    //is src must be refreshed?
                    if (src_offset >= src_size)
                    {
                        unpack.exit_point = 5;
                        goto BUFFER_FULL;
                    }

                    if (unpack.exit_point != 6)
                    {
                        unpack.exit_point = 0;
                        j = src[src_offset++];
                        i |= ((j & 0xf0) << 4);
                        j = (j & 0x0f) + LZSS_THRESHOLD;
                        k_start = 0;
                    }

                    for (k = k_start; k <= j; k++)
                    {
                        if (unpack.exit_point == 0)
                        {
                            c = unpack.text_buf[(i + k) & (LZSS_N - 1)];

                            //is dst must be flushed?
                            if (dst_offset >= dst_size)
                            {
                                unpack.exit_point = 6;
                                goto BUFFER_FULL;
                            }
                        }

                        unpack.exit_point = 0;
                        dst[dst_offset++] = (byte)c;
                        unpack.text_buf[r++] = (byte)c;
                        r &= (LZSS_N - 1);
                    }
                }
            }

            unpack.exit_point = 0;

        BUFFER_FULL:
            unpack.i = i;
            unpack.j = j;
            unpack.k = k;
            unpack.r = r;
            unpack.c = c;
            unpack.flags = flags;
            unpack.src_offset = src_offset;

            return dst_offset;
        }
    }
}

