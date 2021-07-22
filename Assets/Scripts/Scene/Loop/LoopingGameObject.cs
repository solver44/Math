using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopingGameObject : MonoBehaviour
{
    [Header("Main")]
    public bool Infinite = true;
    public int Count = 0;
    public OnCollision2D ParentPlace = null;
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
    }

    public void DeleteFirst()
    {
        tempPrefabs.RemoveAt(0);
    }
    void Start()
    {
        if(ParentPlace != null)
            ParentPlace.changingColl += ParentPlace_changingColl;

        if (Infinite)
        {
            for (int i = 0; i < 2; i++)
            {
                tempPrefabs.Add(Instantiate(Prefab, this.transform.position, Quaternion.identity));
                tempPrefabs[i].transform.parent = this.transform;
                tempPrefabs[i].name = Name;
            }
        }
        else
        {
            for (int i = 0; i < Count; i++)
            {
                tempPrefabs.Add(Instantiate(Prefab, this.transform.position, Quaternion.identity));
                tempPrefabs[i].transform.parent = this.transform;
                tempPrefabs[i].name = Name;
            }
        }
    }

}
