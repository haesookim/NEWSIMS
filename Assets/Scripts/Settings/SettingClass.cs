using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting
{
    public int startingReporters = 6 ; //시작할 때, 기자의 수
    public int startingMoney = 20; //시작할 때, 시작 금액
    public int newsPoint= 100; //신문에 기사를 배정할 수 있는 포인트


    public float startingPerkChance = 0.5f; //시작할 때 퍽을 가질 수 있는 확률
    public float fakePossibility = 0.5f; //기사의 검증되지 않은 부분이 실제로 가짜일 확률 


    public enum Fields { 연예, 사회, 스포츠, 경제, 정치, 생활, 사건사고 }; //관심사 종류들
    public enum Names { 넥슨, 넷마블, 엔씨, 스마게, 데브 }; //기자 이름들
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
}

public class Company
{
    public List<Reporter> reporters = new List<Reporter>(); //기자 목록
    public List<EmReporter> em_reporters = new List<EmReporter>(); //고용 가능 기자 목록
    
    public List<Article> articles = new List<Article>(); //현재 가지고 있는 기사 목록

    public List<Article> articlesArchive = new List<Article>(); // 지금까지 써낸 기사 리스트 (기사 백업이 팔요한가..?)

    public int index = 0; //기자를 구분하기 위한 인덱스
    public int money; //돈. 기자 1인당 하루에 (기자 레벨 + 1) 지출
    public int circulation = 0; //발행부수
    float credibility; //신뢰도

    public Company(int _money, float _credibility)
    {
        money = _money;
        credibility = _credibility;
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
        List<Dictionary<string, object>> data = CSVReader.Read("PaperName"); //CSV를 불러옴

        article_field = _field;
        write_reporter_index = reporter.reporter_index;
        int temp_j = 0;

       while (true) //끝이 나올 때까지 인덱스를 증가(주의 : CSV에서 첫째 줄이 가장 긴 친구가 아니면 에러남.)
        {
            if (data[temp_j][article_field].ToString() == "끝")
            {
                break;
            }
            temp_j++;
        }

        int temp_name_index = Random.Range(0, temp_j);
        article_name = data[temp_name_index][article_field].ToString();
        virality = _virality;
        date = _date;
        vertification = _vertification;


        if (article_field == "경제") //경제 기사라면
        {
            centerStance = Mathf.Floor(Random.Range(reporter.econStance - 0.1f, reporter.econStance + 0.1f) * 10000) / 10000;
            tolerance = 0.1f + (reporter.logic * 0.01f);
        }
        else if (article_field == "사회") //사회 기사라면
        {
            centerStance = Mathf.Floor(Random.Range(reporter.socialStance - 0.1f, reporter.socialStance + 0.1f) * 10000) / 10000;
            tolerance = 0.1f + (reporter.logic * 0.01f);
        }
    }
}

