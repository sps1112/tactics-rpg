using UnityEngine;
using UnityEditor;

public class GridTool : EditorWindow
{
    string path = "Assets/Data/GridLayout.asset"; // Path to grid layout asset

    GridLayout asset;

    [MenuItem("Window/Grid")]
    public static void ShowWindow()
    {
        GetWindow<GridTool>("Grid");
    }

    void OnGUI()
    {
        asset = (GridLayout)AssetDatabase.LoadAssetAtPath(path, typeof(GridLayout));
        GUILayout.Label("Map Layout:-");
        asset.rows = EditorGUILayout.IntField("Rows:- ", asset.rows);
        asset.columns = EditorGUILayout.IntField("Columns:- ", asset.columns);
        asset.bottom = (GameObject)EditorGUILayout.ObjectField("Bottom", asset.bottom, typeof(GameObject), true);
        asset.mid = (GameObject)EditorGUILayout.ObjectField("Mid", asset.mid, typeof(GameObject), true);
        asset.top = (GameObject)EditorGUILayout.ObjectField("Top", asset.top, typeof(GameObject), true);
        if (GUILayout.Button("Refresh Layout"))
        {
            asset.layout = new int[asset.rows * asset.columns];
        }
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
        if (GUILayout.Button("Save Layout"))
        {
            AssetDatabase.Refresh();
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
        }
    }
}

