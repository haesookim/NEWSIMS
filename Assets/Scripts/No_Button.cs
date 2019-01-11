using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class No_Button : MonoBehaviour
{
    public InputField inputField; //인풋필드를 저장함

    // Start is called before the first frame update
    void Start()
    {
        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry_PointerDown = new EventTrigger.Entry();
        entry_PointerDown.eventID = EventTriggerType.PointerDown;
        entry_PointerDown.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_PointerDown);
    }

    void OnPointerDown(PointerEventData data)
    {
        if (GameManager.instance.in_menu) //메뉴가 켜져있는 상태라면
        {
            inputField.text = ""; //인풋필드 값을 초기화해라
            GameManager.instance.in_menu = false;
            GameObject lookpaper = GameObject.Find("LookPaper");
            lookpaper.transform.GetChild(0).gameObject.SetActive(false); //확대창 끄기
        }
    }
}
