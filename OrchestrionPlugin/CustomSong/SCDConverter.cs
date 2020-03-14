using NAudio.Vorbis;
using NVorbis;
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

        public AudioConvertError Convert(string path, int? loopStart, int? loopEnd, out byte[] convertedData)
        {
            AudioConvertError error = VorbisConverter.OpenFile(path, out VorbisWaveReader oggFile);
            if (error != AudioConvertError.None)
            {
                convertedData = new byte[0];
                return error;
            }
            return Convert(loopStart, loopEnd, oggFile, out convertedData);
        }

        public AudioConvertError Convert(int? loopStart, int? loopEnd, VorbisWaveReader oggFile, out byte[] convertedData)
        {
            var meta = new VorbisReader(oggFile);

            var volume = 1.0f;
            var numChannels = meta.Channels;
            var sampleRate = meta.SampleRate;
            loopStart = loopStart ?? 0;
            loopEnd = loopEnd ?? (int)oggFile.Length;

            MemoryStream scd = CreateSCDHeader((int)oggFile.Length, volume, numChannels, sampleRate, (int)loopStart, (int)loopEnd);
            scd.Seek(0, SeekOrigin.End);
            oggFile.CopyTo(scd);
            scd.Seek(0, SeekOrigin.Begin);

            convertedData = scd.ToArray();

            scd.Dispose();
            oggFile.Dispose();

            return AudioConvertError.None;
        }

        private MemoryStream CreateSCDHeader(int oggLength, float volume, int numChannels, int sampleRate, int loopStart, int loopEnd)
        {
            var templateFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), scdHeaderFile);
            var scdHeader = new MemoryStream(File.ReadAllBytes(templateFile));
            scdHeader.SetValue(0x0010, scdHeader.Length + oggLength);
            scdHeader.SetValue(0x00A8, volume);
            scdHeader.SetValue(0x01B0, oggLength - 0x10);
            scdHeader.SetValue(0x01B4, numChannels);
            scdHeader.SetValue(0x01B8, sampleRate);
            scdHeader.SetValue(0x01C0, loopStart);
            scdHeader.SetValue(0x01C2, loopEnd);
            return scdHeader;
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
