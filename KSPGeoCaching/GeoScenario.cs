using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP;
using KSP.IO;


namespace KSPGeoCaching
{
    [KSPScenario(ScenarioCreationOptions.AddToAllGames | ScenarioCreationOptions.AddToExistingGames, GameScenes.FLIGHT, GameScenes.SPACECENTER)]
    internal class GeoScenario : ScenarioModule
    {
        internal static GeoScenario Instance;
        internal static GeoCacheCollection activeGeoCacheCollection = null;
        internal static GeoCacheData activeGeoCacheData;
        internal bool useGeoCache = false;

        const string SCENARIO = "GEOCACHINGSCENARIO";
        public override void OnAwake()
        {
            base.OnAwake();
            Instance = this;
        }
        public override void OnLoad(ConfigNode node)
        {
            if (node != null)
            {
                base.OnLoad(node);
                if (node.HasNode(SCENARIO))
                {

                }
            }
        }
        public override void OnSave(ConfigNode node)
        {
            Log.Info("OnSave");
            if (node != null)
                Log.Info("Node is not null");
            if (node != null)
            {
                base.OnSave(node);
                if (GeoCacheDriver.Instance != null)
                {
                    node.AddValue("useGeoCache", GeoCacheDriver.Instance.useGeoCache);
                    if (GeoCacheDriver.Instance.useGeoCache && GeoCacheDriver.activeGeoCacheCollection != null)
                    {
                        ConfigNode configNode = FileIO.SaveToConfigNode(GeoCacheDriver.activeGeoCacheCollection);
                        Log.Info("After SaveToConfigNode");
                        if (configNode == null)
                            Log.Info("configNode is null");
                        if (configNode != null)
                            node.SetNode(SCENARIO, configNode, true);
                    }
                }
            }
        }
    }
}