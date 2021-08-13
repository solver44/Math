using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpacityEffect : MonoBehaviour
{

    public float From = 20f;
    public float To = 80f;

    [Space]
    public float Value = -1;
    public int Unit;

    private float currentAlpha = 0f;

    private SpriteRenderer renderer = null;
    private Image imageRender = null;

    private bool image = false;

    private float r, g, b;

    public static IDictionary<string, string> AllKeybordValues = new Dictionary<string, string>();
    private void Start()
    {
        Stop = false;
        From /= 100f;
        To /= 100f;

        if (Value != -1)
            AllKeybordValues.Add(this.transform.parent.name, Value.ToString() + ";" + Unit.ToString());

        if (this.TryGetComponent<Image>(out imageRender))
            image = true;

        if (!image) {
            renderer = this.GetComponent<SpriteRenderer>();
            currentAlpha = this.GetComponent<SpriteRenderer>().color.a;
            r = renderer.color.r; g = renderer.color.g; b = renderer.color.b;
            renderer.color = new Color(r, g, b, From);
        }
        else
        {
            currentAlpha = this.GetComponent<Image>().color.a;
            r = imageRender.color.r; g = imageRender.color.g; b = imageRender.color.b;
            imageRender.color = new Color(r, g, b, From);
        }
        currentAlpha = From;
    }

    private bool up = false;
    private float timer;
    private bool _stop = false;
    public bool Stop { set { _stop = value; if (!value) { imageRender = GetComponent<Image>(); r = imageRender.color.r; g = imageRender.color.g; b = imageRender.color.b; } } get { return _stop; } }
    private void Update()
    {
        if (Stop)
            return;

        if (Time.unscaledTime > timer + .5f)
        {
            if (!image)
                currentAlpha = renderer.color.a;
            else
                currentAlpha = imageRender.color.a;

            if (currentAlpha <= From)
                up = true;
            else if (currentAlpha >= To)
                up = false;

            if (up)
            {
                currentAlpha += .003f;
                if (!image)
                    renderer.color = new Color(r, g, b, currentAlpha);
                else
                    imageRender.color = new Color(r, g, b, currentAlpha);
            }
            else
            {
                currentAlpha -= .003f;
                if (!image)
                    renderer.color = new Color(r, g, b, currentAlpha);
                else
                    imageRender.color = new Color(r, g, b, currentAlpha);
            }
        }
    }
}
