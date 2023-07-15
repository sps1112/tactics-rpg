using UnityEngine;
using UnityEditor;

public class ObstacleTool : EditorWindow
{
    string path = "Assets/Data/Obstacles/"; // Path to Obstacle Layout folder

    ObstacleLayout asset = null; // Reference to the asset being changed

    WindowStates state = WindowStates.EMPTY; // Current state of the Obstacle Editor window

    [MenuItem("Window/Obstacles")]
    public static void ShowWindow()
    {
        GetWindow<ObstacleTool>("Obstacles");
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
            if (GUILayout.Button("Load Obstacle Layout"))
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
                    ObstacleLayout layout = new ObstacleLayout();
                    string newPath = AssetDatabase.GenerateUniqueAssetPath(path + "NewObstacleLayout.asset");
                    AssetDatabase.CreateAsset(layout, newPath);
                    AssetDatabase.SaveAssets();
                    asset = layout;
                }
            }
            else if (state == WindowStates.LOAD)
            {
                asset = (ObstacleLayout)EditorGUILayout.ObjectField("Obstacle Layout", asset, typeof(ObstacleLayout), true);
            }
            if (asset != null)
            {
                GUILayout.Label("Edit Grid Obstacles:-");
                asset.rows = EditorGUILayout.IntField("Rows:- ", asset.rows);
                asset.columns = EditorGUILayout.IntField("Columns:- ", asset.columns);
                asset.obstacle = (GameObject)EditorGUILayout.ObjectField("Obstacle", asset.obstacle, typeof(GameObject), true);
                asset.gridNoAction = (GameObject)EditorGUILayout.ObjectField("Grid No Action", asset.gridNoAction, typeof(GameObject), true);
                if (GUILayout.Button("Refresh Layout"))
                {
                    asset.layout = new int[asset.rows * asset.columns];
                }
                GUILayout.Label("Layout:- ");
                GUILayout.Label("Note the values are:-\n0 => Normal\n1=> Obstacle\n2=> No Action Grid\n3=> Enemy Spawn\n4=> Player Spawn");
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
                GUILayout.Label("No Obstacle Layout Selected");
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save Obstacle Layout"))
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
