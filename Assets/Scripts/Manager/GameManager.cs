using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [HideInInspector] public Setting setting; // 뉴게임 시작시의 초기 세팅

    [HideInInspector] public int point; //남은 포인트
    [HideInInspector] public int temp_point; //배정할 포인트의 임시 저장공간

    [HideInInspector] public Society society;
    public Company company;
    
    
    [HideInInspector] public bool in_DeskMenu = false; 
    [HideInInspector] public bool in_PaperMenu = false; //페이퍼메뉴가 켜져있는가?


    /////////////////////////////////////////화면컨트롤용 
    [HideInInspector] public GameObject officeWindow; //오피스 화면
    [HideInInspector] public GameObject deskWindow; //책상 화면
    [HideInInspector] public GameObject paperWindow; //상세 기사 화면
    public Paper selectedPaper; //상세 기사화면에 출력될 기사
    //////////////////////////////////////////


    [Header("기사 오브젝트 작성용 데이터 풀")]
    public List<GameObject> papers; //현재 생성되어있는 기사 오브젝트들
    public Sprite[] PaperImages; //이미지 데이터
    public Sprite[] ReporterImages; //이미지 데이터
    public GameObject PaperPrefab; //게임 내에 생성될 기사 오브젝트의 프리팹

    [Header("Office Window 오브젝트")]
    public Text dateText;
    public Text moneyText;
    public Text numberOfreporterText;

    [Header("Desk Window 오브젝트")]
    public ScrollRect paperTextView;
    public GameObject newspaper;

    ///////////////////////////////////////////////////////////디버그용. 나중에 삭제
    [Header("FOR DEBUG")]
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

        SetWindowDefault();

        AudioManager.Instance.StartMusic("gameplay");
    }

    public void CreatGame()
    {
        /* 게임 최초 생성시 */
        setting = new Setting();
        point = setting.newsPoint; //뉴스 포인트를 지정
        society = new Society(); //사회 구축
        company = new Company(setting.startingMoney, 50f); //회사 생성

        for (int i = 0; i < setting.startingReporters; i++) //startingReporters만큼의 회사의 기자 생성
        {
            Reporter newReporter = new Reporter(setting, company, i+1);
            company.AddReporterToList(newReporter);
            company.index++; //
        }
        company.reporters[company.reporters.Count - 1].LevelUp(); //마지막 기자에게 즉시 레벨업 한 번

        for (int i = 0; i < 1000; i++) //시작할 때 1,000명의 시민 생성
        {
            Citizen who = new Citizen();
            society.AddCitizenToList(who);
        }
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

    void Update() 
     {
         //기본 표시 사항 업데이트
        dateText.text = society.day.ToString();
        moneyText.text = company.money.ToString();
        numberOfreporterText.text = company.reporters.Count.ToString();
    }

    public void LoadGame()
    {
        //todo: 저장한 게임 데이터를 불러와 매칭한다.
    }

    public void BeginningofDay()
    {
        EventManager.Instance.Do_BeginningofDay(society,company); //하루의 시작 이벤트 호출
        PublishArticle();
    }

    public void EndofDay()
    {
        

        while(papers.Count != 0)
        {
            //Destroy(papers[0]);
            //papers.RemoveAt(0);
        }
        EventManager.Instance.Do_EndofDay(society,company);

        society.day++;
        SetWindowDefault();
        BeginningofDay();

        System.GC.Collect(); //매일 메모리 찌꺼기를 비움.

        //todo: 신문발행하고 기자들 액션 취한 후 시민들이 보고 스탯변동. 
    }

    ///<summary>
    ///게임 내 DeskWindow에 기사 오브젝트를 생성.   
    ///기자와 기사에 관한 정보를 모두 담고 있고, 인스펙터에서 확인 가능
    ///</summary>
    public void PublishArticle() 
    {
        Text view = paperTextView.content.GetComponentInChildren<Text>();
        view.text =" ";
        
        for(int i=0; i<company.articles.Count; i++)
        {
            Vector3 publishPosition = new Vector3(Random.Range(-2.0f,5.0f),Random.Range(-3f,3.0f),0); //WorkDesk위 랜덤위치에 등장
            CreateArticle(publishPosition,i);
            
            view.text += "\n" + company.articles[i].article_name + "\t" + "배분도: n";         
            //지난 기록을 보려면.. 삭제하면 안되고 계속 쌓아야 할듯? 구조도 변경해야 할거고.
        }

        int count = company.articles.Count;
        company.articles.RemoveRange(0,count);
    }

    public void CreateArticle(Vector3 position, int _article_index)
    {
        GameObject paperObject = Instantiate(PaperPrefab,position,Quaternion.identity,deskWindow.transform);
        Paper temp_paper = paperObject.GetComponent<Paper>();
        temp_paper.reporter = company.reporters[company.articles[_article_index].write_reporter_index-1];
        temp_paper.article = company.articles[_article_index];
        //기사 오브젝트에 데이터 입력

        Vector3 paprerPosition = new Vector3(paperObject.transform.position.x,paperObject.transform.position.y,temp_paper.reporter.reporter_index);
        paperObject.transform.position = paprerPosition; //z값 변경
        papers.Add(paperObject);
    }

}
