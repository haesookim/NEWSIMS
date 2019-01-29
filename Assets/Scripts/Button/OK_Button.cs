using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OK_Button : MonoBehaviour
{
    public InputField inputField; //인풋필드를 저장함
    public Drag paperObject; //선택한 기사를 저장함
    public ListSort lsst;

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
            if (inputField.text != "") //인풋필드가 빈 상태가 아니라면
            {
                if (int.Parse(inputField.text) <= GameManager.instance.point && (paperObject.eachPoint + int.Parse(inputField.text)) >= 0) //현재 남아있는 포인트보다 인풋필드 값이 작다면
                {
                    GameManager.instance.point -= int.Parse(inputField.text); //포인트를 그만큼 감소시켜라
                    paperObject.eachPoint += int.Parse(inputField.text); //기사의 포인트를 그만큼 증가시켜라

                    inputField.text = ""; //인풋필드 값을 초기화해라
                    GameManager.instance.in_menu = false;
                    GameObject lookpaper = GameObject.Find("LookPaper");
                    lookpaper.transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text = GameManager.instance.point.ToString(); //남아있는 포인트 텍스트 수정
                    lookpaper.transform.GetChild(0).transform.GetChild(2).transform.GetChild(1).transform.GetChild(1).GetComponent<Text>().text = "0"; //배정된 포인트 텍스트 수정
                    lsst.Sorting(); //정렬
                    
                    lookpaper.transform.GetChild(0).gameObject.SetActive(false); //확대창 끄기
                }
            }
        }
    }
}
