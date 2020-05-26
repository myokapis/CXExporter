using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Exporter.Extensions
{

    public static class Extensions
    {

        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return (array?.Length ?? 0) == 0;
        }

        public static T2 GetSafe<T1, T2>(this Dictionary<T1, T2> dictionary, T1 key)
        {
            dictionary.TryGetValue(key, out T2 value);
            return value;
        }

        public static DirectoryInfo CreateSafe(this DirectoryInfo directoryInfo)
        {
            if (!directoryInfo.Exists) directoryInfo.Create();
            return directoryInfo;
        }

        public static DirectoryInfo DirectoryIn(this DirectoryInfo directoryInfo, params string[] subdirectoryNames)
        {
            var dirInfo = directoryInfo;

            foreach(var subdirectoryName in subdirectoryNames)
            {
                var path = Path.Combine(dirInfo.FullName, subdirectoryName);
                dirInfo = new DirectoryInfo(path);
            }

            return dirInfo;
        }

        public static bool CopySafe(this FileInfo fileInfo, DirectoryInfo destination)
        {
            if (!fileInfo.Exists || !destination.Exists) return false;

            var path = Path.Combine(destination.FullName, fileInfo.Name);
            fileInfo.CopyTo(path);
            return true;
        }

        public static FileInfo FileIn(this DirectoryInfo directoryInfo, string fileName)
        {
            var path = Path.Combine(directoryInfo.FullName, fileName);
            return new FileInfo(path);
        }

        public static bool MoveSafe(this FileInfo fileInfo, DirectoryInfo destination)
        {
            if (!fileInfo.Exists || !destination.Exists) return false;

            var path = Path.Combine(destination.FullName, fileInfo.Name);
            fileInfo.MoveTo(path);
            return true;
        }

        public static bool MoveSafe(this FileInfo fileInfo, DirectoryInfo destination, string fileName)
        {
            if (!fileInfo.Exists || !destination.Exists) return false;

            var path = Path.Combine(destination.FullName, fileName);
            fileInfo.MoveTo(path);
            return true;
        }

        public static string Concat<T>(this List<T> list)
        {
            return string.Join("", list);
        }

        public static string Concat<T>(this List<T> list, string separator)
        {
            return string.Join(separator, list);
        }

    }

}
