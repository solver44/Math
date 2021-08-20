using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellScript : MonoBehaviour
{
    public delegate void Cell(int r, int c, GameObject obj);
    public event Cell TouchedCell;

    public static bool collAnotherObj = false;
    GameObject tempObj = null;


    void Start()
    {
        MoveObject.MouseUp += MoveObject_MouseUp;
    }

    private void MoveObject_MouseUp(GameObject currObj)
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
            collAnotherObj = true;
            tempObj = coll.gameObject;
            onCollision = true;
            stop = true;
        }
    }
    void OnTriggerStay2D(Collider2D coll)
    {
        collAnotherObj = true;
    }
    void OnTriggerExit2D(Collider2D coll)
    {
        collAnotherObj = false;
        onCollision = false;
        tempObj = null;
        stop = false;
    }
}
