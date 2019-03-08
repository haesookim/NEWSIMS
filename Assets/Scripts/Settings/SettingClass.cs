using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting
{
    public int startingReporters = 6 ; //시작할 때, 기자의 수
    public int startingMoney = 400; //시작할 때, 시작 금액
    //public int newsPoint= 100; //신문에 기사를 배정할 수 있는 포인트


    public float startingPerkChance = 0.5f; //시작할 때 퍽을 가질 수 있는 확률
    public float fakePossibility = 0.3f/*0.5f*/; //기사의 검증되지 않은 부분이 실제로 가짜일 확률 //수정 : 개빡쳐서 수정함


    public enum Fields { 정치사회, 경제, 연예스포츠, 일반 }; //관심사 종류들
    public enum Names { 저널, 리즘, 엔젤, 파인, 미디어, 테크, 놀로지, GDC, 사이드, 가너, 가데니아, 가델, 가르시아, 가먼, 가스, 가자라, 가펑클, 가필드, 갈린, 강, 넬슨, 나이트, 네이버스, 노리스, 던, 더루이즈, 데이비스, 딜런, 라이블리, 램버트, 램지, 레이시, 레인, 로드먼, 로메로, 로이드, 마이너, 마이트, 마티네즈, 마이어, 머피, 미호크, 밀러, 버기, 베넷, 사도스키, 사반트, 사이토, 섀넌, 소프, 엄복동, 스미스, 스트리머, 아널드, 아이작, 클로바, 오블리비언, 단테, 주니어, 윈터, 윌슨 }; //기자 이름들
    public enum Perks { 제목학원, 천재, 철저함, 학습, 다작, 멀티태스킹 }; //특성들

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

    public float CalculateAverageApproval()
    {
        float socialAverageApproval = 0f;

        for (int i = 0; i < citizens.Count; i++)
        {
            socialAverageApproval += citizens[i].approval;
        }

        socialAverageApproval = Mathf.Floor((socialAverageApproval / citizens.Count) * 10000) / 10000;

        return socialAverageApproval;
    }
}

public class Company
{
    public List<Reporter> reporters = new List<Reporter>(); //기자 목록
    public List<EmReporter> em_reporters = new List<EmReporter>(); //고용 가능 기자 목록
    
    public List<Article> articles = new List<Article>(); //현재 가지고 있는 기사 목록

    public List<Article> articlesArchive = new List<Article>(); // 지금까지 써낸 기사 리스트 (기사 백업이 팔요한가..?)

    public int index = 0; //기자를 구분하기 위한 인덱스
    public int money; //돈. 기자 1인당 하루에 (기자 레벨 + salary) 지출 <<이부분 
    public int salary = 5;
    public int totalCirculation;
    public int circulation = 0; //발행부수

    public float obo = 0; //오보 웨이트 합
    public float gisa = 0; //발행한 모든 기사 웨이트 합
    public float fakeRate; //오보율

    public Dictionary<Setting.Fields, int> fieldRate; //각 관심사에 대한 기사배정 수

    public Company(int _money, float _fakeRate)
    {
        money = _money;
        fakeRate = _fakeRate;
        fieldRate = new Dictionary<Setting.Fields, int>();
        foreach (Setting.Fields field in System.Enum.GetValues(typeof(Setting.Fields)))
        {
            fieldRate.Add(field, 0);
        }
        EventManager.DayEvent_Beginning += Beginning;
    }

    void Beginning(Society society, Company company)
    {
        totalCirculation += circulation;

        circulation = 0;  //누적 발행부수에 당일 발행부수를 더해주고, 당일 발행부수 초기화

        if(GameManager.Instance.society.day == 1) return; //첫날은 월급 안줌
        foreach (Reporter reporter in company.reporters)
        {
            money -= (reporter.level+1) * salary;
            Debug.Log(reporter.name + " 의 월급으로 " + (reporter.level+1) * salary + " 을 지출합니다");
        }

        if (gisa != 0)
        {
            fakeRate = obo / gisa;
        }
    }

    public void AddReporterToList(Reporter _reporter) //Reporter를 리스트에 추가하는 함수
    {
        reporters.Add(_reporter);
    }

    public void RemoveReporterToList(Reporter _reporter) //Reporter를 리스트에서 지우는 함수 <-- 퇴사했을 때 사용
    {
        reporters.Remove(_reporter);
    }

    public void AddArticleToList(Article _article) //Article을 리스트에 추가하는 함수
    {
        articles.Add(_article);
    }

}

[System.Serializable]
public class Article
{
    public string article_field; //어떤 관심사의 기사인가?
    public string article_name; //어떤 제목의 기사인가?
    public string write_reporter_name; //작성한 기자의 이름
    public int write_reporter_index; //어떤 기자에 의해 작성된 기사인가?
    public int virality; //파급력
    public int date; //생성된 날짜
    public float vertification; //검증도
    public float centerStance = 0f; //기사의 입장
    public float tolerance = 0f; //Center로부터 얼마나 떨어진 사람까지 우호적으로 받아들이는가?


    public bool advance = false; //심화 취재의 대상인가?
    public bool up_virality = false; //심화 취재를 통한 파급력 증가 여부

    public Article(string _field, int _virality, int _date, float _vertification, Reporter reporter)
    {
        article_field = _field;
        article_name = "";
        write_reporter_index = reporter.reporter_index;
        write_reporter_name = reporter.name;
        virality = _virality;
        date = _date;
        vertification = _vertification;

        if (article_field == "경제") //경제 기사라면
        {
            centerStance = Mathf.Floor(Random.Range(reporter.econStance - 0.1f, reporter.econStance + 0.1f) * 10000) / 10000;
            tolerance = 0.1f + (reporter.logic * 0.01f);
        }
        else if (article_field == "정치사회") //사회/정치 기사라면
        {
            centerStance = Mathf.Floor(Random.Range(reporter.socialStance - 0.1f, reporter.socialStance + 0.1f) * 10000) / 10000;
            tolerance = 0.1f + (reporter.logic * 0.01f);
        }

        string ver = "";
        if (vertification < 0.35)
        {
            ver = "하";
        } else if (vertification >= 0.65)
        {
            ver = "상";
        } else
        {
            ver = "중";
        }

        while (true)
        {
            int random_index = Random.Range(0,GameManager.Instance.data.Count);
            if (article_field == GameManager.Instance.data[random_index]["분야"].ToString())
            {
                for (int i = 0; i < GameManager.Instance.originName.Count; i++)
                {
                    if (GameManager.Instance.data[random_index]["제목"].ToString() == GameManager.Instance.originName[i])
                    {
                        if (centerStance >= float.Parse(GameManager.Instance.data[random_index]["범위1"].ToString()) && centerStance <= float.Parse(GameManager.Instance.data[random_index]["범위2"].ToString()))
                        {
                            if (ver == GameManager.Instance.data[random_index]["검증도"].ToString())
                            {
                                article_name = GameManager.Instance.originName[i];
                                GameManager.Instance.originName.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
                break;
            }
        }
    }
}

