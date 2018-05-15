﻿using System;
using System.IO;
using System.Net;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileName = "File.mp4";
            var buffer = new byte[1024];
            using (var file = File.Open(fileName, FileMode.Append))
            using (var client = new WebClient())
            using (var stream = client.OpenRead($"http://localhost:54680/api/v1/Files?seek={file.Length}"))
            {
                while (stream.CanRead)
                {
                    var countReaded = stream.Read(buffer, 0, buffer.Length);
                    if (countReaded < 1)
                        break;
                    file.Write(buffer, 0, countReaded);
                    file.Flush();
                }
            }
        }
    }
}