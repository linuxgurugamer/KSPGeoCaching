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

            GUILayout.BeginHorizontal();
            GUILayout.Label("Description:");
            activeGeoCacheData.description = GUILayout.TextArea(activeGeoCacheData.description, GUILayout.Height(75), GUILayout.Width(300));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("nextGeocacheId:");
            activeGeoCacheData.nextGeocacheId = GUILayout.TextField(activeGeoCacheData.nextGeocacheId);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            scrollPos3 = GUILayout.BeginScrollView(scrollPos3, GUILayout.MinHeight(60), GUILayout.Width(300), GUILayout.MaxHeight(200));
            foreach (var g in activeGeoCacheData.hints)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(g.hint))
                {

                }

                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Close"))
                visibleGeoCache = false;
            if (GUILayout.Button("Save and Close"))
            {
                visibleGeoCache = false;
            }
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }
    }
}
