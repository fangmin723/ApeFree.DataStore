﻿using ApeFree.DataStore.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace ApeFree.DataStore.Adapters
{
    /// <summary>
    /// GZip算法压缩适配器
    /// </summary>
    public class GZipCompressionAdapter : BaseCompressionAdapter
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public override Stream Compress(Stream stream)
        {
            MemoryStream compressStream = new MemoryStream();
            using (var zipStream = new GZipStream(compressStream, CompressionMode.Compress, true))
            {
                stream.CopyTo(zipStream);
            }
            return compressStream;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public override Stream Decompress(Stream stream)
        {
            return new GZipStream(stream, CompressionMode.Decompress);
        }

        public override void Dispose() { }
    }
}
