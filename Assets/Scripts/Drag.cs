using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drag : MonoBehaviour {

    int eachDate; //기사가 가지는 날짜

    bool inDesk = true; //책상 안에 존재하는가?
    float distance = 10;
    float timer1; //눌렀을 때 시간
    float timer2; //뗐을 때 시간

    public int eachPoint = 0; //기사가 가지는 포인트
    public int eachVirality; //기사가 가지는 파급력
    public string eachField; //기사가 가지는 관심사
    public int eachIndex; //기사가 가지는 인덱스
    public float eachVertification; //기사가 가지는 검증도
    public float eachminRange; //최소 수용 영역
    public float eachmaxRange; //최대 수용 영역
    public bool eachUp_virality; //기사가 가지는 심화 취재를 통한 파급력 증가 여부
    public bool advanced = false; //기사에 대해 심화 취재를 지시했는가?

    SpriteRenderer sr;

    Vector3 originPosition;
    Vector3 objPosition;

    private void Start()
    {
        GameManager.instance.AddPaperToList(this);
        Article new_Article = GameManager.instance.myCompany.articles[GameManager.instance.papers.Count - 1];
        eachField = new_Article.article_field;
        eachIndex = new_Article.write_reporter_index;
        eachVirality = new_Article.virality;
        eachDate = new_Article.date;
        eachminRange = new_Article.minRange;
        eachmaxRange = new_Article.maxRange;
        eachVertification = new_Article.vertification;
        eachUp_virality = new_Article.up_virality; //article로부터 기사의 정보 받아옴
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
                if (!advanced) //심화 취재를 지시하지 않은 기사라면
                {
                    GameManager.instance.in_menu = true;
                    GameObject lookpaper = GameObject.Find("LookPaper");
                    lookpaper.transform.GetChild(0).gameObject.SetActive(true); //확대창 켜기
                    lookpaper.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(true);
                    lookpaper.transform.GetChild(0).transform.GetChild(3).gameObject.SetActive(false);
                    lookpaper.transform.GetChild(0).transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = sr.sprite;
                    lookpaper.transform.GetChild(0).transform.GetChild(2).transform.GetChild(3).GetComponent<OK_Button>().paperObject = this;
                    lookpaper.transform.GetChild(0).transform.GetChild(2).transform.GetChild(4).GetComponent<Advance_Button>().paperObject = this; //클릭한 기사 정보 보내기
                    lookpaper.transform.GetChild(0).transform.GetChild(2).transform.GetChild(0).transform.GetChild(1).GetComponent<Text>().text = GameManager.instance.point.ToString(); //남아있는 포인트 텍스트 수정
                    lookpaper.transform.GetChild(0).transform.GetChild(2).transform.GetChild(1).transform.GetChild(1).GetComponent<Text>().text = eachPoint.ToString(); //배정된 포인트 텍스트 수정

                    SimTest();
                }
                else //심화 취재를 지시한 기사라면
                {
                    GameManager.instance.in_menu = true;
                    GameObject lookpaper = GameObject.Find("LookPaper");
                    lookpaper.transform.GetChild(0).gameObject.SetActive(true); //확대창 켜기
                    lookpaper.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
                    lookpaper.transform.GetChild(0).transform.GetChild(3).gameObject.SetActive(true);
                    lookpaper.transform.GetChild(0).transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = sr.sprite;
                }
            }
        }
    }

    void SimTest()
    {
        GameObject lookpaper = GameObject.Find("LookPaper");
        lookpaper.transform.GetChild(0).GetChild(2).GetChild(6).GetChild(1).GetComponent<Text>().text = eachField;
        lookpaper.transform.GetChild(0).GetChild(2).GetChild(6).GetChild(3).GetComponent<Text>().text = eachVirality.ToString();
        lookpaper.transform.GetChild(0).GetChild(2).GetChild(6).GetChild(5).GetComponent<Text>().text = eachVertification.ToString();
        lookpaper.transform.GetChild(0).GetChild(2).GetChild(6).GetChild(7).GetComponent<Text>().text = eachminRange.ToString();
        lookpaper.transform.GetChild(0).GetChild(2).GetChild(6).GetChild(9).GetComponent<Text>().text = eachmaxRange.ToString();
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
