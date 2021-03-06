﻿using UnityEngine;
using System.Collections;
using ClickThroughFix;

// Downloaded from the Unity Store (free): https://www.assetstore.unity3d.com/en/#!/content/18308

namespace KeoCaching
{

    public class FileSelection : MonoBehaviour
    {
        const int WIDTH = 510; //675;
        const int HEIGHT = 250; // 500;
        Rect windowPos = new Rect((Screen.width - WIDTH) / 2, (Screen.height - HEIGHT) / 2, WIDTH, HEIGHT);
        internal Rect parentRect;
        internal KeoCacheDriver.ParentWin pWin;

        //skins and textures
        public GUISkin[] skins;
        public Texture2D file, folder, back, drive;

        string[] layoutTypes = { "Type 0", "Type 1" };
        //initialize file browser
        FileBrowser fb = new FileBrowser();
        
       
        string output = "no file";
        public bool visible = false;
        public bool done = false;
        

        public string SelectedFile
        {
            get
            {
                if (output != "no file" && output != "cancel hit")
                    return output;
                else
                    return "";
            }
            
        }
        public string SelectedDirectory
        { 
            get
            {
                if (fb.outputDirectory != null && fb.outputDirectory.FullName != null)
                    return fb.outputDirectory.FullName;
                else
                    return "";
            }
            set
            {
                fb.setDirectory(value);
            }
        }
        public void SetSelectedDirectory(string dir, bool dirSelect = false)
        {
            if (dir == "")
                dir = ".";
            fb.setDirectory(dir, dirSelect);
        }
        public void SetExtensions(string extensions)
        {
            char[] stringSeparators = new char[] { ';' };

            fb.extensions = extensions.Split(stringSeparators, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in fb.extensions)
                Log.Info("Extension set for file selection: " + s);
        }
        public void startDialog()
        {
            visible = true;
            done = false;
        }
        public void Close()
        {
            visible = false;
            done = false;
        }
        // Use this for initialization
        void Start()
        {

            //fb.setGUIRect(new Rect(150, 25, WIDTH - 150, HEIGHT - 25));
            fb.setGUIRect(new Rect(5, 25, WIDTH - 10, HEIGHT - 30));
            visible = true;
            done = false;
            //setup file browser style
            if (HighLogic.CurrentGame.Parameters.CustomParams<KeoCacheOptions>().useKSPskin)
                fb.guiSkin = HighLogic.Skin; //set the starting skin
                                         //set the various textures
            file = GameDatabase.Instance.GetTexture("LoadingScreenManager/Textures/file", false);
            folder = GameDatabase.Instance.GetTexture("LoadingScreenManager/Textures/folder", false);
            back = GameDatabase.Instance.GetTexture("LoadingScreenManager/Textures/back", false);
            drive = GameDatabase.Instance.GetTexture("LoadingScreenManager/Textures/drive", false);


            fb.fileTexture = file;
            fb.directoryTexture = folder;
            fb.backTexture = back;
            fb.driveTexture = drive;

            fb.setLayout(0);

            //public GUIStyle backStyle, cancelStyle, selectStyle; //styles used for specific buttons

            //        private GUIStyle style = new GUIStyle(HighLogic.Skin.window);

            fb.backStyle = new GUIStyle(fb.guiSkin.textField);

            fb.cancelStyle = new GUIStyle(fb.guiSkin.textField);

            fb.selectStyle = new GUIStyle(fb.guiSkin.textField);


            //show the search bar
            fb.showSearch = true;
            //search recursively (setting recursive search may cause a long delay)
            fb.searchRecursively = true;

            GameEvents.onHideUI.Add(this.HideUI);
            GameEvents.onShowUI.Add(this.ShowUI);
            GameEvents.onGamePause.Add(this.HideUIWhenPaused);
            GameEvents.onGameUnpause.Add(this.ShowUIwhenPaused);
        }
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

        bool guiInitted = false;
        GUIStyle window;

        void OnGUI()
        {
            if (hideUI || hideUIwhenPaused || !visible)
                return;
            //if (visible)
            {
                if (!guiInitted)
                {
                    guiInitted = true;
                    //GUI.color = Color.grey;
                    window = new GUIStyle(HighLogic.Skin.window);
                    //window.normal.background.SetPixels( new[] { new Color(0.5f, 0.5f, 0.5f, 1f) });
                    window.active.background = window.normal.background;
                    Texture2D tex = window.normal.background; //.CreateReadable();

                    var pixels = tex.GetPixels32();

                    for (int i = 0; i < pixels.Length; ++i)
                        pixels[i].a = 255;

                    tex.SetPixels32(pixels); tex.Apply();
                    //#if DEBUG
                    //                tex.SaveToDisk("usermodified_window_bkg.png");
                    //#endif
                    // one of these apparently fixes the right thing
                    // window.onActive.background =
                    // window.onFocused.background =
                    // window.onNormal.background =
                    //window.onHover.background =
                    window.active.background =
                    window.focused.background =
                    //window.hover.background =
                    window.normal.background = tex;
                }
                parentRect = KeoCacheDriver.Instance.GetParentRect(pWin);
                windowPos.x = parentRect.x + parentRect.width;
                windowPos.y = parentRect.y;
                 windowPos = ClickThruBlocker.GUILayoutWindow(GetInstanceID() + 1, windowPos, Window, "File Selection ", window);
                parentRect.x = windowPos.x - parentRect.width;
                parentRect.y = windowPos.y;
                KeoCacheDriver.Instance.UpdateParentWin(pWin, parentRect);
            }
        }

        void Window(int id)
        {
            GUILayout.BeginHorizontal(GUILayout.Width(100));
            GUILayout.BeginVertical();
            // GUILayout.Label("Layout Type");

// fb.setLayout(GUILayout.SelectionGrid(fb.layoutType, layoutTypes, 1));

            if (!fb.directorySelection)
                fb.setLayout(0);
            else
                fb.setLayout(1);
            //GUILayout.Space(10);

#if false
            //select from available gui skins
            GUILayout.Label("GUISkin");
            foreach (GUISkin s in skins)
            {
                if (GUILayout.Button(s.name))
                {
                    fb.guiSkin = s;
                }
            }
#endif
            fb.guiSkin = GameObject.Instantiate(HighLogic.Skin) as GUISkin;
                       
            fb.guiSkin.button.padding = new RectOffset() { left = 3, right = 3, top = 3, bottom = 3 };
            fb.guiSkin.button.wordWrap = true;
            fb.guiSkin.button.fontSize = 12;
            fb.guiSkin.button.alignment = TextAnchor.MiddleLeft;
            fb.guiSkin.button.fixedHeight = 20;
#if false
            if (!fb.directorySelection)
            {
                GUILayout.Space(10);
                fb.showSearch = GUILayout.Toggle(fb.showSearch, "Show Search Bar");
                fb.searchRecursively = GUILayout.Toggle(fb.searchRecursively, "Search Sub Folders");

                GUILayout.Space(10);

                GUILayout.Label("Selected File: " + output);
            }
            else
#endif
            fb.showSearch = false;
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            //draw and display output
            if (fb.draw())
            { //true is returned when a file has been selected
              //the output file is a member if the FileInfo class, if cancel was selected the value is null

                Debug.Log("fb.draw is true");
                if (fb.directorySelection == false)
                    output = (fb.outputFile == null) ? "cancel hit" : fb.outputFile.ToString();
                //else
                //    ?????;
                visible = false;
                done = true;
            }
            GUI.DragWindow();
        }
    }

}
