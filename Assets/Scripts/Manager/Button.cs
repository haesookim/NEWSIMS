using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Button : MonoBehaviour
{
    public enum function
    {
        DisplayDesk,
        DisplayOffice,
        DisappearPaperMenu,
        DividePoint,
        SetDeepenArticle,
        EndofDay,
        DisplayReporter,
        DisappearReporter,
        PageNumber,
        DetailReporter,
        BackButton,
        MyButton,
        EmployButton,
        FireButton
    }

    [Header("클릭 시 실행될 함수")]
    public function functionName;

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
        SelectFunction(); 
    }

    void SelectFunction()
    {
        switch(functionName)
        {
            case function.DisplayDesk :
                DisplayDesk();
                break;
            
            case function.DisplayOffice :
                DisplayOffice();
                break;

            case function.DisappearPaperMenu :
                DisappearPaperMenu();
                break;

            case function.DividePoint :
                DividePoint();
                break;

            case function.SetDeepenArticle :
                SetDeepenArticle();
                break;

            case function.EndofDay :
                EndofDay();
                break;
            case function.DisplayReporter :
                ReporterWindow();
                break;
            case function.DisappearReporter :
                DisappearReporterWindow();
                break;
            case function.PageNumber :
                PageChange();
                break;
            case function.DetailReporter :
                ReporterDetail();
                break;
            case function.BackButton :
                BackButton();
                break;
            case function.MyButton :
                MyButton();
                break;
            case function.EmployButton :
                EmployButton();
                break;
            case function.FireButton :
                FireButton();
                break;
        }
    }

    public void DisplayDesk() //책상 화면으로
   {
       if(!GameManager.Instance.in_DeskMenu)
       {
           AudioManager.Instance.StartSFX("DeskClick");

           GameManager.Instance.deskWindow.SetActive(true);
           GameManager.Instance.officeWindow.SetActive(false);
           GameManager.Instance.in_DeskMenu = true;
       }
   }

   public void DisplayOffice() // 메인 화면으로
   {
       if(GameManager.Instance.in_DeskMenu)
       {
           GameManager.Instance.officeWindow.SetActive(true);
           GameManager.Instance.deskWindow.SetActive(false);
           GameManager.Instance.in_DeskMenu = false;
       }
   }

   public void DisappearPaperMenu() //상세 기사 화면 없애기
   {
        GameManager.Instance.in_PaperMenu = false;
        GameManager.Instance.paperWindow.SetActive(false);
        GameManager.Instance.paperWindow.transform.GetChild(1).gameObject.SetActive(false);
        GameManager.Instance.paperWindow.transform.GetChild(2).gameObject.SetActive(false);
   }

    public void DividePoint() //확인 버튼 클릭시 포인트 분배 처리
    {
        GameManager.Instance.selectedPaper.article.assignedPoint = GameManager.Instance.temp_point;
        GameManager.Instance.point -= GameManager.Instance.temp_point;
        GameManager.Instance.temp_point = 0;
        DisappearPaperMenu();
    }
    
    public void SetDeepenArticle() //심화취재 버튼 클릭시
    {
        GameManager.Instance.selectedPaper.reporter.advance_news = true;
        GameManager.Instance.selectedPaper.article.advance = true;
        GameManager.Instance.selectedPaper.reporter.adn = GameManager.Instance.selectedPaper.article;
        GameManager.Instance.temp_point = 0;
        DisappearPaperMenu();
    }

    public void EndofDay()
    {
        GameManager.Instance.EndofDay();
    }

    public void ReporterWindow() //인사관리 명부 클릭시
    {
        if (GameManager.Instance.in_DeskMenu && !GameManager.Instance.in_ReporterMenu)
        {
            GameManager.Instance.reporterManager.SetActive(true);
            GameManager.Instance.in_ReporterMenu = true;
        }
    }

    public void DisappearReporterWindow() //인사관리창 X버튼 클릭시
    {
        if (GameManager.Instance.in_DeskMenu && GameManager.Instance.in_ReporterMenu)
        {
            MyButton();
            GameManager.Instance.reporterManager.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
            GameManager.Instance.Page.SetActive(true);
            for (int i = 0; i < GameManager.Instance.detail.transform.GetChild(13).childCount; i++)
            {
                GameManager.Instance.detail.transform.GetChild(13).GetChild(i).GetComponent<Text>().text = "";
            }
            GameManager.Instance.detail.gameObject.SetActive(false);
            GameManager.Instance.HideOtherReporter(0);
            GameManager.Instance.reporterManager.SetActive(false);
            GameManager.Instance.in_ReporterMenu = false;
        }
    }

    public void PageChange()
    {
        int num = transform.GetComponent<PageButton>().page_num;
        GameManager.Instance.HideOtherReporter(num-1);
    }

    public void ReporterDetail()
    {
        GameManager.Instance.reporterManager.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
        GameManager.Instance.Page.SetActive(false);
        GameManager.Instance.detail.gameObject.SetActive(true);

        GameManager.Instance.reporterManager.GetComponent<ReporterManager>().myReporter = transform.GetComponent<VisualizeReporter>().reporter;
        Reporter rp = GameManager.Instance.reporterManager.GetComponent<ReporterManager>().myReporter;

        if (!rp.is_fired)
        {
            GameManager.Instance.detail.transform.GetChild(14).GetComponent<Image>().color = new Color(255f,255f,255f);
        }
        else
        {
            GameManager.Instance.detail.transform.GetChild(14).GetComponent<Image>().color = new Color(255f, 255f, 0f);
        }

        //Detail에 정보 넣기
        GameManager.Instance.detail.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = rp.reporterImage;
        GameManager.Instance.detail.transform.GetChild(2).GetComponent<Text>().text = "이름 : " + rp.name;
        GameManager.Instance.detail.transform.GetChild(3).GetComponent<Text>().text = "주 분야 : " + transform.GetComponent<VisualizeReporter>().mf;
        GameManager.Instance.detail.transform.GetChild(4).GetComponent<Text>().text = "레벨 : " + rp.level.ToString();
        GameManager.Instance.detail.transform.GetChild(5).GetComponent<Text>().text = "경험치 : " + rp.exp.ToString();
        GameManager.Instance.detail.transform.GetChild(6).GetComponent<Text>().text = "필력 : " + rp.writing.ToString();
        GameManager.Instance.detail.transform.GetChild(7).GetComponent<Text>().text = "논리력 : " + rp.logic.ToString();
        GameManager.Instance.detail.transform.GetChild(8).GetComponent<Text>().text = "조사력 : " + rp.survey.ToString();
        GameManager.Instance.detail.transform.GetChild(9).GetComponent<Text>().text = "경제적 입장 : " + rp.econStance.ToString();
        GameManager.Instance.detail.transform.GetChild(10).GetComponent<Text>().text =  "사회적 입장 : " + rp.socialStance.ToString();
        GameManager.Instance.detail.transform.GetChild(11).GetComponent<Text>().text = "만족도 : " + rp.satisfaction.ToString();
        for (int i = 0; i < rp.perks.Count; i++)
        {
            GameManager.Instance.detail.transform.GetChild(13).GetChild(i).GetComponent<Text>().text = rp.perks[i];
        }
        //
    }

    public void BackButton()
    {
        GameManager.Instance.reporterManager.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        GameManager.Instance.Page.SetActive(true);
        for(int i = 0; i < GameManager.Instance.detail.transform.GetChild(13).childCount; i++){
            GameManager.Instance.detail.transform.GetChild(13).GetChild(i).GetComponent<Text>().text = "";
        }
        GameManager.Instance.detail.gameObject.SetActive(false);
    }

    public void MyButton()
    {
        GameManager.Instance.reporterManager.transform.GetChild(1).gameObject.SetActive(true);
        GameManager.Instance.reporterManager.transform.GetChild(2).gameObject.SetActive(false);
    }

    public void EmployButton()
    {
        GameManager.Instance.reporterManager.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        GameManager.Instance.Page.SetActive(true);
        for (int i = 0; i < GameManager.Instance.detail.transform.GetChild(13).childCount; i++)
        {
            GameManager.Instance.detail.transform.GetChild(13).GetChild(i).GetComponent<Text>().text = "";
        }
        GameManager.Instance.detail.gameObject.SetActive(false);
        GameManager.Instance.HideOtherReporter(0);

        GameManager.Instance.reporterManager.transform.GetChild(1).gameObject.SetActive(false);
        GameManager.Instance.reporterManager.transform.GetChild(2).gameObject.SetActive(true);
    }

    public void FireButton()
    {
        if (!ReporterManager.Instance.myReporter.is_fired)
        {
            for (int i = 0; i < GameManager.Instance.company.reporters.Count; i++)
            {
                if (ReporterManager.Instance.myReporter.reporter_index == GameManager.Instance.company.reporters[i].reporter_index)
                {
                    GameManager.Instance.company.reporters[i].is_fired = true;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < GameManager.Instance.company.reporters.Count; i++)
            {
                if (ReporterManager.Instance.myReporter.reporter_index == GameManager.Instance.company.reporters[i].reporter_index)
                {
                    GameManager.Instance.company.reporters[i].is_fired = false;
                    break;
                }
            }
        }

        for (int i = 0; i < ReporterManager.Instance.vrs.Count; i++)
        {
            ReporterManager.Instance.vrs[i].UpdateStatus();
            ReporterManager.Instance.vrs[i].DisplayFireIcon();
        }

        BackButton();
    }
}
