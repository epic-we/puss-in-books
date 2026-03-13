using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SetIconWindow : EditorWindow
{
    const string menuPath = "Assets/Create/Set Icon..";

    List<Texture2D> icons = null;
    int selectedIcon = 0;


    // Function to open the window
    [MenuItem(menuPath, priority = 0)]
    public static void ShowMenuItem()
    {
        SetIconWindow window = (SetIconWindow)EditorWindow.GetWindow(typeof(SetIconWindow));
        window.titleContent = new GUIContent("Set Icon");
        window.Show();
    }

    // Checks if we have scripts selected
    [MenuItem(menuPath, validate = true)]
    public static bool ShowMenuItemValidation()
    {
        foreach (Object asset in Selection.objects)
        {
            if(asset.GetType() != typeof(MonoScript))
            {
                return false;
            }
        }
        return true;
    }

    private void OnGUI()
    {
        // If icons are not loaded, load them
        if (icons == null)
        {
            icons = new List<Texture2D>();
            string[] assetGuids = AssetDatabase.FindAssets("t:texture2D l:ScriptIcon");

            foreach(string assetGuid in assetGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(assetGuid);
                icons.Add(AssetDatabase.LoadAssetAtPath<Texture2D>(path));
            }
        }

        // display all icons from the asset database
        if(icons == null)
        {
            GUILayout.Label("No icons to display");

            if(GUILayout.Button("Close", GUILayout.Width(100)))
            {
                Close();
            }
        }
        else
        {
            int columns = 5;
            float padding = 10f; // Padding between buttons
            float totalPadding = padding * (columns + 1);
            float buttonWidth = (position.width - totalPadding) / columns;
            float buttonHeight = buttonWidth; // Make it square

            GUIStyle iconStyle = new GUIStyle(GUI.skin.button);
            iconStyle.fixedWidth = 32; // buttonWidth
            iconStyle.fixedHeight = 32; // buttonHeight

            selectedIcon = GUILayout.SelectionGrid(
                selectedIcon,
                icons.ToArray(),
                columns,
                iconStyle
            );


            // listens to input
            if (Event.current != null)
            {
                if(Event.current.isKey)
                {
                    switch(Event.current.keyCode)
                    {
                        case KeyCode.KeypadEnter:
                        case KeyCode.Return:
                            ApplyIcon(icons[selectedIcon]);
                            Close();
                            break;
                        case KeyCode.Escape:
                            Close();
                            break;
                        default:
                            break;
                    }
                }
                else // Check for double click
                {
                    if(Event.current.button == 0 && Event.current.clickCount == 2)
                    {
                        ApplyIcon(icons[selectedIcon]);
                        Close();
                    }
                }
            }
            if (GUILayout.Button("Apply", GUILayout.Width(100)))
            {
                ApplyIcon(icons[selectedIcon]);
                Close();
            }
        }
    }

    void ApplyIcon(Texture2D icon)
    {
        // The imports is held between StartAssetEditing and StopAssetEditing
        AssetDatabase.StartAssetEditing();

        // Selection.objects returns all selected objects in the project view
        foreach (Object asset in Selection.objects)
        {
            string path = AssetDatabase.GetAssetPath(asset);

            MonoImporter monoImporter = AssetImporter.GetAtPath(path) as MonoImporter;

            monoImporter.SetIcon(icon);

            // Imports
            AssetDatabase.ImportAsset(path);
        }

        // The imports is held between StartAssetEditing and StopAssetEditing
        AssetDatabase.StopAssetEditing();

        AssetDatabase.Refresh();
    }
}
