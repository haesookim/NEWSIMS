using System.Collections;
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
    public enum Perks { 제목학원, 천재, 철저함, 학습, 다작 }; //특성들

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
    public int[] citizens_knowledge = new int[4]; //인지도별 시민 수

    public Society()
    {
        for (int i = 0; i < 4; i++) //초기화
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
    public int money; //돈. 기자 1인당 하루에 (기자 레벨 + 1) 지출
    public int circulation = 0; //발행부수
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

    public void RemoveReporterToList(Reporter reporter) //Reporter를 리스트에서 지우는 함수 <-- 퇴사했을 때 사용
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

        for (int i = 0; i < 4; i++) //인지도 정하기
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
    public int writing; //필력, 시작할 때 0~20 사이의 무작위 값
    public int logic; //논리력, 시작할 때 0~20 사이의 무작위 값
    public int survey; //조사 능력, 시작할 때 0~20 사이의 무작위 값
    public int level; //레벨
    public int exp; //경험치. (2^level)*totalPaper만큼의 경험치를 쌓으면 레벨업
    public int satisfaction = 100; //만족도 (0이 되면 퇴사)
    public bool advance_news; //심화 취재 체크 여부
    public advancedNews adn; //심화 취재 정보 저장용
    public List<string> perks = new List<string>(); //기자에게 붙은 특성들

    public class advancedNews
    {
        public string adv_field = ""; //관심 분야
        public int adv_virality = 0; //파급력
        public float adv_vertification = 0f; //검증도
        public float adv_minRange = 0f; //최소 수용 영역
        public float adv_maxRange = 1f; //최대 수용 영역
        public bool adv_up_virality = false; //파급력이 올랐는가?
    }

    public Reporter(Setting setting, Company company)
    {
        level = 0; //레벨 0에서 시작
        exp = 0; //경험치 0에서 시작
        writing = Random.Range(0, 21);
        logic = Random.Range(0, 21);
        survey = Random.Range(0, 21);
        reporter_index = company.index;
        advance_news = false;
        adn = new advancedNews();
        name = System.Enum.GetName(typeof(Setting.Names),Random.Range(0, System.Enum.GetValues(typeof(Setting.Names)).Length));
        float rand_value = Mathf.Floor(Random.Range(0.0f, 1.0f) * 10000) / 10000;
        if (rand_value <= setting.startingPerkChance) //특성 넣기
        {
            string temp_perk = System.Enum.GetName(typeof(Setting.Perks), Random.Range(0, System.Enum.GetValues(typeof(Setting.Perks)).Length));
            AddPerkToList(temp_perk);
        }
    }

    public void AddPerkToList(string perk) //특성을 리스트에 추가하는 함수
    {
        perks.Add(perk);
        switch (perk) //넣어졌을 때 바로 발동하는 특성들
        {
            case "제목학원":
                writing += 10;
                break;
            case "천재":
                logic += 10;
                break;
            case "철저함":
                survey += 10;
                break;
            default:
                break;
        }
    }

    public void LevelUp() //레벨업
    {
        writing += Random.Range(0, 6);
        logic += Random.Range(0, 6);
        survey += Random.Range(0, 6); //모든 능력치가 0~5 사이의 무작위 값만큼 상승

        if (perks.Count != System.Enum.GetValues(typeof(Setting.Perks)).Length) //가지고 있는 퍽의 갯수가 전체 퍽의 갯수와 같지 않다면
        {
            float rand_value = Mathf.Floor(Random.Range(0.0f, 1.0f) * 10000) / 10000;
            if (rand_value <= 0.5f) //특성 넣기
            {
                string temp_perk = "";
                int temp_count = 0;
                while (temp_count < perks.Count)
                {
                    temp_count = 0;
                    string temp_temp_perk = System.Enum.GetName(typeof(Setting.Perks), Random.Range(0, System.Enum.GetValues(typeof(Setting.Perks)).Length));
                    for (int i = 0; i < perks.Count; i++)
                    {
                        if (temp_temp_perk == perks[i])
                        {
                            break;
                        }
                        temp_count++;
                    }
                    temp_perk = temp_temp_perk;
                }
                AddPerkToList(temp_perk);
            }
        }
        level++;
    }

    public void WriteArticle(Society society, Company company, Reporter reporter) //기사 쓰기
    {
        if (!reporter.advance_news) //심화 취재를 요구하지 않았다면
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

            float temp_rand_value = Mathf.Floor(Random.Range(0.0f, 1.0f) * 10000) / 10000;
            if (temp_rand_value <= reporter.writing * 0.01)
            {
                temp_virality += 1;
            }

            float temp_vertification = (60 + reporter.survey + (Mathf.Floor(Random.Range(0.0f, 20.0f) * 100) / 100)) / 100f; //검증도 정보를 저장할 변수

            Article article = new Article(temp_field, temp_virality, society.day, temp_vertification, reporter);
            company.AddArticleToList(article);
        }
        else //심화 취재를 요구했다면
        {
            Article article = new Article(reporter.adn.adv_field, reporter.adn.adv_virality, society.day, reporter.adn.adv_vertification, reporter);
            article.minRange = reporter.adn.adv_minRange;
            article.maxRange = reporter.adn.adv_maxRange;
            article.up_virality = reporter.adn.adv_up_virality;

            if (!article.up_virality) //파급력 상승
            {
                float rand_value = Mathf.Floor(Random.Range(0.0f, 1.0f) * 10000) / 10000;

                if (rand_value <= reporter.interests[(Setting.Fields)System.Enum.Parse(typeof(Setting.Fields), article.article_field)])
                {
                    article.virality++;
                    article.up_virality = true;
                }
            }

            float temp_vertification = (60 + reporter.survey + (Mathf.Floor(Random.Range(0.0f, 20.0f) * 100) / 100)) / 100f; //검증도 정보를 저장할 변수
            article.vertification = 1 - ((1 - article.vertification) * (1 - temp_vertification));
            company.AddArticleToList(article);
            reporter.advance_news = false; //심화 취재 여부 체크 해제
        }
    }
}

public class Article
{
    public string article_field; //어떤 관심사의 기사인가?
    public int write_reporter_index; //어떤 기자에 의해 작성된 기사인가?
    public int virality; //파급력
    public int date; //생성된 날짜
    public float vertification; //검증도
    public float minRange = 0f; //최소 수용 영역
    public float maxRange = 1f; //최대 수용 영역
    public bool up_virality = false; //심화 취재를 통한 파급력 증가 여부

    public Article(string af, int v, int d, float ve, Reporter reporter)
    {
        article_field = af;
        write_reporter_index = reporter.reporter_index;
        virality = v;
        date = d;
        vertification = ve;
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

        MC = GameObject.Find("Main Camera"); //메인 카메라 오브젝트 저장

        papers = new List<Drag>();

        starting = new Setting(4, 20, 100); //시작 조건
        point = starting.newsPoint; //뉴스 포인트를 지정
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
        myCompany.reporters[myCompany.reporters.Count - 1].LevelUp(); //마지막 기자에게 즉시 레벨업 한 번

        /*Debug.Log("총 사람 수 : " + start_society.citizens.Count);
        Debug.Log("회사 기자 수 : " + myCompany.reporters.Count);*/

        //Debug.Log는 랜덤하게 생성되었는지 확인하기 위해 넣어둠.

        WriteArticles(start_society, myCompany); //첫 기사 작성

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
                    if ((start_society.citizens[j].knowledge > 0) && (start_society.citizens[j].interests[(Setting.Fields)System.Enum.Parse(typeof(Setting.Fields), papers[i].eachField)] >= (10-papers[i].eachVirality)*0.2f / ((float)papers[i].eachPoint/starting.newsPoint))) //인지도 > 0 && 해당 기사의 분야에 대한 관심도가 (10-파급력)*0.2 / 지면배정비율 보다 높으면
                    {
                        Debug.Log("1");
                        myCompany.money += start_society.citizens[j].knowledge; //인지도만큼 돈을 올려라.
                        myCompany.circulation++; //발행부수를 하나 올려라.
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

        int minus_sum = 0; //오보가 났을 경우, 그만큼 빼줌

        for (int i = 0; i < papers.Count; i++) //기사가 선정되지 않으면 만족도 감소
        {
            if (papers[i].eachPoint == 0 && !papers[i].advanced)
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
                        break;
                    }
                }
            }

            float temp_rand_value = Mathf.Floor(Random.Range(0.0f, 1.0f) * 10000) / 10000;
            if (temp_rand_value < 1 - papers[i].eachVertification) //오보가 난 경우
            {
                minus_sum += papers[i].eachPoint;
                myCompany.money -= (int)(myCompany.circulation * 3 * ((float)papers[i].eachPoint / starting.newsPoint));
                for (int j = 0; j < myCompany.reporters.Count; j++)
                {
                    if (papers[i].eachIndex == myCompany.reporters[j].reporter_index)
                    {
                        myCompany.reporters[j].satisfaction -= 20;
                        if (myCompany.reporters[j].satisfaction <= 0) //퇴사...
                        {
                            myCompany.RemoveReporterToList(myCompany.reporters[j]);
                        }
                        break;
                    }
                }
            }
            else
            {
                if (papers[i].eachPoint > 0) //경험치 획득 및 레벨업
                {
                    for (int j = 0; j < myCompany.reporters.Count; j++)
                    {
                        if (papers[i].eachIndex == myCompany.reporters[j].reporter_index)
                        {
                            myCompany.reporters[j].exp += papers[i].eachPoint;
                            for (int k = 0; k < myCompany.reporters[j].perks.Count; k++)
                            {
                                if (myCompany.reporters[j].perks[k] == "학습") //학습 특성을 가지고 있다면 경험치 한 번 더 얻음
                                {
                                    myCompany.reporters[j].exp += papers[i].eachPoint;
                                }
                                break;
                            }
                            if (myCompany.reporters[j].exp >= starting.newsPoint * Mathf.Pow(2, myCompany.reporters[j].level))
                            {
                                myCompany.reporters[j].exp = myCompany.reporters[j].exp - starting.newsPoint * (int)Mathf.Pow(2, myCompany.reporters[j].level);
                                myCompany.reporters[j].LevelUp();
                            }
                            break;
                        }
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
        for (int i = 0; i < myCompany.reporters.Count; i++)
        {
            myCompany.money -= myCompany.reporters[i].level + 1; //기자 수만큼 돈 나감(레벨 + 1)
        }

        MC.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text = start_society.day.ToString();
        MC.transform.GetChild(0).GetChild(0).GetChild(3).GetComponent<Text>().text = myCompany.money.ToString();
        MC.transform.GetChild(0).GetChild(0).GetChild(5).GetComponent<Text>().text = myCompany.reporters.Count.ToString();
        point = starting.newsPoint - minus_sum; //포인트 리셋
        myCompany.circulation = 0; //발행부수 리셋
        WriteArticles(start_society, myCompany); //다시 기사 씀
    }
}