using Dalamud.Configuration;
using System;
using System.Collections.Generic;

namespace OrchestrionPlugin.CustomSong
{
    [Serializable]
    class CustomSongConfiguration : IPluginConfiguration
    {
        int IPluginConfiguration.Version { get; set; }

        public IList<string> SongPaths;

        public CustomSongConfiguration()
        {
            SongPaths = new List<string>();
        }
    }
}
