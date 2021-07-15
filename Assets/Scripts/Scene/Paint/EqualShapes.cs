using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EqualShapes : MonoBehaviour
{
    public GameObject TargetShape = null;
    private string nameOfShape => TargetShape.transform.name;
    private PaintInfo info => this.GetComponentInParent<PaintInfo>();

    public bool CheckIt(string name, bool sumCount, bool dontCheck)
    {
        if(dontCheck)
        {
            info.Count++;
            return true;
        }


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
