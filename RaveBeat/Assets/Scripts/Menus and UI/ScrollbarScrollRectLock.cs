using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ScrollbarScrollRectLock : MonoBehaviour
{
    private Scrollbar scrollbar;
    private float topYLock;
    private float botYLock;
    private bool once;

    // Start is called before the first frame update
    void Start()
    {
        once = false;
        scrollbar = GetComponent<Scrollbar>();  
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = GameObject.Find("EventSystem").GetComponent<EventSystem>().currentSelectedGameObject.transform.position;

        if (!once)
        {

            botYLock = MainMenu.chartLastOnScreen.transform.position.y - 10f;
            topYLock = pos.y;
            once = true;
        }
        else
        {

            if (pos.y > topYLock)
            {
                scrollbar.value += 0.007f;
            }
            else if (pos.y < botYLock)
            {
                scrollbar.value -= 0.007f;
            }
        }

    }

    void LockHeights()
    {

    }

}
