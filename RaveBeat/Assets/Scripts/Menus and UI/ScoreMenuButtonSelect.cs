using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class ScoreMenuButtonSelect : MonoBehaviour
{
    public GameObject scoreSelected;

    void Start()
    {
        GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(null);
        GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(scoreSelected);
    }
}
