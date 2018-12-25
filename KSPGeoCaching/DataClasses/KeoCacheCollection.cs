using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeoCaching
{
    // All Id's will be GUIDs for uniqueness

    public class KeoCacheCollection
    {
        public KeoCacheCollectionData keocacheCollectionData;
        public List<KeoCacheData> keoCacheDataList;

        public string fullPath;

        internal KeoCacheCollection()
        {
            keocacheCollectionData = new KeoCacheCollectionData();
            keoCacheDataList = new List<KeoCacheData>();
        }

        static internal ConfigNode SaveCollectionToConfigNode(KeoCacheCollection keoCache)
        {
            Log.Info("SaveCollectionToConfigNode");
            ConfigNode collectionData = new ConfigNode();
            //ConfigNode fileNode = new ConfigNode();

            collectionData.AddValue("id", keoCache.keocacheCollectionData.collectionId);
            collectionData.AddValue("name", keoCache.keocacheCollectionData.name);
            collectionData.AddValue("title", keoCache.keocacheCollectionData.title);
            collectionData.AddValue("author", keoCache.keocacheCollectionData.author);
            //collectionData.AddValue("description",  keoCache.fileData.description);
            FileIO.AddTextArea("description", keoCache.keocacheCollectionData.description, ref collectionData);
            FileIO.AddTextArea("initialHint", keoCache.keocacheCollectionData.initialHint, ref collectionData);

            collectionData.AddValue("difficulty", keoCache.keocacheCollectionData.difficulty);
            collectionData.AddValue("assignedTravelBug", keoCache.keocacheCollectionData.assignedTravelBug);
            collectionData.AddValue("lastCacheFound", keoCache.keocacheCollectionData.lastCacheFound);
            foreach (var s in keoCache.keocacheCollectionData.requiredMods)
            {
                collectionData.AddValue("requiredMod", s);
            }

            foreach (var keocacheData in keoCache.keoCacheDataList)
            {
                Log.Info("keoCacheName: " + keocacheData.keoCacheName);
                ConfigNode data = new ConfigNode();

                data.AddValue("name", keocacheData.keoCacheName);
                data.AddValue("scienceNodeRequired", keocacheData.scienceNodeRequired);
                data.AddValue("found", keocacheData.found);
                data.AddValue("body", keocacheData.body.name);


                FileIO.AddTextArea("description", keocacheData.description, ref data);
                
                data.AddValue("lastHintFound", keocacheData.lastHintFound);

                //data.AddValue("nextKeocacheId", keocacheData.nextKeocacheId);
                foreach (var hint in keocacheData.hints)
                {
                    Log.Info("hint.hintTitle: " + hint.hintTitle);
                    ConfigNode h = new ConfigNode();
                    h.AddValue("hintTitle", hint.hintTitle);
                    h.AddValue("distance", hint.hintDistance);
                    h.AddValue("scale", (int)hint.scale);
                    h.AddValue("hint", hint.hint);
                    h.AddValue("situations", (int)hint.situations);
                    //h.AddValue("spawn", hint.spawn);
                    data.AddNode(FileIO.HINT, h);
                }

                data.AddValue("distance", keocacheData.distanceFromCache);
                data.AddValue("scale", (int)keocacheData.scale);

                // Now save the vessel associated with this node
                if (keocacheData.CacheVessel != null)
                {
                    data.AddValue("latitude", keocacheData.CacheVessel.latitude);
                    data.AddValue("longitude", keocacheData.CacheVessel.longitude);
                    ConfigNode vesselNode = new ConfigNode();
                    keocacheData.CacheVessel.BackupVessel().Save(vesselNode);
                    data.AddNode(FileIO.PROTOVESSEL, vesselNode);
                }
                else
                {
                    data.AddValue("latitude", keocacheData.latitude);
                    data.AddValue("longitude", keocacheData.longitude);
                    data.AddNode(FileIO.PROTOVESSEL, keocacheData.protoVessel);
                }
                data.AddValue("spawned", keocacheData.spawned);
                collectionData.AddNode(FileIO.KEOCACHEDATA, data);
            }
            Log.Info("SaveCollectionToConfigNode, id: " + keoCache.keocacheCollectionData.collectionId);
            Log.Info("SaveCollectionToConfigNode, name: " + keoCache.keocacheCollectionData.name);
            Log.Info("SaveCollectionToConfigNode, title: " + keoCache.keocacheCollectionData.title);
            Log.Info("SaveCollectionToConfigNode, description: " + keoCache.keocacheCollectionData.description);

            KeoCacheCollection tmpK = LoadCollectionFromConfigNode(collectionData);

            return collectionData;
            //fileNode.AddNode(FileIO.KEOCACHE_COLLECTION, collectionData);
            //return fileNode;
        }

        internal static KeoCacheCollection LoadCollectionFromConfigNode(ConfigNode loadedConfigNode)
        {
            Log.Info("LoadCollectionFromConfigNode");
            KeoCacheCollection keoCache = new KeoCacheCollection();
            //keoCache.keocacheCollectionData = new keoCacheCollectionData();

            loadedConfigNode.TryGetValue("id", ref keoCache.keocacheCollectionData.collectionId);
            loadedConfigNode.TryGetValue("name", ref keoCache.keocacheCollectionData.name);
            loadedConfigNode.TryGetValue("title", ref keoCache.keocacheCollectionData.title);
            loadedConfigNode.TryGetValue("author", ref keoCache.keocacheCollectionData.author);
#if false
            int cnt = 0;
            string str = "";
            keoCache.keocacheCollectionData.description = "";
            while (cnt >= 0)
            {  
                if (loadedConfigNode.HasValue("description-" + cnt.ToString()))
                {
                    loadedConfigNode.TryGetValue("description-" + cnt.ToString(), ref str);
                    if (keoCache.keocacheCollectionData.description != "")
                        keoCache.keocacheCollectionData.description += "\n";
                    keoCache.keocacheCollectionData.description += str;
                    cnt++;
                }
                else
                    cnt = -1;
            }
#endif
            FileIO.LoadTextArea("description", ref keoCache.keocacheCollectionData.description, ref loadedConfigNode);
            FileIO.LoadTextArea("initialHint", ref keoCache.keocacheCollectionData.initialHint, ref loadedConfigNode);




            loadedConfigNode.TryGetEnum<Difficulty>("difficulty", ref keoCache.keocacheCollectionData.difficulty, keoCache.keocacheCollectionData.difficulty);
            loadedConfigNode.TryGetValue("assignedTravelBug", ref keoCache.keocacheCollectionData.assignedTravelBug);

            Log.Info("LoadCollectionFromConfigNode, id: " + keoCache.keocacheCollectionData.collectionId);
            Log.Info("LoadCollectionFromConfigNode, name: " + keoCache.keocacheCollectionData.name);
            Log.Info("LoadCollectionFromConfigNode, title: " + keoCache.keocacheCollectionData.title);
            Log.Info("LoadCollectionFromConfigNode, description: " + keoCache.keocacheCollectionData.description);


            keoCache.keocacheCollectionData.requiredMods = loadedConfigNode.GetValuesList("requiredMod");
            loadedConfigNode.TryGetValue("lastCacheFound", ref keoCache.keocacheCollectionData.lastCacheFound);



            ConfigNode[] nodes = loadedConfigNode.GetNodes(FileIO.KEOCACHEDATA);
            Log.Info("nodes.count: " + nodes.Count());
            for (int i = 0; i < nodes.Length; i++)
            {
                ConfigNode node = nodes[i];
                KeoCacheData keocacheData = new KeoCacheData();

                node.TryGetValue("name", ref keocacheData.keoCacheName);
                node.TryGetValue("scienceNodeRequired", ref keocacheData.scienceNodeRequired);
                node.TryGetValue("found", ref keocacheData.found);

                Log.Info("LoadCollectionFromConfigNode.node, name: " + keocacheData.keoCacheName);
                Log.Info("LoadCollectionFromConfigNode.node, scienceNodeRequired: " + keocacheData.scienceNodeRequired);
                Log.Info("LoadCollectionFromConfigNode.node, found: " + keocacheData.found);

                string bodyName = "";
                node.TryGetValue("body", ref bodyName);
                // need to find celestial body and set keocacheData.body to it
                foreach (var p in PSystemManager.Instance.localBodies)
                    if (p.name == bodyName)
                    {
                        keocacheData.body = p;
                        break;
                    }
                int x = 0;
                node.TryGetValue("scale", ref x);
                keocacheData.scale = (Scale)x;

                double d = 0;
                node.TryGetValue("distance", ref d);
                keocacheData.distanceFromCache = d;
                keocacheData.absoluteDistance = keocacheData.distanceFromCache;                
                if (keocacheData.scale == Scale.km)
                    keocacheData.absoluteDistance *= 1000;

                node.TryGetValue("latitude", ref keocacheData.latitude);
                node.TryGetValue("longitude", ref keocacheData.longitude);

                FileIO.LoadTextArea("description", ref keocacheData.description, ref node);


                node.TryGetValue("lastHintFound", ref keocacheData.lastHintFound);

                var SavedHints = node.GetNodes(FileIO.HINT);
                keocacheData.hints = new List<Hint>();
                if (SavedHints != null)
                    Log.Info("SavedHints.Count: " + SavedHints.Count());
                foreach (var h in SavedHints)
                {
                    Hint hint = new Hint();
                    h.TryGetValue("hintTitle", ref hint.hintTitle);
                    d = 0;
                    h.TryGetValue("distance", ref d);
                    hint.hintDistance = d;
                    x = 0;
                    h.TryGetValue("scale", ref x);
                    hint.scale = (Scale)x;
                    hint.absoluteDistance = hint.hintDistance;
                    if (hint.scale == Scale.km)
                        hint.absoluteDistance *= 1000;
                    x = 0;
                    h.TryGetValue("situations", ref x);
                    hint.situations = (Vessel.Situations)x;
                    h.TryGetValue("hint", ref hint.hint);

                    // h.TryGetValue("spawn", ref hint.spawn);
                    keocacheData.hints.Add(hint);
                }
                keocacheData.hints = keocacheData.hints.OrderByDescending(ad => ad.absoluteDistance).ToList();

                keocacheData.protoVessel = node.GetNode(FileIO.PROTOVESSEL);
                node.TryGetValue("spawned", ref keocacheData.spawned);
                //if (KeoCacheDriver.Instance != null && !KeoCacheDriver.Instance.useKeoCache)
                /* keocacheData.protoVessel = */
                //    VesselRespawn.Respawn(keocacheData.protoVessel);
                keoCache.keoCacheDataList.Add(keocacheData);
            }
            return keoCache;
        }

    }


}
