using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class OnCollision2D : MonoBehaviour
{
    public string[] NameOfObjects = null;

    private bool _onCollision = false;
    public bool OnCollision
    {
        get { return _onCollision; }
    }

    private string _nameOfObject = "";
    private string _nameOfTrggeredObject = "";
    public string NameOfObject
    {
        get { return _nameOfObject; }
    }
    public string NameOfTriggeredObject
    {
        get { return _nameOfTrggeredObject; }
    }

    public static void AddTag(string tag)
    {
        UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if ((asset != null) && (asset.Length > 0))
        {
            SerializedObject so = new SerializedObject(asset[0]);
            SerializedProperty tags = so.FindProperty("tags");

            for (int i = 0; i < tags.arraySize; ++i)
            {
                if (tags.GetArrayElementAtIndex(i).stringValue == tag)
                {
                    return;     // Tag already present, nothing to do.
                }
            }

            tags.InsertArrayElementAtIndex(0);
            tags.GetArrayElementAtIndex(0).stringValue = tag;
            so.ApplyModifiedProperties();
            so.Update();
        }
    }

    void Awake()
    {
        _nameOfObject = this.transform.name;
    }

    List<string> nameOfObjectsInside = new List<string>();
    List<MoveObject> scriptsOfObjectsInside = new List<MoveObject>();

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (locked)
            return;

        _onCollision = true;
        _nameOfTrggeredObject = coll.gameObject.name;
        nameOfObjectsInside.Add(_nameOfTrggeredObject);
        scriptsOfObjectsInside.Add(coll.transform.GetComponent<MoveObject>());

        checkHasAll();
    }

    bool locked = false;
    private void checkHasAll()
    {
        var a = NameOfObjects.All(nameOfObjectsInside.Contains) && NameOfObjects.Length == nameOfObjectsInside.Count;
        if (a)
        {
            locked = true;
            StartCoroutine(makeDisableAllObjects());
        }
        else
        {
        }
    }

    private IEnumerator makeDisableAllObjects()
    {
        yield return new WaitForSeconds(1);

        for (int i = 0; i < scriptsOfObjectsInside.Count; i++)
        {
            scriptsOfObjectsInside[i].DontMove();
        }
    }

    private bool checkInside(Collider2D c, Vector3 pos)
    {
        Vector3 closest = c.ClosestPoint(pos);
        // Because closest=point if point is inside - not clear from docs I feel
        return closest == pos;
    }
    void OnTriggerExit2D(Collider2D coll)
    {
        _onCollision = false;
        nameOfObjectsInside.Remove(coll.gameObject.name);
        scriptsOfObjectsInside.Remove(coll.transform.GetComponent<MoveObject>());

        checkHasAll();
    }
}
