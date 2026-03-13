using UnityEngine;
using UnityEditor;
using System;
using System.Linq;


[InitializeOnLoad]
public static class HierarchyIconDisplay
{
    static bool hierarchyHasFocus = false;

    static EditorWindow hierarchyEditorWindow;
    static HierarchyIconDisplay()
    {
        // Whenver the hierarchy draws a particular item OnHierarchyWindowItemOnGUI is called
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
        EditorApplication.update += OnEditorUpdate;
    }

    private static void OnEditorUpdate()
    {
        if(hierarchyEditorWindow == null)
        {
            hierarchyEditorWindow = EditorWindow.GetWindow(System.Type.GetType("UnityEditor.SceneHierarchyWindow,UnityEditor"));
        }

        hierarchyHasFocus = EditorWindow.focusedWindow != null && EditorWindow.focusedWindow == hierarchyEditorWindow;
    }

    // Colors for the backround rectangle that will be drawn over the box icon
    static readonly Color defaultColor = new Color(0.7843f, 0.7843f, 0.7843f);
    static readonly Color defaultProColor = new Color(0.2196f, 0.2196f, 0.2196f);

    static readonly Color selectedColor = new Color(0.22745f, 0.447f, 0.6902f);
    static readonly Color selectedProColor = new Color(0.1725f, 0.3647f, 0.5294f);

    static readonly Color selectedUnFocusedColor = new Color(0.68f, 0.68f, 0.68f);
    static readonly Color selectedUnFocusedProColor = new Color(0.3f, 0.3f, 0.3f);

    static readonly Color hoveredColor = new Color(0.698f, 0.698f, 0.698f);
    static readonly Color hoveredProColor = new Color(0.2706f, 0.2706f, 0.2706f);
    public static Color Get(bool isSelected, bool isHovered, bool isWindowFocused)
    {
        if (isSelected)
        {
            if (isWindowFocused)
            {
                return EditorGUIUtility.isProSkin ? selectedProColor : selectedColor;
            }
            else
            {
                return EditorGUIUtility.isProSkin ? selectedUnFocusedProColor : selectedUnFocusedColor;
            }
        }
        else if (isHovered)
        {
            return EditorGUIUtility.isProSkin ? hoveredProColor : hoveredColor;
        }
        else
        {
            return EditorGUIUtility.isProSkin ? defaultProColor : defaultColor;
        }
    }


    // It redraws every icon on the hierarchy, not just the one we hovered/selected
    private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        // Get the object from the instance ID that has been drawn
        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (obj == null)
        {
            return;
        }
        
        // Get all of its components
        Component[] components = obj.GetComponents<Component>();
        if (components == null || components.Length == 0)
        {
            return;
        }

        // Can use this to access a custom script for icons for example, and get a custom icon for each object in the hierarchy
        foreach (Component c in components)
        {
            if (c is Transform)
            {
                //Debug.Log(c.transform.position.z);
                break;
            }
        }

        // If we have more than one component, we will only draw the first one after transform, if not, we will draw transform icon
        Component component = components.Length > 1 ? components[1] : components[0];

        // We get the type of the first component
        Type type = component.GetType();

        // Create a new GUI content object that will hold the icon and the tooltip
        GUIContent content = EditorGUIUtility.ObjectContent(component, type);
        content.text = null; // We don't want the text or it will write over the icon
        content.tooltip = type.Name;  
        // Change the icon as such:
        //content.image = EditorGUIUtility.ObjectContent(components[0], type).image; 

        // If the content has no image we dont do the next step
        if (content.image == null)
            return;

        // Draw a square to cover the default box icon from unity
        if (PrefabUtility.GetCorrespondingObjectFromSource(obj) == null)
        {
            bool isSelected = Selection.instanceIDs.Contains(instanceID);
            bool isHovered = selectionRect.Contains(Event.current.mousePosition);

            Color color = Get(isSelected, isHovered, hierarchyHasFocus);
            Rect backgroundRect = selectionRect;
            backgroundRect.width = 18.5f;
            EditorGUI.DrawRect(backgroundRect, color);
        }
        
        // Draw the icon 
        EditorGUI.LabelField(selectionRect, content);
    }
}
