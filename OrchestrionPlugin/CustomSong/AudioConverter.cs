using System;
using System.IO;

namespace OrchestrionPlugin.CustomSong
{
    enum AudioConvertError
    {
        None = 0,
        NullPath = 1,
        PathTooLong = 2,
        DirectoryNotFound = 3,
        UnauthorizedAccess = 4,
        FileNotFound = 5,
        Failed = 6,
    }

    static class AudioConverter
    {
        public static byte[] Mp3ToOgg(string path)
        {
            return new byte[0];
        }

        public static AudioConvertError OpenFile(string path, out byte[] data)
        {
            data = new byte[0];
            try
            {
                data = File.ReadAllBytes(path);
            }
            catch (Exception e)
            {
                if (e is ArgumentNullException)
                    return AudioConvertError.NullPath;
                if (e is PathTooLongException)
                    return AudioConvertError.PathTooLong;
                if (e is DirectoryNotFoundException)
                    return AudioConvertError.DirectoryNotFound;
                if (e is UnauthorizedAccessException)
                    return AudioConvertError.UnauthorizedAccess;
                if (e is FileNotFoundException)
                    return AudioConvertError.FileNotFound;
                return AudioConvertError.Failed;
            }
            return AudioConvertError.None;
        }
    }
}
