using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PresentationHandler : MonoBehaviour {

    [SerializeField] public RawImage RefUIBlack;
    [SerializeField] public GameObject AlienSlide;
    [SerializeField] public GameObject LastSlide;
    [SerializeField] public GameObject LastLastSlide;

    IEnumerator PlayTransition()
    {
        yield return new WaitForSeconds(4.0f);

        float time = 0.0f;

        while (RefUIBlack.color.a != 0)
        {
            time += Time.deltaTime;
            Color temp = RefUIBlack.color;
            temp.a = Mathf.Lerp(1, 0, time);
            RefUIBlack.color = temp;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator PlayDoubleTranstionFiFo(GameObject toBeActivate)
    {
        float time = 0.0f;

        while (RefUIBlack.color.a != 1)
        {
            time += Time.deltaTime;
            Color temp = RefUIBlack.color;
            temp.a = Mathf.Lerp(0, 1, time);
            RefUIBlack.color = temp;
            yield return new WaitForEndOfFrame();
        }

        toBeActivate.SetActive(true);
        yield return new WaitForSeconds(1.0f);

        time = 0.0f;

        while (RefUIBlack.color.a != 0)
        {
            time += Time.deltaTime;
            Color temp = RefUIBlack.color;
            temp.a = Mathf.Lerp(1, 0, time);
            RefUIBlack.color = temp;
            yield return new WaitForEndOfFrame();
        }
    }

    // Use this for initialization
    private void Awake()
    {
        StartCoroutine(PlayTransition());
    }
  

	
	// Update is called once per frame
	void Update ()
    {
		if(Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            if(!AlienSlide.activeSelf)
            {
                StartCoroutine(PlayDoubleTranstionFiFo(AlienSlide));
            }
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter) && AlienSlide.activeSelf)
        {
            if (!LastSlide.activeSelf)
            {
                StartCoroutine(PlayDoubleTranstionFiFo(LastSlide));
            }
        }

        if(Input.GetKeyDown(KeyCode.KeypadPeriod) && LastSlide.activeSelf)
        {
            if(!LastLastSlide.activeSelf)
            {
                StartCoroutine(PlayDoubleTranstionFiFo(LastLastSlide)); 
            }
        }
    }
}
