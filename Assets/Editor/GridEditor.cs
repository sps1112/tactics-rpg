using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridSpawner))]
public class GridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GridSpawner spawner = (GridSpawner)target;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Grid"))
        {
            spawner.GenerateGrid();
        }
        if (GUILayout.Button("DeleteGrid"))
        {
            spawner.DeleteGrid();
        }
        GUILayout.EndHorizontal();
    }
}
