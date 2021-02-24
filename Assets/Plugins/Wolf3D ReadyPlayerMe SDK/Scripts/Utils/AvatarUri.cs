using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Wolf3D.ReadyPlayerMe.AvatarSDK
{
    public class AvatarUri
    {
        private readonly string[] Extensions = { ".glb", ".gltf" };

        public string Extension { get; private set; }
        public string ModelName { get; private set; }
        public string ModelPath { get; private set; }
        public string AbsoluteUrl { get; private set;}
        public string AbsolutePath {get; private set; }
        public string AbsoluteName { get; private set; }

        public AvatarUri(string url, string saveFolder)
        {
            Uri uri = new Uri(url);

            AbsoluteUrl = url;
            AbsolutePath = uri.AbsolutePath;
            AbsoluteName = Path.GetFileNameWithoutExtension(AbsolutePath);

            Extension = Path.GetExtension(AbsolutePath);
            if (!Extensions.Contains(Extension))
            {
                throw new Exception($"Exceptions.UnsupportedExtensionException: Unsupported model extension { Extension }. Only .gltf and .glb formats are supported");
            }

            ModelName = AbsolutePath.Split('/').Last();
            ModelPath = $"{ Application.dataPath }/{ saveFolder }/{ ModelName }";
        }
    }
}
