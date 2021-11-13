using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(MoveObject))]
#endif
#if UNITY_EDITOR
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
        if (GUILayout.Button("Set Current Name To Tag"))
        {
            moveScript.Tag = moveScript.transform.name;
        }

        EditorGUILayout.Space(20);
    }
}
#endif
