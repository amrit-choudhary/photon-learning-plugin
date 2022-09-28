using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.Hive.Plugin;

namespace MyFirstPlugin
{
    public class PluginFactory : IPluginFactory
    {
        public IGamePlugin Create(IPluginHost gameHost, string pluginName, Dictionary<string, string> config, out string errorMsg)
        {
            IGamePlugin plugin = new EmptyPlugin(); // default
            switch (pluginName)
            {
                case "Default":
                    // name not allowed, throw error
                    break;
                case "DemoPlugin":
                    plugin = new DemoPlugin();
                    break;
                case "MyFirstPlugin":
                    plugin = new MyFirstPlugin();
                    break;
                default:
                    //plugin = new DefaultPlugin();
                    break;
            }
            if (plugin.SetupInstance(gameHost, config, out errorMsg))
            {
                return plugin;
            }
            return null;
        }
    }
}
