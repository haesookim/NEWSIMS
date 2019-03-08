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
        DisappearReporter2,
        PageNumber,
        DetailReporter,
        DetailReporter2,
        BackButton,
        BackButton2,
        MyButton,
        EmployButton,
        FireButton,
        EmploymentButton
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

            case function.SetDeepenArticle :
                //SetDeepenArticle();
                break;

            case function.EndofDay :
                EndofDay();
                break;
            case function.DisplayReporter :
                ReporterWindow();
                break;
            case function.DisappearReporter :
                DisappearReporterWindow(1);
                break;
            case function.DisappearReporter2 :
                DisappearReporterWindow(2);
                break;
            case function.PageNumber :
                PageChange();
                break;
            case function.DetailReporter :
                ReporterDetail(1);
                break;
            case function.DetailReporter2 :
                ReporterDetail(2);
                break;
            case function.BackButton :
                BackButton(1);
                break;
            case function.BackButton2 :
                BackButton(2);
                break;
            case function.MyButton :
                InsaGwanRi(1,2);
                break;
            case function.EmployButton :
                InsaGwanRi(2,1);
                break;
            case function.FireButton :
                FireButton();
                break;
            case function.EmploymentButton :
                EmploymentButton();
                break;
        }
    }

    public void DisplayDesk() //책상 화면으로
   {
       if(!GameManager.Instance.in_DeskMenu)
       {

           GameManager.Instance.EndingRoot();

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

    
    /*public void SetDeepenArticle() //심화취재 버튼 클릭시
    {
        GameManager.Instance.selectedPaper.reporter.advance_news = true;
        GameManager.Instance.selectedPaper.article.advance = true;
        GameManager.Instance.selectedPaper.reporter.adn = GameManager.Instance.selectedPaper.article;
        GameManager.Instance.temp_point = 0;
        DisappearPaperMenu();
    }*/

    public void EndofDay()
    {
        EventManager.DayEvent_ReporterManage += GameManager.Instance.Process; //해고/고용 과정 이벤트
        GameManager.Instance.EndofDay();
    }

    public void ReporterWindow() //인사관리 명부 클릭시
    {
        if (GameManager.Instance.in_DeskMenu && !GameManager.Instance.in_ReporterMenu)
        {
            GameManager.Instance.reporterManager.SetActive(true);
            GameManager.Instance.in_ReporterMenu = true;

            GameManager.Instance.UpdateReporterButton(GameManager.Instance.company.reporters.Count);
        }
    }

    public void DisappearReporterWindow(int hide) //인사관리창 X버튼 클릭시
    {
        if (GameManager.Instance.in_DeskMenu && GameManager.Instance.in_ReporterMenu)
        {
            InsaGwanRi(1,2);
            GameManager.Instance.reporterManager.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
            GameManager.Instance.Pages[0].SetActive(true);
            for (int i = 0; i < GameManager.Instance.details[hide-1].transform.GetChild(13-(hide-1)).childCount; i++)
            {
                GameManager.Instance.details[hide-1].transform.GetChild(13 - (hide - 1)).GetChild(i).GetComponent<Text>().text = "";
            }
            GameManager.Instance.details[hide - 1].gameObject.SetActive(false);
            GameManager.Instance.HideOtherReporter(1,0);
            GameManager.Instance.reporterManager.SetActive(false);
            GameManager.Instance.in_ReporterMenu = false;
        }
    }

    public void PageChange()
    {
        int num = transform.GetComponent<PageButton>().page_num;
        int child_num = transform.GetComponent<PageButton>().child_num;
        GameManager.Instance.HideOtherReporter(child_num,num-1);
    }

    public void ReporterDetail(int show)
    {
        GameManager.Instance.reporterManager.transform.GetChild(show).GetChild(0).gameObject.SetActive(false);
        GameManager.Instance.Pages[show-1].SetActive(false);
        GameManager.Instance.details[show-1].gameObject.SetActive(true);

        if (show == 1)
        {
            ReporterManager.Instance.myReporter = transform.GetComponent<VisualizeReporter>().reporter;
            Reporter rp = ReporterManager.Instance.myReporter;

            if (!rp.is_fired)
            {
                GameManager.Instance.details[show-1].transform.GetChild(14).GetComponent<Image>().color = new Color(255f, 255f, 255f);
            }
            else
            {
                GameManager.Instance.details[show - 1].transform.GetChild(14).GetComponent<Image>().color = new Color(255f, 255f, 0f);
            }

            //Detail에 정보 넣기
            GameManager.Instance.details[show - 1].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = rp.reporterImage;
            GameManager.Instance.details[show - 1].transform.GetChild(2).GetComponent<Text>().text = "이름 : " + rp.name;
            GameManager.Instance.details[show - 1].transform.GetChild(3).GetComponent<Text>().text = "주 분야 : " + transform.GetComponent<VisualizeReporter>().mf;
            GameManager.Instance.details[show - 1].transform.GetChild(4).GetComponent<Text>().text = "레벨 : " + rp.level.ToString();
            GameManager.Instance.details[show - 1].transform.GetChild(5).GetComponent<Text>().text = "경험치 : " + rp.exp.ToString();
            GameManager.Instance.details[show - 1].transform.GetChild(6).GetComponent<Text>().text = "필력 : " + rp.writing.ToString();
            GameManager.Instance.details[show - 1].transform.GetChild(7).GetComponent<Text>().text = "논리력 : " + rp.logic.ToString();
            GameManager.Instance.details[show - 1].transform.GetChild(8).GetComponent<Text>().text = "조사력 : " + rp.survey.ToString();

            if (rp.econStance < 0.35)
            {
                GameManager.Instance.details[show - 1].transform.GetChild(9).GetComponent<Text>().text = "경제적 입장 : 보수";
            } else if (rp.econStance >= 0.65)
            {
                GameManager.Instance.details[show - 1].transform.GetChild(9).GetComponent<Text>().text = "경제적 입장 : 진보";
            }
            else
            {
                GameManager.Instance.details[show - 1].transform.GetChild(9).GetComponent<Text>().text = "경제적 입장 : 중도";
            }

            if (rp.socialStance < 0.35)
            {
                GameManager.Instance.details[show - 1].transform.GetChild(10).GetComponent<Text>().text = "사회적 입장 : 보수";
            }
            else if (rp.socialStance >= 0.65)
            {
                GameManager.Instance.details[show - 1].transform.GetChild(10).GetComponent<Text>().text = "사회적 입장 : 진보";
            }
            else
            {
                GameManager.Instance.details[show - 1].transform.GetChild(10).GetComponent<Text>().text = "사회적 입장 : 중도";
            }

            GameManager.Instance.details[show - 1].transform.GetChild(11).GetComponent<Text>().text = "만족도 : " + rp.satisfaction.ToString();
            for (int i = 0; i < rp.perks.Count; i++)
            {
                GameManager.Instance.details[show - 1].transform.GetChild(13).GetChild(i).GetComponent<Text>().text = rp.perks[i];
            }
            //
        }
        else if (show == 2)
        {
            ReporterManager.Instance.employReporter = transform.GetComponent<VisualizeEmReporter>().emreporter;
            EmReporter erp = ReporterManager.Instance.employReporter;

            if (!erp.is_employed)
            {
                GameManager.Instance.details[show - 1].transform.GetChild(13).GetComponent<Image>().color = new Color(255f, 255f, 255f);
            }
            else
            {
                GameManager.Instance.details[show - 1].transform.GetChild(13).GetComponent<Image>().color = new Color(255f, 255f, 0f);
            }

            //Detail에 정보 넣기
            GameManager.Instance.details[show - 1].transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = erp.reporterImage;
            GameManager.Instance.details[show - 1].transform.GetChild(2).GetComponent<Text>().text = "이름 : " + erp.name;
            GameManager.Instance.details[show - 1].transform.GetChild(3).GetComponent<Text>().text = "주 분야 : " + transform.GetComponent<VisualizeEmReporter>().mf;
            GameManager.Instance.details[show - 1].transform.GetChild(4).GetComponent<Text>().text = "레벨 : " + erp.level.ToString();
            GameManager.Instance.details[show - 1].transform.GetChild(5).GetComponent<Text>().text = "필력 : " + erp.writing.ToString();
            GameManager.Instance.details[show - 1].transform.GetChild(6).GetComponent<Text>().text = "논리력 : " + erp.logic.ToString();
            GameManager.Instance.details[show - 1].transform.GetChild(7).GetComponent<Text>().text = "조사력 : " + erp.survey.ToString();

            if (erp.econStance < 0.35)
            {
                GameManager.Instance.details[show - 1].transform.GetChild(8).GetComponent<Text>().text = "경제적 입장 : 보수";
            }
            else if (erp.econStance >= 0.65)
            {
                GameManager.Instance.details[show - 1].transform.GetChild(8).GetComponent<Text>().text = "경제적 입장 : 진보";
            }
            else
            {
                GameManager.Instance.details[show - 1].transform.GetChild(8).GetComponent<Text>().text = "경제적 입장 : 중도";
            }

            if (erp.socialStance < 0.35)
            {
                GameManager.Instance.details[show - 1].transform.GetChild(9).GetComponent<Text>().text = "사회적 입장 : 보수";
            }
            else if (erp.socialStance >= 0.65)
            {
                GameManager.Instance.details[show - 1].transform.GetChild(9).GetComponent<Text>().text = "사회적 입장 : 진보";
            }
            else
            {
                GameManager.Instance.details[show - 1].transform.GetChild(9).GetComponent<Text>().text = "사회적 입장 : 중도";
            }

            GameManager.Instance.details[show - 1].transform.GetChild(10).GetComponent<Text>().text = erp.buyout.ToString();
            for (int i = 0; i < erp.perks.Count; i++)
            {
                GameManager.Instance.details[show - 1].transform.GetChild(12).GetChild(i).GetComponent<Text>().text = erp.perks[i];
            }
            //
        }
    }

    public void BackButton(int show)
    {
        GameManager.Instance.reporterManager.transform.GetChild(show).GetChild(0).gameObject.SetActive(true);
        GameManager.Instance.Pages[show-1].SetActive(true);
        for(int i = 0; i < GameManager.Instance.details[show-1].transform.GetChild(13 - (show - 1)).childCount; i++){
            GameManager.Instance.details[show-1].transform.GetChild(13 - (show - 1)).GetChild(i).GetComponent<Text>().text = "";
        }
        GameManager.Instance.details[show-1].gameObject.SetActive(false);
    }

    public void InsaGwanRi(int show, int hide)
    {
        GameManager.Instance.reporterManager.transform.GetChild(hide).GetChild(0).gameObject.SetActive(true);
        GameManager.Instance.Pages[hide-1].SetActive(true);
        for (int i = 0; i < GameManager.Instance.details[hide-1].transform.GetChild(13 - (hide - 1)).childCount; i++)
        {
            GameManager.Instance.details[hide-1].transform.GetChild(13 - (hide - 1)).GetChild(i).GetComponent<Text>().text = "";
        }
        GameManager.Instance.details[hide-1].gameObject.SetActive(false);
        GameManager.Instance.HideOtherReporter(hide, 0);

        GameManager.Instance.reporterManager.transform.GetChild(hide).gameObject.SetActive(false);
        GameManager.Instance.reporterManager.transform.GetChild(show).gameObject.SetActive(true);
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

        BackButton(1);
    }

    public void EmploymentButton()
    {
        if (!ReporterManager.Instance.employReporter.is_employed)
        {
            for (int i = 0; i < GameManager.Instance.company.em_reporters.Count; i++)
            {
                if (ReporterManager.Instance.employReporter.reporter_index == GameManager.Instance.company.em_reporters[i].reporter_index)
                {
                    GameManager.Instance.company.em_reporters[i].is_employed = true;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < GameManager.Instance.company.em_reporters.Count; i++)
            {
                if (ReporterManager.Instance.employReporter.reporter_index == GameManager.Instance.company.em_reporters[i].reporter_index)
                {
                    GameManager.Instance.company.em_reporters[i].is_employed = false;
                    break;
                }
            }
        }

        for (int i = 0; i < ReporterManager.Instance.evrs.Count; i++)
        {
            ReporterManager.Instance.evrs[i].UpdateStatus();
            ReporterManager.Instance.evrs[i].DisplayFireIcon();
        }

        BackButton(2);
    }
}
