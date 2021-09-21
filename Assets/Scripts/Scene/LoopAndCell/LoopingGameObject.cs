using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

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
    public WasUnitComplete Unit = null;
    //Values
    public Vector2[] dontCreateThesePlaces;
    public Vector2[] AnswersPlaces;
    [Header("Additional")]
    public float RangeX = 0f;
    public float RangeY = 0f;

    private RectTransform[,] cells;

    public List<GameObject> tempPrefabs = new List<GameObject>();

    public static GameObject CurrentObj = null;

    RectTransform rectT;


    Touch touch;
    private void Awake()
    {
        if(Cells)
        {
            cells = new RectTransform[Rows, Columns];
            rectT = ParentPlace[0].transform.GetComponent<RectTransform>();
            float width = rectT.rect.width / Columns;
            float height = rectT.rect.height / Rows;

            BoxCollider2D coll = new BoxCollider2D();
            Rigidbody2D rb = new Rigidbody2D();

            Vector2 leftUpCorner = new Vector2(0 - (rectT.rect.width / 2) + width / 2, 0 - rectT.rect.height / 2 + height / 2);
            float firstX = leftUpCorner.x;
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    if(dontCreateThesePlaces.Contains(new Vector2(r, c)))
                    {
                        leftUpCorner = new Vector2(leftUpCorner.x + width, leftUpCorner.y);
                        continue;
                    }
                    cells[r, c] = Instantiate(Cell, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<RectTransform>();
                    cells[r, c].parent = rectT.transform;
                    cells[r, c].name = "cell" + r.ToString() + "." + c.ToString();
                    cells[r, c].localScale = new Vector3(1, 1, 1);
                    cells[r, c].rect.Set(leftUpCorner.x, leftUpCorner.y, width, height);
                    cells[r, c].localPosition = leftUpCorner;
                    coll = cells[r, c].transform.gameObject.AddComponent<BoxCollider2D>();
                    rb = cells[r, c].transform.gameObject.AddComponent<Rigidbody2D>();
                    cells[r, c].transform.gameObject.AddComponent<CellScript>();
                    cells[r, c].transform.GetComponent<CellScript>().TouchedCell += LoopingGameObject_TouchedCell;
                    rb.bodyType = RigidbodyType2D.Kinematic;
                    rb.useFullKinematicContacts = true;
                    coll.isTrigger = true;
                    coll.size = new Vector2(width/ 4, height / 4);
                    leftUpCorner = new Vector2(leftUpCorner.x + width, leftUpCorner.y);
                }
                leftUpCorner = new Vector2(firstX, leftUpCorner.y + height);
            }
        }
    }

    List<Vector2> objPlaces = new List<Vector2>();
    private void LoopingGameObject_TouchedCell(int r, int c, GameObject obj)
    {
        obj.transform.position = cells[r, c].transform.position;
        if (objPlaces.Contains(new Vector2(r, c)))
            objPlaces.Remove(new Vector2(r, c));

        objPlaces.Add(new Vector2(r, c));
        if(objPlaces.Count == AnswersPlaces.Count())
            checkIt();
    }

    private void checkIt()
    {
        bool getBoolean = objPlaces.All(AnswersPlaces.Contains);
        if (getBoolean)
            Unit.CompleteUnit();
    }

    private void ParentPlace_changingColl(bool coll, GameObject child)
    {
        if (child.transform.parent.name != this.name)
            return;

        if (coll)
            CreateOneMore(false);
        else
        {
            DeleteFirst();
            CurrentObj = child;
        }
    }

    GameObject lastCraetedElem = null;
    public void CreateOneMore(bool dontCreateMoreOne)
    {
        if ((dontCreateMoreOne && tempPrefabs.Count < 2) || !dontCreateMoreOne)
        {
            tempPrefabs.Add(Instantiate(Prefab, Vector3.zero, Quaternion.identity));
            int cnt = tempPrefabs.Count - 1;
            tempPrefabs[cnt].transform.parent = this.transform;
            tempPrefabs[cnt].name = Name;
            tempPrefabs[cnt].transform.localPosition = Vector3.zero;
            CurrentObj = tempPrefabs[cnt];
            lastCraetedElem = tempPrefabs[cnt];
        }
        //tempPrefabs[cnt].GetComponent<MoveObject>().Uping += ParentPlace_changingColl;
    }

    public void DeleteFirst()
    {
        if (tempPrefabs.Count < 1)
            return;

        Destroy(lastCraetedElem);
        tempPrefabs.Remove(lastCraetedElem);
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
