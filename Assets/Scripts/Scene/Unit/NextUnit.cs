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
        if (unit.Length < 1)
            stop = true;

        objects = objects1;
        nextUnit = unit;
        textPrefs = textPrefs1;
        speed = speed1;
        location = location1;
        locationCanvas = locationCanvas1;
    }

    private int countOfObjectsToExit = 0, countOfObjectsToEnter = 0;
    public void DoExitAndSaveUnit()
    {
        if (destroy || stopExit)
            return;

        PlayerPrefs.SetInt(textPrefs, 1);

        for (int i = 0; i < objects.Length; i++)
        {
            Transform temp = objects[i].transform;
            string parentName = objects[i].transform.parent.name;

            if (temp.parent.name != "Backgrounds")
                temp.position = Vector2.Lerp(temp.position, location, speed * Time.deltaTime);
            else
                temp.localPosition = Vector2.Lerp(temp.localPosition, locationCanvas, speed * Time.deltaTime);

            if ((Vector2.Distance(temp.position, location) > .05f && temp.parent.name != "Backgrounds") || (Vector2.Distance(temp.localPosition, locationCanvas) > .05f && temp.parent.name == "Backgrounds"))
            {
                if (temp.parent.name != "Backgrounds")
                    temp.position = Vector2.Lerp(temp.position, location, speed * Time.deltaTime);
                else
                    temp.localPosition = Vector2.Lerp(temp.localPosition, locationCanvas, speed * Time.deltaTime);
            }
            else
            {
                if (temp.parent.name != "Backgrounds")
                    temp.position = location;
                else
                    temp.localPosition = locationCanvas;
                //temp.position = new Vector2(0, temp.localPosition.y);
                countOfObjectsToExit++;
            }

            if (countOfObjectsToExit >= objects.Length)
            {
                stopExit = true;
                if (stop)
                    DestroyObject(objects);
            }
        }
        //StartCoroutine(destroyObject());
    }
    private bool destroy = false, stopExit = false;
    public void DestroyObject(GameObject[] tempParents)
    {
        if (!destroy)
        {
            destroy = true;
            for (int i = 0; i < tempParents.Length; i++)
            {
                if (tempParents[i].name != "GameObjects")
                    Destroy(tempParents[i]);
                else
                    Destroy(tempParents[i].transform.parent.gameObject);
            }
        }
    }
    bool stop = false;
    public void DoEnterNewUnit()
    {
        if (stop)
            return;

        for (int i = 0; i < nextUnit.Length; i++)
        {
            Transform temp = nextUnit[i].transform;
            string parentName = nextUnit[i].transform.parent.name;

            if ((Vector2.Distance(temp.position, new Vector2(0, temp.position.x)) > .05f && temp.parent.name != "Backgrounds") 
                || (Vector2.Distance(temp.localPosition, new Vector2(0, temp.localPosition.y)) > .05f && temp.parent.name == "Backgrounds"))
            {
                if (parentName != "Backgrounds")
                    temp.position = Vector2.Lerp(temp.position, new Vector2(0, temp.position.y), speed * Time.deltaTime);
                else
                    temp.localPosition = Vector2.Lerp(temp.localPosition, new Vector2(0, temp.localPosition.y), speed * Time.deltaTime);
            }
            else
            {
                if (parentName != "Backgrounds")
                    temp.position = new Vector2(0, temp.position.y);
                else
                    temp.localPosition = new Vector2(0, temp.localPosition.y);
                //temp.position = new Vector2(0, temp.localPosition.y);
                countOfObjectsToEnter++;
            }

            if (countOfObjectsToEnter >= nextUnit.Length)
            {
                stop = true;
                DestroyObject(objects);
            }
        }
        
    }

}
