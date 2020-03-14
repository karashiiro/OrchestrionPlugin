using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OrchestrionPlugin.CustomSong
{
    // https://github.com/goaaats/ffxiv-explorer-fork/blob/develop/src/com/fragmenterworks/ffxivextract/gui/SCDConverterWindow.java#L275
    class SCDConverter
    {
        public static readonly int SCD_HEADER_SIZE = 0x540;
        
        private static readonly string scdHeaderFile = "scd_header.bin";

        public AudioConvertError Convert(string path, out byte[] convertedData) => Convert(path, null, null, out convertedData);

        public AudioConvertError Convert(string path, int? loopStart, int? loopEnd, out byte[] convertedData)
        {
            byte[] oggFile;
            AudioConvertError error = AudioConverter.OpenFile(path, out oggFile);
            if (error != AudioConvertError.None)
            {
                convertedData = new byte[0];
                return error;
            }
            return Convert(loopStart, loopEnd, oggFile, out convertedData);
        }

        public AudioConvertError Convert(int? loopStart, int? loopEnd, byte[] oggFile, out byte[] convertedData)
        {
            var volume = 1.0f;
            var numChannels = 2;
            var sampleRate = 44100;
            loopStart = loopStart ?? 0;
            loopEnd = loopEnd ?? oggFile.Length;

            byte[] header = CreateSCDHeader(oggFile.Length, volume, numChannels, sampleRate, (int)loopStart, (int)loopEnd);

            convertedData = header.Concat(oggFile).ToArray();

            return AudioConvertError.None;
        }

        private byte[] CreateSCDHeader(int oggLength, float volume, int numChannels, int sampleRate, int loopStart, int loopEnd)
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

        private int GetBytePosition(float samplePosition, float numSamples, float filesize)
            => (int)((filesize/numSamples)*samplePosition);
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
