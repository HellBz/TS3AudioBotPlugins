﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using TS3AudioBot;
using TS3AudioBot.Plugins;
using TS3Client;
using TS3Client.Messages;
using TS3AudioBot.CommandSystem;
using TS3AudioBot.Helper;
using System.Reflection;
using TS3Client.Full;

namespace TestPlugin
{
    public class TestPlugin : ITabPlugin
    {
        MainBot bot;

        public class PluginInfo
        {
            public static readonly string Name = typeof(PluginInfo).Namespace;
            public const string Description = "Sends a message to the current channel everytime the track changes.";
            public const string URL = "";
            public const string Author = "Bluscream <admin@timo.de.vc>";
            public const int Version = 2;
        }

        public void PluginLog(Log.Level logLevel, string Message) {
            switch (logLevel)
            {
                case Log.Level.Debug:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case Log.Level.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case Log.Level.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }
            Log.Write(logLevel, Message);
            Console.ResetColor();
        }

        public void Initialize(MainBot mainBot) {
            bot = mainBot;
            bot.RightsManager.RegisterRights("TestPlugin.dummyperm");
            bot.QueryConnection.OnClientConnect += QueryConnection_OnClientConnect;
            bot.QueryConnection.OnClientDisconnect += QueryConnection_OnClientDisconnect;
            bot.QueryConnection.OnMessageReceived += QueryConnection_OnMessageReceived;
            bot.PlayManager.AfterResourceStarted += PlayManager_AfterResourceStarted;
            PluginLog(Log.Level.Debug, "Plugin " + PluginInfo.Name + " v" + PluginInfo.Version + " by " + PluginInfo.Author + " loaded.");
            var lib = bot.QueryConnection.GetLowLibrary<Ts3FullClient>();
            lib.SendGlobalMessage("hallo");

        }

        private void PlayManager_AfterResourceStarted(object sender, PlayInfoEventArgs e) {
            Console.WriteLine("sd");
            var title = e.ResourceData.ResourceTitle;
            var length = 2.0m.ToString(bot.PlayerConnection.Length.ToString());
            bot.QueryConnection.SendChannelMessage("Now playing " + title + " (" + length + ")");
        }

        private void QueryConnection_OnMessageReceived(object sender, TextMessage e) {
            Console.WriteLine("got message " + e.Message);
            bot.QueryConnection.SendMessage(e.Message, e.InvokerId);
        }

        private void QueryConnection_OnClientDisconnect(object sender, ClientLeftView e) {
            bot.QueryConnection.SendMessage("ciao", e.ClientId);
        }

        private void QueryConnection_OnClientConnect(object sender, ClientEnterView e) {
            bot.QueryConnection.SendMessage("hallo", e.ClientId);
        }

        public void SendChannelMessage(string Message) {
            bot.QueryConnection.ChangeName(PluginInfo.Name);
            bot.QueryConnection.SendChannelMessage(Message);
            bot.QueryConnection.ChangeName("Bluscream's Bitch");
        }

        public void Dispose() {
            bot.QueryConnection.OnClientConnect -= QueryConnection_OnClientConnect;
            bot.QueryConnection.OnClientDisconnect -= QueryConnection_OnClientDisconnect;
            bot.QueryConnection.OnMessageReceived -= QueryConnection_OnMessageReceived;
            bot.RightsManager.UnregisterRights("TestPlugin.dummyperm");
            PluginLog(Log.Level.Debug, "Plugin " + PluginInfo.Name + " unloaded.");
        }

        [Command("isowner", "Bla")]
        public string CommandCheckOwner(ExecutionInformation info) {
            return info.HasRights("TestPlugin.dummyperm").ToString();
        }

        [Command("tprequest", "Request Talk Power!")]
        public string CommandTPRequest(string name, int number) {
            var owner = bot.QueryConnection.GetClientByName("Bluscream").UnwrapThrow();
            return "Hi " + name + ", you choose " + number + ". My owner is " + owner.Uid;
        }
    }
}
