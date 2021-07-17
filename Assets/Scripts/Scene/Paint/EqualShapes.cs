using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EqualShapes : MonoBehaviour
{
    public GameObject TargetShape = null;
    private string nameOfShape = "";
    private PaintInfo info => this.GetComponentInParent<PaintInfo>();

    void Start()
    {
        if(TargetShape != null)
            nameOfShape = TargetShape.transform.name;
    }
    public bool CheckIt(string name, bool sumCount, bool dontCheck)
    {
        if(dontCheck)
        {
            info.Count++;
            return true;
        }

        if (TargetShape == null)
            nameOfShape = name;

        if (nameOfShape.Equals(name))
        {
            if(sumCount)
                info.Count++;
            return true;
        }
        else
        {
            return false;
        }
    }
}
