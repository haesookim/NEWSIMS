﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector] public Setting setting; // 뉴게임 시작시의 초기 세팅

    [HideInInspector] public Society society;
    public Company company;
    
    
    [HideInInspector] public bool in_DeskMenu = false; 
    [HideInInspector] public bool in_PaperMenu = false; //페이퍼메뉴가 켜져있는가?
    [HideInInspector] public bool in_ReporterMenu = false; //인사관리 창이 켜져있는가?

    [HideInInspector] public List<Dictionary<string, object>> data;
    [HideInInspector] public List<string> originName; //사용 가능 기사 리스트

    /////////////////////////////////////////화면컨트롤용 
    [HideInInspector] public GameObject officeWindow; //오피스 화면
    [HideInInspector] public GameObject deskWindow; //책상 화면
    [HideInInspector] public GameObject paperWindow; //상세 기사 화면
    [HideInInspector] public GameObject reporterManager; //인사관리 화면
    public Paper selectedPaper; //상세 기사화면에 출력될 기사
    //////////////////////////////////////////


    [Header("기사 오브젝트 작성용 데이터 풀")]
    public List<GameObject> papers; //현재 생성되어있는 기사 오브젝트들
    public Sprite[] PaperImages; //이미지 데이터
    public Sprite[] ReporterImages; //이미지 데이터
    public GameObject PaperPrefab; //게임 내에 생성될 기사 오브젝트의 프리팹

    public Sprite[] AssignedPaperPrefab; //사이즈별 기사 프리팹 모음
    public Dictionary<Vector2,Sprite> AssignedPaperToSize; // assignedPaperPrefab을 사이즈값 벡터로 변환하여 저장한 딕셔너리



    [Header("Office Window 오브젝트")]
    public Text dateText;
    public Text moneyText;
    //public Text numberOfreporterText;

    [Header("Desk Window 오브젝트")]
    public Transform textView;
 
    public GameObject scrollviewText;
    [Space(20)]

    public GameObject newspaper;
    public GameObject ReportButton;
    public GameObject[] Pages;
    public GameObject PageButton;
    public GameObject[] details;
    public GameObject EmReportButton;

    ///////////////////////////////////////////////////////////디버그용. 나중에 삭제
    [Header("FOR DEBUG")]
    public Text reportText;
    string tempText;
    
    public void AddReportText(string _text)
    {
        tempText += _text;
        tempText += "\n";
    }

    public void DisplayReportText()
    {
        reportText.text = tempText;
        tempText = "";
    }

    [Tooltip("체크 시 리포터 정보 콘솔에 출력")]
    public bool print_reporter_data;
    [Tooltip("체크 시 사람수,기자수,기사정보 콘솔에 출력")]
    public bool print_companyandarticle;
    //////////////////////////////////////////////////////////

    public override void Awake() {

        CreatGame(); //아직 세이브기능이 없어서 항상 새 게임.
        InitGame();
    }

    void Start() {

        //하루의 시작
        BeginningofDay();
                 
        DEBUG_PrintReporter();
        DEBUG_PrintCompanyandArticle();
    }
   
    void DEBUG_PrintReporter()
    {
        if(! print_reporter_data) return;
        for(int i =0; i < company.reporters.Count ; i++)
        {
            Debug.Log("리포터 " + i + " 의 이름 : " + company.reporters[i].name);
            Debug.Log("리포터 " + i + " 의 경제적 입장 : " + company.reporters[i].econStance);
            foreach (Setting.Fields field in System.Enum.GetValues(typeof(Setting.Fields)))
            {
                Debug.Log("리포터 " + i + " 의 " + field + " 관심사 : " + company.reporters[i].interests[field]);
            }
        }
    }

    void DEBUG_PrintCompanyandArticle()
    {
        if(!print_companyandarticle) return;
        Debug.Log("총 사람 수 : " + society.citizens.Count);
        Debug.Log("회사 기자 수 : " + company.reporters.Count);

        //Debug.Log는 랜덤하게 생성되었는지 확인하기 위해 넣어둠.

        for (int i = 0; i<  company.articles.Count; i++) //Debug.Log는 랜덤하게 생성되었는지 확인하기 위해 넣어둠.
        {
            Debug.Log(company.articles[i].article_field);
            Debug.Log(company.articles[i].virality);
            Debug.Log(company.articles[i].date);
        }
    }

    void InitGame()
    {
        //화면 전환용 캐싱
        GameObject window = GameObject.Find("Window").gameObject;
        officeWindow = window.transform.GetChild(0).gameObject;
        deskWindow = window.transform.GetChild(1).gameObject;
        paperWindow = deskWindow.transform.GetChild(1).gameObject;
        reporterManager = deskWindow.transform.GetChild(6).gameObject;

        //지면배정 기사 프리팹을 사이즈로 변환하여 저장하는 초기화
        // 이후부턴 AssignedPaperToSie(Grid grid) 를 호출하여 사이즈별로 바로 해당 프리팹을 불러올 수 있음.
        AssignedPaperToSize = new Dictionary<Vector2, Sprite>();
        int t =0;
        for(int i=0; i<4; i++)
        {
            for(int j =0; j<5; j++)
            {
                Vector2 size = new Vector2(i+1,j+1);
                AssignedPaperToSize.Add(size,AssignedPaperPrefab[t]);
                t++;
            }
        }


        SetWindowDefault();

        AudioManager.Instance.StartMusic("gameplay");
    }

    public void CreatGame()
    {
        /* 게임 최초 생성시 */
        setting = new Setting();
        society = new Society(); //사회 구축
        company = new Company(setting.startingMoney, 0f); //회사 생성

        data = CSVReader.Read("PaperName"); //CSV를 불러옴
        for (int i = 0; i < data.Count; i++)
        {
            originName.Add(data[i]["제목"].ToString());
        }

        for (int i = 0; i < setting.startingReporters; i++) //startingReporters만큼의 회사의 기자 생성
        {
            Reporter newReporter = new Reporter(setting, company, i+1);
            company.AddReporterToList(newReporter);
            company.index++; //
            CreateReporterButton(i, newReporter);
        }
        company.reporters[company.reporters.Count - 1].LevelUp(); //마지막 기자에게 즉시 레벨업 한 번

        for (int i = 0; i < 6; i++) //고용 가능한 기자 생성
        {
            EmReporter emReporter = new EmReporter(setting, company, i);
            company.em_reporters.Add(emReporter);
            CreateEmReporterButton(i, emReporter);
        }

        PageNumbering(company, 2);
        HideOtherReporter(2, 0);

        PageNumbering(company, 1);
        HideOtherReporter(1, 0);

        for (int i = 0; i < 1000; i++) //시작할 때 1,000명의 시민 생성
        {
            Citizen who = new Citizen();
            society.AddCitizenToList(who);
        }

        //기본 표시 사항 업데이트
        dateText.text = society.day.ToString() + "일";
        moneyText.text = company.money.ToString();
    }


    public void SetWindowDefault()
    {
        officeWindow.SetActive(true);
        deskWindow.SetActive(false); 
        paperWindow.SetActive(false);
        in_PaperMenu = false;
        in_DeskMenu = false;
        //테스트마다 껏다켰다하기 귀찮아서
    }

    public void LoadGame()
    {
        //todo: 저장한 게임 데이터를 불러와 매칭한다.
    }

    public void BeginningofDay()
    {
        EventManager.Instance.Do_BeginningofDay(society,company); //하루의 시작 이벤트 호출
        AddReportText("현재 자금 : " + company.money);
        DisplayReportText();
        PublishArticle();
    }

    public void EndingRoot() //엔딩 분기 판별 함수
    {

        if (company.money < 0)
        {
            EndingManager.instance.ending_phase = 0;
            LoadingSceneManager.LoadScene("02.Ending");
        }

        else if (society.day >= 5 && company.fakeRate >= 0.25 * setting.fakePossibility)
        {
            EndingManager.instance.ending_phase = 1;
            LoadingSceneManager.LoadScene("02.Ending");
        }

        else if (society.day > 7)
        {
            float endRate = (float)(company.fieldRate[(Setting.Fields)0] + company.fieldRate[(Setting.Fields)1]) / (company.fieldRate[(Setting.Fields)0] + company.fieldRate[(Setting.Fields)1] + company.fieldRate[(Setting.Fields)2] + company.fieldRate[(Setting.Fields)3]);
            float endStance = 0f;
            for (int i = 0; i < company.reporters.Count; i++)
            {
                endStance += (company.reporters[i].econStance+company.reporters[i].socialStance) / 2;
            }
            endStance = endStance / company.reporters.Count;

            if ((endRate >= 2.0 / System.Enum.GetNames(typeof(Setting.Fields)).Length) && (endStance < 0.35) && (company.fakeRate < 0.05f * setting.fakePossibility))
            {
                EndingManager.instance.ending_phase = 4;
            }
            else if ((endRate >= 2.0 / System.Enum.GetNames(typeof(Setting.Fields)).Length) && (endStance < 0.35) && (company.fakeRate >= 0.05f * setting.fakePossibility) && (company.fakeRate < 0.15f * setting.fakePossibility))
            {
                EndingManager.instance.ending_phase = 5;
            }
            else if ((endRate >= 2.0 / System.Enum.GetNames(typeof(Setting.Fields)).Length) && (endStance < 0.35) && (company.fakeRate >= 0.15f * setting.fakePossibility))
            {
                EndingManager.instance.ending_phase = 6;
            }
            else if ((endRate >= 2.0 / System.Enum.GetNames(typeof(Setting.Fields)).Length) && (endStance >= 0.35) && (endStance < 0.65) && (company.fakeRate < 0.05f * setting.fakePossibility))
            {
                EndingManager.instance.ending_phase = 7;
            }
            else if ((endRate >= 2.0 / System.Enum.GetNames(typeof(Setting.Fields)).Length) && (endStance >= 0.35) && (endStance < 0.65) && (company.fakeRate >= 0.05f * setting.fakePossibility) && (company.fakeRate < 0.15f * setting.fakePossibility))
            {
                EndingManager.instance.ending_phase = 8;
            }
            else if ((endRate >= 2.0 / System.Enum.GetNames(typeof(Setting.Fields)).Length) && (endStance >= 0.35) && (endStance < 0.65) && (company.fakeRate >= 0.15f * setting.fakePossibility))
            {
                EndingManager.instance.ending_phase = 9;
            }
            else if ((endRate >= 2.0 / System.Enum.GetNames(typeof(Setting.Fields)).Length) && (endStance >= 0.65) && (company.fakeRate < 0.05f * setting.fakePossibility))
            {
                EndingManager.instance.ending_phase = 10;
            }
            else if ((endRate >= 2.0 / System.Enum.GetNames(typeof(Setting.Fields)).Length) && (endStance >= 0.65) && (company.fakeRate >= 0.05f * setting.fakePossibility) && (company.fakeRate < 0.15f * setting.fakePossibility))
            {
                EndingManager.instance.ending_phase = 11;
            }
            else if ((endRate >= 2.0 / System.Enum.GetNames(typeof(Setting.Fields)).Length) && (endStance >= 0.65) && (company.fakeRate >= 0.15f * setting.fakePossibility))
            {
                EndingManager.instance.ending_phase = 12;
            } else if ((endRate < 2.0 / System.Enum.GetNames(typeof(Setting.Fields)).Length) && (company.fieldRate[(Setting.Fields)2] >= company.fieldRate[(Setting.Fields)3]))
            {
                EndingManager.instance.ending_phase = 2;
            }
            else if ((endRate < 2.0 / System.Enum.GetNames(typeof(Setting.Fields)).Length) && (company.fieldRate[(Setting.Fields)3] > company.fieldRate[(Setting.Fields)2]))
            {
                EndingManager.instance.ending_phase = 3;
            }
            else
            {
                EndingManager.instance.ending_phase = 13;
            }
            LoadingSceneManager.LoadScene("02.Ending");
        }
    }

    public void EndofDay()
    {

        //////사운드
        AudioManager.instance.StartSFX("publish");


        while(papers.Count != 0)
        {
            papers.RemoveAt(0);
        }
        EventManager.Instance.Do_EndofDay(society,company);

        society.day++;   

        SetWindowDefault();
        BeginningofDay();

        //기본 표시 사항 업데이트
        dateText.text = society.day.ToString() + "일";
        moneyText.text = company.money.ToString();

        EventManager.Instance.DayEvent_ReporterManage -= Process;
        System.GC.Collect(); //매일 메모리 찌꺼기를 비움.

        //todo: 신문발행하고 기자들 액션 취한 후 시민들이 보고 스탯변동. 
    }

    ///<summary>
    ///게임 내 DeskWindow에 기사 오브젝트를 생성.   
    ///기자와 기사에 관한 정보를 모두 담고 있고, 인스펙터에서 확인 가능
    ///</summary>
    public void PublishArticle() 
    {
        for(int i=0; i<company.articles.Count; i++)
        {
            Vector3 publishPosition = new Vector3(Random.Range(-2.0f,5.0f),Random.Range(-3f,3.0f),0); //WorkDesk위 랜덤위치에 등장
            CreateArticle(publishPosition,i);            
        }

        int count = company.articles.Count;
        company.articles.RemoveRange(0,count);
    }

    public void CreateArticle(Vector3 position, int _article_index)
    {
        GameObject paperObject = Instantiate(PaperPrefab,position,Quaternion.identity,deskWindow.transform);
        Paper temp_paper = paperObject.GetComponent<Paper>();
        temp_paper.SetImage();
        temp_paper.article = company.articles[_article_index];
        temp_paper.reporter_name = temp_paper.article.write_reporter_name;
        
        //기사 오브젝트에 데이터 입력
        Text view = Instantiate(scrollviewText,scrollviewText.transform.position,Quaternion.identity,textView).GetComponent<Text>();
        temp_paper.viewText = view;         
        temp_paper.UpdateViewText("0");
        //텍스트뷰에 기사 제목 업데이트
        //지난 기록을 보려면.. 삭제하면 안되고 계속 쌓아야 할듯? 구조도 변경해야 할거고.

        Vector3 paprerPosition = new Vector3(paperObject.transform.position.x,paperObject.transform.position.y,temp_paper.article.write_reporter_index*0.01f);
        paperObject.transform.position = paprerPosition; //z값 변경
        papers.Add(paperObject);
    }

    //인사관리창에서 회사 기자들을 표시하는 프리팹 생성
    public void CreateReporterButton(int number, Reporter reporter)
    {
        switch (number%3)
        {
            case 0:
                GameObject temp_rep = Instantiate(ReportButton,reporterManager.transform.GetChild(1).GetChild(0));
                temp_rep.GetComponent<VisualizeReporter>().reporter = reporter;
                break;
            case 1:
                GameObject temp_rep2 = Instantiate(ReportButton, reporterManager.transform.GetChild(1).GetChild(0));
                temp_rep2.transform.localPosition = new Vector3(0f, 0f, 0f);
                temp_rep2.GetComponent<VisualizeReporter>().reporter = reporter;
                break;
            case 2:
                GameObject temp_rep3 = Instantiate(ReportButton, reporterManager.transform.GetChild(1).GetChild(0));
                temp_rep3.transform.localPosition = new Vector3(0f, -150f, 0f);
                temp_rep3.GetComponent<VisualizeReporter>().reporter = reporter;
                break;
        }
    }

    public void UpdateReporterButton(int number)
    {
        for (int i = 0; i < number; i++)
        {
            switch (i % 3)
            {
                case 0:
                    reporterManager.transform.GetChild(1).GetChild(0).GetChild(i).transform.localPosition = new Vector3(0f, 150f, 0f);
                    break;
                case 1:
                    reporterManager.transform.GetChild(1).GetChild(0).GetChild(i).transform.localPosition = new Vector3(0f, 0f, 0f);
                    break;
                case 2:
                    reporterManager.transform.GetChild(1).GetChild(0).GetChild(i).transform.localPosition = new Vector3(0f, -150f, 0f);
                    break;
            }
        }
        PageNumberingUpdate(company, 2);
        HideOtherReporter(2, 0);

        PageNumberingUpdate(company, 1);
        HideOtherReporter(1, 0);
    }

    public void CreateEmReporterButton(int number, EmReporter emreporter)
    {
        switch (number % 3)
        {
            case 0:
                GameObject temp_rep = Instantiate(EmReportButton, reporterManager.transform.GetChild(2).GetChild(0));
                temp_rep.GetComponent<VisualizeEmReporter>().emreporter = emreporter;
                ReporterManager.Instance.AddeVrsToList(temp_rep.GetComponent<VisualizeEmReporter>());
                break;
            case 1:
                GameObject temp_rep2 = Instantiate(EmReportButton, reporterManager.transform.GetChild(2).GetChild(0));
                temp_rep2.transform.localPosition = new Vector3(0f, 0f, 0f);
                temp_rep2.GetComponent<VisualizeEmReporter>().emreporter = emreporter;
                ReporterManager.Instance.AddeVrsToList(temp_rep2.GetComponent<VisualizeEmReporter>());
                break;
            case 2:
                GameObject temp_rep3 = Instantiate(EmReportButton, reporterManager.transform.GetChild(2).GetChild(0));
                temp_rep3.transform.localPosition = new Vector3(0f, -150f, 0f);
                temp_rep3.GetComponent<VisualizeEmReporter>().emreporter = emreporter;
                ReporterManager.Instance.AddeVrsToList(temp_rep3.GetComponent<VisualizeEmReporter>());
                break;
        }
    }

    //인사관리창에서 한 페이지만 보여주기
    public void HideOtherReporter(int child_num, int a)
    {
        for (int i = 0; i < reporterManager.transform.GetChild(child_num).GetChild(0).childCount; i++)
        {
            reporterManager.transform.GetChild(child_num).GetChild(0).GetChild(i).gameObject.SetActive(false);
        }

        if (child_num == 1)
        {
            if (3 * a + 1 <= company.reporters.Count)
            {
                reporterManager.transform.GetChild(child_num).GetChild(0).GetChild(3 * a + 0).gameObject.SetActive(true);
            }
            if (3 * a + 2 <= company.reporters.Count)
            {
                reporterManager.transform.GetChild(child_num).GetChild(0).GetChild(3 * a + 1).gameObject.SetActive(true);
            }
            if (3 * a + 3 <= company.reporters.Count)
            {
                reporterManager.transform.GetChild(child_num).GetChild(0).GetChild(3 * a + 2).gameObject.SetActive(true);
            }
        }
        else if(child_num == 2)
        {
            if (3 * a + 1 <= company.em_reporters.Count)
            {
                reporterManager.transform.GetChild(child_num).GetChild(0).GetChild(3 * a + 0).gameObject.SetActive(true);
            }
            if (3 * a + 2 <= company.em_reporters.Count)
            {
                reporterManager.transform.GetChild(child_num).GetChild(0).GetChild(3 * a + 1).gameObject.SetActive(true);
            }
            if (3 * a + 3 <= company.em_reporters.Count)
            {
                reporterManager.transform.GetChild(child_num).GetChild(0).GetChild(3 * a + 2).gameObject.SetActive(true);
            }
        }
        

        for (int i = 0; i < Pages[child_num-1].transform.childCount; i++)
        {
            Pages[child_num-1].transform.GetChild(i).GetChild(0).GetComponent<Text>().color = new Color(0f, 0f, 0f);
        }
        if (Pages[child_num - 1].transform.childCount != 0)
        {
            Pages[child_num - 1].transform.GetChild(a).GetChild(0).GetComponent<Text>().color = new Color(255f, 0f, 0f);
        }
    }

    //인사관리창 페이지 넘버 달기
    public void PageNumbering(Company company, int child_num)
    {
        if (child_num == 1)
        {
            int namuge = company.reporters.Count % 3;
            if (namuge == 0)
            {
                int div = company.reporters.Count / 3;
                for (int i = 1; i <= div; i++)
                {
                    GameObject pbt = Instantiate(PageButton, Pages[child_num - 1].transform);
                    pbt.GetComponent<PageButton>().page_num = i;
                    pbt.GetComponent<PageButton>().child_num = child_num;
                    pbt.transform.GetChild(0).GetComponent<Text>().text = i.ToString();
                    pbt.transform.localPosition = new Vector3(-400f + 45f * (i - 1), -240f, 0f);
                }
            }
            else
            {
                int div = (company.reporters.Count / 3) + 1;
                for (int i = 1; i <= div; i++)
                {
                    GameObject pbt = Instantiate(PageButton, Pages[child_num - 1].transform);
                    pbt.GetComponent<PageButton>().page_num = i;
                    pbt.GetComponent<PageButton>().child_num = child_num;
                    pbt.transform.GetChild(0).GetComponent<Text>().text = i.ToString();
                    pbt.transform.localPosition = new Vector3(-400f + 45f * (i - 1), -240f, 0f);
                }
            }
        }
        else if (child_num == 2)
        {
            int namuge = company.em_reporters.Count % 3;
            if (namuge == 0)
            {
                int div = company.em_reporters.Count / 3;
                for (int i = 1; i <= div; i++)
                {
                    GameObject pbt = Instantiate(PageButton, Pages[child_num - 1].transform);
                    pbt.GetComponent<PageButton>().page_num = i;
                    pbt.GetComponent<PageButton>().child_num = child_num;
                    pbt.transform.GetChild(0).GetComponent<Text>().text = i.ToString();
                    pbt.transform.localPosition = new Vector3(-400f + 45f * (i - 1), -240f, 0f);
                }
            }
            else
            {
                int div = (company.em_reporters.Count / 3) + 1;
                for (int i = 1; i <= div; i++)
                {
                    GameObject pbt = Instantiate(PageButton, Pages[child_num - 1].transform);
                    pbt.GetComponent<PageButton>().page_num = i;
                    pbt.GetComponent<PageButton>().child_num = child_num;
                    pbt.transform.GetChild(0).GetComponent<Text>().text = i.ToString();
                    pbt.transform.localPosition = new Vector3(-400f + 45f * (i - 1), -240f, 0f);
                }
            }
        }
    }

    void PageNumberingUpdate(Company company, int child_num)
    {
        if (child_num == 1)
        {
            int namuge = company.reporters.Count % 3;
            if (namuge == 0)
            {
                int div = company.reporters.Count / 3;

                if (div < Pages[child_num -1].transform.childCount)
                {
                    int del = Pages[child_num - 1].transform.childCount - div;
                    while (del != 0)
                    {
                        Destroy(Pages[child_num - 1].transform.GetChild(div+del-1).gameObject);
                        del--;
                    }
                }
                else if (div > Pages[child_num - 1].transform.childCount)
                {
                    int plus = div - Pages[child_num - 1].transform.childCount;
                    while (plus != 0)
                    {
                        GameObject pbt = Instantiate(PageButton, Pages[child_num - 1].transform);
                        pbt.GetComponent<PageButton>().page_num = div + 1 - plus;
                        pbt.GetComponent<PageButton>().child_num = child_num;
                        pbt.transform.GetChild(0).GetComponent<Text>().text = (div + 1 - plus).ToString();
                        pbt.transform.localPosition = new Vector3(-400f + 45f * ((div + 1 - plus) - 1), -240f, 0f);
                        plus--;
                    }
                }
            }
            else
            {
                int div = (company.reporters.Count / 3) + 1;
                if (div < Pages[child_num - 1].transform.childCount)
                {
                    int del = Pages[child_num - 1].transform.childCount - div;
                    while (del != 0)
                    {
                        Destroy(Pages[child_num - 1].transform.GetChild(div + del - 1).gameObject);
                        del--;
                    }
                }
                else if (div > Pages[child_num - 1].transform.childCount)
                {
                    int plus = div - Pages[child_num - 1].transform.childCount;
                    while (plus != 0)
                    {
                        GameObject pbt = Instantiate(PageButton, Pages[child_num - 1].transform);
                        pbt.GetComponent<PageButton>().page_num = div + 1 - plus;
                        pbt.GetComponent<PageButton>().child_num = child_num;
                        pbt.transform.GetChild(0).GetComponent<Text>().text = (div + 1 - plus).ToString();
                        pbt.transform.localPosition = new Vector3(-400f + 45f * ((div + 1 - plus) - 1), -240f, 0f);
                        plus--;
                    }
                }
            }
        }
        else if (child_num == 2)
        {
            int namuge = company.em_reporters.Count % 3;
            if (namuge == 0)
            {
                int div = company.em_reporters.Count / 3;

                if (div < Pages[child_num - 1].transform.childCount)
                {
                    int del = Pages[child_num - 1].transform.childCount - div;
                    while (del != 0)
                    {
                        Destroy(Pages[child_num - 1].transform.GetChild(div + del - 1).gameObject);
                        del--;
                    }
                }
                else if (div > Pages[child_num - 1].transform.childCount)
                {
                    int plus = div - Pages[child_num - 1].transform.childCount;
                    while (plus != 0)
                    {
                        GameObject pbt = Instantiate(PageButton, Pages[child_num - 1].transform);
                        pbt.GetComponent<PageButton>().page_num = div + 1 - plus;
                        pbt.GetComponent<PageButton>().child_num = child_num;
                        pbt.transform.GetChild(0).GetComponent<Text>().text = (div + 1 - plus).ToString();
                        pbt.transform.localPosition = new Vector3(-400f + 45f * ((div + 1 - plus) - 1), -240f, 0f);
                        plus--;
                    }
                }
            }
            else
            {
                int div = (company.em_reporters.Count / 3) + 1;
                if (div < Pages[child_num - 1].transform.childCount)
                {
                    int del = Pages[child_num - 1].transform.childCount - div;
                    while (del != 0)
                    {
                        Destroy(Pages[child_num - 1].transform.GetChild(div + del - 1).gameObject);
                        del--;
                    }
                }
                else if (div > Pages[child_num - 1].transform.childCount)
                {
                    int plus = div - Pages[child_num - 1].transform.childCount;
                    while (plus != 0)
                    {
                        GameObject pbt = Instantiate(PageButton, Pages[child_num - 1].transform);
                        pbt.GetComponent<PageButton>().page_num = div + 1 - plus;
                        pbt.GetComponent<PageButton>().child_num = child_num;
                        pbt.transform.GetChild(0).GetComponent<Text>().text = (div + 1 - plus).ToString();
                        pbt.transform.localPosition = new Vector3(-400f + 45f * ((div + 1 - plus) - 1), -240f, 0f);
                        plus--;
                    }
                }
            }
        }
    }

    //한솔이 만듬
    public void Process(Society society, Company company)
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
                    EventManager.Instance.DayEvent_Beginning -= company.reporters[i].WriteArticle; //기사쓰는 이벤트를 지우고

                    Debug.Log(company.reporters[i].name + "이 해고당했습니다.");
                    AddReportText(company.reporters[i].name + "이/가 해고당했습니다.");

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
                        Reporter newReporter = new Reporter(setting, company, company.index);

                        ReporterOverwrite(newReporter, company.em_reporters[i]);

                        company.AddReporterToList(newReporter);
                        CreateReporterButton(company.reporters.Count - 1, newReporter);

                        company.em_reporters[i].is_employed = false;
                        Debug.Log(newReporter.name + "을/를 고용했습니다.");
                        AddReportText(newReporter.name + "을/를 고용했습니다.");
                        break;
                    }
                    else
                    {
                        company.em_reporters[i].is_employed = false;
                        Debug.Log("돈이 부족해 고용할 수 없습니다.");
                        AddReportText("돈이 부족해 고용할 수 없습니다.");

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
            EmReporter emReporter = new EmReporter(setting, company, i);
            company.em_reporters.Add(emReporter);
            CreateEmReporterButton(i, emReporter);
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
}
