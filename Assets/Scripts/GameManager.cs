using System.Collections;   
using System.Collections.Generic;
using UnityEngine;

public class Setting
{
    public int startingReporters; //시작할 때, 기자의 수
    public int startingMoney; //시작할 때, 시작 금액
    public int newsPoint; //신문에 기사를 배정할 수 있는 포인트
    public enum Fields { Game, Entertainment, Social, Sports }; //관심사 종류들
    public enum Names { 넥슨, 넷마블, 엔씨, 스마게, 데브 }; //기자 이름들

    public Setting(int start_reporter, int start_money, int point)
    {
        startingReporters = start_reporter;
        startingMoney = start_money;
        newsPoint = point;
    }
}

public class Society
{
    public int day = 1; //사회 현재 날짜
    public List<Human> citizens = new List<Human>(); //사회 전체 시민 목록

    public void AddHumanToList(Human person) //Human을 리스트에 추가하는 함수
    {
        citizens.Add(person);
    }
}

public class Company
{
    public List<Reporter> reporters = new List<Reporter>(); //기자 목록
    public List<Article> articles = new List<Article>(); //현재 가지고 있는 기사 목록
    int money; //돈. 기자 1인당 하루에 1 지출
    int famous; //인지도
    float credibility; //신뢰도

    public Company(int m, int f, float c)
    {
        money = m;
        famous = f;
        credibility = c;
    }

    public void AddReporterToList(Reporter reporter) //Reporter를 리스트에 추가하는 함수
    {
        reporters.Add(reporter);
    }

    public void AddArticleToList(Article article) //Article을 리스트에 추가하는 함수
    {
        articles.Add(article);
    }
}

public class Human //사람 성향 --> 생성할 때 모든 float 값에 0~1 사이의 무작위 값을 부여 --> 일단 소수점 넷째 자리에서 반올림하게 만듬
{
    public float econStance; //경제적 입장(0 : 극보수, 1: 극진보)
    float socialStance; //사회적 입장(0 : 극보수, 1: 극진보)
    public Dictionary<Setting.Fields, float> interests; //각 관심사에 대한 관심도

    public Human()
    {
        econStance = Mathf.Round(Random.Range(0.0f, 1.0f) * 10000) / 10000;
        socialStance = Mathf.Round(Random.Range(0.0f, 1.0f) * 10000) / 10000;
        interests = new Dictionary<Setting.Fields, float>();
        foreach (Setting.Fields field in System.Enum.GetValues(typeof(Setting.Fields)))
        {
            interests.Add(field, Mathf.Round(Random.Range(0.0f, 1.0f) * 10000) / 10000);
        }
    }
}

public class Reporter : Human
{
    public string name; //이름
    int satisfaction = 100; //만족도 (0이 되면 퇴사?)

    public Reporter()
    {
        name = System.Enum.GetName(typeof(Setting.Names),(int)Random.Range(0.0f, 4.0f));
    }

    public void WriteArticle(Society society, Company company) //기사 쓰기
    {
        float temp = 0; //랜덤값의 최대값 제한을 위해 넣음
        float sum = 0; //랜덤값에서 관심사 정보를 뽑아내기 위해 넣음
        string temp_field = ""; //관심사 정보를 저장할 변수

        foreach (Setting.Fields field in System.Enum.GetValues(typeof(Setting.Fields)))
        {
            temp += interests[field];
        }

        float rand_value = Mathf.Round(Random.Range(0.0f, temp) * 10000) / 10000;

        foreach (Setting.Fields field in System.Enum.GetValues(typeof(Setting.Fields)))
        {
            if (rand_value >= sum && rand_value < sum + interests[field])
            {
                temp_field = field.ToString();
                break;
            }
            sum += interests[field];
        }

        int temp_virality = 0; //파급력 정보를 저장할 변수

        for (int i = 1; i <= 10; i++)
        {
            temp_virality = i;
            float temp_rand = Random.Range(0.0f, 1.0f);
            if (temp_rand <= 0.5f)
            {
                break;
            }
        }

        Article article = new Article(temp_field, temp_virality, society.day);
        company.AddArticleToList(article);
    }
}

public class Article
{
    public string article_field; //어떤 관심사의 기사인가?
    public int virality; //파급력
    public int date; //생성된 날짜

    public Article(string af, int v, int d)
    {
        article_field = af;
        virality = v;
        date = d;
    }
}

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    public GameObject news;

    [HideInInspector] public int point; //남은 포인트
    [HideInInspector] public bool in_menu = false; //메뉴가 켜져있는가?
    [HideInInspector] public Company myCompany;
    [HideInInspector] public List<Drag> papers;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject); //파괴되지않아!

        papers = new List<Drag>();

        Setting starting = new Setting(4, 20, 100); //시작 조건
        point = starting.newsPoint;
        Society start_society = new Society(); //사회 구축
        myCompany = new Company(starting.startingMoney, 0, 50f); //우리 회사 생성

        for (int i = 0; i < 1000; i++) //시작할 때 1,000명의 시민 생성
        {
            Human who = new Human();
            start_society.AddHumanToList(who);
        }
        for (int i = 0; i < starting.startingReporters; i++) //startingReporters만큼의 우리 회사의 기자 생성
        {
            Reporter newReporter = new Reporter();
            myCompany.AddReporterToList(newReporter);
            Debug.Log("리포터 " + i + " 의 이름 : " + myCompany.reporters[i].name);
            Debug.Log("리포터 " + i + " 의 경제적 입장 : " + myCompany.reporters[i].econStance);
            foreach (Setting.Fields field in System.Enum.GetValues(typeof(Setting.Fields)))
            {
                Debug.Log("리포터 " + i + " 의 " + field + " 관심사 : " + myCompany.reporters[i].interests[field]);
            }
        }
        Debug.Log("총 사람 수 : " + start_society.citizens.Count);
        Debug.Log("회사 기자 수 : " + myCompany.reporters.Count);

        //Debug.Log는 랜덤하게 생성되었는지 확인하기 위해 넣어둠.

        WriteArticles(start_society, myCompany); //나중에 날짜 넘어가는 함수에 넣어야 함.

        for (int i = 0; i<  myCompany.articles.Count; i++) //Debug.Log는 랜덤하게 생성되었는지 확인하기 위해 넣어둠.
        {
            Debug.Log(myCompany.articles[i].article_field);
            Debug.Log(myCompany.articles[i].virality);
            Debug.Log(myCompany.articles[i].date);
        }
    }

    public void WriteArticles(Society society,Company company) //기자들이 각자 기사를 쓰는 함수
    {
        for (int i = 0; i < company.reporters.Count; i++)
        {
            company.reporters[i].WriteArticle(society, company);
            if (company == myCompany)
            {
                Instantiate(news); //프리팹 생성
            }
        }
    }

   public void AddPaperToList(Drag script) //리스트에 추가
    {
        papers.Add(script);
    }
}