using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WasUnitComplete : MonoBehaviour
{
    public GameObject[] ObjectsToExitScene;
    [Space]
    public GameObject ParentGameObjects;
    [SerializeField] private Vector2 toThatLocation;
    [SerializeField] private Vector2 toThatLocationCanvas;
    [SerializeField] private float _speedMove = 5;
    [SerializeField] private int unitNumber;
    [SerializeField] private int countOfDifference = 5;
    [SerializeField] private float waitForSecondToExit = 0f;

    public GameObject[] ObjectsToEnterScene;

    NextUnit _sceneManager;

    string _lvlPrefs;
    private void Start()
    {
        _sceneManager = new NextUnit();
        _lvlPrefs = "lvl" + PlayerPrefs.GetInt("ClickingLevel") + "." + (SceneManager.GetActiveScene().buildIndex - 1) + "." + unitNumber;
    }
    private int _countDifference;
    public int SetCountOfDifference
    {
        get { return _countDifference;  }
        set { _countDifference = value; }
    }
    private bool wait = true;
    private void Update()
    {
        if (ParentGameObjects.transform.childCount <= 0 || _countDifference >= countOfDifference)
        {
            _sceneManager.SetItemsValue(ObjectsToExitScene, ObjectsToEnterScene, _lvlPrefs, _speedMove, toThatLocation, toThatLocationCanvas);
            
            if (wait)
            {
                StartCoroutine(waitForSec());
            }
            else
            {
                _sceneManager.DoExitAndSaveUnit();
                _sceneManager.DoEnterNewUnit();
                _sceneManager.DestroyObject(this.transform.gameObject);
            }
        }
    }

    private IEnumerator waitForSec()
    {
        wait = true;
        yield return new WaitForSeconds(waitForSecondToExit);
        wait = false;
    }
}
