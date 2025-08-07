using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Easing.Tweening;

public class End : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private RectTransform trans;
    [SerializeField] private Text endText;
    [SerializeField] private List<Image> backGround_Image;

    [SerializeField] private float endPos;
    [SerializeField] private float scrollSpeed;
    [SerializeField] private float speedUpval;

    [SerializeField] private bool is_BackGround_Image_FadeOn;
    [SerializeField] private bool isScrolling;
    [SerializeField] private bool scrollingEnd;
    [SerializeField] private bool speedUp;
    [SerializeField] private AudioSource audioSource;
    private void Start()
    {
        isScrolling = true;
        scrollingEnd = false;

        // Audio Play
        audioSource.Play();

        StartCoroutine(Sc());
    }

    private void Update()
    {
        if (isScrolling && Input.anyKey && !speedUp)
        {
            speedUp = true;
            scrollSpeed *= speedUpval;
        }
        else
        {
            speedUp = false;
            scrollSpeed = 100;
        }

        if (trans.anchoredPosition.y > endPos && isScrolling)
        {
            StartCoroutine(nameof(EndText));
        }

        if (!isScrolling && scrollingEnd && Input.GetKeyDown(KeyCode.Space))
        {
            audioSource.Stop();
            Scene_Loading.LoadScene("Start_Scene");
        }
    }

    private IEnumerator Sc()
    {
        // Delay
        yield return new WaitForSeconds(3f);

        // Image On
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        image.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);

        // Image Off
        float a = 1;
        while(a > 0)
        {
            a -= 1f * Time.deltaTime;
            image.color = new Color(image.color.r, image.color.g, image.color.b, EasingFunctions.InOutElastic(a));
            yield return null;
        }
        image.color = new Color(image.color.r, image.color.g, image.color.b, a);
        image.gameObject.SetActive(false);

        // Delay
        yield return new WaitForSeconds(1.5f);

        // StartCoroutine(BackGround_Image());
        // Move
        while(isScrolling)
        {
            trans.anchoredPosition = new Vector2(0, trans.anchoredPosition.y + scrollSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator EndText()
    {
        float a = 0;
        while (a < 1)
        {
            a += Time.deltaTime;
            endText.color = new Color(endText.color.r, endText.color.g, endText.color.b, a);
            yield return null;
        }

        endText.color = new Color(endText.color.r, endText.color.g, endText.color.b, 1);

        scrollingEnd = true;
        isScrolling = false;
    }

    /*
    private IEnumerator BackGround_Image()
    {
        float timer = 0;

        for (int i = 0; i < backGround_Image.Count; i++)
        {
            is_BackGround_Image_FadeOn = true;
            while (timer < 1 && is_BackGround_Image_FadeOn)
            {
                timer += Time.deltaTime;
                backGround_Image[i].color = new Color(backGround_Image[i].color.r, backGround_Image[i].color.g, backGround_Image[i].color.b, timer);
                yield return null;
            }
            is_BackGround_Image_FadeOn = false;
            yield return new WaitForSeconds(10f);

            timer = 1;
            is_BackGround_Image_FadeOn = true;
            while (timer > 0 && is_BackGround_Image_FadeOn)
            {
                timer -= Time.deltaTime;
                backGround_Image[i].color = new Color(backGround_Image[i].color.r, backGround_Image[i].color.g, backGround_Image[i].color.b, timer);
                yield return null;
            }
            is_BackGround_Image_FadeOn = false;
            yield return new WaitForSeconds(1f);
        }
    }
    */
}
