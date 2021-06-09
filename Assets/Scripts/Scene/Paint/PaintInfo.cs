using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintInfo : MonoBehaviour
{
    //Количество фигур
    public int SetCount;
    //Текущий количество
    private int count = 0;

    public int Count
    {
        get { return count; }
        set { count = value; checkToNextUnit(count); }
    }

    private void checkToNextUnit(int count)
    {
        try
        {
            if (count != SetCount)
                return;

            WasUnitComplete unit = this.GetComponentInParent<Transform>().transform.GetComponentInParent<WasUnitComplete>();
            unit.CompleteUnit();
        }catch{}
    }
}
