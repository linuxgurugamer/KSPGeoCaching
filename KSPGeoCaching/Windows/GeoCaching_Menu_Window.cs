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
        void SpaceCenterMenuWindow()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("KerbalX"))
            {
                visibleMenu = false;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Active GeoCache Collections Window"))
            {
                visibleMenu = false;
                visibleActiveCollections = true;
                visibleActiveCollectionsReadOnly = false;
                //useGeoCache = true;
                //activeGeoCacheCollection = new GeoCacheCollection();
                //StartLoadDialog();
            }
            GUILayout.EndHorizontal();



            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Show TravelBugs"))
            {
                visibleTravelBug = true;
                visibleMenu = false;
            }
            GUILayout.EndHorizontal();
        }

        void FlightWindow()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("New GeoCache Collection"))
            {
                activeGeoCacheCollection = new GeoCacheCollection();
                visibleEditCollection = true;
                visibleMenu = false;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load GeoCache for editing"))
            {
                visibleEditCollection = true;
                visibleMenu = false;
                loadDialog = true;
            }
            GUILayout.EndHorizontal();

            if (activeGeoCacheCollection != null)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Edit GeoCache Collection"))
                {
                    visibleEditCollection = true;
                    visibleMenu = false;
                }
                GUILayout.EndHorizontal();
            }
        }

        void GeoCaching_Menu_Window(int windowId)
        {
            if (HighLogic.LoadedSceneIsFlight)
                FlightWindow();
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
                SpaceCenterMenuWindow();
  
            GUI.DragWindow();
        }

    }
}