using System;
using System.IO;
using System.Linq;
using KSP.UI.Screens;
using System.Collections.Generic;
using UnityEngine;

namespace KSPGeoCaching
{
    public class FileIO
    {
        private const string GeoCacheCollectionFilePath = "GameData/KSPGeoCaching/PluginData/GeoCaches";
        //static public string dirSeperator = "\\";
        //static public string altSeperator = "/";

        const string GEOCACHEDATA = "GeoCacheData";
        internal const string GEOCACHE_COLLECTION = "GeoCacheCollection";
        const string HINT = "Hint";
        public const string SUFFIX = ".geocache";
        const string PROTOVESSEL = "ProtoVessel";
        const string TMPFILE = "tmpvessel.craft";

        static public string DirSeperator
        {
            get
            {
                if (Application.platform != RuntimePlatform.WindowsPlayer)
                {
                    return "/";
                    //dirSeperator = "/";
                    //altSeperator = "\\";
                }
                return "\\";
                //return dirSeperator;
            }
        }

        static public string GetCacheDir
        {
            get
            {
                string s = Path.Combine(KSPUtil.ApplicationRootPath, GeoCacheCollectionFilePath).Replace('\\', '/') + "/";
                Log.Info("configFilePath: " + s);
                return s;
            }
        }
        static public string CollectionFileName(string fname)
        {
            return GetCacheDir + fname + SUFFIX;
        }

        internal static GeoCacheCollection LoadGeoCacheDataFromConfigNode(ConfigNode loadedConfigNode)
        {
            GeoCacheCollection geoCache = new GeoCacheCollection();
            //geoCache.geocacheCollectionData = new GeoCacheCollectionData();

            loadedConfigNode.TryGetValue("id", ref geoCache.geocacheCollectionData.collectionId);
            loadedConfigNode.TryGetValue("name", ref geoCache.geocacheCollectionData.name);
            loadedConfigNode.TryGetValue("title", ref geoCache.geocacheCollectionData.title);
            loadedConfigNode.TryGetValue("author", ref geoCache.geocacheCollectionData.author);
            loadedConfigNode.TryGetValue("description", ref geoCache.geocacheCollectionData.description);
            loadedConfigNode.TryGetEnum<Difficulty>("difficulty", ref geoCache.geocacheCollectionData.difficulty, geoCache.geocacheCollectionData.difficulty);

            Log.Info("id: " + geoCache.geocacheCollectionData.collectionId);
            Log.Info("name: " + geoCache.geocacheCollectionData.name);
            Log.Info("title: " + geoCache.geocacheCollectionData.title);

            geoCache.geocacheCollectionData.requiredMods = loadedConfigNode.GetValuesList("requiredMod");

            ConfigNode[] nodes = loadedConfigNode.GetNodes(GEOCACHEDATA);
            foreach (var node in nodes)
            {
                Log.Info("LoadGeoCacheDataFromConfigNode 2");
                GeoCacheData geocacheData = new GeoCacheData();

                node.TryGetValue("name", ref geocacheData.geoCacheName);
                node.TryGetValue("scienceNodeRequired", ref geocacheData.scienceNodeRequired);
                node.TryGetValue("found", ref geocacheData.found);

                Log.Info("LoadGeoCacheDataFromConfigNode 2a");
                string bodyName = "";
                node.TryGetValue("body", ref bodyName);
                // need to find celestial body and set geocacheData.body to it
                foreach (var p in PSystemManager.Instance.localBodies)
                    if (p.name == bodyName)
                    {
                        geocacheData.body = p;
                        break;
                    }

                Log.Info("LoadGeoCacheDataFromConfigNode 2b");
                node.TryGetValue("latitude", ref geocacheData.latitude);
                node.TryGetValue("longitude", ref geocacheData.longitude);
                node.TryGetValue("description", ref geocacheData.description);
                node.TryGetValue("nextGeocacheId", ref geocacheData.nextGeocacheId);

                var SavedHints = node.GetNodes(HINT);
                geocacheData.hints = new List<Hint>();
                if (SavedHints != null)
                    Log.Info("SavedHints.Count: " + SavedHints.Count());
                foreach (var h in SavedHints)
                {
                    Hint hint = new Hint();
                    h.TryGetValue("distance", ref hint.distance);
                    h.TryGetValue("hint", ref hint.hint);
                    h.TryGetValue("spawn", ref hint.spawn);
                    geocacheData.hints.Add(hint);
                }
                geocacheData.protoVessel = node.GetNode(PROTOVESSEL);

                if (GeoCacheDriver.Instance != null && !GeoCacheDriver.Instance.useGeoCache)
                    /* geocacheData.protoVessel = */
                    VesselRespawn.Respawn(geocacheData.protoVessel);
                geoCache.geocacheData.Add(geocacheData);
            }
            return geoCache;
        }

        static public GeoCacheCollection LoadGeoCacheData(string fname)
        {
            GeoCacheCollection geoCache = null;
            Log.Info("LoadGeoCacheData, file:" + fname);
            if (File.Exists(fname))
            {
                Log.Info("File exists");

                ConfigNode file = ConfigNode.Load(fname);
                ConfigNode loadedCollectionNode = file.GetNode(GEOCACHE_COLLECTION);

                if (loadedCollectionNode != null)
                {
                    geoCache = LoadGeoCacheDataFromConfigNode(loadedCollectionNode);

                }
            }
            return geoCache;
        }

        static void AddDescription(string descr, ref ConfigNode node)
        {
            int cnt = 0;
            while (descr != "")
            {
                int x = descr.IndexOf("\n");
                if (x == -1)
                    x = descr.Length;
                var s1 = descr.Substring(0, x);
                node.AddValue("description-" + cnt.ToString(), s1);
                if (x < descr.Length - 1)
                    descr = descr.Substring(x + 1);
                else
                    descr = "";
                cnt++;
            }
        }

        static internal ConfigNode SaveToConfigNode(GeoCacheCollection geoCache)
        {
            ConfigNode collectionData = new ConfigNode();
            ConfigNode fileNode = new ConfigNode();
            
            collectionData.AddValue("id", geoCache.geocacheCollectionData.collectionId);
            collectionData.AddValue("name", geoCache.geocacheCollectionData.name);
            collectionData.AddValue("title", geoCache.geocacheCollectionData.title);
            collectionData.AddValue("author", geoCache.geocacheCollectionData.author);
            //collectionData.AddValue("description",  geoCache.fileData.description);
            AddDescription(geoCache.geocacheCollectionData.description, ref collectionData);

            collectionData.AddValue("difficulty", geoCache.geocacheCollectionData.difficulty);

            foreach (var s in geoCache.geocacheCollectionData.requiredMods)
            {
                collectionData.AddValue("requiredMod", s);
            }
;
            foreach (var geocacheData in geoCache.geocacheData)
            {

                ConfigNode data = new ConfigNode();
                
                data.AddValue("name", geocacheData.geoCacheName);
                data.AddValue("scienceNodeRequired", geocacheData.scienceNodeRequired);
                data.AddValue("found", geocacheData.found);
                data.AddValue("body", geocacheData.body.name);



                AddDescription(geocacheData.description, ref data);
                
                data.AddValue("nextGeocacheId", geocacheData.nextGeocacheId);
                foreach (var hint in geocacheData.hints)
                {
                    ConfigNode h = new ConfigNode();
                    h.AddValue("distance", hint.distance);
                    h.AddValue("hint", hint.hint);
                    h.AddValue("spawn", hint.spawn);
                    data.AddNode(HINT, h);
                }

                // Now save the vessel associated with this node
                if (geocacheData.CacheVessel != null)
                {
                    data.AddValue("latitude", geocacheData.CacheVessel.latitude);
                    data.AddValue("longitude", geocacheData.CacheVessel.longitude);
                    ConfigNode vesselNode = new ConfigNode();
                    geocacheData.CacheVessel.BackupVessel().Save(vesselNode);
                    data.AddNode(PROTOVESSEL, vesselNode);
                } else
                {
                    data.AddValue("latitude", geocacheData.latitude);
                    data.AddValue("longitude", geocacheData.longitude);
                    data.AddNode(PROTOVESSEL, geocacheData.protoVessel);
                }
                
                collectionData.AddNode(GEOCACHEDATA, data);
            }
            fileNode.AddNode(GEOCACHE_COLLECTION, collectionData);
            return fileNode;
        }

        static public bool SaveGeocacheFile(GeoCacheCollection geoCache)
        {
            ConfigNode fileNode = SaveToConfigNode(geoCache);

            Log.Info("Creating directory: " + GetCacheDir + GeoCacheCollectionFilePath);
            Directory.CreateDirectory(GetCacheDir);
            try
            {
                Log.Info("Saving to file: " + CollectionFileName(geoCache.geocacheCollectionData.name));
                fileNode.Save(CollectionFileName(geoCache.geocacheCollectionData.name));
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Error saving GeoCache Collection: " + ex.Message);
            }
            return false;
        }


        static internal string SaveVesselTmp(ConfigNode node)
        {
            try
            {
                Log.Info("Saving to file: " + CollectionFileName(TMPFILE));
                node.Save(CollectionFileName(TMPFILE));
                return CollectionFileName(TMPFILE);
            }
            catch (Exception ex)
            {
                Log.Error("Error saving GeoCache Collection: " + ex.Message);
            }
            return null;
        }
    }
}
