using System;
using System.Collections.Generic;
using KSP.UI.Screens;
using UnityEngine;

using ToolbarControl_NS;
using ClickThroughFix;

namespace KeoCaching
{


    [KSPAddon(KSPAddon.Startup.FlightEditorAndKSC, false)]
    partial class KeoCacheDriver : MonoBehaviour
    {
        internal static KeoCacheDriver Instance;

        FileSelection fsDialog = null;
        ToolbarControl toolbarControl;

        string dialogEntry = null;

        internal const string MODID = "keocache_NS";
        internal const string MODNAME = "KeoCaching";
        const string NORMAL_BUTTON_BIG = "KeoCaching/PluginData/Icons/keo-38";
        const string NORMAL_BUTTON_SML = "KeoCaching/PluginData/Icons/keo-24";


        const string PluginPath = "/GameData/KeoCaching/PluginData/Ships/";
        const string KeoCacheCraft = "KeoCache.craft";

        internal bool visibleMenu = false;
        internal static bool editInProgress = false;
        internal bool visibleEditCollection = false;
        internal bool visibleAllCollections = false;
        internal bool visibleActiveCollectionsReadOnly = false;
        internal bool loadDialog = false;
        internal bool visibleDelete = false;
        internal bool visibleKeoCache = false;
        internal bool visibleHint = false;
        internal bool useKeoCache = false;
        internal bool visibleTravelBug = false;
        internal bool visibleImportWindow = false;
        internal bool showKerbalX = false;

        const int COL_WIDTH = 725;
        const int COL_HEIGHT = 200;
        const int KEO_WIDTH = 400;
        const int KEO_HEIGHT = 200;
        const int MENU_WIDTH = 300;
        const int MENU_HEIGHT = 100;
        internal const int HINT_WIDTH = 400;
        internal const int HINT_HEIGHT = 200;
        internal const int CURRENT_HINT_HEIGHT = 150;
        const int TRAVELBUG_WIDTH = 625;
        const int TRAVELBUG_HEIGHT = 200;
        const int ACTIVECOLLECTIONS_WIDTH = 300;
        const int ACTIVECOLLECTIONS_HEIGHT = 200;
        const int IMPORT_WIDTH = 600;
        const int IMPORT_HEIGHT = 150;

        Vector2 scrollPos, scrollPos2, scrollPos3, scrollPosActiveCollections;

        Rect collectionWinRect = new Rect((Screen.width - COL_WIDTH) / 2, (Screen.height - COL_HEIGHT) / 2, COL_WIDTH, COL_HEIGHT);
        Rect collectionROWinRect = new Rect((Screen.width - COL_WIDTH / 3*2) / 2, (Screen.height - COL_HEIGHT) / 2, COL_WIDTH / 3*2, COL_HEIGHT);
        Rect keocacheWinRect = new Rect((Screen.width - KEO_WIDTH) / 2, (Screen.height - KEO_HEIGHT) / 2, KEO_WIDTH, KEO_HEIGHT);
        Rect menuWinRect = new Rect((Screen.width - MENU_WIDTH) / 2, (Screen.height - MENU_HEIGHT) / 2, MENU_WIDTH, MENU_HEIGHT);
        Rect hintWinRect = new Rect((Screen.width - HINT_WIDTH) / 2, (Screen.height - HINT_HEIGHT) / 2, HINT_WIDTH, HINT_HEIGHT);
        Rect travelbugWinRect = new Rect((Screen.width - TRAVELBUG_WIDTH) / 2, (Screen.height - TRAVELBUG_HEIGHT) / 2, TRAVELBUG_WIDTH, TRAVELBUG_HEIGHT);
        Rect allCollectionsWinRect = new Rect((Screen.width - ACTIVECOLLECTIONS_WIDTH) / 2, (Screen.height - ACTIVECOLLECTIONS_HEIGHT) / 2, ACTIVECOLLECTIONS_WIDTH, ACTIVECOLLECTIONS_HEIGHT);
        Rect importwinRect = new Rect((Screen.width - IMPORT_WIDTH) / 2, (Screen.height - IMPORT_HEIGHT) / 2, IMPORT_WIDTH, IMPORT_HEIGHT);
        Rect kerbalXWinRect = new Rect(0, 0, 100, 100);

        int collectionWinID;
        int KeocacheWinID;
        int menuWinID;
        int hintWinID;
        int travelbugWinID;
        int importWinID;
        int activeCollectionsID;
        int kerbalXWinID;

        internal static KeoCacheCollection activeKeoCacheCollection;
        internal static KeoCacheData activeKeoCacheData;
        internal bool newKeoCacheData;

        // KerbalXInterface kerbalXInterface = new KerbalXInterface();
        internal static Dictionary<string, TravelBugEntry> activeTravelBugs;

        static Texture2D upArrow;
        static Texture2D downArrow;
        static internal GUIContent upContent;
        static internal GUIContent downContent;

        void CreateButton()
        {
            toolbarControl = gameObject.AddComponent<ToolbarControl>();
            toolbarControl.AddToAllToolbars(Toggle, Toggle,
                ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.SPACECENTER | ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH,
                MODID,
                "keocacheButton",
                NORMAL_BUTTON_BIG,
                NORMAL_BUTTON_SML,
                MODNAME
            );
        }

        void Start()
        {
            Instance = this;
            Log.Info("KeoCacheDriver.Start");
            CreateButton();
            //if (activeKeoCacheCollection == null)
            //    activeKeoCacheCollection = new KeoCacheCollection();

            //
            // Dynamically generate the window IDs based off of the time
            //
            collectionWinID = (DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second) * 1000 + DateTime.Now.Millisecond;
            KeocacheWinID = (DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second) * 1000 + DateTime.Now.Millisecond + 1;
            menuWinID = (DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second) * 1000 + DateTime.Now.Millisecond + 2;
            hintWinID = (DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second) * 1000 + DateTime.Now.Millisecond + 3;
            travelbugWinID= (DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second) * 1000 + DateTime.Now.Millisecond + 4;
            importWinID = (DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second) * 1000 + DateTime.Now.Millisecond + 5;
            activeCollectionsID = (DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second) * 1000 + DateTime.Now.Millisecond + 6;
            kerbalXWinID = (DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second) * 1000 + DateTime.Now.Millisecond + 7;

            GameEvents.onHideUI.Add(this.HideUI);
            GameEvents.onShowUI.Add(this.ShowUI);
            GameEvents.onGamePause.Add(this.HideUIWhenPaused);
            GameEvents.onGameUnpause.Add(this.ShowUIwhenPaused);
            GameEvents.OnVesselRollout.Add(OnVesselRollout);
            GameEvents.onLevelWasLoaded.Add(OnLevelLoaded);

            if (upArrow == null)
            {
                upArrow = new Texture2D(2, 2);
                if (ToolbarControl.LoadImageFromFile(ref upArrow, "GameData/KeoCaching/PluginData/Icons/up"))
                    upContent = new GUIContent("", upArrow, "");
                else
                    Log.Error("Unable to load up arrow");
            }
            if (downArrow == null)
            {
                downArrow = new Texture2D(2, 2);
                if (ToolbarControl.LoadImageFromFile(ref downArrow, "GameData/KeoCaching/PluginData/Icons/down"))
                    downContent = new GUIContent("", downArrow, "");
                else
                    Log.Error("Unable to load down arrow");
            }


            InitiatePlay();
            ReadAllCaches();
        }

        void OnLevelLoaded(GameScenes gx)
        {
            GetActiveTravelBugs();
        }
        void OnVesselRollout(ShipConstruct sc)
        {
            GetActiveTravelBugs();
        }

        void closeFSDialog()
        {
            Log.Info("KeoCacheDriver.closeFSDialog");
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
        void CloseKeoCacheWindow()
        {
            visibleEditCollection = false;
            editInProgress = false;
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
            GameEvents.OnVesselRollout.Remove(OnVesselRollout);
            toolbarControl.OnDestroy();
            Destroy(toolbarControl);
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
            if (HighLogic.CurrentGame.Parameters.CustomParams<KeoCacheOptions>().hideWhenPaused)
                hideUIwhenPaused = true;
        }
        void ShowUIwhenPaused()
        {
            hideUIwhenPaused = false;
        }

        static GUIStyle normalButton, redButton = null;
        static GUILayoutOption buttonWidth;
        static GUIStyle arrowButtonStyle = null;
       // static GUIStyle labelNormal, labelRed;
        static Color textNormalBackground, textRedBackground;
        void InitializeGUIVars()
        {
            if (redButton == null)
            {
                if (HighLogic.CurrentGame.Parameters.CustomParams<KeoCacheOptions>().useKSPskin)
                    GUI.skin = HighLogic.Skin;
                normalButton = new GUIStyle(GUI.skin.button);
                redButton = new GUIStyle(GUI.skin.button);
                redButton.normal.textColor = Color.red;
                redButton.fontStyle = FontStyle.Bold;
                buttonWidth = GUILayout.Width(150);

                arrowButtonStyle = new GUIStyle(GUI.skin.button);
                arrowButtonStyle.padding = new RectOffset(0, 0, 0, 0);
                arrowButtonStyle.fixedWidth = 15;
                arrowButtonStyle.fixedHeight = 12;

                //labelNormal = new GUIStyle(GUI.skin.label);
                //labelRed = new GUIStyle(GUI.skin.label);
                //labelRed.normal.textColor = Color.red;
                //labelRed = labelNormal;

                textNormalBackground = GUI.backgroundColor;
                textRedBackground = Color.red;
                
            }
        }
        internal static bool SetBackground(bool b)
        {
            GUI.backgroundColor = (bool)b ? textRedBackground : textNormalBackground;
            return b;
        }

        void OnGUI()
        {
            InitializeGUIVars();

            if (hideUI || hideUIwhenPaused)
                return;

            if (HighLogic.CurrentGame.Parameters.CustomParams<KeoCacheOptions>().useKSPskin)
                GUI.skin = HighLogic.Skin;

            if (fsDialog != null && fsDialog.done &&
                           fsDialog.SelectedDirectory != null && fsDialog.SelectedFile != null && dialogEntry == null)
            {
                if (fsDialog.SelectedDirectory != "" || fsDialog.SelectedFile != "")
                {
                   
                    KeoScenario.Instance.lastDirectory = fsDialog.SelectedDirectory;
                    localFilePath = fsDialog.SelectedDirectory + FileIO.DirSeperator + fsDialog.SelectedFile;
                    localFilePath= localFilePath.Replace(@"\", "/");
                    if (visibleEditCollection)
                    {
                        var cache = FileIO.LoadKeoCacheData(fsDialog.SelectedDirectory + FileIO.DirSeperator + fsDialog.SelectedFile);
                        if (!visibleAllCollections)
                            activeKeoCacheCollection = cache;
                        else
                        {
                            if (!KeoScenario.AddCollection(cache))
                            {
                                // error message here, already in cache
                                ScreenMessages.PostScreenMessage("Collection already loaded", 10f, ScreenMessageStyle.UPPER_CENTER);
                            }
                        }
                    }
                }
                closeFSDialog();
            }
            if (visibleMenu)
            {
                menuWinRect = ClickThruBlocker.GUILayoutWindow(menuWinID, menuWinRect, KeoCaching_Menu_Window, "KeoCaching");
            }
            if (visibleEditCollection && !visibleKeoCache && !visibleAllCollections)
            {
                if (!visibleActiveCollectionsReadOnly)
                    collectionWinRect = ClickThruBlocker.GUILayoutWindow(collectionWinID, collectionWinRect, KeoCaching_Collection_Window, "KeoCache Collection");
                else
                    collectionROWinRect = ClickThruBlocker.GUILayoutWindow(collectionWinID, collectionROWinRect, KeoCaching_Collection_Window, "KeoCache Collection");
            }
            if (visibleKeoCache)
            {
                keocacheWinRect = ClickThruBlocker.GUILayoutWindow(KeocacheWinID, keocacheWinRect, KeoCache_Window, "KeoCache");
            }
            if (visibleHint)
            {
                // Only set the following two IF the hint window hasn't been moved
                // Once it gets moved, then use the saved coordinates
                hintWinRect.x = keocacheWinRect.x + keocacheWinRect.width;
                hintWinRect.y = keocacheWinRect.y;
                hintWinRect = ClickThruBlocker.GUILayoutWindow(hintWinID, hintWinRect, Hint_Window, "Hint");
            }
            if (visibleTravelBug)
            {
                travelbugWinRect = ClickThruBlocker.GUILayoutWindow(travelbugWinID, travelbugWinRect, TravelBug_Window, "Travel Bugs");
            }
            if (visibleImportWindow)
            {
                importwinRect  = ClickThruBlocker.GUILayoutWindow(importWinID, importwinRect, Import_Window, "Import Collection");
            }

            if (visibleAllCollections && !visibleEditCollection)
            {
                string s = "All Collections";
                if (selectForEdit || selectCollection)
                    s = "Available Collections";

                allCollectionsWinRect = ClickThruBlocker.GUILayoutWindow(activeCollectionsID, allCollectionsWinRect, All_Collections_Window, s);
            }
            if (showKerbalX)
            {
                kerbalXWinRect = ClickThruBlocker.GUILayoutWindow(kerbalXWinID, kerbalXWinRect, KerbalXInterface.Instance.IODisplay, "KerbalX Interface");
            }
        }

        

        public void LaunchEvent()
        {
            Log.Info("LaunchEvent");
            if (VesselSpawn.instance == null)
                VesselSpawn.instance = new VesselSpawn();
            VesselSpawn.instance.StartVesselSpawn(Environment.CurrentDirectory + PluginPath + KeoCacheCraft);
        }
    }
}
