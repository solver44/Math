using UnityEngine;

public class CloudsAnimation : MonoBehaviour
{
    public GameObject parent = null; 
    public GameObject[] clouds;

    private int index = 0;

    [SerializeField]
    private float speedCloud=1;

    private GameObject cloud, cloud1;
    void Start()
    {
        cloud = Instantiate(clouds[index], GameObject.FindGameObjectWithTag("Clouds").transform, false) as GameObject;
        cloud.transform.localPosition = new Vector3(1920, 0);
    }
    void Update()
    {
        if (cloud.transform.localPosition.x >= 0)
        {
            cloud.transform.Translate(Vector3.left * speedCloud * Time.deltaTime);
            try
            {
                cloud1.transform.Translate(Vector3.left * speedCloud * Time.deltaTime);
            }
            catch { }
        }
        else
        {
            index = Random.Range(0, 3);
            cloud1 = cloud as GameObject;
            cloud = null;
            cloud = Instantiate(clouds[index], parent.transform, false) as GameObject;
            cloud.transform.localPosition = new Vector3(1920, 0);
        }

        try
        {
            if (cloud1.transform.localPosition.x <= -1920)
            {
                Destroy(cloud1);
            }
        }
        catch { }
    }
}
