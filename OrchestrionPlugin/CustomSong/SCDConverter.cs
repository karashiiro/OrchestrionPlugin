using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OrchestrionPlugin.CustomSong
{
    enum SCDConvertError
    {
        None = 0,
        NullPath = 1,
        PathTooLong = 2,
        DirectoryNotFound = 3,
        UnauthorizedAccess = 4,
        FileNotFound = 5,
        Failed = 6,
    }

    class SCDConverter
    {
        public static readonly int SCD_HEADER_SIZE = 0x540;
        
        private static readonly string scdHeaderFile = "scd_header.bin";

        public SCDConverter()
        {
        }

        // https://github.com/goaaats/ffxiv-explorer-fork/blob/develop/src/com/fragmenterworks/ffxivextract/gui/SCDConverterWindow.java#L275
        public SCDConvertError Convert(string path, out byte[] convertedData)
        {
            byte[] oggFile;
            convertedData = new byte[0];
            try
            {
                oggFile = File.ReadAllBytes(path);
            }
            catch (Exception e)
            {
                if (e is ArgumentNullException)
                    return SCDConvertError.NullPath;
                if (e is PathTooLongException)
                    return SCDConvertError.PathTooLong;
                if (e is DirectoryNotFoundException)
                    return SCDConvertError.DirectoryNotFound;
                if (e is UnauthorizedAccessException)
                    return SCDConvertError.UnauthorizedAccess;
                if (e is FileNotFoundException)
                    return SCDConvertError.FileNotFound;
                return SCDConvertError.Failed;
            }

            var volume = 1.0f;
            var numChannels = 2;
            var sampleRate = 44100;
            var loopStart = 0;
            var loopEnd = oggFile.Length;

            byte[] header = CreateSCDHeader(oggFile.Length, volume, numChannels, sampleRate, loopStart, loopEnd);

            convertedData = header.Concat(oggFile).ToArray();

            return SCDConvertError.None;
        }

        public byte[] CreateSCDHeader(int oggLength, float volume, int numChannels, int sampleRate, int loopStart, int loopEnd)
        {
            var templateFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), scdHeaderFile);
            var scdHeader = new MemoryStream(File.ReadAllBytes(templateFile));
            scdHeader.SetValue(0x0010, scdHeader.Length + oggLength);
            scdHeader.SetValue(0x01B0, oggLength - 0x10);
            scdHeader.SetValue(0x00A8, volume);
            scdHeader.SetValue(0x01B4, numChannels);
            scdHeader.SetValue(0x01B8, sampleRate);
            scdHeader.SetValue(0x01C0, loopStart);
            scdHeader.SetValue(0x01C2, loopEnd);
            return scdHeader.ToArray();
        }
    }

    static class SCDConverterUtil
    {
        public static void SetValue(this MemoryStream stream, int offset, dynamic value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            stream.Write(bytes, offset, bytes.Length);
        }
    }
}
