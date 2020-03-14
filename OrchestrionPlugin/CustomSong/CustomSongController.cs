using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OrchestrionPlugin.CustomSong
{
    class CustomSongController : IPlaybackController
    {
        private readonly CustomSongConfiguration config;
        private readonly DalamudPluginInterface pi;
        private readonly SCDConverter scd;

        private readonly Dictionary<string, byte[]> convertedSongs;

        private IntPtr loadedSong;

        public CustomSongController(CustomSongConfiguration config, DalamudPluginInterface pi)
        {
            this.config = config;
            this.pi = pi;
            this.scd = new SCDConverter();

            this.convertedSongs = new Dictionary<string, byte[]>();
        }

        public void PlaySong(int mode)
        {
            if (loadedSong == null)
                return;
            // Set the pointers to the pointers of the custom song
            // Seek to beginning
        }

        public void StopSong()
        {
            if (loadedSong == null)
                return;
            Marshal.FreeHGlobal(loadedSong);
        }

        public void LoadSong(string path)
        {
            loadedSong = Marshal.AllocHGlobal(convertedSongs[path].Length);
            Marshal.Copy(convertedSongs[path], 0, loadedSong, convertedSongs[path].Length);
        }

        public AudioConvertError ConvertSong(string path)
        {
            AudioConvertError error = scd.Convert(path, out byte[] convertedSong);
            if (error == AudioConvertError.None)
                return error;
            this.convertedSongs.Add(path, convertedSong);
            this.config.SongPaths.Add(path);
            return AudioConvertError.None;
        }
    }
}
