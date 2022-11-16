using System.Collections;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public string menuName;
    [HideInInspector]
    public bool opened;
    private CanvasGroup canvaGroup;
    private void OnEnable()
    {
        canvaGroup = GetComponent<CanvasGroup>();
    }
    // Opens this specific menu throught our MenuMager
    public void Open()
    {
        opened = true;
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(Animation(1));
    }
    // closes this specific meni throught our MenuManager
    public void Close()
    {
        gameObject.SetActive(true);
        opened = false;
        StopAllCoroutines();
        StartCoroutine(Animation(0));
    }
    IEnumerator Animation(int alpha)
    {
        bool anim = true;
        while (anim)
        {
            if (alpha == 0)
            {
                canvaGroup.alpha = Math.dampFloat(canvaGroup.alpha, -0.1f, 10f, Time.deltaTime);
                if (canvaGroup.alpha <= alpha)
                {
                    canvaGroup.interactable = false;
                    gameObject.SetActive(false);
                    anim = false;
                }
            }else if(alpha == 1)
            {
                canvaGroup.alpha = Math.dampFloat(canvaGroup.alpha,1.1f, 10f, Time.deltaTime);
                if (canvaGroup.alpha >=alpha)
                {
                    canvaGroup.interactable = true;
                    anim = false;
                }
            }
            
            yield return null;
        }


    }
}
