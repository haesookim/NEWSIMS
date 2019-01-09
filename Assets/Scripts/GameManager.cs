using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Setting
{
    public int startingReporters; //시작할 때, 기자의 수
    public int startingMoney; //시작할 때, 시작 금액
    int newsPoint; //신문에 기사를 배정할 수 있는 포인트
    public enum Fields { Game, Entertainment, Social, Sports }; //관심사 종류들
    public enum Names { 넥슨, 넷마블, 엔씨, 스마게, 데브 }; //기자 이름들

    public Setting(int start_reporter, int start_money, int point)
    {
        startingReporters = start_reporter;
        startingMoney = start_money;
        newsPoint = point;
    }
}

class Society
{
    public List<Human> citizens = new List<Human>(); //사회 전체 시민 목록

    public void AddHumanToList(Human person) //Human을 리스트에 추가하는 함수
    {
        citizens.Add(person);
    }
}

class Company
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

class Human //사람 성향 --> 생성할 때 모든 float 값에 0~1 사이의 무작위 값을 부여
{
    public float econStance; //경제적 입장(0 : 극보수, 1: 극진보)
    float socialStance; //사회적 입장(0 : 극보수, 1: 극진보)
    public Dictionary<Setting.Fields, float> interests; //각 관심사에 대한 관심도

    public Human()
    {
        econStance = Random.Range(0.0f, 1.0f);
        socialStance = Random.Range(0.0f, 1.0f);
        interests = new Dictionary<Setting.Fields, float>();
        foreach (Setting.Fields field in System.Enum.GetValues(typeof(Setting.Fields)))
        {
            interests.Add(field, Random.Range(0.0f, 1.0f));
        }
    }
}

class Reporter : Human
{
    public string name; //이름
    int satisfaction = 100; //만족도 (0이 되면 퇴사?)

    public Reporter()
    {
        name = System.Enum.GetName(typeof(Setting.Names),(int)Random.Range(0.0f, 4.0f));
    }
}

class Article
{
    Setting.Fields article_field; //어떤 관심사의 기사인가?
    int vitality; //파급력
    int date; //생성된 날짜
}

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

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

        Setting starting = new Setting(4, 20, 100); //시작 조건
        Society start_society = new Society(); //사회 구축
        Company myCompany = new Company(starting.startingMoney, 0, 50f); //우리 회사 생성

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

        papers = new List<Drag>();
    }

    public void AddPaperToList(Drag script) //리스트에 추가
    {
        papers.Add(script);
    }
}
