using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickToolButton : MonoBehaviour
{
    private GameObject obj;
    private Animator anim;
    private SpriteRenderer color;
    int currentUnit = 0;
    int countUnits = 0;

    bool isBtnDisable = false;

    public MoveToLocation[] Units;
    private void Start()
    {
        currInfo = Camera.main.GetComponent<Info>() as Info;
        anim = GetComponent<Animator>() as Animator;
        color = GetComponent<SpriteRenderer>() as SpriteRenderer;
        countUnits = Camera.main.GetComponent<Info>().GetCount;
    }
    private void OnMouseDown()
    {
       // if(!isBtnDisable) if disable
            anim.SetBool("drag", true);
    }
    private void OnMouseUp()
    {
        anim.SetBool("drag", false);
        switch (this.transform.name)
        {
            case "BackButton":
                if (!isBtnDisable)
                {
                    Units[currentUnit].SetLocationX(6.5f, true);
                    Camera.main.GetComponent<Info>().CurrentUnit--;
                    currentUnit--;
                    Units[currentUnit].SetLocationX(0f, true);
                }
                else
                {
                    try
                    {
                        if((SceneManager.GetActiveScene().buildIndex - 1) != 2)
                            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
                        else
                            SceneManager.LoadScene(1);
                    }
                    catch { }
                }
                break;
            case "HomeButton":
                SceneManager.LoadScene(1);
                break;
            case "ForwardButton":
                if (!isBtnDisable)
                {
                    Units[currentUnit].SetLocationX(-6.5f, true);
                    Camera.main.GetComponent<Info>().CurrentUnit++;
                    currentUnit++;
                    Units[currentUnit].SetLocationX(0f, true);
                }
                else
                {
                    try
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    }
                    catch { }
                }
                break;
        }
    }

    private Touch touch;
    private void clickWithFinger()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(touch.deltaPosition).x, Camera.main.ScreenToWorldPoint(touch.deltaPosition).y), Vector2.zero, 0);
            if (hit && transform.name == hit.collider.transform.name)
            {
                if(touch.phase == TouchPhase.Began)
                {
                    if (!isBtnDisable)
                        anim.SetBool("drag", true);
                }
            }
        }
        else if(touch.phase == TouchPhase.Ended)
        {
            anim.SetBool("drag", false);
            switch (this.transform.name)
            {
                case "BackButton":
                    if (!isBtnDisable)
                    {
                        Units[currentUnit].SetLocationX(6.5f, true);
                        Camera.main.GetComponent<Info>().CurrentUnit--;
                        currentUnit--;
                        Units[currentUnit].SetLocationX(0f, true);
                    }
                    else
                    {
                        try
                        {
                            if ((SceneManager.GetActiveScene().buildIndex - 1) != 2)
                                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
                            else
                                SceneManager.LoadScene(1);
                        }
                        catch { }
                    }
                    break;
                case "HomeButton":
                    SceneManager.LoadScene(1);
                    break;
                case "ForwardButton":
                    if (!isBtnDisable)
                    {
                        Units[currentUnit].SetLocationX(-6.5f, true);
                        Camera.main.GetComponent<Info>().CurrentUnit++;
                        currentUnit++;
                        Units[currentUnit].SetLocationX(0f, true);
                    }
                    else
                    {
                        try
                        {
                            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                        }
                        catch { }
                    }
                    break;
            }
        }
    }
    Info currInfo;
    private void Update()
    {
        currentUnit = currInfo.CurrentUnit;
        //currentLevel = int.Parse(pathScene[pathScene.LastIndexOf('.') - 1].ToString());
        if (this.transform.name == "BackButton") 
        {
            if (currentUnit < 1)
            {
                color.color = new Color32(255, 255, 255, 255); //174
                isBtnDisable = true;
            }else
            {
                color.color = new Color32(255, 255, 255, 255);
                isBtnDisable = false;
            }
        }
        else if (this.transform.name == "ForwardButton")
        {
            if (currentUnit >= countUnits)
            {
                color.color = new Color32(255, 255, 255, 255);
                isBtnDisable = true;
            }
            else
            {
                color.color = new Color32(255, 255, 255, 255);
                isBtnDisable = false;
            }
        }
    }
}
