using System;
using UnityEngine;
using ClickThroughFix;

namespace KeoCaching
{
    public class KeoTravelBugModule : PartModule
    {
        [KSPField(isPersistant = true, guiActive = false)]
        public string travelbugId = "";

        [KSPField(isPersistant = true, guiActive = false)]
        public string travelbugIdInEditor = "";


        [KSPField(isPersistant = true, guiActive = false)]
        internal string collectionID = "";

        [KSPField(isPersistant = true, guiActive = false)]
        internal string collectionIDInEditor = "";

        Rect hintWinRect = new Rect((Screen.width - KeoCacheDriver.HINT_WIDTH) / 2, (Screen.height - KeoCacheDriver.CURRENT_HINT_HEIGHT) / 2, KeoCacheDriver.HINT_WIDTH, KeoCacheDriver.CURRENT_HINT_HEIGHT);
        int hintWinID;
        bool displayCurrentHint = false;

        internal KeoCacheCollection tbKcc;

        [KSPEvent(name = "SelectTravelbug", guiActive = false, guiActiveEditor = true, guiName = "Select Collection for Travelbug")]
        public void SelectTravelbug()
        {
            Log.Info("KeoTravelBugModule.SelectTravelbug");

            KeoCacheDriver.Instance.selectCollection = true;
            KeoCacheDriver.Instance.visibleAllCollections = true;
            KeoCacheDriver.Instance.activeBug = this;
            KeoCacheDriver.Instance.selectForEdit = false;
        }
        [KSPEvent(name = "ClearTravelbug", guiActive = false, guiActiveEditor = false, guiName = "Clear Travelbug Collection")]
        public void ClearTravelbug()
        {
            Log.Info("KeoTravelBugModule.ClearTravelbug");
            SetCollection("");
        }

        [KSPEvent(name = "CurrentHint", guiActive = true, guiActiveEditor = false, guiName = "Display current hint")]
        public void CurrentHint()
        {
            Log.Info("KeoTravelBugModule.CurrentHint");
            //KeoCacheDriver.Instance.selectCollection = true;
            //KeoCacheDriver.Instance.visibleActiveCollections = true;
            displayCurrentHint = true;
            KeoCacheDriver.Instance.activeBug = this;
            Log.Info("Selected collection id: " + collectionID);
            Log.Info("Selected collection name: " + tbKcc.keocacheCollectionData.name);
            Log.Info("TravelBug id: " + travelbugId);
            Log.Info("assignedTravelBug: " + tbKcc.keocacheCollectionData.assignedTravelBug);
            int currentCache = tbKcc.keocacheCollectionData.lastCacheFound + 1;
            int currentHint = tbKcc.keoCacheDataList[currentCache].lastHintFound + 1;
            Log.Info("currentCache: " + currentCache);
            Log.Info("currentHint: " + currentHint);
        }

        public void SetCollection(string g, KeoCacheCollection kcc = null)
        {
            Log.Info("SetCollection, g: " + g);
            if (g != null && g != "")
            {
                Events["SelectTravelbug"].guiActiveEditor = false;
                //Events["SelectTravelbug"].guiActive = false;

                //if (HighLogic.LoadedSceneIsEditor)
                Events["ClearTravelbug"].guiActiveEditor = true;
                //KeoScenario.activeKeoCacheCollections.Add(g, kcc);

                this.tbKcc = kcc;
                if (tbKcc == null)
                    Log.Info("SetColleciton, tbKcc is null");

            }
            else
            {
                Events["SelectTravelbug"].guiActiveEditor = true;
                Events["ClearTravelbug"].guiActiveEditor = false;
                //KeoScenario.activeKeoCacheCollections[collectionID].keocacheCollectionData.assignedTravelBug = "";
                this.tbKcc = null;
            }
            collectionID = g;
            //if (HighLogic.LoadedSceneIsEditor)
            collectionIDInEditor = g;
            //else
            //    collectionID = g;
        }

        void Start()
        {
            Log.Info("KeoTravelBugModule.Start, scene: " + HighLogic.LoadedScene + ", collectionID: " + collectionID);
            hintWinID = (DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second) * 1000 + DateTime.Now.Millisecond + 3;

            if (travelbugId == null || travelbugId == "")
            {
                travelbugId = Guid.NewGuid().ToString("N"); ;
                Log.Info("New travel bug id generated: " + travelbugId);
            }

            //
            // If in the editor, make sure the collectionID is empty, and if not, then clear the assignedTravelBug in that collection
            //
            if (HighLogic.LoadedSceneIsEditor)
            {
                if (collectionIDInEditor != "")
                {
                    Events["ClearTravelbug"].guiActiveEditor = true;
                    Events["SelectTravelbug"].guiActiveEditor = false;
                }
                else
                {
                    Events["ClearTravelbug"].guiActiveEditor = false;
                    Events["SelectTravelbug"].guiActiveEditor = true;
                }
                Log.Info("Clearing assignedTravelBug in collection: " + collectionID);
                collectionID = "";
            }
            

            //
            // When in flight, if collectionIDinEditor is not empty, then assign this travelbug to that collection
            //
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (collectionIDInEditor != "" && collectionID == "")
                {
                    collectionID = collectionIDInEditor;
                    collectionIDInEditor = "";
                }
                Log.Info("LoadedSceneIsFlight, situation: " + vessel.situation);
                if (this.vessel != null && this.vessel.situation == Vessel.Situations.PRELAUNCH)
                {
                    Log.Info("Prelaunch");
                    displayCurrentHint = true;
                }
            }

            GameEvents.onPartDie.Add(onPartDie);
            GameEvents.onPartDestroyed.Add(onPartDestroyed);
            if (HighLogic.LoadedSceneIsEditor)
                GameEvents.onEditorPartEvent.Add(evt: OnPartEvent);
        }

        private void OnPartEvent(ConstructionEventType ct, Part p)
        {
            if (!HighLogic.CurrentGame.Parameters.CustomParams<KeoCacheOptions>().allowMultipleOnVessel && p != null && p == this.part )
            {
                if (ct == ConstructionEventType.PartAttached)
                { 
                    // count up all the TravelBugs here
                    int numBugs = 0;
                    for (int i = EditorLogic.fetch.ship.parts.Count - 1; i >= 0; i--)
                    {
                        numBugs += (EditorLogic.fetch.ship.parts[i].Modules.GetModules<KeoTravelBugModule>().Count);
                    }
                    Log.Info("numBugs on vessel: " + numBugs);
                    if (numBugs > 1)
                    {
                        ScreenMessages.PostScreenMessage("Multiple TravelBugs on same vessel not allowed!", 10f, ScreenMessageStyle.UPPER_CENTER);
                        ScreenMessages.PostScreenMessage("Deleting extra TravelBug", 10f, ScreenMessageStyle.UPPER_CENTER);
                        EditorLogic.DeletePart(this.part);
                    }
                }
            }
        }

        void OnDestroy()
        {
            Log.Info("KeoTravelBugModule.OnDestroy");
            GameEvents.onPartDie.Remove(onPartDie);
            GameEvents.onPartDestroyed.Remove(onPartDestroyed);
            if (HighLogic.LoadedSceneIsEditor)
                GameEvents.onEditorPartEvent.Remove(evt: OnPartEvent);
        }

        void onPartDie(Part p)
        {
            Log.Info("KeoTravelBugModule.onPartDie");
            if (p == this.part)
            {
                Log.Info("KeoTravelBugModule.onPartDie");
                // need to decide what to do if a travelbug dies
#if false
                KeoScenario.travelBugs.Remove(travelbugId);
#endif
            }
        }
        void onPartDestroyed(Part p)
        {
            Log.Info("KeoTravelBugModule.onPartDestroyed");
            if (p == this.part)
            {
                Log.Info("KeoTravelBugModule.onPartDestroyed");
            }
        }
        public override string GetModuleDisplayName()
        {
            return "KeoCache";
        }

        public override string GetInfo()
        {
            string s = "TravelBug part\n";
            if (tbKcc != null && tbKcc.keocacheCollectionData != null)
            {
                s += "Collection: " + tbKcc.keocacheCollectionData.title + "\n";

                int currentCache = tbKcc.keocacheCollectionData.lastCacheFound + 1;
                int currentHint = tbKcc.keoCacheDataList[currentCache].lastHintFound;

                if (tbKcc.keoCacheDataList[currentCache].lastHintFound >= 0)
                    s += "Last Hint Found: " + tbKcc.keoCacheDataList[currentCache].hints[currentHint].hintTitle;
            }
            return s;
        }

        internal void CurrentHintWindow(int i)
        {
            int currentCache = tbKcc.keocacheCollectionData.lastCacheFound + 1;
            if (currentCache < 0)
                return;
            int currentHint = tbKcc.keoCacheDataList[currentCache].lastHintFound;

            Hint currentHintr = new Hint();
            if (currentHint >= 0)
                currentHintr = tbKcc.keoCacheDataList[currentCache].hints[currentHint];
            else
            {
                currentHintr.hintTitle = "Initial Hint";
                currentHintr.hint = tbKcc.keocacheCollectionData.initialHint;
            }
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Title:");
            GUILayout.TextField(currentHintr.hintTitle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Hint: ");
            GUILayout.TextArea(currentHintr.hint, GUILayout.Height(75), GUILayout.Width(300));

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();

            if (!HighLogic.CurrentGame.Parameters.CustomParams<KeoCacheOptions>().pauseAtNewHint)
            {
                if (GUILayout.Button("Close"))
                {
                    displayCurrentHint = false;
                }
            }
            else
            {
                if (GUILayout.Button("Unpause"))
                {
                    FlightDriver.SetPause(false, false);
                    displayCurrentHint = false;
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        void OnGUI()
        {
            if (displayCurrentHint)
            {
                if (HighLogic.CurrentGame.Parameters.CustomParams<KeoCacheOptions>().useKSPskin)
                    GUI.skin = HighLogic.Skin;
                hintWinRect = ClickThruBlocker.GUILayoutWindow(hintWinID, hintWinRect, CurrentHintWindow, "Hint");
            }
        }


        void FixedUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (this.vessel == FlightGlobals.ActiveVessel)
                {
                    //Log.Info("FixedUpdate, collectionID: " + collectionID);
                    if (tbKcc == null)
                    {
                        //Log.Info("tbKcc is null");
                        return;
                    }
                    else
                        Log.Info("tbKcc " + tbKcc.keocacheCollectionData.collectionId);
#if false
                    if (!KeoScenario.activeKeoCacheCollections.ContainsKey(collectionID))
                        return;
#endif
                    int currentCache = tbKcc.keocacheCollectionData.lastCacheFound + 1;
                    if (tbKcc.keoCacheDataList.Count <= currentCache)
                        return;
                    int currentHint = tbKcc.keoCacheDataList[currentCache].lastHintFound + 1;

                    Log.Info("lastCacheFound: " + tbKcc.keocacheCollectionData.lastCacheFound +
                                ", lastHintFound: " + tbKcc.keoCacheDataList[currentCache].lastHintFound);
                    KeoCacheData kcd = tbKcc.keoCacheDataList[currentCache];

                    if (kcd.body == FlightGlobals.currentMainBody)
                    {

                        double spawnDistance = 0;
                        double hintDistance = 0;
                        float lat = 0, lon = 0, alt = 0;
                        kcd.protoVessel.TryGetValue("lat", ref lat);
                        kcd.protoVessel.TryGetValue("lon", ref lon);
                        kcd.protoVessel.TryGetValue("alt", ref alt);

                        float d = GetStraightDistance(lat, lon, alt);
                        //if (tbKcc.keoCacheDataList[currentCache].hints[currentHint].scale == Scale.km)
                        //    d /= 1000;
                        Log.Info("distance: " + d);
                        if (currentCache < tbKcc.keoCacheDataList.Count)
                        {
                            spawnDistance = tbKcc.keoCacheDataList[currentCache].absoluteDistance;
                        }
                        if (currentHint < tbKcc.keoCacheDataList[currentCache].hints.Count)
                        {
                            hintDistance = tbKcc.keoCacheDataList[currentCache].hints[currentHint].absoluteDistance;
                        }
#if false
                        Log.Info("hintDistance: " + hintDistance + ", spawnDistance: " + spawnDistance);

                            Log.Info("currentCache: " + currentCache + ", currentHint: " + currentHint);
                            Log.Info("KeoScenario.activeKeoCacheCollections[collectionID].keoCacheDataList[currentCache].hints[currentHint].distance: " + tbKcc.keoCacheDataList[currentCache].hints[currentHint].distance);
                            Log.Info("KeoScenario.activeKeoCacheCollections[collectionID].keoCacheDataList[currentCache].hints.Count: " + tbKcc.keoCacheDataList[currentCache].hints.Count);
#endif

                        if (d <= hintDistance)
                        {
                            bool landed = (Vessel.Situations.LANDED & tbKcc.keoCacheDataList[currentCache].hints[currentHint].situations) > 0;
                            bool splashed = (Vessel.Situations.SPLASHED & tbKcc.keoCacheDataList[currentCache].hints[currentHint].situations) > 0;
                            bool flying = (Vessel.Situations.FLYING & tbKcc.keoCacheDataList[currentCache].hints[currentHint].situations) > 0;

                            if (landed || splashed || flying)
                            {
                                Log.Info("Crossed currentHint.distance, currentHint: " + currentHint);
                                if (currentHint < tbKcc.keoCacheDataList[currentCache].hints.Count)
                                {
                                    tbKcc.keoCacheDataList[currentCache].lastHintFound++;
                                    displayCurrentHint = true;
                                    if (HighLogic.CurrentGame.Parameters.CustomParams<KeoCacheOptions>().pauseAtNewHint)
                                        FlightDriver.SetPause(true, false);
                                }
                            }
                        }

                        if (d <= spawnDistance && tbKcc.keoCacheDataList[currentCache].lastHintFound == tbKcc.keoCacheDataList[currentCache].hints.Count - 1)
                        {
                            if (!kcd.spawned)
                            {
                                Log.Info("Spawning KeoCache");
                                VesselRespawn.Respawn(kcd.protoVessel);
                                kcd.spawned = true;
                            }
                            tbKcc.keoCacheDataList[currentCache].lastHintFound++;
                        }
                    }
                }
            }
        }


        float GetStraightDistance(float latitude, float longitude, float altitude)
        {
            Vessel v = FlightGlobals.ActiveVessel;

            Vector3 wpPosition = FlightGlobals.currentMainBody.GetWorldSurfacePosition(latitude, longitude, altitude);
            return Vector3.Distance(wpPosition, v.transform.position);
        }

        public override void OnLoad(ConfigNode node)
        {
            Log.Info("KeoTravelBugModule.OnLoad");
            if (this.vessel != null)
                Log.Info("vessel: " + this.vessel.vesselName);
            ConfigNode loadedCollectionNode = node.GetNode(FileIO.KEOCACHE_COLLECTION);

            if (loadedCollectionNode != null)
            {
                Log.Info("OnLoad Loading KeoCacheCollection");
                tbKcc = KeoCacheCollection.LoadCollectionFromConfigNode(loadedCollectionNode);
            }
            base.OnLoad(node);
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);
            Log.Info("KeoTravelBugModule.OnSave");
            if (this.vessel != null)
                Log.Info("vessel: " + this.vessel.vesselName);
            if (tbKcc != null)
            {
                Log.Info("OnSave Saving KeoCacheCollection, keocacheCollectionData.name: " + tbKcc.keocacheCollectionData.name);
                ConfigNode n = KeoCacheCollection.SaveCollectionToConfigNode(tbKcc);
                Log.Info("OnSave 2");
                var t = KeoCacheCollection.LoadCollectionFromConfigNode(n);

                node.RemoveNodes(FileIO.KEOCACHE_COLLECTION); // probably not necessory 
                node.AddNode(FileIO.KEOCACHE_COLLECTION, n);
            }
            else
                Log.Info("In OnSave, tbKss is null");

        }
    }

}