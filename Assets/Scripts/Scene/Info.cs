using UnityEngine;

public class Info : MonoBehaviour
{
    [SerializeField] private int countOfUnits = 0;
    public int GetCount
    { get { return countOfUnits; } }

    private int currentUnit = 0;
    public int CurrentUnit
    {
        get { return currentUnit; }
        set { currentUnit = value; }
    }

    private void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
    }

    void Resize()
    {
        float xFactor = Screen.width / 1080f;
        float yFactor = Screen.height / 1920f;
        float yPos = (1 - (xFactor / yFactor)) / 2;

        Camera.main.rect = new Rect(0, yPos, 1, xFactor / yFactor);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        transform.localScale = new Vector3(1, 1, 1);

        float width = sr.sprite.bounds.size.x;
        float height = sr.sprite.bounds.size.y;


        float worldScreenHeight = Camera.main.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        Vector3 xWidth = transform.localScale;
        xWidth.x = worldScreenWidth / width;
        transform.localScale = xWidth;

        Vector3 yHeight = transform.localScale;
        yHeight.y = worldScreenHeight / height;
        transform.localScale = yHeight;

    }

}
