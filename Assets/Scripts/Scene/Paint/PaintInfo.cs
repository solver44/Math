using UnityEngine;

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
            WasUnitComplete unit = Parent;
            unit.CompleteUnit();
        }catch{}
    }
}
