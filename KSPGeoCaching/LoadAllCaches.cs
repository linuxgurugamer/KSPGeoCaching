using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KeoCaching
{
    partial class KeoCacheDriver : MonoBehaviour   
    {
       // bool allCollectionsLoaded = false;

        public class AvailableCache
        {
            public string id;
            public string name;
            public string path;
            public int numCaches;
        }
        internal Dictionary<string, AvailableCache> availableCaches;

        internal void ReadAllCaches()
        {
            Log.Info("ReadAllCaches");
            availableCaches = new Dictionary<string, AvailableCache>();

            StartCoroutine(ReadAllCachesCoroutine());
            //allCollectionsLoaded = true;
        }

        IEnumerator ReadAllCachesCoroutine()
        {

            yield return null;
            Log.Info("CacheDir: " + FileIO.GetCacheDir + ",   suffix: " + FileIO.SUFFIX);
            string[] files = Directory.GetFiles(FileIO.GetCacheDir, "*" + FileIO.SUFFIX);
            foreach (var f in files)
            {
                Log.Info("File: " + f);
                KeoCacheCollection s = FileIO.LoadKeoCacheData(f);
                if (s != null && s.keocacheCollectionData != null)
                {
                    AvailableCache ac = new AvailableCache();

                    ac.path = f;
                    ac.id = s.keocacheCollectionData.collectionId;
                    ac.name = s.keocacheCollectionData.name;
                    ac.numCaches = s.keoCacheDataList.Count;
                    Log.Info("ReadAllCachesCoroutine, id: " + ac.id + ", name: " + ac.name);
                    availableCaches.Add(ac.name, ac);
                }
                else
                {
                    if (s == null)
                        Log.Info("s is null");
                    if (s.keocacheCollectionData == null)
                        Log.Info("s.keocacheCollectionData is null");
                }

#if false
                KeoCacheCollection s = FileIO.LoadKeoCacheData(f);
                if (s != null && s.keocacheCollectionData != null)
                {
                    KeoCacheCollection data;
                    if (KeoScenario.activeKeoCacheCollections.TryGetValue(s.keocacheCollectionData.collectionId, out data))
                    {
                        if (data.keocacheCollectionData.assignedTravelBug != "")
                            continue;
                        KeoScenario.activeKeoCacheCollections.Remove(s.keocacheCollectionData.collectionId);
                    }
                    KeoScenario.activeKeoCacheCollections.Add(s.keocacheCollectionData.collectionId, s);

                }
#endif
                //yield return null;
                yield return null;
            }
            
        }
    }
}
