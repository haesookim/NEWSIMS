using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Advance_Button : MonoBehaviour
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
            int temp_index = 0;
            for (int i = 0; i < GameManager.instance.myCompany.reporters.Count; i++)
            {
                if (paperObject.eachIndex == GameManager.instance.myCompany.reporters[i].reporter_index)
                {
                    temp_index = i;
                    break;
                }
            }
            if (!GameManager.instance.myCompany.reporters[temp_index].advance_news)
            {
                inputField.text = ""; //인풋필드 값을 초기화해라
                GameManager.instance.point += paperObject.eachPoint; //기사에 배정된 포인트 반납
                paperObject.eachPoint = 0; //포인트 제거
                paperObject.advanced = true;

                //저장용 데이터로 옮기기
                GameManager.instance.myCompany.reporters[temp_index].adn.adv_field = paperObject.eachField;
                GameManager.instance.myCompany.reporters[temp_index].adn.adv_name = paperObject.eachName;
                GameManager.instance.myCompany.reporters[temp_index].adn.adv_virality = paperObject.eachVirality;
                GameManager.instance.myCompany.reporters[temp_index].adn.adv_vertification = paperObject.eachVertification;
                GameManager.instance.myCompany.reporters[temp_index].adn.adv_minRange = paperObject.eachminRange;
                GameManager.instance.myCompany.reporters[temp_index].adn.adv_maxRange = paperObject.eachmaxRange;
                GameManager.instance.myCompany.reporters[temp_index].adn.adv_up_virality = paperObject.eachUp_virality; //
                GameManager.instance.myCompany.reporters[temp_index].advance_news = true;

                GameManager.instance.in_menu = false;
                GameObject lookpaper = GameObject.Find("LookPaper");
                lookpaper.transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text = GameManager.instance.point.ToString(); //남아있는 포인트 텍스트 수정
                lookpaper.transform.GetChild(0).transform.GetChild(2).transform.GetChild(1).transform.GetChild(1).GetComponent<Text>().text = "0"; //배정된 포인트 텍스트 수정
                lsst.Sorting(); //정렬 함수 실행
                lookpaper.transform.GetChild(0).gameObject.SetActive(false); //확대창 끄기
            }
        }
    }
}
