﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting
{
    public int startingReporters; //시작할 때, 기자의 수
    public int startingMoney; //시작할 때, 시작 금액
    public int newsPoint; //신문에 기사를 배정할 수 있는 포인트
    public float startingPerkChance = 0.5f; //시작할 때 퍽을 가질 수 있는 확률
    public enum Fields { Game, Entertainment, Social, Sports, Economy }; //관심사 종류들
    public enum Names { 넥슨, 넷마블, 엔씨, 스마게, 데브 }; //기자 이름들
    public enum Perks { 제목학원, 다작왕, 필력 }; //특성들

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
    public List<Citizen> citizens = new List<Citizen>(); //사회 전체 시민 목록
    public int[] citizens_knowledge = new int[4]; //인지도에 따른 시민 수

    public Society()
    {
        for (int i = 0; i < 4; i++)
        {
            citizens_knowledge[i] = 0;
        }
    }

    public void AddCitizenToList(Citizen person) //Citizen을 리스트에 추가하는 함수
    {
        citizens.Add(person);
        if (person.knowledge == 0)
        {
            citizens_knowledge[0]++;
        }
        else if (person.knowledge == 1)
        {
            citizens_knowledge[1]++;
        }
        else if (person.knowledge == 2)
        {
            citizens_knowledge[2]++;
        }
        else if (person.knowledge == 3)
        {
            citizens_knowledge[3]++;
        }
    }
}

public class Company
{
    public List<Reporter> reporters = new List<Reporter>(); //기자 목록
    public List<Article> articles = new List<Article>(); //현재 가지고 있는 기사 목록
    public int index = 0; //기자를 구분하기 위한 인덱스
    public int money; //돈. 기자 1인당 하루에 1 지출
    float credibility; //신뢰도

    public Company(int m, float c)
    {
        money = m;
        credibility = c;
    }

    public void AddReporterToList(Reporter reporter) //Reporter를 리스트에 추가하는 함수
    {
        reporters.Add(reporter);
    }

    public void RemoveReporterToList(Reporter reporter)
    {
        reporters.Remove(reporter);
    }

    public void AddArticleToList(Article article) //Article을 리스트에 추가하는 함수
    {
        articles.Add(article);
    }
}

public class Human //사람 성향 --> 생성할 때 모든 float 값에 0~1 사이의 무작위 값을 부여 --> 일단 소수점 넷째 자리에서 반올림하게 만듬
{
    public float econStance; //경제적 입장(0 : 극보수, 1: 극진보)
    public float socialStance; //사회적 입장(0 : 극보수, 1: 극진보)
    public Dictionary<Setting.Fields, float> interests; //각 관심사에 대한 관심도

    public Human()
    {
        econStance = Mathf.Floor(Random.Range(0.0f, 1.0f) * 10000) / 10000;
        socialStance = Mathf.Floor(Random.Range(0.0f, 1.0f) * 10000) / 10000;
        interests = new Dictionary<Setting.Fields, float>();
        foreach (Setting.Fields field in System.Enum.GetValues(typeof(Setting.Fields)))
        {
            interests.Add(field, Mathf.Floor(Random.Range(0.0f, 1.0f) * 10000) / 10000);
        }
    }
}

public class Citizen : Human
{
    public int knowledge; //인지도(0~3)
    public float approval; //지지도(0~1)

    public Citizen()
    {
        float temp_percent = Mathf.Floor(Random.Range(0.0f, 1.0f) * 10000) / 10000;

        for (int i = 0; i < 4; i++)
        {
            knowledge = i;
            
            if (temp_percent > Mathf.Pow(0.1f, i+1))
            {
                break;
            }
        }

        if (knowledge <= 1) //인지도가 1단계 이하라면 지지도 0.5
        {
            approval = 0.5f;
        }
        else if (knowledge == 2) //인지도가 2단계라면 지지도 0.2 ~ 0.8 사이
        {
            approval = Mathf.Floor(Random.Range(0.2f, 0.8f) * 10000) / 10000;
        }
        else //인지도가 3단계라면 지지도 0 ~ 1 사이
        {
            approval = Mathf.Floor(Random.Range(0.0f, 1.0f) * 10000) / 10000;
        }
    }
}

public class Reporter : Human
{
    public int reporter_index; //기자 인덱스
    public string name; //이름
    public int satisfaction = 100; //만족도 (0이 되면 퇴사)
    List<string> perks = new List<string>(); //기자에게 붙은 특성들

    public Reporter(Setting setting, Company company)
    {
        reporter_index = company.index;
        name = System.Enum.GetName(typeof(Setting.Names),Random.Range(0, System.Enum.GetValues(typeof(Setting.Names)).Length));
        float rand_value = Mathf.Floor(Random.Range(0.0f, 1.0f) * 10000) / 10000;
        if (rand_value <= setting.startingPerkChance)
        {
            string temp_perk = System.Enum.GetName(typeof(Setting.Perks), Random.Range(0, System.Enum.GetValues(typeof(Setting.Perks)).Length));
            AddPerkToList(temp_perk);
        }
    }

    public void AddPerkToList(string perk) //특성을 리스트에 추가하는 함수
    {
        perks.Add(perk);
    }

    public void WriteArticle(Society society, Company company, Reporter reporter) //기사 쓰기
    {
        float temp = 0; //랜덤값의 최대값 제한을 위해 넣음
        float sum = 0; //랜덤값에서 관심사 정보를 뽑아내기 위해 넣음
        string temp_field = ""; //관심사 정보를 저장할 변수

        foreach (Setting.Fields field in System.Enum.GetValues(typeof(Setting.Fields)))
        {
            temp += interests[field];
        }

        float rand_value = Mathf.Floor(Random.Range(0.0f, temp) * 10000) / 10000;

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

        Article article = new Article(temp_field, temp_virality, society.day, reporter);
        company.AddArticleToList(article);
    }
}

public class Article
{
    public string article_field; //어떤 관심사의 기사인가?
    public int write_reporter_index; //어떤 기자에 의해 작성된 기사인가?
    public int virality; //파급력
    public int date; //생성된 날짜
    public float minRange = 0f; //최소 수용 영역
    public float maxRange = 1f; //최대 수용 영역

    public Article(string af, int v, int d, Reporter reporter)
    {
        article_field = af;
        write_reporter_index = reporter.reporter_index;
        virality = v;
        date = d;
        if (article_field == "Economy") //경제 기사라면
        {
            float temp1 = Mathf.Floor(Random.Range(reporter.econStance - 0.3f, reporter.econStance + 0.3f) * 10000) / 10000;
            float temp2 = Mathf.Floor(Random.Range(reporter.econStance - 0.3f, reporter.econStance + 0.3f) * 10000) / 10000;
            if (temp1 >= temp2)
            {
                minRange = temp2;
                maxRange = temp1;
            }
            else
            {
                minRange = temp1;
                maxRange = temp2;
            }
        }
        else if (article_field == "Social") //사회 기사라면
        {
            float temp1 = Mathf.Floor(Random.Range(reporter.socialStance - 0.3f, reporter.socialStance + 0.3f) * 10000) / 10000;
            float temp2 = Mathf.Floor(Random.Range(reporter.socialStance - 0.3f, reporter.socialStance + 0.3f) * 10000) / 10000;
            if (temp1 >= temp2)
            {
                minRange = temp2;
                maxRange = temp1;
            }
            else
            {
                minRange = temp1;
                maxRange = temp2;
            }
        }
    }
}

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    public GameObject news;

    [HideInInspector] public int point; //남은 포인트
    [HideInInspector] public bool in_menu = false; //메뉴가 켜져있는가?
    [HideInInspector] public Setting starting;
    [HideInInspector] public Society start_society;
    [HideInInspector] public Company myCompany;
    [HideInInspector] public List<Drag> papers;
    [HideInInspector] public GameObject MC; //메인 카메라 오브젝트

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

        MC = GameObject.Find("Main Camera");

        papers = new List<Drag>();

        starting = new Setting(4, 20, 100); //시작 조건
        point = starting.newsPoint;
        start_society = new Society(); //사회 구축
        myCompany = new Company(starting.startingMoney, 50f); //우리 회사 생성

        for (int i = 0; i < 1000; i++) //시작할 때 1,000명의 시민 생성
        {
            Citizen who = new Citizen();
            start_society.AddCitizenToList(who);          
        }
        
        for (int i = 0; i < starting.startingReporters; i++) //startingReporters만큼의 우리 회사의 기자 생성
        {
            Reporter newReporter = new Reporter(starting, myCompany);
            myCompany.AddReporterToList(newReporter);
            myCompany.index++;
            /*Debug.Log("리포터 " + i + " 의 이름 : " + myCompany.reporters[i].name);
            Debug.Log("리포터 " + i + " 의 경제적 입장 : " + myCompany.reporters[i].econStance);
            foreach (Setting.Fields field in System.Enum.GetValues(typeof(Setting.Fields)))
            {
                Debug.Log("리포터 " + i + " 의 " + field + " 관심사 : " + myCompany.reporters[i].interests[field]);
            }*/
        }
        /*Debug.Log("총 사람 수 : " + start_society.citizens.Count);
        Debug.Log("회사 기자 수 : " + myCompany.reporters.Count);*/

        //Debug.Log는 랜덤하게 생성되었는지 확인하기 위해 넣어둠.

        WriteArticles(start_society, myCompany);

        /*for (int i = 0; i<  myCompany.articles.Count; i++) //Debug.Log는 랜덤하게 생성되었는지 확인하기 위해 넣어둠.
        {
            Debug.Log(myCompany.articles[i].article_field);
            Debug.Log(myCompany.articles[i].virality);
            Debug.Log(myCompany.articles[i].date);
        }*/
    }

    public void WriteArticles(Society society,Company company) //기자들이 각자 기사를 쓰는 함수
    {
        for (int i = 0; i < company.reporters.Count; i++)
        {
            company.reporters[i].WriteArticle(society, company, company.reporters[i]);
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

    public void AfterDay() //신문 발행 후 하루가 지남
    {
        for (int i = 0; i < papers.Count; i++)
        {
            if (papers[i].eachPoint > 0) //기사에 0보다 높은 포인트를 배정했다면
            {
                for (int j = 0; j < start_society.citizens.Count; j++) //모든 시민들에 대해
                {
                    if ((start_society.citizens[j].knowledge > 0) && (start_society.citizens[j].interests[(Setting.Fields)System.Enum.Parse(typeof(Setting.Fields), papers[i].eachField)] >= (10-papers[i].eachVirality)*0.1f)) //인지도 > 0 && 해당 기사의 분야에 대한 관심도가 (10-파급력)*0.1보다 높으면
                    {
                        myCompany.money += start_society.citizens[j].knowledge; //인지도만큼 돈을 올려라.
                    }
                    int sum = 0; //사회 전체에서 자기보다 인지도가 높은 시민의 수
                    for (int k = 3; k >= 0; k--)
                    {
                        if (k == start_society.citizens[j].knowledge)
                        {
                            break;
                        }
                        sum += start_society.citizens_knowledge[k];
                    }
                    float rand_value = Random.Range(0.0f, 1.0f);
                    if (rand_value <= (Mathf.Pow(2, papers[i].eachVirality)*((float)sum/start_society.citizens.Count)*Mathf.Pow(3, (float)papers[i].eachPoint/starting.newsPoint))/100) //(2 ^ 파급력) * (사회 전체에서 자기보다 인지도가 높은 시민의 비율) * 3^(지면 갯수/totalPaper)%의 확률
                    {
                        start_society.citizens_knowledge[start_society.citizens[j].knowledge]--;
                        start_society.citizens[j].knowledge++;
                        start_society.citizens_knowledge[start_society.citizens[j].knowledge]++; //인지도 상승
                    }
                }
            }
        }
        myCompany.articles.Clear();
        for (int i = 0; i < papers.Count; i++)
        {
            Destroy(papers[i].gameObject);
        }
        papers.Clear();  //하루가 지났으니 전 기사는 지움
        start_society.day++; //하루가 지남
        myCompany.money -= myCompany.reporters.Count; //기자 수만큼 돈 나감
        for (int i = 0; i < papers.Count; i++) //기사가 선정되지 않으면 만족도 감소
        {
            if (papers[i].eachPoint == 0)
            {
                for (int j = 0; j < myCompany.reporters.Count; j++)
                {
                    if (papers[i].eachIndex == myCompany.reporters[j].reporter_index)
                    {
                        myCompany.reporters[j].satisfaction -= 5;
                        if (myCompany.reporters[j].satisfaction <= 0) //퇴사...
                        {
                            myCompany.RemoveReporterToList(myCompany.reporters[j]);
                        }
                    }
                }
            }
        }
        MC.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text = start_society.day.ToString();
        MC.transform.GetChild(0).GetChild(0).GetChild(3).GetComponent<Text>().text = myCompany.money.ToString();
        MC.transform.GetChild(0).GetChild(0).GetChild(5).GetComponent<Text>().text = myCompany.reporters.Count.ToString();
        point = starting.newsPoint; //포인트 리셋
        WriteArticles(start_society, myCompany); //다시 기사 씀
    }
}