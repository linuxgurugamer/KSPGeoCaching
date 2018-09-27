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

namespace KSPGeoCaching
{
    
    partial class GeoCacheDriver : MonoBehaviour
    {
        
        void Active_Collections_Window(int id)
        {
            Guid toDelete = Guid.Empty;
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            scrollPosActiveCollections = GUILayout.BeginScrollView(scrollPosActiveCollections, GUILayout.MinHeight(60), GUILayout.Width(300), GUILayout.MaxHeight(200));
            foreach (var g in GeoScenario.activeGeoCacheCollections)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(g.Value.geocacheCollectionData.name))
                {
                    visibleEditCollection = true;
                    visibleActiveCollectionsReadOnly = true;
                    GeoCacheDriver.activeGeoCacheCollection = g.Value;
                }

                if (GUILayout.Button("X", redButton, GUILayout.Width(20)))
                {
                    toDelete = g.Key;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
            if (toDelete != Guid.Empty)
            {
                GeoScenario.activeGeoCacheCollections.Remove(toDelete);
            }
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                StartLoadDialog();
            }

            if (GUILayout.Button("Close"))
            {
                visibleActiveCollections = false;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUI.DragWindow();
        }
    }

}
