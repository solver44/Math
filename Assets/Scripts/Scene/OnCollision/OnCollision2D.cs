using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class OnCollision2D : MonoBehaviour
{
    public delegate void ChangeCollision(bool coll, GameObject child);
    public event ChangeCollision changingColl;

    public bool CheckInside = true;
    public GameObject CheckButton = null;
    public GameObject ErrorPanel = null; 
    public string[] NameOfObjects = null;

    private bool hasCheckButton = false;
    private bool _onCollision = false;
    public bool OnCollision
    {
        get { return _onCollision; }
    } 

    private bool collObject = false;
    public bool OnCollisionRayObject
    {
        get { return collObject; }
        set { collObject = value; changingColl?.Invoke(collObject, currentGameObj); }
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

    //public static void AddTag(string tag)
    //{
    //    UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
    //    if ((asset != null) && (asset.Length > 0))
    //    {
    //        SerializedObject so = new SerializedObject(asset[0]);
    //        SerializedProperty tags = so.FindProperty("tags");

    //        for (int i = 0; i < tags.arraySize; ++i)
    //        {
    //            if (tags.GetArrayElementAtIndex(i).stringValue == tag)
    //            {
    //                return;     // Tag already present, nothing to do.
    //            }
    //        }

    //        tags.InsertArrayElementAtIndex(0);
    //        tags.GetArrayElementAtIndex(0).stringValue = tag;
    //        so.ApplyModifiedProperties();
    //        so.Update();
    //    }
    //}

    void Awake()
    {
        _nameOfObject = this.transform.name;

        if (CheckButton != null)
        {
            hasCheckButton = true;
            CheckButton.GetComponent<Button>().onClick.AddListener(delegate () { checkHaveAll(true); });
        }
    }

    List<string> nameOfObjectsInside = new List<string>();
    List<MoveObject> scriptsOfObjectsInside = new List<MoveObject>();

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (locked)
            return;

        _onCollision = true;
        _nameOfTrggeredObject = coll.gameObject.name;

        if (!CheckInside)
            return;

        if (coll.transform.tag == "RayObjects") {
            currentGameObj = coll.gameObject;

            OnCollisionRayObject = true;

            nameOfObjectsInside.Add(_nameOfTrggeredObject);
            scriptsOfObjectsInside.Add(currentGameObj.transform.GetComponent<MoveObject>());
            scriptsOfObjectsInside[scriptsOfObjectsInside.Count - 1].DontMoveTo1stPosition = true;

            if(!hasCheckButton)
                checkHaveAll(false);
        }
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.tag.Equals("RayObjects") || scriptsOfObjectsInside.Count < 1)
            return;

        bool a = scriptsOfObjectsInside[scriptsOfObjectsInside.Count - 1].DontMoveTo1stPosition;
        if (!a)
            scriptsOfObjectsInside[scriptsOfObjectsInside.Count - 1].DontMoveTo1stPosition = true;
    }

    bool locked = false;
    private void checkHaveAll(bool showMessage)
    {
        var a = NameOfObjects.All(nameOfObjectsInside.Contains) && NameOfObjects.Length == nameOfObjectsInside.Count;
        if (a)
        {
            locked = true;
            StartCoroutine(makeDisableAllObjects(!hasCheckButton));
        }
        else if (showMessage)
        {
            if(ErrorPanel != null)
                StartCoroutine(ErrorPanelAnimation());
        }
    }

    private IEnumerator ErrorPanelAnimation()
    {
        MoveObject.currentCnt1 = 0;
        locked = false;

        ErrorPanel.SetActive(true);
        ErrorPanel.GetComponent<Animator>().SetBool("start", true);
        yield return new WaitForSeconds(1f);
        ErrorPanel.GetComponent<Animator>().SetBool("start", false);
        yield return new WaitForSeconds(0.2f);
        ErrorPanel.SetActive(false);
    }

    private IEnumerator makeDisableAllObjects(bool _lock)
    {
        yield return new WaitForSeconds(1);
        if (ErrorPanel != null && ErrorPanel.activeSelf)
            yield break;

        for (int i = 0; i < scriptsOfObjectsInside.Count; i++)
        {
            scriptsOfObjectsInside[i].DontMove(_lock);
        }
    }

    private bool checkInside(Collider2D c, Vector3 pos)
    {
        Vector3 closest = c.ClosestPoint(pos);
        // Because closest=point if point is inside - not clear from docs I feel
        return closest == pos;
    }

    GameObject currentGameObj = null;

    void OnTriggerExit2D(Collider2D coll)
    {
        _onCollision = false;

        if (!CheckInside)
            return;

        if (coll.transform.tag == "RayObjects")
        {
            currentGameObj = coll.gameObject;

            OnCollisionRayObject = false;
             
            coll.transform.GetComponent<MoveObject>().DontMoveTo1stPosition = false;
            nameOfObjectsInside.Remove(currentGameObj.name);
            scriptsOfObjectsInside.Remove(currentGameObj.transform.GetComponent<MoveObject>());

            if (!hasCheckButton)
                checkHaveAll(false);
        }
    }
}
