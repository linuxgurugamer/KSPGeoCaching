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


    [KSPAddon(KSPAddon.Startup.Flight, false)]
    partial class GeoCacheDriver : MonoBehaviour
    {
        internal static GeoCacheDriver Instance;

        FileSelection fsDialog = null;
        ToolbarControl toolbarControl;

        string dialogEntry = null;

        internal const string MODID = "geocache_NS";
        internal const string MODNAME = "Geo Caching";
        const string NORMAL_BUTTON_BIG = "KSPGeoCaching/PluginData/Icons/geo-38";
        const string NORMAL_BUTTON_SML = "KSPGeoCaching/PluginData/Icons/geo-24";

        internal bool visibleMenu = false;
        internal bool visibleCollection = false;
        internal bool loadDialog = false;
        internal bool visibleDelete = false;
        internal bool visibleGeoCache = false;
        internal bool visibleHint = false;
        internal bool useGeoCache = false;

        const int COL_WIDTH = 725;
        const int COL_HEIGHT = 200;
        const int GEO_WIDTH = 400;
        const int GEO_HEIGHT = 200;
        const int MENU_WIDTH = 200;
        const int MENU_HEIGHT = 100;
        const int HINT_WIDTH = 400;
        const int HINT_HEIGHT = 200;
        Vector2 scrollPos, scrollPos2, scrollPos3;

        Rect collectionWinRect = new Rect((Screen.width - COL_WIDTH) / 2, (Screen.height - COL_HEIGHT) / 2, COL_WIDTH, COL_HEIGHT);
        Rect geocacheWinRect = new Rect((Screen.width - GEO_WIDTH) / 2, (Screen.height - GEO_HEIGHT) / 2, GEO_WIDTH, GEO_HEIGHT);
        Rect menuWinRect = new Rect((Screen.width - MENU_WIDTH) / 2, (Screen.height - MENU_HEIGHT) / 2, MENU_WIDTH, MENU_HEIGHT);
        Rect hintWinRect = new Rect((Screen.width - HINT_WIDTH) / 2, (Screen.height - HINT_HEIGHT) / 2, HINT_WIDTH, HINT_HEIGHT);


        int collectionWinID;
        int geocacheWinID;
        int menuWinID;
        int hintWinID;

        internal static GeoCacheCollection activeGeoCacheCollection = null;
        internal static GeoCacheData activeGeoCacheData;
        internal bool newGeoCacheData;


        static Texture2D upArrow;
        static Texture2D downArrow;
        internal GUIContent upContent;
        internal GUIContent downContent;

        void CreateButton()
        {
            toolbarControl = gameObject.AddComponent<ToolbarControl>();
            toolbarControl.AddToAllToolbars(Toggle, Toggle,
                ApplicationLauncher.AppScenes.FLIGHT,
                MODID,
                "geocacheButton",
                NORMAL_BUTTON_BIG,
                NORMAL_BUTTON_SML,
                MODNAME
            );
        }

        void Start()
        {
            Instance = this;
            Log.Info("GeoCacheDriver.Start");
            CreateButton();
            //if (activeGeoCacheCollection == null)
            //    activeGeoCacheCollection = new GeoCacheCollection();

            //
            // Dynamically generate the window IDs based off of the time
            //
            collectionWinID = (DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second) * 1000 + DateTime.Now.Millisecond;
            geocacheWinID = (DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second) * 1000 + DateTime.Now.Millisecond + 1;
            menuWinID = (DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second) * 1000 + DateTime.Now.Millisecond + 2;
            hintWinID = (DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second) * 1000 + DateTime.Now.Millisecond + 3;

            GameEvents.onHideUI.Add(this.HideUI);
            GameEvents.onShowUI.Add(this.ShowUI);
            GameEvents.onGamePause.Add(this.HideUIWhenPaused);
            GameEvents.onGameUnpause.Add(this.ShowUIwhenPaused);

            if (upArrow == null)
            {
                upArrow = new Texture2D(2, 2);
                if (ToolbarControl.LoadImageFromFile(ref upArrow, "GameData/KSPGeoCaching/PluginData/Icons/up"))
                    upContent = new GUIContent("", upArrow, "");
                else
                    Log.Error("Unable to load up arrow");
            }
            if (downArrow == null)
            {
                downArrow = new Texture2D(2, 2);
                if (ToolbarControl.LoadImageFromFile(ref downArrow, "GameData/KSPGeoCaching/PluginData/Icons/down"))
                    downContent = new GUIContent("", downArrow, "");
                else
                    Log.Error("Unable to load down arrow");
            }

        }

        void closeFSDialog()
        {
            Log.Info("GeoCacheDriver.closeFSDialog");
            fsDialog.Close();
            Destroy(fsDialog);
            fsDialog = null;
            dialogEntry = null;
        }

        void Toggle()
        {
            visibleMenu = !visibleMenu;
            return;

        }
        void CloseGeoCacheWindow()
        {
            visibleCollection = false;
            if (fsDialog != null)
            {
                closeFSDialog();
                fsDialog = null;
            }
        }

        public void OnDestroy()
        {
            GameEvents.onHideUI.Remove(this.HideUI);
            GameEvents.onShowUI.Remove(this.ShowUI);
            GameEvents.onGamePause.Remove(this.HideUIWhenPaused);
            GameEvents.onGameUnpause.Remove(this.ShowUIwhenPaused);
            Instance = null;
            Log.Info("OnDestroy");
        }
        /// <summary>
        /// Hides all user interface elements.
        /// </summary>
        bool hideUI = false;
        bool hideUIwhenPaused = false;
        public void HideUI()
        {
            hideUI = true;
        }
        void ShowUI()
        {
            hideUI = false;
        }
        public void HideUIWhenPaused()
        {
            if (HighLogic.CurrentGame.Parameters.CustomParams<GeoCacheOptions>().hideWhenPaused)
                hideUIwhenPaused = true;
        }
        void ShowUIwhenPaused()
        {
            hideUIwhenPaused = false;
        }

        static GUIStyle redButton = null;
        static GUILayoutOption buttonWidth;
        static GUIStyle arrowButtonStyle = null;
        void OnGUI()
        {
            if (redButton == null)
            {
                redButton = new GUIStyle(GUI.skin.button);
                redButton.normal.textColor = Color.red;
                buttonWidth = GUILayout.Width(150);

                arrowButtonStyle = new GUIStyle(GUI.skin.button);
                arrowButtonStyle.padding = new RectOffset(0, 0, 0, 0);
                arrowButtonStyle.fixedWidth = 15;
                arrowButtonStyle.fixedHeight = 12;

            }

            if (hideUI || hideUIwhenPaused)
                return;

            if (fsDialog != null && fsDialog.done &&
                           fsDialog.SelectedDirectory != null && fsDialog.SelectedFile != null && dialogEntry == null)
            {
                if (fsDialog.SelectedDirectory != "" || fsDialog.SelectedFile != "")
                {
                    GeoCacheDriver.activeGeoCacheCollection = FileIO.LoadGeoCacheData(fsDialog.SelectedDirectory + FileIO.DirSeperator + fsDialog.SelectedFile);
                }
                closeFSDialog();
                if (useGeoCache && activeGeoCacheCollection != null)
                {
                    InitiatePlay();
                }
            }
            if (visibleMenu)
            {
                if (HighLogic.CurrentGame.Parameters.CustomParams<GeoCacheOptions>().useKSPskin)
                    GUI.skin = HighLogic.Skin;

                menuWinRect = ClickThruBlocker.GUILayoutWindow(menuWinID, menuWinRect, GeoCaching_Menu_Window, "KSP GeoCache");
            }
            if (visibleCollection && !visibleGeoCache)
            {
                if (HighLogic.CurrentGame.Parameters.CustomParams<GeoCacheOptions>().useKSPskin)
                    GUI.skin = HighLogic.Skin;

                collectionWinRect = ClickThruBlocker.GUILayoutWindow(collectionWinID, collectionWinRect, GeoCaching_Collection_Window, "GeoCache Collection");
            }
            if (visibleGeoCache)
            {
                if (HighLogic.CurrentGame.Parameters.CustomParams<GeoCacheOptions>().useKSPskin)
                    GUI.skin = HighLogic.Skin;
                geocacheWinRect = ClickThruBlocker.GUILayoutWindow(geocacheWinID, geocacheWinRect, GeoCache_Window, "GeoCache");
            }
            if (visibleHint)
            {
                // Only set the following two IF the hint window hasn't been moved
                // Once it gets moved, then use the saved coordinates
                hintWinRect.x = geocacheWinRect.x + geocacheWinRect.width;
                hintWinRect.y = geocacheWinRect.y;
                hintWinRect = ClickThruBlocker.GUILayoutWindow(hintWinID, hintWinRect, Hint_Window, "Hint");
            }
        }

        void GeoCaching_Menu_Window(int windowId)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("KerbalX"))
            {
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load GeoCache for editing"))
            {
                visibleCollection = true;
                visibleMenu = false;
                loadDialog = true;
            }
            GUILayout.EndHorizontal();

            if (activeGeoCacheCollection != null)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Edit GeoCache Collection"))
                {
                    visibleCollection = true;
                    visibleMenu = false;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Delete GeoCache Collection"))
            {
                visibleDelete = true;
                visibleMenu = false;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("New GeoCache Collection"))
            {
                activeGeoCacheCollection = new GeoCacheCollection();
                visibleCollection = true;
                visibleMenu = false;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (!useGeoCache)
            {
                if (GUILayout.Button("Use GeoCache Collection"))
                {
                    activeGeoCacheCollection = new GeoCacheCollection();
                    visibleMenu = false;
                    useGeoCache = true;
                    activeGeoCacheCollection = new GeoCacheCollection();
                    StartLoadDialog();
                }
            }
            else
            {
                if (GUILayout.Button("Unload current GeoCache Collection"))
                {
                    StopCoroutine(GeoCacheUpdate());
                    activeGeoCacheCollection = null;
                    activeGeoCacheData = null;
                }
            }
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }


        const string PluginPath = "/GameData/KSPGeoCaching/PluginData/Ships/";
        const string GeoCacheCraft = "GeoCache.craft";
        public void LaunchEvent()
        {
            if (VesselSpawn.instance == null)
                VesselSpawn.instance = new VesselSpawn();
            VesselSpawn.instance.StartVesselSpawn(Environment.CurrentDirectory + PluginPath + GeoCacheCraft);
        }

    }
}
