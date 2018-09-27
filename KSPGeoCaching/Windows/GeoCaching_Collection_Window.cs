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

        bool saveable;
        GUIStyle curLabelStyle, curbuttonStyle;
        void DisplayCollectionData()
        {
            curLabelStyle = labelNormal;
            if (activeGeoCacheCollection.geocacheCollectionData.name == "")
            {
                saveable = false;
                curLabelStyle = labelRed;
            }
            else saveable = true;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:", curLabelStyle);
            GUILayout.FlexibleSpace();
         
            string s = GUILayout.TextField(activeGeoCacheCollection.geocacheCollectionData.name, GUILayout.Width(300));
            if (!visibleActiveCollectionsReadOnly)
                activeGeoCacheCollection.geocacheCollectionData.name = s;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Title:");
            GUILayout.FlexibleSpace();
            s = GUILayout.TextField(activeGeoCacheCollection.geocacheCollectionData.title, GUILayout.Width(300));
            if (!visibleActiveCollectionsReadOnly)
                activeGeoCacheCollection.geocacheCollectionData.title = s;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Author:");
            GUILayout.FlexibleSpace();
        
            s = GUILayout.TextField(activeGeoCacheCollection.geocacheCollectionData.author, GUILayout.Width(300));
            if (!visibleActiveCollectionsReadOnly)
                activeGeoCacheCollection.geocacheCollectionData.author = s;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Description:");
            GUILayout.FlexibleSpace();
            s = GUILayout.TextArea(activeGeoCacheCollection.geocacheCollectionData.description, GUILayout.Height(75), GUILayout.Width(300));
            if (!visibleActiveCollectionsReadOnly)
                activeGeoCacheCollection.geocacheCollectionData.description = s;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Difficulty: ");
            GUILayout.FlexibleSpace();
            if (visibleActiveCollectionsReadOnly)
                GUI.enabled = false;
            if (GUILayout.Button("<", GUILayout.Width(20)))
            {
                if (activeGeoCacheCollection.geocacheCollectionData.difficulty > 0)
                    activeGeoCacheCollection.geocacheCollectionData.difficulty--;
            }
            GUI.enabled = true;
            GUILayout.Label(activeGeoCacheCollection.geocacheCollectionData.difficulty.ToString(), GUILayout.Width(100));
            if (visibleActiveCollectionsReadOnly)
                GUI.enabled = false;
            if (GUILayout.Button(">", GUILayout.Width(20)))
            {
                if (activeGeoCacheCollection.geocacheCollectionData.difficulty < Difficulty.Insane)
                    activeGeoCacheCollection.geocacheCollectionData.difficulty++;
            }
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("Required Mods:");
            GUILayout.FlexibleSpace();
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(60), GUILayout.Width(300));
            foreach (var s1 in activeGeoCacheCollection.geocacheCollectionData.requiredMods)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(s1);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
        }

        void DisplayCollectionButtons()
        {
            if (!visibleActiveCollectionsReadOnly)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Load Collection", buttonWidth))
                {
                    StartLoadDialog();
                }
                GUILayout.FlexibleSpace();
                if (!saveable)
                    GUI.enabled = false;
                if (GUILayout.Button("Save Collection", buttonWidth))
                {
                    FileIO.SaveGeocacheFile(activeGeoCacheCollection);
                }
                GUI.enabled = true;
                
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close", buttonWidth))
                CloseGeoCacheWindow();
            GUILayout.FlexibleSpace();

            //GUILayout.EndHorizontal();
            //GUILayout.BeginHorizontal();
            //GUILayout.FlexibleSpace();
            if (!visibleActiveCollectionsReadOnly)
            {
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
                if (GUILayout.Button(g.geoCacheName))
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
            saveable = true;
            if (loadDialog)
            {
                loadDialog = false;
                // following to avoid some nullrefs 
                Log.Info("Setting activeGeoCacheCollection to null 2");
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
            Log.Info("WaitForSpawn");
            Log.Info("part.craftID: "+ part.craftID);
            partmod.UpdateData(activeGeoCacheData.geocacheId, GeoCacheModule.AssignStatus.inprogress, activeGeoCacheCollection.geocacheCollectionData.collectionId);       

            //activeGeoCacheCollection.geocacheData.Add(activeGeoCacheData);
        }

    }
}
