using System;
using System.IO;
using System.Text.RegularExpressions;
using KSP.UI.Screens;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ToolbarControl_NS;
using ClickThroughFix;

namespace KSPGeoCaching
{
    partial class GeoCacheDriver : MonoBehaviour
    {
        void StartLoadDialog()
        {
            fsDialog = gameObject.AddComponent<FileSelection>();
            fsDialog.SetExtensions(FileIO.SUFFIX);
            dialogEntry = null;
            fsDialog.SetSelectedDirectory(FileIO.GetCacheDir, false);
            fsDialog.startDialog();
        }
       
        
        void DisplayCollectionData()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:");
            GUILayout.FlexibleSpace();
            activeGeoCacheCollection.geocacheCollectionData.name = GUILayout.TextField(activeGeoCacheCollection.geocacheCollectionData.name, GUILayout.Width(300));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Title:");
            GUILayout.FlexibleSpace();
            activeGeoCacheCollection.geocacheCollectionData.title = GUILayout.TextField(activeGeoCacheCollection.geocacheCollectionData.title, GUILayout.Width(300));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Author:");
            GUILayout.FlexibleSpace();
            activeGeoCacheCollection.geocacheCollectionData.author = GUILayout.TextField(activeGeoCacheCollection.geocacheCollectionData.author, GUILayout.Width(300));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Description:");
            GUILayout.FlexibleSpace();
            activeGeoCacheCollection.geocacheCollectionData.description = GUILayout.TextArea(activeGeoCacheCollection.geocacheCollectionData.description, GUILayout.Height(75), GUILayout.Width(300));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Difficulty: ");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<", GUILayout.Width(20)))
            {
                if (activeGeoCacheCollection.geocacheCollectionData.difficulty > 0)
                    activeGeoCacheCollection.geocacheCollectionData.difficulty--;
            }

            GUILayout.Label(activeGeoCacheCollection.geocacheCollectionData.difficulty.ToString(), GUILayout.Width(100));
            if (GUILayout.Button(">", GUILayout.Width(20)))
            {
                if (activeGeoCacheCollection.geocacheCollectionData.difficulty < Difficulty.Insane)
                    activeGeoCacheCollection.geocacheCollectionData.difficulty++;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("Required Mods:");
            GUILayout.FlexibleSpace();
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(60), GUILayout.Width(300));
            foreach (var s in activeGeoCacheCollection.geocacheCollectionData.requiredMods)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(s);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
        }

        void DisplayCollectionButtons()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Load Collection", buttonWidth))
            {
                StartLoadDialog();
            }
            GUILayout.FlexibleSpace();
            if (activeGeoCacheCollection.geocacheCollectionData.name == "")
                GUI.enabled = false;
            if (GUILayout.Button("Save Collection", buttonWidth))
            {
                FileIO.SaveGeocacheFile(activeGeoCacheCollection);
            }
            GUI.enabled = true;
;
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close", buttonWidth))
                CloseGeoCacheWindow();
            GUILayout.FlexibleSpace();

            //GUILayout.EndHorizontal();
            //GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("New GeoCache", buttonWidth))
            {
                visibleGeoCache = true;
                activeGeoCacheData = new GeoCacheData();
                newGeoCacheData = true;
                //LaunchEvent();
                //StartCoroutine(WaitForSpawn());
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        void DisplayCollectionCaches()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("GeoCaches");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            scrollPos2 = GUILayout.BeginScrollView(scrollPos2, GUILayout.MinHeight(60), GUILayout.Width(300), GUILayout.MaxHeight(200));

            int indexToMove = -1, indexToMoveTo = -1;
            GeoCacheData itemToMove = null;
            GeoCacheData itemToDelete = null;
            for (int i = 0; i < activeGeoCacheCollection.geocacheData.Count; i++)
            {
                GeoCacheData g = activeGeoCacheCollection.geocacheData[i];
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(g.name))
                {
                    activeGeoCacheData = g;
                    newGeoCacheData = false;
                    visibleGeoCache = true;
                }

                GUILayout.BeginVertical(GUILayout.Width(15));
                GUILayout.Space(3);
                if (i == 0)
                    GUI.enabled = false;
                if (GUILayout.Button(upContent, arrowButtonStyle))
                {
                    indexToMove = i;
                    indexToMoveTo = i - 1;
                    itemToMove = g;
                }
                if (i < activeGeoCacheCollection.geocacheData.Count - 1)
                    GUI.enabled = true;
                else
                    GUI.enabled = false;
                if (GUILayout.Button(downContent, arrowButtonStyle))
                {
                    indexToMove = i;
                    indexToMoveTo = i + 1;
                    itemToMove = g;
                }
                GUILayout.EndVertical();
                GUI.enabled = true;
                if (GUILayout.Button("X", redButton, GUILayout.Width(20)))
                {
                    itemToDelete = g;
                }

                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
            if (itemToDelete != null)
            {
                activeGeoCacheCollection.geocacheData.Remove(itemToDelete);
                itemToDelete.DeleteVessel();
                itemToDelete.hints.Clear();
            }
            if (indexToMove != -1)
            {
                activeGeoCacheCollection.geocacheData.Remove(itemToMove);
                activeGeoCacheCollection.geocacheData.Insert(indexToMoveTo, itemToMove);
            }
        }

        void GeoCaching_Collection_Window(int windowId)
        {
            if (loadDialog)
            {
                loadDialog = false;
                // following to avoid some nullrefs 
                activeGeoCacheCollection = new GeoCacheCollection();
                StartLoadDialog();
            }
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            DisplayCollectionData();            
           // GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            DisplayCollectionCaches();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            DisplayCollectionButtons();
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }

        private IEnumerator WaitForSpawn()
        {
           // activeGeoCacheData = new GeoCacheData();
            while (VesselSpawn.instance.spawnedVessel == null)
                yield return null;
            activeGeoCacheData.CacheVessel = VesselSpawn.instance.spawnedVessel;
            Part part = activeGeoCacheData.CacheVessel.Parts[0];
            GeoCacheModule partmod = part.FindModuleImplementing<GeoCacheModule>();
            partmod.geocachId = activeGeoCacheData.geocachId;
            partmod.collectionId = activeGeoCacheCollection.geocacheCollectionData.collectionId;
            //activeGeoCacheCollection.geocacheData.Add(activeGeoCacheData);
        }

    }
}
