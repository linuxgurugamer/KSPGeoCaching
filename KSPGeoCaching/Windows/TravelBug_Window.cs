using System.IO;
using System.Text.RegularExpressions;
using KSP.UI.Screens;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using ToolbarControl_NS;
using ClickThroughFix;


namespace KSPGeoCaching
{
    partial class GeoCacheDriver : MonoBehaviour
    {
        void TravelBug_Window(int id)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Close"))
            {
                visibleTravelBug = false;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUI.DragWindow();
        }
    }
}