using NAudio.Vorbis;
using NAudio.Wave;
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
        FileTypeNotSupported = 6,
        Failed = 7,
    }

    static class VorbisConverter
    {
        public static VorbisWaveReader Mp3ToOgg(string path)
        {
            using (var mp3 = new Mp3FileReader(path))
            {
                using (var pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
                {
                    return WavToOgg(pcm);
                }
            }
        }

        public static VorbisWaveReader WavToOgg(string path)
        {
            using (var ogg = new VorbisWaveReader(path))
            {
                return ogg;
            }
        }

        public static VorbisWaveReader WavToOgg(WaveStream wav)
        {
            using (var ogg = new VorbisWaveReader(wav))
            {
                return ogg;
            }
        }

        public static AudioConvertError OpenFile(string path, out VorbisWaveReader data)
        {
            var ext = Path.GetExtension(path);
            data = new VorbisWaveReader(Stream.Null);
            try
            {
                if (ext == "mp3")
                    data = Mp3ToOgg(path);
                else if (ext == "wav")
                    data = WavToOgg(path);
                else if (ext == "ogg")
                    data = new VorbisWaveReader(path);
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
