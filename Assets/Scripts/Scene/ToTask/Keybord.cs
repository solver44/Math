using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Keybord : MonoBehaviour
{
    public Transform ParentKeyboard = null;
    public GameObject KeyboardPrefab = null;
    public int FontSize = 300;

    [Space]
    public WasUnitComplete[] Units;

    Touch touch;
    Vector2 firstPos;

    IDictionary<string, string> listAnswers;
    IDictionary<string, string> listValueAnswers;

    private void Awake()
    {
        listValueAnswers = new Dictionary<string, string>();
        listAnswers = new Dictionary<string, string>();
        makeReady();
        firstPos = keyboard.transform.localPosition; 
    }

    private void Start()
    {
        WasUnitComplete.Finishing += WasUnitComplete_Finishing;
        foreach (var item in OpacityEffect.AllKeybordValues.Where(c => c.Value.Split(';')[1] == WasUnitComplete.CurrentUnit.ToString()))
        {
            string val = item.Value.Split(';')[0];
            listValueAnswers.Add(item.Key, val);
        }
    }

    private void WasUnitComplete_Finishing(int unit)
    {
        foreach (var item in OpacityEffect.AllKeybordValues.Where(c => c.Value.Split(';')[1] == WasUnitComplete.CurrentUnit.ToString()))
        {
            string val = item.Value.Split(';')[0];
            listValueAnswers.Add(item.Key, val);
        }
    }

    private GameObject currentText = null;

    Image tempImg = null;
    void SetRayCast(RaycastHit2D hitTouch)
    {
        if (hitTouch && hitTouch.collider.transform.CompareTag("KeyboardValue"))
        {
            if (currentText != null && currentText.TryGetComponent<Image>(out tempImg))
            {
                currentText.GetComponent<OpacityEffect>().Stop = true;
                if(currentSprite != null)
                    currentText.GetComponent<Image>().sprite = currentSprite;
                currentText.GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
                currentText.GetComponent<OpacityEffect>().Stop = false;
            }

            currentText = hitTouch.transform.gameObject;
            if (currentText.TryGetComponent<Image>(out tempImg))
            {
                currentText.GetComponent<OpacityEffect>().Stop = true;
                currentText.GetComponent<Image>().color = new Color32(255, 195, 100, 150);
                currentSprite = currentText.GetComponent<Image>().sprite;
            }
            else
                currentText.TryGetComponent<Text>(out textComponent);

            parentName = currentText.transform.parent.name;
            ChangeKeyboardType();
            StartCoroutine(showKeyboard());
        }
    }

    KeyboardType type => keyboard.GetComponent<KeyboardType>();
    private void ChangeKeyboardType()
    {
        string contentType = OpacityEffect.AllKeybordValues.FirstOrDefault(c => c.Key == parentName).Value;
        contentType = contentType.Split(';')[2];

        switch (contentType)
        {
            case "Numbers":
                type.Numbers.SetActive(true);
                type.Symbols.SetActive(false);
                type.Alphabetics.SetActive(false);
                break;
            case "Symbols":
                type.Numbers.SetActive(false);
                type.Symbols.SetActive(true);
                type.Alphabetics.SetActive(false);
                break;
            case "Alphabetics":
                type.Numbers.SetActive(false);
                type.Symbols.SetActive(false);
                type.Alphabetics.SetActive(true);
                break;
        }
    }

    private GameObject keyboard = null;
    private void makeReady()
    {
        if (keyboard != null || ParentKeyboard == null)
            return;

        keyboard = Instantiate(KeyboardPrefab, KeyboardPrefab.transform.position, Quaternion.identity);
        keyboard.name = "Keyboard";
        keyboard.transform.parent = ParentKeyboard;
        keyboard.transform.localScale = new Vector3(1, 1, 1);
        keyboard.transform.localPosition = KeyboardPrefab.transform.localPosition;

        foreach (var item in keyboard.GetComponentsInChildren<Button>())
        {
            item.onClick.AddListener(delegate { onClickButton(item.gameObject, item.name); });
        }
    }

    Image img;
    Text textComponent;

    private void checkAllWrongAnswers()
    {
        GameObject parent = currentText.transform.parent.transform.parent.transform.parent.gameObject;
        List<GameObject> parents = new List<GameObject>();
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            parents.Add(parent.transform.GetChild(i).transform.gameObject);
        }

        for (int k = 0; k < parents.Count; k++)
        {
            for (int i = 0; i < parents[k].transform.childCount; i++)
            {
                try {
                    if (parents[k].transform.GetChild(i).transform.GetChild(0).name == "ef")
                    {
                        GameObject temp = parents[k].transform.GetChild(i).transform.GetChild(0).gameObject;

                        StartCoroutine(setWhiteColor(temp));
                    }
                }
                catch { }
            }
        }
    }

    private IEnumerator setWhiteColor(GameObject temp)
    {
        Text tempText = null;
        if (!temp.TryGetComponent<Text>(out tempText))
        {
            temp.GetComponent<OpacityEffect>().Stop = true;
            temp.GetComponent<Image>().color = new Color32(255, 0, 0, 100);
            temp.GetComponent<OpacityEffect>().Stop = false;
        }
        else if (listValueAnswers.ContainsKey(temp.transform.parent.name) && tempText.text != listValueAnswers[temp.transform.parent.name])
        {
            tempText.color = new Color32(255, 0, 0, 255);
        }
        yield return new WaitForSeconds(2);
        if (!temp.TryGetComponent<Text>(out tempText))
        {
            temp.GetComponent<OpacityEffect>().Stop = true;
            temp.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
            temp.GetComponent<OpacityEffect>().Stop = false;
        }
        else
        {
            tempText.color = currentTextColor;
        }
    }

    Text tempText = null;
    private void write(string num)
    {
        if (currentText == null)
            return;

        if (!listAnswers.ContainsKey(parentName))
            listAnswers.Add(parentName, num.ToString());
        else {
            listAnswers.Remove(parentName);
            listAnswers.Add(parentName, num.ToString());
        }


        if (!currentText.TryGetComponent<Text>(out tempText)) {
            currentText.GetComponent<OpacityEffect>().Stop = true;
            Destroy(currentText.GetComponent<Image>());
            StartCoroutine(addComponent(num));
        }else
        {
            textComponent = tempText;
            if(type.Symbols.activeSelf && textComponent.text.Length < 1)
                textComponent.text += num.ToString();
            else if(!type.Symbols.activeSelf)
                textComponent.text += num.ToString();

            if (listAnswers.ContainsKey(parentName))
            {
                listAnswers.Remove(parentName);
                listAnswers.Add(parentName, textComponent.text);
            }
        }
    }
    string parentName;

    Color currentTextColor;
    Sprite currentSprite;
    private IEnumerator addComponent(string num)
    {
        yield return new WaitForSeconds(0.1f);

        textComponent = currentText.AddComponent<Text>();

        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.fontSize = FontSize;
        string[] rgb = OpacityEffect.AllKeybordValues[parentName].Split(';')[3].Split('/');
        currentTextColor = new Color(float.Parse(rgb[0]), float.Parse(rgb[1]), float.Parse(rgb[2]));

        textComponent.color = currentTextColor;
        textComponent.resizeTextForBestFit = true;
        textComponent.resizeTextMaxSize = FontSize;

        textComponent.text += num.ToString();
    }
    private void onClickButton(GameObject button, string nameBtn)
    {
        if (isAnim)
            return;
        switch (nameBtn)
        {
            case "Num1":
                write("1");
                break;
            case "Num2":
                write("2");
                break;
            case "Num3":
                write("3");
                break;
            case "Num4":
                write("4");
                break;
            case "Num5":
                write("5");
                break;
            case "Num6":
                write("6");
                break;
            case "Num7":
                write("7");
                break;
            case "Num8":
                write("8");
                break;
            case "Num9":
                write("9");
                break;
            case "Num0":
                write("0");
                break;
            case "DontEqual":
                write("≠");
                break;
            case "Equal":
                write("=");
                break;
            case "BigSymbol":
                write("<");
                break;
            case "SmallSymbol":
                write(">");
                break;
            case "PlusSymbol":
                write("+");
                break;
            case "MinusSymbol":
                write("-");
                break;
            case "BackSpace":
                delete();
                break;
            case "Check":
                checkAllWrongAnswers();
                check();
                break;
        }
    }
    private void check()
    {
        if (listAnswers.All(listValueAnswers.Contains) && listAnswers.Count == listValueAnswers.Count)
        {
            Units[WasUnitComplete.currentUnit - 1].CompleteUnit();
        }

        StartCoroutine(hideKeyboard());
    }
    private void delete()
    {
        if (textComponent == null)
            return;

        if (textComponent.text.Length > 1)
        {
            textComponent.text = textComponent.text.Remove(textComponent.text.Length - 1);
            if (listAnswers.ContainsKey(parentName))
            {
                listAnswers.Remove(parentName);
                listAnswers.Add(parentName, textComponent.text);
            }
        }
        else
        {
            if (listAnswers.ContainsKey(parentName))
                listAnswers.Remove(parentName);
            StartCoroutine(deleteComponent());
        }
    }
    private IEnumerator deleteComponent()
    {
        if(currentText.TryGetComponent<Text>(out textComponent))
            Destroy(textComponent);
        yield return new WaitForSeconds(0.1f);
        Image temp = currentText.AddComponent<Image>();
        if (currentSprite != null) 
            temp.sprite = currentSprite;
        yield return new WaitForSeconds(0.1f);
        currentText.GetComponent<OpacityEffect>().Stop = false;
    }

    Vector2 target;

    private bool isAnim = false;
    private IEnumerator showKeyboard()
    {
        isAnim = true;
        if (keyboard == null)
            yield break;

        target = new Vector2(firstPos.x, firstPos.y - (keyboard.GetComponent<RectTransform>().rect.height - 470));
        float scaleDuration = 2f;
        for (float i = 0; i < 1; i += Time.deltaTime / scaleDuration)
        {
            keyboard.transform.localPosition = Vector2.Lerp(keyboard.transform.localPosition, target, i);
            if (keyboard.transform.localPosition.Equals(target))
            {
                isAnim = false;
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator hideKeyboard()
    {
        isAnim = true;
        if (keyboard == null)
            yield break;

        float scaleDuration = 2f;
        for (float i = 0; i < 1; i += Time.deltaTime / scaleDuration)
        {
            keyboard.transform.localPosition = Vector2.Lerp(keyboard.transform.localPosition, firstPos, i);
            if (keyboard.transform.localPosition.Equals(firstPos))
            {
                isAnim = false;
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hitTouch = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero, 0);

            SetRayCast(hitTouch);
        }

        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            RaycastHit2D hitTouch = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(touch.position).x, Camera.main.ScreenToWorldPoint(touch.position).y), Vector2.zero, 0);

            SetRayCast(hitTouch);
        }
    }
}
