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
        void SpaceCenterMenuWindow()
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

             GUILayout.BeginHorizontal();
            if (GUILayout.Button("Show TravelBugs"))
            {
                visibleTravelBug = true;
                visibleMenu = false;
            }
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("KerbalX"))
            {
                visibleMenu = false;
                showKerbalX = true;
            }
            GUILayout.EndHorizontal();

#if false
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Available KeoCache Collections Window"))
            {
                visibleMenu = false;
                visibleAllCollections = true;
                //visibleActiveCollectionsReadOnly = false;
                //useKeoCache = true;
                //activeKeoCacheCollection = new KeoCacheCollection();
                //StartLoadDialog();
            }
            GUILayout.EndHorizontal();
#endif

            GUILayout.EndVertical();

        }

        void EditorMenuWindow()
        {
            SpaceCenterMenuWindow();
        }

        void FlightWindow()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("New KeoCache Collection"))
            {
                activeKeoCacheCollection = new KeoCacheCollection();
                visibleEditCollection = true;
                visibleMenu = false;
                editInProgress = true;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Select KeoCache for editing"))
            {
                //visibleEditCollection = true;
                visibleMenu = false;
                visibleAllCollections = true;
                // need to get the active collections window to only display collections not active, and a way to select one
                selectForEdit = true;                
            }
            GUILayout.EndHorizontal();

            if (activeKeoCacheCollection != null)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Edit: " + activeKeoCacheCollection.keocacheCollectionData.name))
                {
                    visibleEditCollection = true;
                    visibleMenu = false;
                }
                GUILayout.EndHorizontal();
            }
        }

        void KeoCaching_Menu_Window(int windowId)
        {
            if (HighLogic.LoadedSceneIsFlight)
                FlightWindow();
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
                SpaceCenterMenuWindow();
            if (HighLogic.LoadedScene == GameScenes.EDITOR)
                EditorMenuWindow();

            GUI.DragWindow();
        }

    }
}