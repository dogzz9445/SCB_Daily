using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.UIElements;
using SCB.Cores.UIs;
using Gpm.Ui;



#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace SCB.Cores.Tools
{
    public class SceneManagementWindow : EditorWindow
    {
        [MenuItem("Tools/SCB/Scene Management")]
        public static void ShowWindow()
        {
            EditorWindow wnd = GetWindow(typeof(SceneManagementWindow), false, "Scene Management");
        }

        int selected;
        int changedSelected;
        List<string> scenes = new List<string>();
        
        public void CreateGUI()
        {

            // var splitView = new TwoPaneSplitView(0, 200, TwoPaneSplitViewOrientation.Vertical);
            // rootVisualElement.Add(splitView);

            // var headerPane = new VisualElement();
            // headerPane.Add(tab);
            
            // var bottomSplitView = new TwoPaneSplitView(1, 200, TwoPaneSplitViewOrientation.Vertical);
            
            // splitView.Add(headerPane);
            // splitView.Add(bottomSplitView);

            // var bottomPane = new ListView();
            // bottomSplitView.Add(bottomPane);
            // bottomSplitView.Add(new ListView());

            // var listView1 = new ListView();

            // var listView2 = new ListView();

            // var listView3 = new ListView();

            // rootVisualElement.Add(listView1);
            // rootVisualElement.Add(listView2);
            // rootVisualElement.Add(listView3);

            // var tab = new TabbedMenuController(rootVisualElement);
            // tab.RegisterTabCallbacks();
        }
    }
}
