using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeskButton : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject OfficeWindow; //오피스 화면
    public GameObject DeskWindow; //책상 화면
    
    private float timer1;
    private float timer2;

    // Use this for initialization
    void Start()
    {
        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry_PointerDown = new EventTrigger.Entry();
        EventTrigger.Entry entry_PointerUp = new EventTrigger.Entry();
        entry_PointerDown.eventID = EventTriggerType.PointerDown;
        entry_PointerUp.eventID = EventTriggerType.PointerUp;
        entry_PointerDown.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
        entry_PointerUp.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_PointerDown);
        eventTrigger.triggers.Add(entry_PointerUp);
        timer1 = 0;
        timer2 = 0;
    }

    void OnPointerDown(PointerEventData data)
    {
            if (!GameManager.instance.in_menu) //메뉴가 안 켜져있을 때만 눌림
            {
                timer1 = Time.realtimeSinceStartup;
            }
    }

    void OnPointerUp(PointerEventData data)
    {
        if (!GameManager.instance.in_menu) //메뉴가 안 켜져있을 때만 눌림
        {
            timer2 = Time.realtimeSinceStartup;
            if (timer2 - timer1 < 0.14f)
            {
                DeskWindow.SetActive(true);
                OfficeWindow.SetActive(false);
            }
        }
    }
}
