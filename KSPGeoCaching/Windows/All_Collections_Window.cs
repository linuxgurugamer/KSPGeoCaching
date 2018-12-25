using System.IO;
using System.Text.RegularExpressions;
using KSP.UI.Screens;
using System;
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
        internal bool selectCollection = false;
        internal bool selectForEdit = false;
        
        internal KeoTravelBugModule activeBug;

        void SelectKeoCacheForBug(string path)
        {
            KeoCacheCollection s1 = FileIO.LoadKeoCacheData(path);

            if (s1 != null)
            {
                KeoCacheDriver.activeKeoCacheCollection = s1;
                activeBug.SetCollection(s1.keocacheCollectionData.collectionId, s1);

                //Log.Info("ActiveCollectionsWindow, collectionId: " + s1.keocacheCollectionData.collectionId + ",  g.Key: " + g.Key);
                //g.Value.keocacheCollectionData.assignedTravelBug = activeBug.travelbugId;

                selectCollection = selectForEdit = false;
                KeoCacheDriver.Instance.visibleAllCollections = false;
            }
            else
            {
                ScreenMessages.PostScreenMessage("KeoCache collection not available anymore", 10f, ScreenMessageStyle.UPPER_CENTER);
                ReadAllCaches();
            }
        }

        void All_Collections_Window(int id)
        {
            //string toDelete = null;
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            scrollPosActiveCollections = GUILayout.BeginScrollView(scrollPosActiveCollections, GUILayout.MinHeight(60), GUILayout.Width(300), GUILayout.MaxHeight(200));
#if true
            FindKeoCaches();
            foreach (var g in availableCaches)
            {
                if (selectCollection)
                {
                    if (g.Value.numCaches < 0)
                        continue;
                }
                if (!KeoCacheUsed(g.Value.id))
                {
                    Log.Info("g.value.id: " + g.Value.id);
#if false
                if (selectForEdit && g.Value.keocacheCollectionData.assignedTravelBug != "")
                    continue;
#endif

                    GUILayout.BeginHorizontal();


#if false
                string s = g.Value.name;
                if (g.Value.keocacheCollectionData.assignedTravelBug != "")
                {
                    s += " on vessel ";
                    // need to find vessel from the TravelBugs list
                }
#endif
                    if (GUILayout.Button(g.Value.name))
                    {
                        if (selectForEdit)
                            editInProgress = true;
                        visibleEditCollection = true;
                        visibleAllCollections = false;
                        visibleActiveCollectionsReadOnly = !selectForEdit;
                        selectCollection = selectForEdit = false;

                        KeoCacheCollection s1 = FileIO.LoadKeoCacheData(g.Value.path);
                        KeoCacheDriver.activeKeoCacheCollection = s1;
                        KeoCacheDriver.activeKeoCacheCollection.fullPath = g.Value.path;
                    }

#if false
                if (g.Value.keocacheCollectionData.assignedTravelBug ==  null || g.Value.keocacheCollectionData.assignedTravelBug == "")
#endif
                    {
                        if (selectCollection)
                        {
                            if (GUILayout.Button("Select", GUILayout.Width(60)))
                            {
                                SelectKeoCacheForBug(g.Value.path);

                            }
                        }
#if false
                    if (GUILayout.Button("X", redButton, GUILayout.Width(20)))
                    {
                        toDelete = g.Key;
                    }
#endif
                    }
                    GUILayout.EndHorizontal();
                }
            }
#endif
                GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
#if false
            if (toDelete != null)
            {
                Log.Info("toDelete: " + toDelete);
                KeoScenario.activeKeoCacheCollections.Remove(toDelete);
            }
#endif
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Import"))
            {
                //StartLoadDialog();
                ShowImportWindow();
            }

            if (GUILayout.Button("Close"))
            {
                visibleAllCollections = false;
                //editInProgress = false;
                visibleMenu = true;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUI.DragWindow();
        }
    }

}
