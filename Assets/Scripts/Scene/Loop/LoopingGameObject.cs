using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopingGameObject : MonoBehaviour
{
    [Header("Main")]
    public bool Infinite = true;
    public int Count = 0;
    public OnCollision2D[] ParentPlace = null;
    public GameObject Prefab;
    public string Name = "temp";
    [Header("Cells main")]
    public bool Cells = false;
    public int Columns = 0;   //bo'yi 
    public int Rows = 0;    //eni
    public GameObject Cell = null;
    [Header("Additional")]
    public float RangeX = 0f;
    public float RangeY = 0f;

    private RectTransform[,] cells;

    private List<GameObject> tempPrefabs = new List<GameObject>();

    public static GameObject CurrentObj = null;

    RectTransform rectT;
    private void Awake()
    {
        if(Cells)
        {
            cells = new RectTransform[Rows, Columns];
            rectT = ParentPlace[0].transform.GetComponent<RectTransform>();
            float width = rectT.rect.width / Columns;
            float height = rectT.rect.height / Rows;

            Vector2 leftUpCorner = new Vector2(0 - (rectT.rect.width / 2), 0 - rectT.rect.height / 2);
            float firstX = leftUpCorner.x;
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    cells[r, c] = Instantiate(Cell, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<RectTransform>();
                    cells[r, c].parent = rectT.transform;
                    cells[r, c].name = "cell" + (r * 10 + c + 1).ToString();
                    cells[r, c].localScale = new Vector3(1, 1, 1);
                    cells[r, c].rect.Set(leftUpCorner.x, leftUpCorner.y, width, height);
                    cells[r, c].localPosition = leftUpCorner;
                    leftUpCorner = new Vector2(leftUpCorner.x + width, leftUpCorner.y);
                }
                leftUpCorner = new Vector2(firstX, leftUpCorner.y + height);
            }
        }
    }
    private void ParentPlace_changingColl(bool coll, GameObject child)
    {
        if (child.transform.parent.name != this.name)
            return;

        if (coll)
            CreateOneMore();
        else
        {
            DeleteFirst();
            CurrentObj = child;
        }
    }

    public void CreateOneMore()
    {
        tempPrefabs.Add(Instantiate(Prefab, this.transform.position, Quaternion.identity));
        int cnt = tempPrefabs.Count - 1;
        tempPrefabs[cnt].transform.parent = this.transform;
        tempPrefabs[cnt].name = Name;
        CurrentObj = tempPrefabs[cnt];
        //tempPrefabs[cnt].GetComponent<MoveObject>().Uping += ParentPlace_changingColl;
    }

    public void DeleteFirst()
    {
        if (tempPrefabs.Count < 1)
            return;

        Destroy(CurrentObj);
        tempPrefabs.Remove(CurrentObj);
    }
    void Start()
    {
        if (Infinite)
        {
            if (ParentPlace.Length > 0)
            {
                for (int i = 0; i < ParentPlace.Length; i++)
                {
                    ParentPlace[i].changingColl += ParentPlace_changingColl;
                }
            }

            tempPrefabs.Add(Instantiate(Prefab, this.transform.position, Quaternion.identity));
            tempPrefabs[0].transform.parent = this.transform;
            tempPrefabs[0].name = Name;
            //tempPrefabs[0].GetComponent<MoveObject>().Uping += ParentPlace_changingColl;
        }
        else
        {
            for (int i = 0; i < Count; i++)
            {
                tempPrefabs.Add(Instantiate(Prefab, this.transform.position, Quaternion.identity));
                tempPrefabs[i].transform.parent = this.transform;
                tempPrefabs[i].name = Name;
                //tempPrefabs[i].GetComponent<MoveObject>().Uping += ParentPlace_changingColl;
            }
        }
    }

}
