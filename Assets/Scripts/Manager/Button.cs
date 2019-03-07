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
        EventManager.DayEvent_ReporterManage += Process; //해고/고용 과정 이벤트
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

            GameManager.Instance.details[show - 1].transform.GetChild(10).GetComponent<Text>().text = "가격 : " + erp.buyout.ToString();
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

    //한솔이 만듬
    void Process(Society society, Company company)
    {
        while (company.reporters.Exists(x => x.is_fired == true)) //리포터 리스트에서 is_fired가 true인 값이 존재한다면
        {
            for (int i = 0; i < company.reporters.Count; i++)
            {
                if (company.reporters[i].is_fired)
                {
                    for (int j = 0; j < ReporterManager.Instance.vrs.Count; j++)
                    {
                        if (ReporterManager.Instance.vrs[j].reporter.reporter_index == company.reporters[i].reporter_index)
                        {
                            Destroy(ReporterManager.Instance.vrs[j].gameObject);
                            ReporterManager.Instance.RemoveVrsToList(ReporterManager.Instance.vrs[j]);
                            break;
                        }
                    }
                    EventManager.DayEvent_Beginning -= company.reporters[i].WriteArticle; //기사쓰는 이벤트를 지우고

                    Debug.Log(company.reporters[i].name + "이 해고당했습니다.");

                    company.RemoveReporterToList(company.reporters[i]); //리스트에서 삭제해라
                    break;
                }
            }
        }

        while (company.em_reporters.Exists(x => x.is_employed == true))
        {
            for (int i = 0; i < company.em_reporters.Count; i++)
            {
                if (company.em_reporters[i].is_employed)
                {
                    if (company.money >= company.em_reporters[i].buyout)
                    {
                        company.money -= company.em_reporters[i].buyout;
                        company.index++;
                        Reporter newReporter = new Reporter(GameManager.Instance.setting, company, company.index);

                        ReporterOverwrite(newReporter, company.em_reporters[i]);

                        company.AddReporterToList(newReporter);
                        GameManager.Instance.CreateReporterButton(company.reporters.Count - 1, newReporter);

                        company.em_reporters[i].is_employed = false;
                        Debug.Log(newReporter.name + "을/를 고용했습니다.");
                        break;
                    }
                    else
                    {
                        company.em_reporters[i].is_employed = false;
                        Debug.Log("돈이 부족해 고용할 수 없습니다.");
                        break;
                    }
                }
            }
        }

        for (int i = 0; i < ReporterManager.Instance.vrs.Count; i++)
        {
            ReporterManager.Instance.vrs[i].UpdateStatus();
        }

        company.em_reporters.Clear();
        while (ReporterManager.Instance.evrs.Count != 0)
        {
            Destroy(ReporterManager.Instance.evrs[0].gameObject);
            ReporterManager.Instance.RemoveeVrsToList(ReporterManager.Instance.evrs[0]);
        }
        for (int i = 0; i < 6; i++) //고용 가능한 기자 생성
        {
            EmReporter emReporter = new EmReporter(GameManager.Instance.setting, company, i);
            company.em_reporters.Add(emReporter);
            GameManager.Instance.CreateEmReporterButton(i, emReporter);
        }
    }

    void ReporterOverwrite(Reporter reporter, EmReporter emReporter)
    {
        reporter.reporterImage = emReporter.reporterImage;
        reporter.name = emReporter.name;
        reporter.level = emReporter.level;
        reporter.perks.Clear();
        for (int i = 0; i < emReporter.perks.Count; i++)
        {
            reporter.AddPerkToList(emReporter.perks[i]);
        }
        reporter.writing = emReporter.writing;
        reporter.logic = emReporter.logic;
        reporter.survey = emReporter.survey;
        reporter.econStance = emReporter.econStance;
        reporter.socialStance = emReporter.socialStance;
        reporter.interests.Clear();
        foreach (Setting.Fields field in System.Enum.GetValues(typeof(Setting.Fields)))
        {
            reporter.interests.Add(field, emReporter.interests[field]);
        }
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
