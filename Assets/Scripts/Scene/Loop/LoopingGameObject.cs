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
    [Header("Additional")]
    public float RangeX = 0f;
    public float RangeY = 0f;

    private List<GameObject> tempPrefabs = new List<GameObject>();

    private void ParentPlace_changingColl(bool coll, string name)
    { 
        if (name != this.name)
            return;

        if (coll)
            CreateOneMore();
        else
            DeleteFirst();
    }

    public void CreateOneMore()
    {
        tempPrefabs.Add(Instantiate(Prefab, this.transform.position, Quaternion.identity));
        int cnt = tempPrefabs.Count - 1;
        tempPrefabs[cnt].transform.parent = this.transform;
        tempPrefabs[cnt].name = Name;
        //tempPrefabs[cnt].GetComponent<MoveObject>().Uping += ParentPlace_changingColl;
    }

    public void DeleteFirst()
    {
        if (tempPrefabs.Count < 1)
            return;

        Destroy(tempPrefabs[tempPrefabs.Count - 1]);
        tempPrefabs.RemoveAt(tempPrefabs.Count - 1);
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
