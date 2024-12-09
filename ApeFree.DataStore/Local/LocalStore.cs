﻿using ApeFree.DataStore.Core;
using ApeFree.DataStore.IO;
using STTech.CodePlus.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApeFree.DataStore.Local
{
    /// <summary>
    /// 本地存储器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LocalStore<T> : Store<T, LocalStoreAccessSettings> where T : class
    {
        public LocalStore(LocalStoreAccessSettings accessSettings, Func<T> valueFactory = null) : base(accessSettings, valueFactory) { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void OnLoad()
        {
            var path = AccessSettings.SavePath;

            if (File.Exists(path))
            {
                using (var steam = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    OnReadStream(steam);
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void OnSave()
        {
            var path = AccessSettings.SavePath;
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            OnWriteStream(stream =>
            {
                // TODO: 此处应使用文件流写入

                MemoryStream memoryStream;
                if (stream is MemoryStream)
                {
                    memoryStream = stream as MemoryStream;
                }
                else
                {
                    memoryStream = new MemoryStream();
                    stream.CopyTo(memoryStream);
                }
                File.WriteAllBytes(path, memoryStream.ToArray());
            });
        }
    }
}
