using System;
using System.IO;
using System.Text.RegularExpressions;
using KSP.UI.Screens;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using KSP.Localization;
using ToolbarControl_NS;
using ClickThroughFix;

namespace KeoCaching
{
    partial class KeoCacheDriver : MonoBehaviour
    {
        string collectionUrl;
        string localFilePath;
        bool importResult = false;



        enum downloadStateType
        {
            INACTIVE,
            GUI,
            FILESELECTION,
            IN_PROGRESS,
            COMPLETED,
            FILE_EXISTS,
            ERROR
        };

        private static downloadStateType downloadState = downloadStateType.INACTIVE;
        private static string downloadErrorMessage;

        WWW download = null;
        WWW collectionDownload = null;
        bool overwriteExisting = true;

        void ShowImportWindow()
        {
            collectionUrl = "";
            localFilePath = "";
            visibleImportWindow = true;
            downloadState = downloadStateType.GUI;
        }

        void resetBeforeExit()
        {
            Log.Info("resetBeforeExit");
            if (download != null)
                download.Dispose();
            
            download = null;
            GUI.enabled = false;
            downloadState = downloadStateType.INACTIVE;

            overwriteExisting = false;
            visibleImportWindow = false;
        }

        internal void Import_Window(int id)
        {
            GUILayout.BeginHorizontal();
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();

            switch (downloadState)
            {
                case downloadStateType.GUI:
                    guiCase();
                    break;

                case downloadStateType.IN_PROGRESS:
                    Log.Info("IN_PROGRESS");
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    GUILayout.Label("Download in progress");

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (downloadState == downloadStateType.IN_PROGRESS)
                    {
                        if (download != null)
                            GUILayout.Label("Progress: " + (100 * download.progress).ToString() + "%");
                    }

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Cancel", GUILayout.Width(125.0f)))
                    {
                        resetBeforeExit();
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    break;

                case downloadStateType.COMPLETED:
                    ReadAllCaches();
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (downloadState == downloadStateType.COMPLETED)
                        GUILayout.Label("Download completed");
                    else
                        GUILayout.Label("Upload completed");

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("", GUILayout.Height(10));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("", GUILayout.Height(10));
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("OK", GUILayout.Width(125.0f)))
                    {
                        resetBeforeExit();
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    break;

                case downloadStateType.FILE_EXISTS:
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("File Exists Error");
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("The KeoCache file already exists, will NOT be overwritten");
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("OK", GUILayout.Width(125.0f)))
                    {
                        resetBeforeExit();
                    }
                    GUILayout.EndHorizontal();
                    break;

                case downloadStateType.ERROR:
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (downloadState == downloadStateType.ERROR)
                        GUILayout.Label("Download Error");
                    else
                        GUILayout.Label("Upload Error");
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    GUILayout.Label(downloadErrorMessage);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Cancel", GUILayout.Width(125.0f)))
                    {
                        resetBeforeExit();
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    break;

                default:
                    break;
            }
            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        internal void guiCase()
        {
            bool doImport = false;
            //GUILayout.BeginVertical();



            GUILayout.BeginHorizontal();
            GUILayout.Label("Collection URL:");
            GUILayout.FlexibleSpace();
            if (fsDialog != null)
                GUI.enabled = false;
            collectionUrl = GUILayout.TextField(collectionUrl, GUILayout.Width(250F));
            if (GUILayout.Button("X", redButton, GUILayout.Width(20)))
            {
                collectionUrl = "";
            }
            GUI.enabled = true;
            if (collectionUrl == "")
                GUI.enabled = false;
            if (GUILayout.Button("Import", GUILayout.Width(80)))
            {
                doImport = true;
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Local file:");
            GUILayout.FlexibleSpace();
            if (fsDialog != null)
                GUI.enabled = false;
            localFilePath = GUILayout.TextField(localFilePath, GUILayout.Width(250F));
            if (GUILayout.Button("X", redButton, GUILayout.Width(20)))
            {
                localFilePath = "";
            }
            if (localFilePath == "")
            {
                if (GUILayout.Button("Select File", GUILayout.Width(80)))
                {
                    StartLoadDialog(ParentWin.import, importwinRect);
                }
            }
            else
            {
                if (GUILayout.Button("Import", GUILayout.Width(80)))
                {
                    doImport = true;
                }
            }
            GUILayout.EndHorizontal();
            GUI.enabled = true;

            if (doImport)
            { 
                if (collectionUrl != "")
                    downloadCollection(collectionUrl);
                if (localFilePath != "")
                    downloadCollection("file://" + localFilePath);
                editInProgress = true;
            }
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (fsDialog != null)
                GUI.enabled = false;
            if (GUILayout.Button("Cancel", GUILayout.Width(80)))
            {
                visibleImportWindow = false;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.enabled = true;
            //GUILayout.EndVertical();

        }


        IEnumerator doDownloadCollection(string collectionUrlStr)
        {

            //craftURL.Replace('\\', '/');
            string collectionURL = "";
            for (int i = 0; i < collectionUrlStr.Length; i++)
            {
                if (collectionUrlStr[i] == '\\')
                {
                    collectionURL += "/";
                    i++;
                }
                else
                    collectionURL += collectionUrlStr[i];
            }

            Log.Info("doDownloadCollection 2, collectionURL: " + collectionURL);
            if (collectionURL != "")
            {

                string s = System.Uri.EscapeUriString(collectionURL);

                // some simple error checking
                if (!s.StartsWith("http://") && !s.StartsWith("https://") && !s.StartsWith("ftp://") && !s.StartsWith("file://"))
                {
                    downloadErrorMessage = "Invalid URL or file specified";
                    downloadState = downloadStateType.ERROR;
                    yield break;
                }

                // Create a download object
                Log.Info("s: " + s);

                string fileText = "";
                if (s.StartsWith("file://"))
                {
                    if (File.Exists(s.Substring(7)))
                    {
                        Log.Info("File exists: " + s.Substring(7));
                        fileText = File.ReadAllText(s.Substring(7));
                        Log.Info("File read ");
                        importResult = SaveCollectionFile(fileText);
                    }
                }
                else
                {
                    collectionDownload = new WWW(s);
                    // Wait until the download is done
                    yield return collectionDownload;

                    Log.Info("Download completed   collectionDownload.error: " + collectionDownload.error);

                    if (!String.IsNullOrEmpty(collectionDownload.error))
                    {
                        Log.Error("Error downloading: " + collectionDownload.error);
                        downloadErrorMessage = collectionDownload.error;
                        downloadState = downloadStateType.ERROR;
                        yield break;
                    }
                    else
                    {
                        importResult = SaveCollectionFile(collectionDownload.text);

                        Log.Info("After SaveCollectionFile, b: " + importResult.ToString());

                        collectionURL = "";
                    }
                }
                if (download != null)
                {
                    download.Dispose();
                    download = null;
                }
            }
        }

        void downloadCollection(string s)
        {
            if (!s.StartsWith("http://") && !s.StartsWith("https://") && !s.StartsWith("ftp://") && !s.StartsWith("file://"))
            {
                if (!System.IO.File.Exists(s))
                    s = "http://" + s;
            }
            downloadState = downloadStateType.IN_PROGRESS;
            StartCoroutine(doDownloadCollection(s));
        }

        bool SaveCollectionFile(string collectionFile)
        {
            Log.Info("SaveCollectionFile");
            string saveDir = FileIO.GetCacheDir;

            //string saveDir = KSPUtil.ApplicationRootPath + sandbox + HighLogic.SaveFolder + "/ships";
            string saveFile = "";

            string collectionName = null;
            //Match t = null;
            try
            {
                Log.Info("size of collectionFile: " + collectionFile.Length);
                //Match s = Regex.Match(collectionFile, "^\\s*name\\s*=.*");
                Match s = Regex.Match(collectionFile, "\\s*name\\s*=.*");
                Log.Info("s result: " + s.Success);
                if (!s.Success)
                {
                    downloadState = downloadStateType.ERROR;
                    return false;
                }
                Log.Info("s 1: '" + s.Value + "'");
                s = Regex.Match(s.Value.ToString(), "=.*");
                Log.Info("s 2: '" + s.Value + "'");


                string s1 = s.Value.ToString().Remove(0, 1);
                if (s1.IndexOf("//") >= 0)
                {
                    s1 = s1.Substring(0, s1.IndexOf("//"));
                }
                collectionName = Localizer.Format(s1.Trim());


                string strCraftFile = collectionFile.ToString();

                Log.Info("collectionName: " + collectionName);


                saveFile = saveDir + collectionName + FileIO.SUFFIX;
                Log.Info("saveFile: " + saveFile);
                if (System.IO.File.Exists(saveFile) && !overwriteExisting)
                {
                    downloadState = downloadStateType.FILE_EXISTS;
                    return false;
                }

                File.WriteAllText(saveFile, strCraftFile);

                downloadState = downloadStateType.COMPLETED;
            }
            catch (Exception e)
            {
                Log.Info("Error: " + e);

                //downloadErrorMessage = download.error;
                downloadErrorMessage = "Download URL did not specify a valid KeoCollection file";
                downloadState = downloadStateType.ERROR;
                return false;
            }
            return true;
        }

    }
}
