using System;
using System.IO;
using System.Text.RegularExpressions;
using KSP.UI.Screens;
using System.Collections.Generic;
using UnityEngine;

using ToolbarControl_NS;

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

        void CreateButton()
        {
            toolbarControl = gameObject.AddComponent<ToolbarControl>();
            toolbarControl.AddToAllToolbars(ToggleOn, ToggleOn,
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

        void ToggleOn()
        {
            visible = !visible;
            if (visible)
            {
                fsDialog = gameObject.AddComponent<FileSelection>();
                fsDialog.SetExtensions(".txt");
                dialogEntry = null;
                fsDialog.SetSelectedDirectory(FileIO.GetCacheDir, false);
                fsDialog.startDialog();
            }
            else
            {
                closeFSDialog();
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
        }
        void LoadGeocacheFile(string f)
        {
            Log.Info("LoadGeocacheFile, f: " + f);
        }
    }
}
