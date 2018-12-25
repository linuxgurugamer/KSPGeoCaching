using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;



namespace KeoCaching
{
    public class FileIO
    {
        private const string KeoCacheCollectionFilePath = "GameData/KeoCaching/PluginData/KeoCaches";
        //static public string dirSeperator = "\\";
        //static public string altSeperator = "/";

        internal const string KEOCACHEDATA = "KeoCacheData";
        internal const string KEOCACHE_COLLECTION = "KeoCacheCollection";
        internal const string HINT = "Hint";
        public const string SUFFIX = ".keocache";
        internal const string PROTOVESSEL = "ProtoVessel";
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
                string s = Path.Combine(KSPUtil.ApplicationRootPath, KeoCacheCollectionFilePath).Replace('\\', '/') + "/";
                return s;
            }
        }
        static public string CollectionFileName(string fname)
        {
            return GetCacheDir + fname + SUFFIX;
        }

       

        static public KeoCacheCollection LoadKeoCacheData(string fname)
        {
            KeoCacheCollection keoCache = null;
            Log.Info("LoadKeoCacheData, file:" + fname);
            if (File.Exists(fname))
            {
                Log.Info("File exists");

                ConfigNode file = ConfigNode.Load(fname);
                if (file != null)
                {
                    ConfigNode loadedCollectionNode = file.GetNode(KEOCACHE_COLLECTION);

                    if (loadedCollectionNode != null)
                    {
                        keoCache = KeoCacheCollection.LoadCollectionFromConfigNode(loadedCollectionNode);
                    }
                    else
                        Log.Info("loadedCollectionNode is null");
                }
                else
                    Log.Info("Error loading file: " + fname);
            }
            return keoCache;
        }

        static internal void AddTextArea(string nodeName, string descr, ref ConfigNode node)
        {
            int cnt = 0;
            while (descr != "")
            {
                int x = descr.IndexOf("\n");
                if (x == -1)
                    x = descr.Length;
                var s1 = descr.Substring(0, x);
                node.AddValue(nodeName + "-" + cnt.ToString(), s1);
                if (x < descr.Length - 1)
                    descr = descr.Substring(x + 1);
                else
                    descr = "";
                cnt++;
            }
        }

        static internal void LoadTextArea(string nodename, ref string descr, ref ConfigNode loadedConfigNode)
        {
            int cnt = 0;
            descr = "";
            string str = "";
            while (cnt >= 0)
            {
                if (loadedConfigNode.HasValue("description-" + cnt.ToString()))
                {
                    loadedConfigNode.TryGetValue("description-" + cnt.ToString(), ref str);
                    if (descr != "")
                        descr += "\n";
                    descr += str;
                    cnt++;
                }
                else
                    cnt = -1;
            }
        }

        static public bool SaveKeoCacheFile(KeoCacheCollection keoCache)
        {
            ConfigNode collectionData = KeoCacheCollection.SaveCollectionToConfigNode(keoCache);
            ConfigNode fileNode = new ConfigNode();
            fileNode.AddNode(FileIO.KEOCACHE_COLLECTION, collectionData);
            

            Log.Info("Creating directory: " + GetCacheDir);
            Directory.CreateDirectory(GetCacheDir);
            try
            {
                Log.Info("Saving to file: " + CollectionFileName(keoCache.keocacheCollectionData.collectionId));
                fileNode.Save(CollectionFileName(keoCache.keocacheCollectionData.collectionId));

                KeoCacheDriver.Instance.ReadAllCaches();

                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Error saving KeoCache Collection: " + ex.Message);
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
                Log.Error("Error saving KeoCache Collection: " + ex.Message);
            }
            return null;
        }
    }
}
