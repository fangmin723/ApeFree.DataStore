﻿using ApeFree.DataStore.Adapters;
using System;
using System.IO;

namespace ApeFree.DataStore.Core
{
    public abstract class AccessSettings : IAccessSettings
    {
        public ISerializationAdapter SerializationAdapter { get; set; }
            = DefaultSerializationAdapter.Default.Value;

        public ICompressionAdapter CompressionAdapter { get; set; }
            = DefaultCompressionAdapter.Default.Value;

        public IEncryptionAdapter EncryptionAdapter { get; set; }
            = DefaultEncryptionAdapter.Default.Value;

        public void Dispose()
        {
            SerializationAdapter.Dispose();
            CompressionAdapter.Dispose();
            EncryptionAdapter.Dispose();
        }

        internal class DefaultSerializationAdapter : JsonSerializationAdapter
        {
            internal static Lazy<DefaultSerializationAdapter> Default = new Lazy<DefaultSerializationAdapter>();
        }

        internal class DefaultEncryptionAdapter : IEncryptionAdapter
        {
            internal static Lazy<DefaultEncryptionAdapter> Default = new Lazy<DefaultEncryptionAdapter>();
            public Stream Decode(Stream stream) => stream;
            public Stream Encode(Stream stream) => stream;
            public void Dispose() { }
        }

        internal class DefaultCompressionAdapter : ICompressionAdapter
        {
            internal static Lazy<DefaultCompressionAdapter> Default = new Lazy<DefaultCompressionAdapter>();
            public Stream Compress(Stream stream) => stream;
            public Stream Decompress(Stream stream) => stream;
            public void Dispose() { }
        }
    }
}
