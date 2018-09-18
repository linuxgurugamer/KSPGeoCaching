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
        void GeoCache_Window(int windowId)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:");
            activeGeoCacheData.name = GUILayout.TextField(activeGeoCacheData.name);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Science Node Required:");
            activeGeoCacheData.scienceNodeRequired = GUILayout.TextField(activeGeoCacheData.scienceNodeRequired);
            GUILayout.EndHorizontal();
            if (activeGeoCacheData.body != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Celestial body:");
                GUILayout.TextField(activeGeoCacheData.body.displayName.Substring(0, activeGeoCacheData.body.displayName.Length - 2));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Latitude:");
                /*activeGeoCacheData.latitude = */
                GUILayout.TextField(Util.Latitude_Coordinates(activeGeoCacheData.latitude));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Longitude:");
                /*activeGeoCacheData.longitude = */
                GUILayout.TextField(Util.Longitude_Coordinates(activeGeoCacheData.longitude));
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("Description:");
            activeGeoCacheData.description = GUILayout.TextArea(activeGeoCacheData.description, GUILayout.Height(75), GUILayout.Width(300));
            GUILayout.EndHorizontal();

#if false
            GUILayout.BeginHorizontal();
            GUILayout.Label("nextGeocacheId:");
            activeGeoCacheData.nextGeocacheId = GUILayout.TextField(activeGeoCacheData.nextGeocacheId);
            GUILayout.EndHorizontal();
#endif
            GUILayout.BeginHorizontal();
            GUILayout.Label("Hints:");
            Hint toDelete = null;
            scrollPos3 = GUILayout.BeginScrollView(scrollPos3, GUILayout.MinHeight(60), GUILayout.Width(300), GUILayout.MaxHeight(200));
            for (int g1 = 0; g1 < activeGeoCacheData.hints.Count; g1++)
            {
                var g = activeGeoCacheData.hints[g1];
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(g.hintTitle + ", proximity: " + g.distance.ToString("N0") + " " + g.scale.ToString()))
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
                activeGeoCacheData.hints.Remove(toDelete);
            }
            //GUILayoutOption buttonWidth = GUILayout.Width(150);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add Hint", buttonWidth))
            {
                visibleHint = true;
                editHint = false;
                activeHint = new Hint();
            }
            GUILayout.FlexibleSpace();

            if (activeGeoCacheData.CacheVessel != null)
            {
                if (GUILayout.Button("Delete Cache Vessel", buttonWidth))
                {
                    activeGeoCacheData.DeleteVessel();
                }
            }
            else
            {
                if (GUILayout.Button("Place Cache Vessel", buttonWidth))
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
                visibleGeoCache = false;
            GUILayout.FlexibleSpace();
            if (activeGeoCacheData.CacheVessel == null || activeGeoCacheData.name == "")
                GUI.enabled = false;
            if (GUILayout.Button("Save and Close", buttonWidth))
            {
                visibleGeoCache = false;
                if (newGeoCacheData)
                    activeGeoCacheCollection.geocacheData.Add(activeGeoCacheData);
            }
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }
    }
}
