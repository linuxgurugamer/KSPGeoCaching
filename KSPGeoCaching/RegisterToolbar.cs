﻿
using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

using KSP.UI.Screens;
using ToolbarControl_NS;

namespace KSPGeoCaching
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            Log.Info("RegisterToolbar.STart");
            ToolbarControl.RegisterMod(GeoCacheDriver.MODID, GeoCacheDriver.MODNAME);
        }
    }
}