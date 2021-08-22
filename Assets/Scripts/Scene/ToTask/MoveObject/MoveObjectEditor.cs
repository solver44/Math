using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MoveObject))]
public class MoveObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MoveObject moveScript = (MoveObject)target;

        EditorGUILayout.Space(50);

        if (GUILayout.Button("Set Current Location")) {
            moveScript.toThisLocation = moveScript.transform.localPosition;
        }
        if (GUILayout.Button("Set Current Scale"))
        {
            moveScript.toThisScale = moveScript.transform.localScale;
        }

        EditorGUILayout.Space(20);
    }
}
