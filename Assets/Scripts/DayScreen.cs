using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayScreen : MonoBehaviour
{
    void Awake() {
        EventManager.DayEvent_Beginning += FadeIn;
    }

    
    void FadeIn(Society society, Company company)
    {
        GetComponentInChildren<Text>().text = "Day " + GameManager.Instance.society.day;
        GetComponent<Animator>().SetTrigger("Active");
    }
}
