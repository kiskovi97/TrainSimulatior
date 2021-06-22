using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Path))]
public class PathEditor : Editor
{
    private Path targetComponent;

    private void OnSceneGUI()
    {
        targetComponent = (Path) target;
        targetComponent.SetUpPoints();
        Handles.color = Color.green;

        var positions = targetComponent.Points;

        for (var i = 1; i < positions.Count + 1; i++)
        {
            var prev = positions[i-1];
            var current = positions[i % positions.Count];

            Handles.DrawDottedLine(prev, current, 4f);
            
        }

        for (var i = 0; i < positions.Count; i++)
        {
            positions[i] = Handles.PositionHandle(positions[i], Quaternion.identity);
        }

    }
}
