using UnityEngine;

public class NextUnit : MonoBehaviour
{
    //public GameObject FirstUnit;
    //public GameObject Seconfunit;
    //public GameObject ThirdUnit;
    //public GameObject FourthUnit;
    //public GameObject FifthUnit;

    //string lvlPrefs;

    private GameObject[] objects;
    private GameObject[] nextUnit;
    private string textPrefs;
    private float speed;
    private Vector2 location;
    private Vector2 locationCanvas;

    private bool _isExit = false;
    public bool IsExit { get { return _isExit; } }


    public void SetItemsValue(GameObject[] objects1, GameObject[] unit, string textPrefs1, float speed1, Vector2 location1, Vector2 locationCanvas1)
    {
        objects = objects1;
        nextUnit = unit;
        textPrefs = textPrefs1;
        speed = speed1;
        location = location1;
        locationCanvas = locationCanvas1;
    }

    public void DoExitAndSaveUnit()
    {
        PlayerPrefs.SetInt(textPrefs, 1);

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i].transform.parent.name != "Backgrounds")
                objects[i].transform.position = Vector2.Lerp(objects[i].transform.position, location, speed * Time.deltaTime);
            else
                objects[i].transform.localPosition = Vector2.Lerp(objects[i].transform.localPosition, locationCanvas, speed * Time.deltaTime);
        }
        //StartCoroutine(destroyObject());
    }
    private bool destroy = false;
    public void DestroyObject(GameObject tempParent)
    {
        if (!destroy)
        {
            Destroy(tempParent, 2);
            destroy = true;
        }
    }
    bool stop = false;
    public void DoEnterNewUnit()
    {
        stop = false;
        for (int i = 0; i < nextUnit.Length; i++)
        {
            if ((nextUnit[i].transform.position.x > 0 + .1f || nextUnit[i].transform.position.x < 0 - .1f) && !stop)
            {
                if (nextUnit[i].transform.parent.name != "Backgrounds")
                    nextUnit[i].transform.position = Vector2.Lerp(nextUnit[i].transform.position, new Vector2(0, nextUnit[i].transform.position.y), speed * Time.deltaTime);
                else
                    nextUnit[i].transform.localPosition = Vector2.Lerp(nextUnit[i].transform.localPosition, new Vector2(0, nextUnit[i].transform.localPosition.y), speed * Time.deltaTime);
            }
            else
            {
                if (nextUnit[i].transform.parent.name != "Backgrounds")
                    nextUnit[i].transform.position = new Vector2(0, nextUnit[i].transform.position.y);
                else
                    nextUnit[i].transform.localPosition = new Vector2(0, nextUnit[i].transform.localPosition.y);
                //nextUnit[i].transform.position = new Vector2(0, nextUnit[i].transform.localPosition.y);
                stop = true;
            }
        }
        
    }

}
