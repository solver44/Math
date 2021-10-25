using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TouchObject : MonoBehaviour
{
    [SerializeField] private bool hasSound = false;
    [SerializeField] private bool hasName = false;
    [SerializeField] private string tagNameOfText = "ObjectsName";
    [SerializeField] private bool scaleWithoutAnim = false;
    [SerializeField] private float scaleR = .1f;
    [SerializeField] private bool hide = false;
    [SerializeField] private bool dontChangeEnd = false;
    [Space]
    public int CountOfObjects = 0;
    public WasUnitComplete Parent = null;

    private Animator animator = null;
    private AudioSource sound = null;
    private Text text = null;

    Vector3 scaleS;
    float xScale = 0;
    float yScale = 0;

    private Touch touch;
    private bool calculated = false;

    private static int currentCount = 0;
    private void Start()
    {
        scaleS = transform.localScale;
        xScale = scaleS.x;
        yScale = scaleS.y;

        this.TryGetComponent<SpriteRenderer>(out renderer);

        this.TryGetComponent<AudioSource>(out sound);
        this.TryGetComponent<Animator>(out animator);

        if (hasName && text == null)
            text = GameObject.FindGameObjectWithTag(tagNameOfText).gameObject.GetComponent<Text>() as Text;

        if (hide)
            StartCoroutine(SetSpriteNull());

    }
    private IEnumerator SetSpriteNull()
    {
        yield return new WaitForSeconds(2f);
        GetComponent<SpriteRenderer>().sprite = null;
    }

    SpriteRenderer renderer = null;
    void OnMouseDown()
    {
        //if(hitTouch.collider.CompareTag("Difference"))
        //{
        //    obj = hitTouch.collider.transform.gameObject as GameObject;

        //    if (obj.GetComponent<SpriteRenderer>().color.a == 0)
        //        Parent.GetComponent<WasUnitComplete>().SetCountOfDifference +=  1;
        //    obj.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 255);
        //}


        if (hasSound && !sound.isPlaying)
        {
            sound.Play();

            if (!scaleWithoutAnim && animator != null)
                animator.SetBool("zoom", true);
            else if (scaleWithoutAnim)
            {
                StartCoroutine(Scale(new Vector3(xScale + (xScale * scaleR), yScale + (yScale * scaleR), 1), 3f));
                StartCoroutine(hideText());
            }
        }
        else if (!hasSound)
        {
            if (!scaleWithoutAnim && animator != null)
                animator.SetBool("zoom", true);
            else if (scaleWithoutAnim)
            {
                StartCoroutine(Scale(new Vector3(xScale + (xScale * scaleR), yScale + (yScale * scaleR), 1), 3f));
                StartCoroutine(hideText());
            }
        }
        if (hasName)
            text.text = this.tag;
        if (renderer != null)
            renderer.sortingLayerName = "Selected";
    }
    bool wait = false;
    private IEnumerator hideText()
    {
        if (!calculated)
        {
            calculated = true;
            currentCount++;
            if (currentCount >= CountOfObjects)
            {
                Parent.CompleteUnit();
            }
        }
        if (dontChangeEnd)
            yield break;
        yield return new WaitForSeconds(2);
        if (sound != null)
            sound.Stop();

        unscale();
    }
    private void unscale()
    {
        StartCoroutine(Scale(scaleS, 3f));

        if (renderer != null)
            renderer.sortingLayerName = "Top";

        if (hasName && (text != null && text.text == this.tag))
            text.text = null;
    }


    private IEnumerator Scale(Vector3 target, float duration)
    {
        float scaleDuration = duration;
        for (float t = 0; t < 1; t += Time.deltaTime / scaleDuration)
        {
            this.transform.localScale = Vector3.Lerp(this.transform.localScale, target, t);
            if (this.transform.localScale.Equals(target))
            {
                yield break;
            }
            yield return null;
        }
    }
}
