﻿using ApeFree.DataStore.Core;
using ApeFree.DataStore.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApeFree.DataStore.Local
{
    /// <summary>
    /// 本地存储器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LoaclStore<T> : Store<T, LoaclStoreAccessSettings> where T : new()
    {
        private readonly IReadWriteMangedModel<T> model = new CoalesceMangedModel<T>();

        public LoaclStore(LoaclStoreAccessSettings accessSettings) : base(accessSettings)
        {

        }

        private bool isRunning = false;

        private void Run()
        {
            if (isRunning)
            {
                return;
            }

            isRunning = true;

            lock (this)
            {
                new Thread(() =>
                {
                    EventItem<T> item = null;
                    while (model.Dequeue(out item))
                         {
                             if (item == null)
                             {
                                 break;
                             }

                             Console.Write($"* Dequeue: [{item.EventType}]");
                             if (item.EventType == ReadWriteEventType.Write)
                             {
                                 var path = AccessSettings.SavePath;
                                 Directory.CreateDirectory(Path.GetDirectoryName(path));
                                 SaveHandler(stream =>
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
                             else
                             {
                                 var path = AccessSettings.SavePath;
                                 if (!File.Exists(path))
                                 {
                                     Value = Activator.CreateInstance<T>();
                                     // Save();
                                 }
                                 else
                                 {
                                     using (var steam = File.Open(path, FileMode.Open, FileAccess.Read))
                                     {
                                         LoadHandler(steam);
                                     }
                                 }
                             }

                             item.Release();
                         }
                    isRunning = false;

                }).Start();
            }
        }

        public override void Load()
        {
            model.Enqueue(new EventItem<T>(ReadWriteEventType.Read), Run);
        }

        public override void Save()
        {
            model.Enqueue(new EventItem<T>(ReadWriteEventType.Write) { Value = Value }, Run);
        }
    }
}
