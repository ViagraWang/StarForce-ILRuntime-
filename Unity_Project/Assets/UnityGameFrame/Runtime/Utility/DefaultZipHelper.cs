using GameFramework;
using ICSharpCode.SharpZipLib.GZip;
using System;
using System.IO;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 默认压缩解压缩辅助器
    /// </summary>
    public sealed class DefaultZipHelper : Utility.Zip.IZipHelper
    {
        private readonly byte[] m_BytesCache = new byte[0x10000];   //十六进行表示，4096字节缓存

        /// <summary>
        /// 压缩数据
        /// </summary>
        /// <param name="bytes">要压缩的二进制流数据</param>
        /// <param name="offset">要压缩的数据的二进制流的偏移</param>
        /// <param name="length">要压缩的数据的二进制流的长度</param>
        /// <param name="compressedStream">压缩后的数据的二进制流</param>
        /// <returns>是否压缩数据成功</returns>
        public bool Compress(byte[] bytes, int offset, int length, Stream compressedStream)
        {
            if (bytes == null || offset < 0 || length > bytes.Length || compressedStream == null)
            {
                Debug.LogError("压缩数据失败......");
                return false;
            }

            try
            {
                //压缩到内存流中
                using(GZipOutputStream gZipOutputStream = new GZipOutputStream(compressedStream))
                {
                    gZipOutputStream.Write(bytes, offset, length);
                    if (compressedStream.Length >= 8L)
                    {
                        //强制转换5-8字节？？？
                        long current = compressedStream.Position;
                        compressedStream.Position = 4L;
                        compressedStream.WriteByte(25);
                        compressedStream.WriteByte(134);
                        compressedStream.WriteByte(2);
                        compressedStream.WriteByte(32);
                        compressedStream.Position = current;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("压缩数据异常 -> " + e.ToString());
                return false;
            }
        }

        /// <summary>
        /// 解压缩数据
        /// </summary>
        /// <param name="bytes">要解压缩的数据的二进制流</param>
        /// <param name="offset">要解压缩的数据的二进制流的偏移</param>
        /// <param name="length">要解压缩的数据的二进制流的长度</param>
        /// <param name="decompressedStream">解压缩后的数据的二进制流</param>
        /// <returns>是否解压缩数据成功</returns>
        public bool Decompress(byte[] bytes, int offset, int length, Stream decompressedStream)
        {
            if (bytes == null || offset < 0 || length > bytes.Length || decompressedStream == null)
                return false;

            MemoryStream memoryStream = null;   //内存流
            try
            {
                memoryStream = new MemoryStream(bytes, offset, length, false);
                //从内存流中解压缩
                using(GZipInputStream gZipInputStream = new GZipInputStream(memoryStream))
                {
                    int bytesRead = 0;  //记录读取数量
                    while((bytesRead = gZipInputStream.Read(m_BytesCache, 0, m_BytesCache.Length)) > 0)    //循环读取
                    {
                        decompressedStream.Write(m_BytesCache, 0, bytesRead);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (memoryStream != null)
                {
                    memoryStream.Dispose();
                    memoryStream = null;
                }
            }

        }
    }
}
