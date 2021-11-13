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
    public bool HaveCells = false;
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

    private Transform _nameOfObject;
    private Transform _triggeredObject;
    public Transform NameOfObject
    {
        get { return _nameOfObject; }
    }
    public Transform NameOfTriggeredObject
    {
        get { return _triggeredObject; }
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
        _nameOfObject = this.transform;

        if (CheckButton == null)
            return;

        hasCheckButton = true;
        CheckButton.GetComponent<Button>().onClick.AddListener(delegate () { checkHaveAll(true); });
    }
    private void Start()
    {
        if(HaveCells)
            MoveObject.MouseUp += MoveObject_MouseUp;
    }

    public List<Transform> nameOfObjectsInside = new List<Transform>();
    public List<MoveObject> scriptsOfObjectsInside = new List<MoveObject>();

    //private void OnTriggerStay2D(Collider2D other)
    //{
    //    if (HaveCells || !other.tag.Equals("RayObjects") || scriptsOfObjectsInside.Count < 1)
    //        return;

    //    bool a = scriptsOfObjectsInside[scriptsOfObjectsInside.Count - 1].DontMoveTo1stPosition;
    //    if (!a)
    //        scriptsOfObjectsInside[scriptsOfObjectsInside.Count - 1].DontMoveTo1stPosition = true;
    //}

    bool locked = false;
    private void checkHaveAll(bool showMessage)
    {
        var a = NameOfObjects.All(scriptsOfObjectsInside.Select(c => c.Tag).Contains) && NameOfObjects.Length == nameOfObjectsInside.Count;
        if (a)
        {
            locked = true;
            if(hasCheckButton)
                CheckButton.GetComponent<Button>().interactable = false;
            StartCoroutine(makeDisableAllObjects(!hasCheckButton));
        }
        else if (showMessage)
        {
            if(ErrorPanel != null)
                StartCoroutine(ErrorPanelAnimation());

            if (hasCheckButton)
                CheckButton.GetComponent<Button>().interactable = true;
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

    public void DeleteEntered(GameObject coll, bool onlyThis)
    {
        if (coll.transform.tag != "RayObjects" || !nameOfObjectsInside.Contains(coll.transform))
            return;

        currentGameObj = coll;


        coll.GetComponent<MoveObject>().DontMoveTo1stPosition = false;
        nameOfObjectsInside.Remove(currentGameObj.transform);
        scriptsOfObjectsInside.Remove(currentGameObj.GetComponent<MoveObject>());

        if (!onlyThis)
            OnCollisionRayObject = false;

        if (!hasCheckButton && !HaveCells)
            checkHaveAll(false);
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (locked)
            return;

        _onCollision = true;
        _triggeredObject = coll.gameObject.transform;

        if (!CheckInside)
            return;

        setCollObj(coll.transform);
        if (!hasCheckButton && !HaveCells)
            checkHaveAll(false);
    }
    private void setCollObj(Transform coll)
    {
        if (coll.tag.Equals("RayObjects"))
        {
            currentGameObj = coll.gameObject;

            nameOfObjectsInside.Add(_triggeredObject);
            scriptsOfObjectsInside.Add(coll.GetComponent<MoveObject>());
            scriptsOfObjectsInside.Last().DontMoveTo1stPosition = true;

            OnCollisionRayObject = true;
        }
    }
    private GameObject currentGameObj = null;
    void OnTriggerExit2D(Collider2D coll)
    {
        _onCollision = false;
        _triggeredObject = null;
        DeleteEntered(coll.transform.gameObject, false);
    }

    private void MoveObject_MouseUp(MoveObject currObj)
    {
        if (!CellScript.StaticObjects.Contains(currObj.transform))
            DeleteEntered(currObj.gameObject, true);
    }
}
