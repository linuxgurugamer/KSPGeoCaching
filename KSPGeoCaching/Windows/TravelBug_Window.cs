using System.IO;
using System.Text.RegularExpressions;
using KSP.UI.Screens;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using ToolbarControl_NS;
using ClickThroughFix;


namespace KeoCaching
{
    partial class KeoCacheDriver : MonoBehaviour
    {
        TravelBugEntry CollectionActiveOnVessel(string collectionId)
        {
            TravelBugEntry tbe;
            if (activeTravelBugs.TryGetValue(collectionId, out tbe))
                return tbe;
            return null;
        }
 
        bool KeoCacheUsed(string keoCacheId)
        {
            Log.Info("KeoCacheUsed, sizeof usedKeoCaches: " + usedKeoCaches.Count);
            foreach (var s in usedKeoCaches)
                Log.Info("KeoCacheUsed, value: " + s);
            return usedKeoCaches.Contains(keoCacheId);
        }

        void FindKeoCaches()
        {
            Log.Info("FindKeoCaches, FlightGlobals.Vessels.Count: " + FlightGlobals.Vessels.Count);
            foreach (var x in FlightGlobals.Vessels)
            {
                List<ProtoPartSnapshot> partList = x.protoVessel.protoPartSnapshots;
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
                            Log.Info("FindKeoCaches, travelbugId: " + tBug + ",   collectionID: " + collectionID);
                            usedKeoCaches.Add(collectionID);
                        }
                    }
                }
            }
        }

        internal static List<string> usedKeoCaches;
        void GetActiveTravelBugs()
        {
            if (HighLogic.LoadedSceneIsEditor)
                return;
            Log.Info("GetActiveTravelBugs");
            Log.Info("FlightGlobals.Vessels.Count: " + FlightGlobals.Vessels.Count + 
                ", VesselsUnloaded.Count: " + FlightGlobals.VesselsUnloaded.Count +
                ", VesselsLoaded.Count: " + FlightGlobals.VesselsLoaded.Count);
            activeTravelBugs = new Dictionary<string, TravelBugEntry>();
            usedKeoCaches = new List<string>();

            for (int i = FlightGlobals.Vessels.Count - 1; i >= 0; i--)
            {
                Vessel vessel = FlightGlobals.Vessels[i];
                //Log.Info("TravelBug_Window, i: " + i + ", vessel: " + vessel.GetDisplayName() + ",  loaded: " + vessel.loaded);
                // May need to use moduleHandlers here instead of partModules, needed if vessel is unloaded
                //
                // 
                // if (moduleHandlers.ContainsKey(p.modules[i].moduleName)) {
                //
                if (vessel != null)
                {
                    if (!vessel.loaded)
                    {
                        for (int imh = vessel.protoVessel.protoPartSnapshots.Count - 1; imh >= 0; imh--)
                        {
                            ProtoPartSnapshot mh = vessel.protoVessel.protoPartSnapshots[imh];
                            for (int mi = mh.modules.Count - 1; mi >= 0; mi--)
                            {
                                ProtoPartModuleSnapshot module = mh.modules[mi];
                                if (module.moduleName == "KeoTravelBugModule")
                                {
                                    ConfigNode cn = module.moduleValues.GetNode(FileIO.KEOCACHE_COLLECTION);
                                    if (cn != null)
                                    {
                                        KeoCacheCollection tbKcc = KeoCacheCollection.LoadCollectionFromConfigNode(cn);
                                        if (tbKcc != null)
                                        {
                                            TravelBugEntry tbe = new TravelBugEntry(tbKcc.keocacheCollectionData.collectionId, vessel.vesselName, vessel.protoVessel.vesselID);

                                            activeTravelBugs.Add(tbe.keocachId, tbe);
                                        }
                                        FindKeoCaches();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        List<KeoTravelBugModule> vesselTravelBugs = vessel.FindPartModulesImplementing<KeoTravelBugModule>();

                        if (vesselTravelBugs != null)
                        {
                            Log.Info("vesselTravelBugs.count: " + vesselTravelBugs.Count);
                            for (int i1 = vesselTravelBugs.Count - 1; i1 >= 0; i1--)
                            {
                                KeoTravelBugModule traveBug = vesselTravelBugs[i1];
                                Log.Info("creating TravelBugentry # " + i1);
                                if (traveBug == null) Log.Info("traveBug is null");
                                if (traveBug.tbKcc != null)
                                {
                                    if (traveBug.tbKcc == null) Log.Info("traveBug.tbKcc is null");
                                    if (traveBug.tbKcc.keocacheCollectionData == null) Log.Info("traveBug.tbKcc.keocacheCollectionData is null");
                                    if (traveBug.tbKcc.keocacheCollectionData.collectionId == null) Log.Info("traveBug.tbKcc.keocacheCollectionData.collectionId is null");

                                    if (vessel == null) Log.Info("vessel is null");
                                    if (vessel.vesselName == null) Log.Info("vessel.vesselName is null");
                                    if (vessel.protoVessel == null) Log.Info("vessel.protoVessel is null");

                                    TravelBugEntry tbe = new TravelBugEntry(traveBug.tbKcc.keocacheCollectionData.collectionId, vessel.vesselName, vessel.protoVessel.vesselID);

                                    activeTravelBugs.Add(tbe.keocachId, tbe);
                                    FindKeoCaches();
                                }
                            }
                        }
                    }
                }
            }
        }

        //internal TravelBug activeTravelBug = new TravelBug();
        string SaveGame = "persistent";

        void DisplayTravelBugData(System.Guid vesselID, string vesselName, string keoCacheName, string keoCacheTitle )
        {
            GUILayout.BeginHorizontal();
            GUILayout.TextField(vesselName, GUILayout.Width(175));
            GUILayout.Space(5);
            GUILayout.TextField(keoCacheName, GUILayout.Width(175));
            GUILayout.Space(5);
            GUILayout.TextField(keoCacheTitle, GUILayout.Width(175));
            GUILayout.Space(5);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Select", GUILayout.Width(55)))
            {
                visibleTravelBug = false;

                string _saveGame = GamePersistence.SaveGame(SaveGame, HighLogic.SaveFolder, SaveMode.OVERWRITE);

                int _idx = HighLogic.CurrentGame.flightState.protoVessels.FindLastIndex(pv => pv.vesselID == vesselID);
                if (_idx != -1)
                {
                    FlightDriver.StartAndFocusVessel(_saveGame, _idx);
                }
                else
                {
                    //QDebug.Warning("QStart: invalid idx", "QSpaceCenter");
//DestroyThis();
                }

            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        void TravelBug_Window(int id)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Width(10);
            GUILayout.Label("Vessel", GUILayout.Width(175));
            GUILayout.Space(5);
            GUILayout.Label("Collection Name", GUILayout.Width(175));
            GUILayout.Space(5);
            GUILayout.Label("Collection Title", GUILayout.Width(175));
            GUILayout.EndHorizontal();

            //
            // If this starts to be inefficient, then a new method to gather all the travelbugs will need
            // to be written. that method will be called when this window is opened
            //
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(120), GUILayout.Width(615));
            for (int i = FlightGlobals.Vessels.Count - 1; i >= 0; i--)
            {
                Vessel vessel = FlightGlobals.Vessels[i];
                Log.Info("TravelBug_Window, i: " + i + ", vessel: " + vessel.GetDisplayName() + ",  loaded: " + vessel.loaded);
                // May need to use moduleHandlers here instead of partModules, needed if vessel is unloaded
                //
                // 
                // if (moduleHandlers.ContainsKey(p.modules[i].moduleName)) {
                //
                if (!vessel.loaded)
                {
                    for (int imh = vessel.protoVessel.protoPartSnapshots.Count - 1; imh >= 0; imh--)
                    {
                        Log.Info("imh: " + imh);
                        ProtoPartSnapshot mh = vessel.protoVessel.protoPartSnapshots[imh];
                        for (int mi = mh.modules.Count - 1; mi >= 0; mi--)
                        {
                            Log.Info("mi: " + mi);
                            ProtoPartModuleSnapshot module = mh.modules[mi];
                            if (module.moduleName == "KeoTravelBugModule")
                            {
                                ConfigNode cn = module.moduleValues.GetNode(FileIO.KEOCACHE_COLLECTION);
                                if (cn != null)
                                {
                                    KeoCacheCollection tbKcc = KeoCacheCollection.LoadCollectionFromConfigNode(cn);
                                    if (tbKcc != null)
                                    {
                                        DisplayTravelBugData(
                                            vessel.protoVessel.vesselID,
                                            vessel.vesselName,
                                            tbKcc.keocacheCollectionData.name,
                                            tbKcc.keocacheCollectionData.title);
                                    }
                                }

#if false
                                if (KeoScenario.activeKeoCacheCollections != null)
                                {
                                    string collectionId = module.moduleValues.GetValue("collectionID");
                                    Log.Info("collectionID: " + collectionId);
                                    activeKeoCacheCollection = KeoScenario.activeKeoCacheCollections[collectionId];

                                    if (activeKeoCacheCollection != null)
                                    {
                                        DisplayTravelBugData(
                                            vessel.protoVessel.vesselID, 
                                            vessel.vesselName, 
                                            activeKeoCacheCollection.keocacheCollectionData.name, 
                                            activeKeoCacheCollection.keocacheCollectionData.title);
                                    }
                                }
#endif

                            }
                        }
                    }
                }
                else
                {
                    List<KeoTravelBugModule> vesselTravelBugs = vessel.FindPartModulesImplementing<KeoTravelBugModule>();

                    if (vesselTravelBugs != null)
                    {
                        for (int i1 = vesselTravelBugs.Count - 1; i1 >= 0; i1--)
                        {
                            KeoTravelBugModule traveBug = vesselTravelBugs[i1];

                            var c = traveBug.collectionID;
                            DisplayTravelBugData(vessel.id, vessel.vesselName, traveBug.tbKcc.keocacheCollectionData.name, traveBug.tbKcc.keocacheCollectionData.title);

#if false
                            if (KeoScenario.activeKeoCacheCollections != null)
                            {
                                activeKeoCacheCollection = KeoScenario.activeKeoCacheCollections[c];

                                if (activeKeoCacheCollection != null)
                                {
                                    DisplayTravelBugData(vessel.id,  vessel.vesselName, activeKeoCacheCollection.keocacheCollectionData.name, activeKeoCacheCollection.keocacheCollectionData.title);
                                }
                            }
#endif
                        }
                    }
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close", GUILayout.Width(60)))
            {
                visibleTravelBug = false;
                visibleMenu = true;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUI.DragWindow();

        }
    }
}