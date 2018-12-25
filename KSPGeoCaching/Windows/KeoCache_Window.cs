using System;
using System.IO;
using System.Text.RegularExpressions;
using KSP.UI.Screens;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ToolbarControl_NS;
using ClickThroughFix;

namespace KeoCaching
{
    partial class KeoCacheDriver : MonoBehaviour
    {
        void KeoCache_Window(int windowId)
        {
            saveable = true;
            if (activeKeoCacheData.CacheVessel == null || activeKeoCacheData.keoCacheName == "")
            {
                saveable = false;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:"); //, activeKeoCacheData.keoCacheName == "" ? labelRed : labelNormal);
            SetBackground(activeKeoCacheData.keoCacheName.Length == 0);
            activeKeoCacheData.keoCacheName = GUILayout.TextField(activeKeoCacheData.keoCacheName);
            SetBackground(false);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Science Node Required:");
            activeKeoCacheData.scienceNodeRequired = GUILayout.TextField(activeKeoCacheData.scienceNodeRequired);
            GUILayout.EndHorizontal();
            if (activeKeoCacheData.body != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Celestial body:");
                GUILayout.TextField(activeKeoCacheData.body.displayName.Substring(0, activeKeoCacheData.body.displayName.Length - 2));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Latitude:");
                /*activeKeoCacheData.latitude = */
                GUILayout.TextField(Util.Latitude_Coordinates(activeKeoCacheData.latitude));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Longitude:");
                /*activeKeoCacheData.longitude = */
                GUILayout.TextField(Util.Longitude_Coordinates(activeKeoCacheData.longitude));
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("Description:");
            SetBackground(activeKeoCacheData.description.Length == 0);
            activeKeoCacheData.description = GUILayout.TextArea(activeKeoCacheData.description, GUILayout.Height(75), GUILayout.Width(300));
            SetBackground(false);
            GUILayout.EndHorizontal();

#if false
            GUILayout.BeginHorizontal();
            GUILayout.Label("nextKeocacheId:");
            activeKeoCacheData.nextKeocacheId = GUILayout.TextField(activeKeoCacheData.nextKeocacheId);
            GUILayout.EndHorizontal();
#endif
            saveable = (activeKeoCacheData.hints.Count > 0);

            if (activeKeoCacheData.hints.Count > 0)
            {
                int i = activeKeoCacheData.hints.Count - 1;
                double d1 = activeKeoCacheData.hints[i].absoluteDistance;
                if (activeKeoCacheData.hints[i].scale == Scale.km)
                    d1 *= 1000;

                double d2 = activeKeoCacheData.absoluteDistance;
                SetBackground(d1 <= d2);
            }

            double d = 0;
            Scale sc = 0;
            ShowGetProximity("KeoCache Vessel", ref d, ref activeKeoCacheData.absoluteDistance, ref sc);
            activeKeoCacheData.distanceFromCache = d;
            activeKeoCacheData.scale = sc;

            SetBackground(false);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Hints:");
            Hint toDelete = null;
            scrollPos3 = GUILayout.BeginScrollView(scrollPos3, GUILayout.MinHeight(60), GUILayout.Width(300), GUILayout.MaxHeight(200));

            for (int g1 = 0; g1 < activeKeoCacheData.hints.Count; g1++)
            {
                var g = activeKeoCacheData.hints[g1];
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(g.hintTitle + ", proximity: " + g.hintDistance.ToString("N0") + " " + g.scale.ToString()))
                {
                    activeHint = g.Copy();
                    origHint = g;

                    editHint = true;
                    visibleHint = true;
                }

                if (GUILayout.Button("X", redButton, GUILayout.Width(20)))
                {
                    toDelete = g;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
            if (toDelete != null)
            {
                activeKeoCacheData.hints.Remove(toDelete);
            }
            //GUILayoutOption buttonWidth = GUILayout.Width(150);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add Hint", activeKeoCacheData.hints.Count == 0?redButton:normalButton,  buttonWidth))
            {
                visibleHint = true;
                editHint = false;
                activeHint = new Hint();
            }
            GUILayout.FlexibleSpace();

            if (activeKeoCacheData.CacheVessel != null)
            {
                if (GUILayout.Button("Delete Cache Vessel", buttonWidth))
                {
                    activeKeoCacheData.DeleteVessel();
                }
            }
            else
            {
                if (GUILayout.Button("Place Cache Vessel", redButton, buttonWidth))
                {
                    LaunchEvent();
                    StartCoroutine(WaitForSpawn());
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Cancel", buttonWidth))
                visibleKeoCache = false;
            GUILayout.FlexibleSpace();


            if (!saveable)
                GUI.enabled = false;
            if (GUILayout.Button("Save and Close", buttonWidth))
            {
                visibleKeoCache = false;
                if (newKeoCacheData)
                {
                    Part part = activeKeoCacheData.CacheVessel.Parts[0];
                    KeoCacheModule partmod = part.FindModuleImplementing<KeoCacheModule>();
                    Log.Info("SaveAndClose");
                    Log.Info("part.craftID: " + part.craftID);
                    partmod.UpdateData(activeKeoCacheData.keocacheId, KeoCacheModule.AssignStatus.assigned, activeKeoCacheCollection.keocacheCollectionData.collectionId);

                    activeKeoCacheCollection.keoCacheDataList.Add(activeKeoCacheData);
                }
            }
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }
    }
}
