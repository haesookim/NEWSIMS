using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drag : MonoBehaviour {

    string eachField; //기사가 가지는 관심사
    int eachVirality; //기사가 가지는 파급력
    int eachDate; //기사가 가지는 날짜

    bool inDesk = true; //책상 안에 존재하는가?
    float distance = 10;
    float timer1; //눌렀을 때 시간
    float timer2; //뗐을 때 시간

    public int eachPoint = 0; //기사가 가지는 포인트

    SpriteRenderer sr;

    Vector3 originPosition;
    Vector3 objPosition;

    private void Start()
    {
        GameManager.instance.AddPaperToList(this);
        eachField = GameManager.instance.myCompany.articles[GameManager.instance.papers.Count - 1].article_field;
        eachVirality = GameManager.instance.myCompany.articles[GameManager.instance.papers.Count - 1].virality;
        eachDate = GameManager.instance.myCompany.articles[GameManager.instance.papers.Count - 1].date; //article로부터 기사의 정보 받아옴
        sr = GetComponent<SpriteRenderer>();
        timer1 = 0;
        timer2 = 0;
    }

    private void OnMouseDown() //누를 때 클릭된 프리팹을 앞으로 옮김
    {
        if (!GameManager.instance.in_menu)
        {
            timer1 = Time.realtimeSinceStartup;
            originPosition = transform.position;
            for (int i = 0; i < GameManager.instance.papers.Count; i++)
            {
                if (GameManager.instance.papers[i] != this)
                {
                    if (GameManager.instance.papers[i].transform.position.z < 989f)
                    {
                        GameManager.instance.papers[i].transform.position += new Vector3(0f, 0f, 1f);
                    }
                }
            }
        }
    }

    private void OnMouseDrag() //드래그했을 때 이동
    {
        if (!GameManager.instance.in_menu)
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
            objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            transform.position = objPosition;
        }
    }

    private void OnMouseUp() //마우스를 뗐을 때
    {
        if (!GameManager.instance.in_menu)
        {
            timer2 = Time.realtimeSinceStartup;
            if (inDesk)
            {
                originPosition = transform.position;
            }
            else
            {
                transform.position = originPosition;
            }
            if (timer2 - timer1 < 0.13f) //클릭이라면
            {
                GameManager.instance.in_menu = true;
                GameObject lookpaper = GameObject.Find("LookPaper");
                lookpaper.transform.GetChild(0).gameObject.SetActive(true); //확대창 켜기
                lookpaper.transform.GetChild(0).transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = sr.sprite;
                lookpaper.transform.GetChild(0).transform.GetChild(2).transform.GetChild(3).GetComponent<OK_Button>().paperObject = this; //클릭한 기사 정보 보내기
                lookpaper.transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text = GameManager.instance.point.ToString(); //남아있는 포인트 텍스트 수정
                lookpaper.transform.GetChild(0).transform.GetChild(2).transform.GetChild(1).transform.GetChild(1).GetComponent<Text>().text = eachPoint.ToString(); //배정된 포인트 텍스트 수정
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Desk")
        {
            inDesk = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Desk")
        {
            inDesk = false;
        }
    }
}
