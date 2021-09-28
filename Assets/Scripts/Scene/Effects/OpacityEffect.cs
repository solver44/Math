using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OpacityEffect : MonoBehaviour
{

    public float From = 20f;
    public float To = 80f;
    public bool IsText = false;

    [Space]
    [Header("Other")]
    public string Value = "null";
    public int Unit;
    [Tooltip("Numbers, Symbols, Alphabetics")]
    public string KeyboardType = "Numbers";
    public Color ColorValue = new Color(1, 1, 1, 1);

    private float currentAlpha = 0f;

    private SpriteRenderer renderer = null;
    private Image imageRender = null;
    private Text textRenderer = null;

    private bool image = false;

    private float r, g, b;

    public static IDictionary<string, string> AllKeybordValues = new Dictionary<string, string>();

    private void Awake()
    {
        if (Value != "null")
        {
            AllKeybordValues.Add(this.transform.parent.name, Value.ToString() + ";" + Unit.ToString() + ";" + KeyboardType.ToString() + ";" + ColorValue.r + "/" + ColorValue.g + "/" + ColorValue.b);
        }
    }
    private void Start()
    {
        Stop = false;
        From /= 100f;
        To /= 100f;

        if (!IsText && this.TryGetComponent<Image>(out imageRender))
            image = true;

        if (!image && !IsText) {
            renderer = this.GetComponent<SpriteRenderer>();
            currentAlpha = this.GetComponent<SpriteRenderer>().color.a;
            r = renderer.color.r; g = renderer.color.g; b = renderer.color.b;
            renderer.color = new Color(r, g, b, From);
        }
        else if(!IsText)
        {
            currentAlpha = this.GetComponent<Image>().color.a;
            r = imageRender.color.r; g = imageRender.color.g; b = imageRender.color.b;
            imageRender.color = new Color(r, g, b, From);
        }
        else if(IsText)
        {
            textRenderer = this.GetComponent<Text>();
            currentAlpha = this.GetComponent<Text>().color.a;
            r = textRenderer.color.r; g = textRenderer.color.g; b = textRenderer.color.b;
            textRenderer.color = new Color(r, g, b, From);
        }
        currentAlpha = From;

    }

    private bool up = false;
    private float timer;
    private bool _stop = false;
    public bool Stop { set 
    { _stop = value; 
    if (!value && !IsText) 
    { 
        imageRender = GetComponent<Image>(); r = imageRender.color.r; g = imageRender.color.g; b = imageRender.color.b; }
    } 
    get { return _stop; } 
    }
    private void Update()
    {
        if (Stop)
            return;

        if (Time.unscaledTime > timer + .5f)
        {
            if (!image && !IsText)
                currentAlpha = renderer.color.a;
            else if (!IsText)
                currentAlpha = imageRender.color.a;
            else
                currentAlpha = textRenderer.color.a;

            if (currentAlpha <= From)
                up = true;
            else if (currentAlpha >= To)
                up = false;

            if (up)
            {
                currentAlpha += .005f;
                if (!image && !IsText)
                    renderer.color = new Color(r, g, b, currentAlpha);
                else if (!IsText)
                    imageRender.color = new Color(r, g, b, currentAlpha);
                else
                    textRenderer.color = new Color(r, g, b, currentAlpha);
            }
            else
            {
                currentAlpha -= .005f;
                if (!image && !IsText)
                    renderer.color = new Color(r, g, b, currentAlpha);
                else if (!IsText)
                    imageRender.color = new Color(r, g, b, currentAlpha);
                else
                    textRenderer.color = new Color(r, g, b, currentAlpha);
            }
        }
    }
}
