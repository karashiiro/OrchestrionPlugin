using Dalamud.Plugin;
using System.Collections.Generic;

namespace OrchestrionPlugin.CustomSong
{
    class CustomSongController : IPlaybackController
    {
        private readonly CustomSongConfiguration config;
        private readonly DalamudPluginInterface pi;
        private readonly SCDConverter mp3;

        private readonly Dictionary<string, byte[]> convertedSongs;

        public CustomSongController(CustomSongConfiguration config, DalamudPluginInterface pi)
        {
            this.config = config;
            this.pi = pi;
            this.mp3 = new SCDConverter();

            this.convertedSongs = new Dictionary<string, byte[]>();
        }

        public void PlaySong(int mode)
        {
            // Set the pointers to the pointers of the custom song
            // Seek to beginning
        }

        public void StopSong()
        {
            // Set stuff back to the old stuff
        }

        public void LoadSong()
        {
            // Deallocate current custom song
            // Allocate new custom song
        }

        public SCDConvertError ConvertSong(string path)
        {
            SCDConvertError error = mp3.Convert(path, out byte[] convertedSong);
            if (error == SCDConvertError.None)
                return error;
            this.convertedSongs.Add(path, convertedSong);
            this.config.SongPaths.Add(path);
            return SCDConvertError.None;
        }
    }
}
