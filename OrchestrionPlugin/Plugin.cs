﻿using Dalamud.Game.Command;
using Dalamud.Plugin;
using OrchestrionPlugin.CustomSong;
using System.IO;
using System.Reflection;

namespace OrchestrionPlugin
{
    public class Plugin : IDalamudPlugin, IPlaybackController
    {
        public string Name => "Orchestrion plugin";

        private const string songListFile = "xiv_bgm.csv";
        private const string commandName = "/porch";

        private DalamudPluginInterface pi;
        private CustomSongConfiguration config;
        private CustomSongController customSongs;
        private SongList songList;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.pi = pluginInterface;

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), songListFile);
            this.songList = new SongList(path, this);

            this.config = pi.GetPluginConfig() as CustomSongConfiguration ?? new CustomSongConfiguration();
            this.customSongs = new CustomSongController(this.config, this.pi);

            pluginInterface.CommandManager.AddHandler(commandName, new CommandInfo(OnDisplayCommand));
            pluginInterface.UiBuilder.OnBuildUi += Display;
        }

        public void Dispose()
        {
            this.songList.Dispose();

            this.pi.UiBuilder.OnBuildUi -= Display;
            this.pi.CommandManager.RemoveHandler(commandName);

            this.pi.SavePluginConfig(this.config);

            this.pi.Dispose();
        }

        public void PlaySong(int songId)
        {
            this.pi.CommandManager.Commands["/xlbgmset"].Handler("/xlbgmset", songId.ToString());
        }

        public void StopSong()
        {
            // still no real way to do this
            this.pi.CommandManager.Commands["/xlbgmset"].Handler("/xlbgmset", "9999");
        }

        private void OnDisplayCommand(string command, string args)
        {
            // might be better to fully add/remove the OnBuildUi handler
            this.songList.Visible = true;
        }

        private void Display()
        {
            this.songList.Draw();
        }
    }
}
