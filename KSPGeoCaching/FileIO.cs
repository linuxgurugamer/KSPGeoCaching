using System;
using System.IO;
using System.Linq;
using KSP.UI.Screens;
using System.Collections.Generic;
using UnityEngine;

namespace KSPGeoCaching
{
    public class FileIO
    {
        private const string ConfigFilePath = "GameData/KSPGeoCaching/PluginData/GeoCaches";
        //static public string dirSeperator = "\\";
        //static public string altSeperator = "/";

        static public string DirSeperator
        {
            get
            {
                if (Application.platform != RuntimePlatform.WindowsPlayer)
                {
                    return "/";
                    //dirSeperator = "/";
                    //altSeperator = "\\";
                }
                return "\\";
                //return dirSeperator;
            }
        }

        static public string GetCacheDir
        {
            get
            {
                string s = Path.Combine(KSPUtil.ApplicationRootPath, ConfigFilePath).Replace('\\', '/');
                Log.Info("configFilePath: " + s);
                return s;
            }
        }
    }
}
