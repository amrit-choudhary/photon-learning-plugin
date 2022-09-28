using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.Hive.Plugin;

namespace MyFirstPlugin
{
    public class MyFirstPlugin : PluginBase
    {
        public override string Name => "MyFirstPlugin";

        private IPluginLogger pluginLogger;

        public override bool SetupInstance(IPluginHost host, Dictionary<string, string> config, out string errorMsg)
        {
            this.pluginLogger = host.CreateLogger(this.Name);
            this.pluginLogger.InfoFormat("My Plugin Loaded");
            return base.SetupInstance(host, config, out errorMsg);
        }

        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            this.pluginLogger.InfoFormat("OnCreateGame {0} by user {1}", info.Request.GameId, info.UserId);
            info.Continue();
        }
    }
}
