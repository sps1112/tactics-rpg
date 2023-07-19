using UnityEngine;
using UnityEditor;

public enum WindowStates
{
    EMPTY, // No Asset is selected
    NEW, // Creating a new asset
    LOAD, // Editing a created asset
};

public class GridTool : EditorWindow
{
    string path = "Assets/Data/Levels/"; // Path to Grid Layout folder

    LevelLayout asset = null; // Reference to the asset being changed

    WindowStates state = WindowStates.EMPTY; // Current state of the Grid Editor window

    [MenuItem("Window/Grid")]
    public static void ShowWindow()
    {
        GetWindow<GridTool>("Grid");
    }

    void OnGUI()
    {
        if (state == WindowStates.EMPTY)
        {
            GUILayout.Label("Choose option:-");
            GUILayout.Label("");
            if (GUILayout.Button("Create New Layout"))
            {
                state = WindowStates.NEW;
            }
            if (GUILayout.Button("Load Level Layout"))
            {
                state = WindowStates.LOAD;
            }
        }
        if (state != WindowStates.EMPTY)
        {
            if (state == WindowStates.NEW)
            {
                if (asset == null)
                {
                    LevelLayout layout = new LevelLayout();
                    string newPath = AssetDatabase.GenerateUniqueAssetPath(path + "NewLevelLayout.asset");
                    AssetDatabase.CreateAsset(layout, newPath);
                    AssetDatabase.SaveAssets();
                    asset = layout;
                }
            }
            else if (state == WindowStates.LOAD)
            {
                asset = (LevelLayout)EditorGUILayout.ObjectField("Grid Layout", asset, typeof(LevelLayout), true);
            }
            if (asset != null)
            {
                GUILayout.Label("Edit Grid Layout:-");
                asset.levelName = EditorGUILayout.TextField("Level Name:- ", asset.levelName);
                asset.rows = EditorGUILayout.IntField("Rows:- ", asset.rows);
                asset.columns = EditorGUILayout.IntField("Columns:- ", asset.columns);
                asset.bottom = (GameObject)EditorGUILayout.ObjectField("Bottom", asset.bottom, typeof(GameObject), true);
                asset.mid = (GameObject)EditorGUILayout.ObjectField("Mid", asset.mid, typeof(GameObject), true);
                asset.top = (GameObject)EditorGUILayout.ObjectField("Top", asset.top, typeof(GameObject), true);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Refresh Layout"))
                {
                    asset.layout = new int[asset.rows * asset.columns];
                }
                if (GUILayout.Button("Initialize Layout"))
                {
                    asset.layout = new int[asset.rows * asset.columns];
                    for (int i = 0; i < asset.rows; i++)
                    {
                        for (int j = 0; j < asset.columns; j++)
                        {
                            asset.layout[(asset.rows - i - 1) * asset.columns + j] = 1;
                        }
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Label("Layout:- ");
                for (int i = 0; i < asset.rows; i++)
                {
                    GUILayout.BeginHorizontal();
                    for (int j = 0; j < asset.columns; j++)
                    {
                        asset.layout[(asset.rows - i - 1) * asset.columns + j] = EditorGUILayout.IntField(asset.layout[(asset.rows - i - 1) * asset.columns + j]);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.Label("No Grid Layout Selected");
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save Grid Layout"))
            {
                AssetDatabase.Refresh();
                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssets();
            }
            if (GUILayout.Button("Back"))
            {
                asset = null;
                state = WindowStates.EMPTY;
            }
            GUILayout.EndHorizontal();
        }
    }
}

