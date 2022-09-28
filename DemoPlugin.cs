using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.Hive.Plugin;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Security.Permissions;

namespace MyFirstPlugin
{
    public class DemoPlugin : PluginBase
    {
        public override string Name => "DemoPlugin";

        private IPluginLogger m_pluginLogger;
        private RoomDesignParams m_roomDesignParams;
        private object m_oneTimeTimer;
        private object m_repeatingTimer;
        private int m_player1_health, m_player2_health;
        private Dictionary<byte, object> parameters;
        private int m_timeInSeconds;

        public override bool SetupInstance(IPluginHost host, Dictionary<string, string> config, out string errorMsg)
        {
            string s = File.ReadAllText("RoomDesignParams.json");
            m_roomDesignParams = JsonConvert.DeserializeObject<RoomDesignParams>(s);

            m_pluginLogger = host.CreateLogger(this.Name);
            m_pluginLogger.InfoFormat("Demo Plugin Loaded");

            return base.SetupInstance(host, config, out errorMsg);
        }

        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            m_pluginLogger.InfoFormat("OnCreateGame {0} by user {1}", info.Request.GameId, info.UserId);

            info.Continue();
        }

        public override void OnJoin(IJoinGameCallInfo info)
        {
            m_pluginLogger.InfoFormat("Joined: " + info.ActorNr);

            info.Continue();

            m_oneTimeTimer = PluginHost.CreateOneTimeTimer(
                info,
                SendPlayerInitEvent,
                2000);

            m_repeatingTimer = PluginHost.CreateTimer(
                UpdateTimer,
                2000,
                1000);
        }

        private void SendPlayerInitEvent()
        {
            m_player1_health = m_roomDesignParams.player1_health;
            m_player2_health = m_roomDesignParams.player2_health;

            // Set healths
            SendHealthUpdateEvent();

            // Set player 1 values
            parameters = new Dictionary<byte, object>();
            parameters.Add(0, 1);
            parameters.Add(1, m_roomDesignParams.player1_color_r);
            parameters.Add(2, m_roomDesignParams.player1_color_g);
            parameters.Add(3, m_roomDesignParams.player1_color_b);
            parameters.Add(4, m_roomDesignParams.player1_speed);
            BroadcastEvent(PhotonEventCodes.InitPlayerEventCode, parameters);
            
            // Set player 2 values
            parameters = new Dictionary<byte, object>();
            parameters.Add(0, 2);
            parameters.Add(1, m_roomDesignParams.player2_color_r);
            parameters.Add(2, m_roomDesignParams.player2_color_g);
            parameters.Add(3, m_roomDesignParams.player2_color_b);
            parameters.Add(4, m_roomDesignParams.player2_speed);
            BroadcastEvent(PhotonEventCodes.InitPlayerEventCode, parameters);
        }

        public void SendHealthUpdateEvent()
        {
            parameters = new Dictionary<byte, object>();
            parameters.Add(0, m_player1_health);
            parameters.Add(1, m_player2_health);
            BroadcastEvent(PhotonEventCodes.HealthUpdateEventCode, parameters);
        }

        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            if(info.Request.EvCode == PhotonEventCodes.DamageEventCode)
            {
                if (info.ActorNr == 1)
                {
                    m_player2_health -= m_roomDesignParams.player1_damage;
                }
                else
                {
                    m_player1_health -= m_roomDesignParams.player2_damage;
                }

                SendHealthUpdateEvent();
            }

            if (info.Request.EvCode == PhotonEventCodes.HealEventCode)
            {
                if (info.ActorNr == 1)
                {
                    m_player1_health += m_roomDesignParams.player1_heal;
                }
                else
                {
                    m_player2_health += m_roomDesignParams.player2_heal;
                }

                SendHealthUpdateEvent();
            }

            info.Continue();
        }

        private void UpdateTimer()
        {
            m_timeInSeconds++;

            parameters = new Dictionary<byte, object>();
            parameters.Add(0, m_timeInSeconds);
            BroadcastEvent(PhotonEventCodes.TimerUpdateEventCode, parameters);
        }

        public override void BeforeCloseGame(IBeforeCloseGameCallInfo info)
        {
            PluginHost.StopTimer(m_oneTimeTimer);
            m_oneTimeTimer = null;

            PluginHost.StopTimer(m_repeatingTimer);
            m_repeatingTimer = null;

            info.Continue();
        }
    }

    public class RoomDesignParams
    {
        public int player1_health;
        public int player2_health;
        public int player1_damage;
        public int player2_damage;
        public int player1_heal;
        public int player2_heal;
        public float player1_speed;
        public float player2_speed;
        public float player1_color_r;
        public float player1_color_g;
        public float player1_color_b;
        public float player2_color_r;
        public float player2_color_g;
        public float player2_color_b;
    }
}
