using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpacityEffect : MonoBehaviour
{
    public float From = 20f;
    public float To = 80f;

    private float currentAlpha = 0f;

    private SpriteRenderer renderer = null;
    private Image imageRender = null;

    private bool image = false;

    private float r, g, b;
    private void Start()
    {
        From /= 100f;
        To /= 100f;

        if (this.TryGetComponent<Image>(out imageRender))
            image = true;

        if (!image) { 
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
    private void Update()
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
            currentAlpha += .01f;
            if (!image)
                renderer.color = new Color(r, g, b, currentAlpha);
            else
                imageRender.color = new Color(r, g, b, currentAlpha);
        }
        else
        {
            currentAlpha -= .01f;
            if (!image)
                renderer.color = new Color(r, g, b, currentAlpha);
            else
                imageRender.color = new Color(r, g, b, currentAlpha);
        }
    }
}
