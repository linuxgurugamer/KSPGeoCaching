using System;
using System.IO;
using System.Text.RegularExpressions;
using KSP.UI.Screens;
using System.Collections.Generic;
using UnityEngine;

using ToolbarControl_NS;
using ClickThroughFix;

namespace KSPGeoCaching
{


    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    class GeoCacheDriver : MonoBehaviour
    {
        FileSelection fsDialog = null;
        ToolbarControl toolbarControl;

        string dialogEntry = null;

        internal const string MODID = "geocache_NS";
        internal const string MODNAME = "Geo Caching";
        const string NORMAL_BUTTON_BIG = "KSPGeoCaching/PluginData/Icons/geo-38";
        const string NORMAL_BUTTON_SML = "KSPGeoCaching/PluginData/Icons/geo-24";
        bool visible = false;
        const int WIDTH = 800;
        const int HEIGHT = 200;
        Vector2 scrollPos, scrollPos2;

        Rect gcWinRect = new Rect((Screen.width - WIDTH) / 2, (Screen.height - HEIGHT) / 2, WIDTH, HEIGHT);

        static GeoCache activeGeoCache;

        void CreateButton()
        {
            toolbarControl = gameObject.AddComponent<ToolbarControl>();
            toolbarControl.AddToAllToolbars(Toggle, Toggle,
                ApplicationLauncher.AppScenes.SPACECENTER,
                MODID,
                "geocacheButton",
                NORMAL_BUTTON_BIG,
                NORMAL_BUTTON_SML,
                MODNAME
            );
        }

        void Start()
        {
            Log.Info("GeoCacheDriver.Start");
            CreateButton();
            activeGeoCache = new GeoCache();
        }

        void closeFSDialog()
        {
            Log.Info("GeoCacheDriver.closeFSDialog");
            fsDialog.Close();
            Destroy(fsDialog);
            fsDialog = null;
            dialogEntry = null;
            visible = false;
        }

        void Toggle()
        {
            visible = !visible;
            if (!visible)
            {
                closeFSDialog();
                fsDialog = null;
            }
        }

        void OnGUI()
        {
            if (fsDialog != null && fsDialog.done &&
                           fsDialog.SelectedDirectory != null && fsDialog.SelectedFile != null && dialogEntry == null)
            {
                if (fsDialog.SelectedDirectory != "" || fsDialog.SelectedFile != "")
                {
                    LoadGeocacheFile(fsDialog.SelectedDirectory + FileIO.DirSeperator + fsDialog.SelectedFile);
                }
                closeFSDialog();
            }
            if (visible)
            {
                if (HighLogic.CurrentGame.Parameters.CustomParams<GeoCacheOptions>().useKSPskin)
                    GUI.skin = HighLogic.Skin;

                gcWinRect = ClickThruBlocker.GUILayoutWindow(23874244, gcWinRect, GeoCaching_Window, "GeoCache Collection");
            }
        }

        void GeoCaching_Window(int windowId)
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:");
            GUILayout.FlexibleSpace();
            activeGeoCache.fileData.name = GUILayout.TextField(activeGeoCache.fileData.name, GUILayout.Width(300));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Title:");
            GUILayout.FlexibleSpace();
            activeGeoCache.fileData.title = GUILayout.TextField(activeGeoCache.fileData.title, GUILayout.Width(300));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Author:");
            GUILayout.FlexibleSpace();
            activeGeoCache.fileData.author = GUILayout.TextField(activeGeoCache.fileData.author, GUILayout.Width(300));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Description:");
            GUILayout.FlexibleSpace();
            activeGeoCache.fileData.description = GUILayout.TextArea(activeGeoCache.fileData.description, GUILayout.Height(75), GUILayout.Width(300));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Difficulty: ");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("<", GUILayout.Width(20)))
            {
                if (activeGeoCache.fileData.difficulty > 0)
                    activeGeoCache.fileData.difficulty--;
            }

            GUILayout.TextField(activeGeoCache.fileData.difficulty.ToString(), GUILayout.Width(100));
            if (GUILayout.Button(">", GUILayout.Width(20)))
            {
                if (activeGeoCache.fileData.difficulty < Difficulty.insane)
                    activeGeoCache.fileData.difficulty++;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("Required Mods:");
            GUILayout.FlexibleSpace();
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(60), GUILayout.Width(300));
            foreach (var s in activeGeoCache.fileData.requiredMods)
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
                fsDialog = gameObject.AddComponent<FileSelection>();
                fsDialog.SetExtensions(".txt");
                dialogEntry = null;
                fsDialog.SetSelectedDirectory(FileIO.GetCacheDir, false);
                fsDialog.startDialog();
            }
            if (GUILayout.Button("Close"))
                Toggle();
                
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
   
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("GeoCaches");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            scrollPos2 = GUILayout.BeginScrollView(scrollPos2, GUILayout.Height(60), GUILayout.Width(300));
            foreach (var g in activeGeoCache.geocacheData)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(g.name);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

 
        void LoadGeocacheFile(string f)
        {
            Log.Info("LoadGeocacheFile, f: " + f);
            activeGeoCache = FileIO.LoadGeocacheData(f);
        }
    }
}
