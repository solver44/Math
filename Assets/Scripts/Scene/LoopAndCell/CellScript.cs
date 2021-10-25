using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellScript : MonoBehaviour
{
    public delegate void Cell(int r, int c, GameObject obj);
    public event Cell TouchedCell;

    public static List<Transform> StaticObjects = new List<Transform>();
    GameObject tempObj = null;


    void Start()
    {
        MoveObject.MouseUp += MoveObject_MouseUp;
    }

    private void MoveObject_MouseUp(MoveObject currObj)
    {
        if (!currObj.name.Contains("cube") || !onCollision || tempObj == null)
        {
            return;
        }

        string inds = transform.name.Remove(0, 4);
        TouchedCell?.Invoke(ushort.Parse(inds.Split('.')[0]), ushort.Parse(inds.Split('.')[1]), tempObj);
    }
    bool stop = false;
    bool onCollision = false;
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (stop)
            return;
        if(coll.gameObject.name.Contains("cube"))
        {
            tempObj = coll.gameObject;
            StaticObjects.Add(tempObj.transform);
            onCollision = true;
            stop = true;
        }
    }
    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.name.Contains("cube") && tempObj != null)
            StaticObjects.Remove(tempObj.transform);
        onCollision = false;
        tempObj = null;
        stop = false;
    }
}
