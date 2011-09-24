using System;
using SkypeHistorian.Exporting.Storages;

namespace SkypeHistorian.Exporting
{
    internal static class StorageFactory
    {
        public static Storage Create(string path, bool compressData)
        {
            return compressData ?
                (Storage)(new ZipFileStorage(path)) : new FileSystemStorage(path);
        }
    }
}
