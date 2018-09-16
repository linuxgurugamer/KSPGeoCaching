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

            GUILayout.TextField(activeGeoCacheCollection.geocacheCollectionData.difficulty.ToString(), GUILayout.Width(100));
            if (GUILayout.Button(">", GUILayout.Width(20)))
            {
                if (activeGeoCacheCollection.geocacheCollectionData.difficulty < Difficulty.insane)
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

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Load GeoCache"))
            {
                StartLoadDialog();
            }
            if (GUILayout.Button("Close"))
                CloseGeoCacheWindow();

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("GeoCaches");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            scrollPos2 = GUILayout.BeginScrollView(scrollPos2, GUILayout.MinHeight(60), GUILayout.Width(300), GUILayout.MaxHeight(200));
            foreach (var g in activeGeoCacheCollection.geocacheData)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(g.name))
                {
                    activeGeoCacheData = g;
                    visibleGeoCache = true;
                }

                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("New GeoCache"))
            {
                visibleGeoCache = true;
                LaunchEvent();
                StartCoroutine(WaitForSpawn());
                

            }
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            if (activeGeoCacheCollection.geocacheCollectionData.name == "")
                GUI.enabled = false;
            if (GUILayout.Button("Save Collection"))
            {
                FileIO.SaveGeocacheFile(activeGeoCacheCollection);
            }
            GUI.enabled = true;
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

        private IEnumerator WaitForSpawn()
        {
            activeGeoCacheData = new GeoCacheData();
            while (VesselSpawn.instance.spawnedVessel == null)
                yield return null;
            activeGeoCacheData.vessel = VesselSpawn.instance.spawnedVessel;
            Part part = activeGeoCacheData.vessel.Parts[0];
            GeoCacheModule partmod = part.FindModuleImplementing<GeoCacheModule>();
            partmod.geocachId = activeGeoCacheData.geocachId;
            partmod.collectionId = activeGeoCacheCollection.geocacheCollectionData.collectionId;
            activeGeoCacheCollection.geocacheData.Add(activeGeoCacheData);
        }

    }
}
