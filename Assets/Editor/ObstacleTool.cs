using UnityEngine;
using UnityEditor;

public class ObstacleTool : EditorWindow
{
    string path = "Assets/Data/ObstacleLayout.asset"; // Path to obstacle layout asset

    ObstacleLayout asset;

    [MenuItem("Window/Obstacles")]
    public static void ShowWindow()
    {
        GetWindow<ObstacleTool>("Obstacles");
    }

    void OnGUI()
    {
        asset = (ObstacleLayout)AssetDatabase.LoadAssetAtPath(path, typeof(ObstacleLayout));
        GUILayout.Label("Map Obstacles:-");
        asset.rows = EditorGUILayout.IntField("Rows:- ", asset.rows);
        asset.columns = EditorGUILayout.IntField("Columns:- ", asset.columns);
        asset.obstacle = (GameObject)EditorGUILayout.ObjectField("Obstacle", asset.obstacle, typeof(GameObject), true);
        if (GUILayout.Button("Refresh Layout"))
        {
            asset.layout = new bool[asset.rows * asset.columns];
        }
        GUILayout.Label("Layout:- ");
        for (int i = 0; i < asset.rows; i++)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < asset.columns; j++)
            {
                asset.layout[(asset.rows - i - 1) * asset.columns + j] = EditorGUILayout.Toggle(asset.layout[(asset.rows - i - 1) * asset.columns + j]);
            }
            GUILayout.EndHorizontal();
        }
    }
}
