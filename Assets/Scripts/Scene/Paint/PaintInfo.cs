using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PaintInfo : MonoBehaviour
{
    public WasUnitComplete Parent;
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

            MoreAR ar = null;
            Parent.transform.TryGetComponent<MoreAR>(out ar);
            if (ar != null)
                ar.Finish = true;
            Parent.CompleteUnit();
        }catch{}
    }

    EqualShapes temp = null;
    public void CheckUnit(Button checkBtn)
    {
        bool isTrue = false;
        List<int> tops = new List<int>();
        List<int> other = new List<int>();
        for (int i = 0; i < transform.childCount; i++)
        {
            temp = transform.GetChild(i).GetComponent<EqualShapes>();
            if (temp.OnTop)
                tops.Add(temp.GetComponent<SpriteRenderer>().sortingLayerID);
            else
                other.Add(temp.GetComponent<SpriteRenderer>().sortingLayerID);
        }
        for (int i = 0; i < tops.Count; i++)
        {
            if (other.Where(c => c > tops[i]).Count() < 1)
                isTrue = true;
            else
                isTrue = false;
        }

        if(isTrue)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).tag = "Untagged";
            }
            checkBtn.interactable = false;
            Parent.CompleteUnit();
        }
    }
}
