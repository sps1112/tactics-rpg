using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObstacleManager))]
public class ObstacleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ObstacleManager manager = (ObstacleManager)target;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Obstacles"))
        {
            manager.GenerateObstacles();
        }
        if (GUILayout.Button("Delete Obstacles"))
        {
            manager.DeleteObstacles();
        }
        GUILayout.EndHorizontal();
    }
}
