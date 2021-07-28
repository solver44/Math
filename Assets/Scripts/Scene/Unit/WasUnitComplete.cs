using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
    
[RequireComponent(typeof(MoreAR))]
public class WasUnitComplete : MonoBehaviour
{
    #region Events
    public delegate void Finish(int unit);
    public static event Finish Finishing;
    #endregion

    [Header("Main")]
    public GameObject[] ObjectsToExitScene;
    [Space]
    public GameObject ParentGameObjects;
    [Header("Location")]
    [SerializeField] private Vector2 toThatLocation;
    [SerializeField] private bool freezeY = false;
    [SerializeField] private Vector2 toThatLocationCanvas;
    [SerializeField] private bool freezeYCanvas = false;
    [SerializeField] private float _speedMove = 5;
    [Header("Condition")]
    [SerializeField] private int unitNumber;
    [SerializeField] private int countOfDifference = 5;
    [Header("Additional")]
    [SerializeField] private float waitForSecondToExit = 0f;
    [SerializeField] private Color colorToChange;
    [SerializeField] private GameObject[] objectsToChangeColor = null;
    [Header("Main")]
    public GameObject[] ObjectsToEnterScene;

    NextUnit _sceneManager;

    string _lvlPrefs;

    //MoreAR
    MoreAR more = null;

    //For Color
    private bool changeColor = false;
    private bool[] isSpriteRenderer;
    private void Start()
    {
        if(TryGetComponent<MoreAR>(out more))
            more.Speed = _speedMove;

        try
        {
            isSpriteRenderer = new bool[objectsToChangeColor.Length];
        }
        catch { }
        if (objectsToChangeColor != null)
        {
            changeColor = true;
            SpriteRenderer temp;
            for (int i = 0; i < objectsToChangeColor.Length; i++)
            {
                isSpriteRenderer[i] = objectsToChangeColor[i].TryGetComponent<SpriteRenderer>(out temp);
            }
        }


        _lvlPrefs = "lvl" + PlayerPrefs.GetInt("ClickingLevel") + "." + (SceneManager.GetActiveScene().buildIndex - 1) + "." + unitNumber;

        if (freezeY)
            toThatLocation = new Vector2(toThatLocation.x, ParentGameObjects.transform.position.y);
        if (freezeYCanvas) {
            float constY = 0;
            for (int i = 0; i < ObjectsToExitScene.Length; i++)
            {
                if (ObjectsToExitScene[i].transform.name.ToLower().Contains("panel"))
                {
                    constY = ObjectsToExitScene[i].transform.localPosition.y;
                    break;
                }
            }
            toThatLocationCanvas = new Vector2(toThatLocationCanvas.x, constY);
        }

        _sceneManager = gameObject.AddComponent<NextUnit>();
        _sceneManager.SetItemsValue(ObjectsToExitScene, ObjectsToEnterScene, _lvlPrefs, _speedMove, toThatLocation, toThatLocationCanvas);
    }
    private int _countDifference;
    public int SetCountOfDifference
    {
        get { return _countDifference;  }
        set { _countDifference = value; }
    }
    private bool wait = true;

    protected bool isCompleteThisUnit = false;

    public void CompleteUnit()
    {
        if ((more == null || !more.More) || more.Finish)
            isCompleteThisUnit = true;
        else
        {
            more.Exit();
            more.Enter();
        }
    }

    static int currentUnit = 1;
    bool finish = false;
    public static int CurrentUnit
    {
        get { return currentUnit; }
        set { currentUnit = value; Finishing?.Invoke(currentUnit); }
    }
    private void FixedUpdate()
    {
        if(ParentGameObjects.transform.childCount <= 0 || _countDifference >= countOfDifference)
        {
            if ((more == null || !more.More) || more.Finish)
                isCompleteThisUnit = true;
            else
            {
                more.Exit();
                more.Enter();
            }
        }

        if (isCompleteThisUnit)
        {
            if (!stop && wait)
            {
                StartCoroutine(waitForSec());
            }
            else
            {
                if (!finish)
                {
                    finish = true;
                    CurrentUnit++;
                }
                _sceneManager.DoExitAndSaveUnit();
                _sceneManager.DoEnterNewUnit();
                if (!changingColor && changeColor)
                {
                    for (int i = 0; i < objectsToChangeColor.Length; i++)
                    {
                        StartCoroutine(changeColorIE(i));
                    }
                }
            }
        }
    }

    bool stop = false;
    private IEnumerator waitForSec()
    {
        wait = true;
        yield return new WaitForSeconds(waitForSecondToExit);
        wait = false;
    }

    bool changingColor = false;
    private IEnumerator changeColorIE(int index)
    {
        changingColor = true;
        float scaleDuration = 10f;
        SpriteRenderer spriteR = null;
        Image imageR = null;
        if (isSpriteRenderer[index])
            spriteR = objectsToChangeColor[index].GetComponent<SpriteRenderer>();
        else
            imageR = objectsToChangeColor[index].GetComponent<Image>();
        for (float i = 0; i < 1; i += Time.deltaTime / scaleDuration)
        {
            if (spriteR == null)
                imageR.color = Color.Lerp(imageR.color, colorToChange, i);
            else
                spriteR.color = Color.Lerp(spriteR.color, colorToChange, i);

            if ((spriteR != null && spriteR.color.Equals(colorToChange)) || imageR.color.Equals(colorToChange))
            {
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
