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
        private const string ConfigFilePath = "GameData/KSPGeoCaching/PluginData/GeoCaches";
        //static public string dirSeperator = "\\";
        //static public string altSeperator = "/";

        const string GEOCACHEDATA = "GeoCacheData";
        const string HINT = "Hint";

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
                string s = Path.Combine(KSPUtil.ApplicationRootPath, ConfigFilePath).Replace('\\', '/');
                Log.Info("configFilePath: " + s);
                return s;
            }
        }
       

        public GeoCache LoadGeocacheData(string f)
        {
            GeoCache geoCache;

            if (File.Exists(f))
            {
                ConfigNode loadedConfigNode = ConfigNode.Load(f);
                if (loadedConfigNode != null)
                {
                    geoCache = new GeoCache();
                    geoCache.fileData = new GeoCacheCollectionData();

                    loadedConfigNode.TryGetValue("id", ref geoCache.fileData.id);
                    loadedConfigNode.TryGetValue("name", ref geoCache.fileData.name);
                    loadedConfigNode.TryGetValue("title", ref geoCache.fileData.title);
                    loadedConfigNode.TryGetValue("author", ref geoCache.fileData.author);
                    loadedConfigNode.TryGetValue("description", ref geoCache.fileData.description);
                    loadedConfigNode.TryGetEnum<Difficulty>("difficulty", ref geoCache.fileData.difficulty, geoCache.fileData.difficulty);

                    geoCache.fileData.requiredMods = loadedConfigNode.GetValuesList("requiredMod");

                    var nodes = loadedConfigNode.GetNodes(GEOCACHEDATA);
                    foreach (var node in nodes)
                    {
                        GeoCacheData geocacheData = new GeoCacheData();

                        node.TryGetValue("name", ref geocacheData.name);
                        node.TryGetValue("scienceNodeRequired", ref geocacheData.scienceNodeRequired);
                        node.TryGetValue("found", ref geocacheData.found);
                        string bodyName = "";
                        node.TryGetValue("body", ref bodyName);
                        // need to find celestial body and set geocacheData.body to it

                        node.TryGetValue("latitude", ref geocacheData.latitude);
                        node.TryGetValue("longitude", ref geocacheData.longitude);
                        node.TryGetValue("description", ref geocacheData.description);
                        node.TryGetValue("nextGeocacheId", ref geocacheData.nextGeocacheId);
                        var SavedHints = node.GetNodes(HINT);
                        geocacheData.hints = new List<Hints>();
                        foreach (var h in SavedHints)
                        {
                            Hints hint = new Hints();
                            h.TryGetValue("distance", ref hint.distance);
                            h.TryGetValue("hint", ref hint.hint);
                            h.TryGetValue("spawn", ref hint.spawn);
                            geocacheData.hints.Add(hint);
                        }

                        geoCache.geocacheData.Add(geocacheData);
                    }
                    return geoCache;
                }                
            }
            return null;
        }

        public bool SaveGeocacheFile(GeoCache geoCache)
        {
            ConfigNode fileData = new ConfigNode();

            fileData.AddValue("id", geoCache.fileData.id);
            fileData.AddValue("name",  geoCache.fileData.name);
            fileData.AddValue("title",  geoCache.fileData.title);
            fileData.AddValue("author",  geoCache.fileData.author);
            fileData.AddValue("description",  geoCache.fileData.description);
            fileData.AddValue("difficulty", geoCache.fileData.difficulty);
            foreach (var s in geoCache.fileData.requiredMods)
            {
                fileData.AddValue("requiredMod", s);
            }

            foreach (var geocacheData in geoCache.geocacheData)
            {
                ConfigNode data = new ConfigNode();

                data.AddValue("name", geocacheData.name);
                data.AddValue("scienceNodeRequired", geocacheData.scienceNodeRequired);
                data.AddValue("found", geocacheData.found);
                data.AddValue("body", geocacheData.body.name);

                data.AddValue("latitude", geocacheData.latitude);
                data.AddValue("longitude", geocacheData.longitude);
                data.AddValue("description", geocacheData.description);
                data.AddValue("nextGeocacheId", geocacheData.nextGeocacheId);
                foreach (var hint in geocacheData.hints)
                {
                    ConfigNode h = new ConfigNode();
                    h.AddValue("distance",  hint.distance);
                    h.AddValue("hint",  hint.hint);
                    h.AddValue("spawn",  hint.spawn);
                    data.AddNode(HINT, h);
                }
                fileData.AddNode(GEOCACHEDATA, data);
            }
            try
            {
                fileData.Save(GetCacheDir + fileData.name);
                return true;
            } catch
            { }
            return false;
        }
    }
}
