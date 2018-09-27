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
        internal static Dictionary<Guid, GeoCacheCollection> activeGeoCacheCollections = new Dictionary<Guid, GeoCacheCollection>();

        
        private static Dictionary<Guid, TravelBug> travelBugs = new Dictionary<Guid, TravelBug>();

        const string SCENARIO = "GEOCACHINGSCENARIOS";
        const string TRAVELBUGS = "TRAVELBUGS";
        const string ENTRIES = "ENTRIES";
        public override void OnAwake()
        {
            base.OnAwake();
            Instance = this;
        }

        void LoadScenarios(ConfigNode node)
        {
            Log.Info("LoadScenarios");
            if (node.HasNode(SCENARIO))
            {
                activeGeoCacheCollections = new Dictionary<Guid, GeoCacheCollection>();

                var gNode = node.GetNode(SCENARIO);
                if (gNode != null)
                {
                    var aNode = gNode.GetNodes();

                 
                        for (int i = 0; i < aNode.Length; i++)
                        {
                            ConfigNode loadedCollectionNode = aNode[i].GetNode(FileIO.GEOCACHE_COLLECTION);
                            var cache = FileIO.LoadGeoCacheDataFromConfigNode(loadedCollectionNode);
                            if (cache != null)
                                GeoScenario.activeGeoCacheCollections.Add(cache.geocacheCollectionData.collectionId, cache);
                        }


                }
            }
        }
        void LoadTravelBugs(ConfigNode node)
        {
            if (node.HasNode(SCENARIO))
            {
                travelBugs = new Dictionary<Guid, TravelBug>();
                var bNode = node.GetNodes(TRAVELBUGS);
                if (bNode != null)
                {
                    for (int i = 0; i < bNode.Length; i++)
                    {
                        var travelBug = new TravelBug();

                        bNode[i].TryGetValue("id", ref travelBug.travelbugId);
                        bNode[i].TryGetValue("name", ref travelBug.name);
                        bNode[i].TryGetValue("collectionId", ref travelBug.activeCollectionId);

                        var entries = bNode[i].GetNodes();
                        for (int e = 0; e < entries.Count() - 1; e++)
                        {
                            TravelBugEntry tbe = new TravelBugEntry();
                            entries[e].TryGetValue("geocachId", ref tbe.geocachId);
                            entries[e].TryGetValue("hintId", ref tbe.hintId);
                            entries[e].TryGetValue("timeFound", ref tbe.timeFound);

                            travelBug.entries.Add(tbe);
                        }

                        travelBugs.Add(travelBug.travelbugId, travelBug);
                    }
                }
            }
        }
        public override void OnLoad(ConfigNode node)
        {
            Log.Info("GeoScenario.OnLoad");
            if (node != null)
            {
                base.OnLoad(node);
                LoadScenarios(node);
                LoadTravelBugs(node);
            }
        }


        void SaveScenarios(ConfigNode node)
        {
            base.OnSave(node);
            ConfigNode collectionNode = new ConfigNode();
            foreach (var cache in GeoScenario.activeGeoCacheCollections)
            {
                ConfigNode configNode = FileIO.SaveToConfigNode(cache.Value);
                if (configNode != null)
                {
                    string nodeName = cache.Value.geocacheCollectionData.collectionId.ToString();
                    if (collectionNode.HasNode(nodeName))
                    {
                        collectionNode.SetNode(nodeName, configNode);
                    }
                    else
                    {
                        collectionNode.AddNode(nodeName, configNode);
                    }

                }
            }
            node.AddNode(SCENARIO, collectionNode);
        }

        void SaveTravelBugs(ConfigNode node)
        {
            ConfigNode travelBugsNode = new ConfigNode();
            foreach (var bug in travelBugs)
            {
                ConfigNode bugNode = new ConfigNode();
                bugNode.AddValue("id", bug.Value.travelbugId);
                bugNode.AddValue("name", bug.Value.name);
                bugNode.AddValue("collectionId", bug.Value.activeCollectionId);

                ConfigNode entries = new ConfigNode();
                foreach (var e in bug.Value.entries)
                {
                    ConfigNode entryNode = new ConfigNode();
                    entryNode.AddValue("geocachId", e.geocachId);
                    entryNode.AddValue("hintId", e.hintId);
                    entryNode.AddValue("timeFound", e.timeFound);
                    entries.AddNode(e.timeFound.ToString(), entryNode);
                }
                bugNode.AddNode(ENTRIES, entries);
                travelBugsNode.AddNode(bug.Value.travelbugId.ToString(), bugNode);
            }
            node.AddNode(TRAVELBUGS, travelBugsNode);
        }
        public override void OnSave(ConfigNode node)
        {
            Log.Info("GeoScenario.OnSave");
            if (node != null)
                Log.Info("Node is not null");
            if (node != null)
            {
                SaveScenarios(node);
                SaveTravelBugs(node);
            }
        }

        static public bool AddCollection(GeoCacheCollection cache)
        {
            if (GeoScenario.activeGeoCacheCollections.ContainsKey(cache.geocacheCollectionData.collectionId))
            {
                Log.Info("collectionId exists in active collection");
                foreach (var k in activeGeoCacheCollections)
                {
                    Log.Info("Key: " + k.Key);
                }
                return false;
            }
            GeoScenario.activeGeoCacheCollections.Add(cache.geocacheCollectionData.collectionId, cache);
            return true;
        }

        public static int ActiveCollections { get { return GeoScenario.activeGeoCacheCollections.Count; } }

        public static bool IdInCollection(Guid g)
        {
            foreach (var a in GeoScenario.activeGeoCacheCollections)
            {
                foreach (var b in a.Value.geocacheData)
                {
                    Log.Info("IdInCollection, g: " + g + ", a.Key: " + a.Key + ", b.geocacheId: " + b.geocacheId);
                if (b.geocacheId == g)
                        return true;
                }
            }
            return false;
        }

        public static void HintPosition(int cacheIndex, out double latitude, out double longitude)
        {
            latitude = 0;
            longitude = 0;
        }
    }
}