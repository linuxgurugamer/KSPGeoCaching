using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP;
using KSP.IO;
using UnityEngine;


namespace KeoCaching
{
    [KSPScenario(ScenarioCreationOptions.AddToAllGames | ScenarioCreationOptions.AddToExistingGames, GameScenes.FLIGHT, GameScenes.SPACECENTER, GameScenes.EDITOR)]
    internal class KeoScenario : ScenarioModule
    {

        [KSPField(isPersistant = true)]
        internal string lastDirectory;

        internal static KeoScenario Instance;
#if false
        internal static Dictionary<string, KeoCacheCollection> activeKeoCacheCollections;
#endif
       // internal static Dictionary<string, TravelBug> travelBugs = new Dictionary<string, TravelBug>();

        const string SCENARIO = "KEOCACHINGSCENARIOS";
        const string TRAVELBUGS = "TRAVELBUGS";
        const string ENTRIES = "ENTRIES";
        public override void OnAwake()
        {
            base.OnAwake();
            Instance = this;
#if false
            if (activeKeoCacheCollections == null)
                activeKeoCacheCollections = new Dictionary<string, KeoCacheCollection>();
#endif
        }

        void LoadScenarios(ConfigNode node)
        {
#if false
            Log.Info("LoadScenarios");
            if (node.HasNode(SCENARIO))
            {
                activeKeoCacheCollections = new Dictionary<string, KeoCacheCollection>();

                var gNode = node.GetNode(SCENARIO);
                if (gNode != null)
                {
                    var aNode = gNode.GetNodes();

                    for (int i = 0; i < aNode.Length; i++)
                    {
                        ConfigNode loadedCollectionNode = aNode[i].GetNode(FileIO.KEOCACHE_COLLECTION);
                        var cache = KeoCacheCollection.LoadCollectionFromConfigNode(loadedCollectionNode);
                        if (cache != null)
                        {
                            Log.Info("LoadScenarios, adding collectionId: " + cache.keocacheCollectionData.collectionId);
                            KeoScenario.activeKeoCacheCollections.Add(cache.keocacheCollectionData.collectionId, cache);
                        }
                    }


                }
            
        
#endif
        }
#if false
        void LoadTravelBugs(ConfigNode node)
        {
            if (node.HasNode(SCENARIO))
            {
                travelBugs = new Dictionary<string, TravelBug>();
                var bNode = node.GetNode(TRAVELBUGS);
                if (bNode != null)
                {
                    var bNodes = bNode.GetNodes();
                    for (int i = 0; i < bNodes.Length; i++)
                    {
                        var travelBug = new TravelBug();

                        bNodes[i].TryGetValue("id", ref travelBug.travelbugId);
                        //bNodes[i].TryGetValue("name", ref travelBug.name);
                        bNodes[i].TryGetValue("collectionId", ref travelBug.activeCollectionId);

                        var entries = bNodes[i].GetNodes();
                        for (int e = 0; e < entries.Count() - 1; e++)
                        {
                            TravelBugEntry tbe = new TravelBugEntry();
                            entries[e].TryGetValue("keocachId", ref tbe.keocachId);
                            //entries[e].TryGetValue("hintId", ref tbe.hintId);
                            entries[e].TryGetValue("timeFound", ref tbe.timeFound);

                            travelBug.entries.Add(tbe);
                        }

                        travelBugs.Add(travelBug.travelbugId, travelBug);
                    }
                }
            }
        }
#endif

        public override void OnLoad(ConfigNode node)
        {
            Log.Info("KeoScenario.OnLoad");
            if (node != null)
            {
                base.OnLoad(node);
                LoadScenarios(node);
#if false
                LoadTravelBugs(node);
#endif
            }
        }


        void SaveScenarios(ConfigNode node)
        {
            base.OnSave(node);
#if false
            ConfigNode collectionNode = new ConfigNode();
            foreach (var cache in KeoScenario.activeKeoCacheCollections)
            {
                Log.Info("SaveScenarios, name: " + cache.Value.keocacheCollectionData.name);
                Log.Info("assignedTravelBug: " + cache.Value.keocacheCollectionData.assignedTravelBug);
                if (cache.Value.keocacheCollectionData.assignedTravelBug != "")
                {
                    ConfigNode configNode = KeoCacheCollection.SaveCollectionToConfigNode(cache.Value);
                    if (configNode != null)
                    {
                        Log.Info("configNode not null");
                        string nodeName = cache.Value.keocacheCollectionData.collectionId.ToString();
                        if (collectionNode.HasNode(nodeName))
                        {
                            Log.Info("SetNode");
                            collectionNode.SetNode(nodeName, configNode);
                        }
                        else
                        {
                            Log.Info("AddNode");
                            collectionNode.AddNode(nodeName, configNode);
                        }

                    }
                }
            }
            node.AddNode(SCENARIO, collectionNode);
#endif
        }

#if false
        void SaveTravelBugs(ConfigNode node)
        {
            ConfigNode travelBugsNode = new ConfigNode();
            foreach (var bug in travelBugs)
            {
                ConfigNode bugNode = new ConfigNode();
                bugNode.AddValue("id", bug.Value.travelbugId);
                //bugNode.AddValue("name", bug.Value.name);
                bugNode.AddValue("collectionId", bug.Value.activeCollectionId);

                ConfigNode entries = new ConfigNode();
                foreach (var e in bug.Value.entries)
                {
                    ConfigNode entryNode = new ConfigNode();
                    entryNode.AddValue("keocachId", e.keocachId);
                    //entryNode.AddValue("hintId", e.hintId);
                    entryNode.AddValue("timeFound", e.timeFound);
                    entries.AddNode(e.timeFound.ToString(), entryNode);
                }
                bugNode.AddNode(ENTRIES, entries);
                travelBugsNode.AddNode(bug.Value.travelbugId.ToString(), bugNode);
            }
            node.AddNode(TRAVELBUGS, travelBugsNode);
        }
#endif

        public override void OnSave(ConfigNode node)
        {
            Log.Info("KeoScenario.OnSave");

            if (node != null)
            {
                SaveScenarios(node);
#if false
                SaveTravelBugs(node);
#endif
            }
        }

        static public bool AddCollection(KeoCacheCollection cache)
        {
#if false
            if (KeoScenario.activeKeoCacheCollections.ContainsKey(cache.keocacheCollectionData.collectionId))
            {
                Log.Info("collectionId exists in active collection");
                foreach (var k in activeKeoCacheCollections)
                {
                    Log.Info("Key: " + k.Key);
                }
                return false;
            }
            Log.Info("KeoScenario.AddCollection, adding collectionId: " + cache.keocacheCollectionData.collectionId);
            KeoScenario.activeKeoCacheCollections.Add(cache.keocacheCollectionData.collectionId, cache);
#endif
            return true;
        }
#if false
        public static int ActiveCollections { get { return KeoScenario.activeKeoCacheCollections.Count; } }
#endif
        public static bool IdInCollection(string g)
        {
#if false
            foreach (var a in KeoScenario.activeKeoCacheCollections)
            {
                foreach (var b in a.Value.keoCacheDataList)
                {
                    Log.Info("IdInCollection, g: " + g + ", a.Key: " + a.Key + ", b.keocacheId: " + b.keocacheId);
                    if (b.keocacheId == g)
                        return true;
                }
            }
#endif
            return false;
        }


        public static void HintPosition(int cacheIndex, out double latitude, out double longitude)
        {
            latitude = 0;
            longitude = 0;
        }
    }
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class EventManager : MonoBehaviour
    {
        void Start()
        {
            GameEvents.onVesselRecovered.Add(onVesselRecovered);
        }
        void OnDestroy()
        {
            GameEvents.onVesselRecovered.Remove(onVesselRecovered);
        }
        public void onVesselRecovered(ProtoVessel vessel, bool lb)
        {
            List<ProtoPartSnapshot> partList = vessel.protoPartSnapshots;
            foreach (ProtoPartSnapshot a in partList)
            {
                List<ProtoPartModuleSnapshot> modules = a.modules;
                foreach (ProtoPartModuleSnapshot module in modules)
                {
                    if (module.moduleName == "KeoTravelBugModule")
                    {
                        ConfigNode node = module.moduleValues;
                        string tBug = node.GetValue("travelbugId");
                        string collectionID = node.GetValue("collectionID");
                        Log.Info("EventManager.onVesselRecovered, travelbugId: " + tBug + ",   collectionID: " + collectionID);
                    }
                }
            }
        }
    }
}