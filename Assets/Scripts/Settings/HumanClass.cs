﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        EventManager.Instance.DayEvent_End += EndofDay;
    }

    void EndofDay(Society society, Company company)
    {
        if(knowledge <= 0 ) return;
        
       //신문을 사는 식
        foreach(AssignedPaperDrag assigned in NewsPaper.Instance.ArticlesInAssignedPaper)
        {
            Article article= assigned.article;
            int size = assigned.size.x * assigned.size.y;
            if(interests[(Setting.Fields)System.Enum.Parse(typeof(Setting.Fields), article.article_field)] 
            >= (10-article.virality)*0.003f / (approval * ((float)size/4*5)))
            {
                int moneyBonus = 1;
                if(approval > 0.9f) moneyBonus = 3;
                company.money += knowledge * moneyBonus;
                company.circulation++;

                float difference;
                float socialAverageApproval = society.CalculateAverageApproval();
                switch(article.article_field)
                {
                    case "정치사회":
                        difference = Mathf.Abs(socialStance - article.centerStance);
                        if(article.tolerance >= difference)
                        {
                            approval = 1-((1-approval)*(1*0.1f*article.tolerance*socialAverageApproval/difference));
                            socialStance += (article.centerStance - socialStance) / 10;
                        }
                        else approval *= 1-(0.1f*difference);
                    break;

                    case "경제":
                        difference = Mathf.Abs(econStance - article.centerStance);
                        if(article.tolerance >= difference)
                        {
                            approval = 1-((1-approval)*(1*0.1f*article.tolerance*socialAverageApproval/difference));
                            econStance += (article.centerStance - econStance) / 10;
                        }
                        else approval *= 1-(0.1f*difference);
                    break;
                }

                 int sum = 0; //사회 전체에서 자기보다 인지도가 높은 시민의 수
                for (int k = 3; k >= 0; k--)
                {
                    if (k == knowledge)
                    {
                        break;
                    }
                    sum += society.citizens_knowledge[k];
                }
                float rand_value = Random.Range(0.0f, 1.0f);
                if (rand_value <= (Mathf.Pow(2, article.virality)*((float)sum/society.citizens.Count)*Mathf.Pow(3, (float)size/4*5))/100) //(2 ^ 파급력) * (사회 전체에서 자기보다 인지도가 높은 시민의 비율) * 3^(지면 갯수/totalPaper)%의 확률
                {
                    society.citizens_knowledge[knowledge]--;
                    knowledge++;
                    society.citizens_knowledge[knowledge]++; //인지도 상승
                }

                break;
            }
        }
    }
}

[System.Serializable]
public class Reporter : Human
{
    public Sprite reporterImage; //기자 얼굴
    public int reporter_index; //기자 인덱스
    public string name; //이름
    public int writing; //필력, 시작할 때 0~20 사이의 무작위 값
    public int logic; //논리력, 시작할 때 0~20 사이의 무작위 값
    public int survey; //조사 능력, 시작할 때 0~20 사이의 무작위 값
    public int level; //레벨
    public int EXP; //경험치. (2^level)*totalPaper만큼의 경험치를 쌓으면 레벨업
    public int exp{ //경험치가 쌓이면 레벨이 오르는 속성
        get{ return EXP; }
        set{
            EXP += value;
            if(EXP >= Mathf.Pow(2,level)* 4*5) 
            {
               int overExp = EXP - (int)Mathf.Pow(2,level)* 4*5;
                LevelUp();
                EXP = 0 ;
                EXP += overExp;
            }
        }
    } 
    public int SATISFACTION = 100; 
    public int satisfaction //만족도가 0보다 내려가면 퇴사
    {
        //만족도 (0이 되면 퇴사)
        get { return SATISFACTION; }
        set{
            SATISFACTION += value;
            if (SATISFACTION <= 0)
            {
                for (int j = 0; j < ReporterManager.Instance.vrs.Count; j++)
                {
                    if (ReporterManager.Instance.vrs[j].reporter.reporter_index == reporter_index)
                    {
                        GameObject.Destroy(ReporterManager.Instance.vrs[j].gameObject);
                        ReporterManager.Instance.RemoveVrsToList(ReporterManager.Instance.vrs[j]);
                        break;
                    }
                }
                EventManager.Instance.DayEvent_Beginning -= WriteArticle; //기사쓰는 이벤트를 지우고
                Debug.Log(name + "이/가 퇴사했습니다.");
                GameManager.Instance.AddReportText(name + "이/가 퇴사했습니다.");

                GameManager.Instance.company.RemoveReporterToList(this); //리스트에서 삭제해라
                            

                for (int i = 0; i < ReporterManager.Instance.vrs.Count; i++)
                {
                    ReporterManager.Instance.vrs[i].UpdateStatus();
                }
            }
        }
    }

    public bool advance_news; //심화 취재 체크 여부
    public bool is_fired; //해고되었는지 여부

    //public advancedNews adn; //심화 취재 정보 저장용
    public Article adn; //심화 취재 정보 저장용
    public List<string> perks = new List<string>(); //기자에게 붙은 특성들

    public class advancedNews
    {
        public string adv_field = ""; //관심 분야
        public string adv_name = ""; //제목
        public int adv_virality = 0; //파급력
        public float adv_vertification = 0f; //검증도
        public float adv_centerStance = 0f; //기사의 입장
        public float adv_tolerance = 0f; //Center로부터 얼마나 떨어진 사람까지 우호적으로 받아들이는가?
        public bool adv_up_virality = false; //파급력이 올랐는가?
    }

    public Reporter(Setting setting, Company company, int index)
    {
        int randomFace = Random.Range(0, GameManager.Instance.ReporterImages.Length);
        reporterImage = GameManager.Instance.ReporterImages[randomFace];

        level = 0; //레벨 0에서 시작
        EXP = 0; //경험치 0에서 시작

        writing = Random.Range(0, 21);
        logic = Random.Range(0, 21);
        survey = Random.Range(0, 21);

        reporter_index = index;
        advance_news = false;
        is_fired = false;

        //adn = new advancedNews();
        name = System.Enum.GetName(typeof(Setting.Names),Random.Range(0, System.Enum.GetValues(typeof(Setting.Names)).Length));

        float rand_value = Mathf.Floor(Random.Range(0.0f, 1.0f) * 10000) / 10000;
        if (rand_value <= setting.startingPerkChance) //특성 넣기
        {
            string temp_perk = System.Enum.GetName(typeof(Setting.Perks), Random.Range(0, System.Enum.GetValues(typeof(Setting.Perks)).Length));
            AddPerkToList(temp_perk);
        }

        //DayEvent이벤트를 구독 . 이벤트 발생 시 WriteArticle 함수를 호출한다.
        EventManager.Instance.DayEvent_Beginning += WriteArticle; 
        EventManager.Instance.DayEvent_Publication += Publication;

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
    

    public void WriteArticle(Society society, Company company) //기사 쓰기
    {
        if (!advance_news) //심화 취재를 요구하지 않았다면
        {
            Article article;
            do
            {
                article = CreatArticle(company, society);
            } while (article.article_name == "");
            company.AddArticleToList(article);
        }
        else //심화 취재를 요구했다면
        {
            Article article = adn;

            float new_vertification = (50 + survey - (article.virality * 5) + (Mathf.Floor(Random.Range(0.0f, 20.0f) * 100) / 100)) / 100f; //검증도 정보를 저장할 변수

            float temp_rand_value = Random.Range(0.0f, 1.0f);

            if (temp_rand_value > new_vertification * GameManager.Instance.setting.fakePossibility)
            {
                if (!article.up_virality) //파급력 상승
                {
                    float rand_value = Mathf.Floor(Random.Range(0.0f, 1.0f) * 10000) / 10000;
                    if (rand_value <= interests[(Setting.Fields)System.Enum.Parse(typeof(Setting.Fields), article.article_field)])

                    {
                        article.virality++;
                        article.up_virality = true;

                        Debug.Log("심화취재에 성공하여 기사의 파급력이 증가합니다!");
                        GameManager.Instance.AddReportText("심화취재에 성공하여 기사의 파급력이 증가합니다!");
                    }
                }
                article.vertification = 1 - ((1 - article.vertification) * (1 - new_vertification));
                company.AddArticleToList(article);
            }
            else //강화 실패
                {
                    Debug.Log("심화취재를 했지만 성과가 없습니다.");
                    GameManager.Instance.AddReportText("심화취재를 했지만 성과가 없습니다.");
                }
            
            advance_news = false; //심화 취재 여부 체크 해제

            for (int i = 0; i < perks.Count; i++) //퍽 중에 멀티태스킹이 있다면 심화취재를 보내도 다음날 기사를 생성한다.
            {
                if (perks[i] == "멀티태스킹")
                {
                    Debug.Log(reporter_index+" 가 멀티태스킹하여 오늘도 기사를 씁니다!");
                    GameManager.Instance.AddReportText(name+" 이/가 멀티태스킹하여 오늘도 기사를 씁니다!");

                    Article article2;
                    do
                    {
                        article2 = CreatArticle(company, society);
                    } while (article2.article_name == "");
                    company.AddArticleToList(article2);
                    break;
                }
            }
        }

        for (int i = 0; i < perks.Count; i++) //퍽 중에 다작이 있다면 20%확률로 기사를 하나 더 쓴다.
        {
            if (perks[i] == "다작")
            {
                float rand_value_perk = Random.Range(0.0f, 1.0f);
                if (rand_value_perk > 0.8f)
                {
                    Debug.Log(reporter_index+" 가 기사를 하나 더 씁니다!");
                    GameManager.Instance.AddReportText(name + " 이/가 기사를 하나 더 씁니다!");

                    Article article;
                    do
                    {
                        article = CreatArticle(company, society);
                    } while (article.article_name == "");
                    company.AddArticleToList(article);
                }
                break;
            }
        }
    }

    Article CreatArticle(Company company, Society society)
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
        if (temp_rand_value <= this.writing * 0.01f)

        {
            temp_virality += 1;
        }

        float temp_vertification = (50 + this.survey - (temp_virality * 5) + (Mathf.Floor(Random.Range(0.0f, 20.0f) * 100) / 100)) / 100f; //검증도 정보를 저장할 변수

        Article article = new Article(temp_field, temp_virality, society.day, temp_vertification, this);

        return article;
    }

    public void Publication(Society society, Company company)
    {
        //자기 기사가 있으면 해당 지분만큼 경험치 상승. 없으면 만족도 -5
        
        int getExp =0;
        foreach(AssignedPaperDrag assigned in NewsPaper.Instance.ArticlesInAssignedPaper)
        {
            if(assigned.article.write_reporter_index == reporter_index)
            {
                company.gisa += Mathf.Pow(2, assigned.article.virality - 1);
                int tem = 0;
                switch (assigned.article.article_field)
                {
                    case "정치사회":
                        tem = 0;
                        break;
                    case "경제":
                        tem = 1;
                        break;
                    case "연예스포츠":
                        tem = 2;
                        break;
                    case "일반":
                        tem = 3;
                        break;
                }

                int size = assigned.size.x * assigned.size.y;
                company.fieldRate[(Setting.Fields)tem] += size;

                float temp_rand_value = Mathf.Floor(Random.Range(0.0f, 1.0f) * 10000) / 10000;
                //오보가 난 경우
                if (temp_rand_value < (1 - assigned.article.vertification)* GameManager.Instance.setting.fakePossibility) 
                {
                    company.money -= (int)(company.circulation * /*3 **/ ((float)size / 4*5)); //수정 : 돈 개많이 깨져서 수정함
                    satisfaction = -20;
                    Debug.Log(name + "이/가 오보를 냈습니다!");
                    GameManager.Instance.AddReportText(name + "이/가 오보를 냈습니다!");
                    GameManager.Instance.AddReportText("배상금을 " + ((int)(company.circulation * /*3 **/ ((float)size / 4*5))).ToString() + " 만큼 지불합니다." ); //수정 : 돈 개많이 깨져서 수정함

                    company.obo += Mathf.Pow(2, assigned.article.virality-1);
                }
                else //오보가 아니면 경험치를 제대로 얻음.
                {
                    getExp += size;
                }
            }
        }
        if(getExp == 0) satisfaction = -5;
        else
        {
            exp += getExp;
            if(perks.Contains("학습"))
            {
                exp += getExp;
                 Debug.Log(name + "이/가 학습하여 경험치를 더 얻습니다.");
                    GameManager.Instance.AddReportText(name +"이/가 학습하여 경험치를 더 얻습니다.");
            }
        }

    }
}

//리포터를 그냥 쓰면 이벤트매니저에서 뭐라고 해서 고용 가능 기자를 따로 만듬
[System.Serializable] 
public class EmReporter : Human
{
    public Sprite reporterImage; //기자 얼굴
    public int reporter_index; //기자 인덱스
    public string name; //이름
    public int writing; //필력, 시작할 때 0~20 사이의 무작위 값
    public int logic; //논리력, 시작할 때 0~20 사이의 무작위 값
    public int survey; //조사 능력, 시작할 때 0~20 사이의 무작위 값
    public int level; //레벨

    public bool is_employed; //고용되었는가?
    public int buyout; //고용값

    public List<string> perks = new List<string>(); //기자에게 붙은 특성들

    public EmReporter(Setting setting, Company company, int index)
    {
        int randomFace = Random.Range(0, GameManager.Instance.ReporterImages.Length);
        reporterImage = GameManager.Instance.ReporterImages[randomFace];

        level = 0; //레벨 0에서 시작

        writing = Random.Range(0, 21);
        logic = Random.Range(0, 21);
        survey = Random.Range(0, 21);

        reporter_index = index;
        is_employed = false;

        name = System.Enum.GetName(typeof(Setting.Names), Random.Range(0, System.Enum.GetValues(typeof(Setting.Names)).Length));

        float rand_value = Mathf.Floor(Random.Range(0.0f, 1.0f) * 10000) / 10000;
        if (rand_value <= setting.startingPerkChance) //특성 넣기
        {
            string temp_perk = System.Enum.GetName(typeof(Setting.Perks), Random.Range(0, System.Enum.GetValues(typeof(Setting.Perks)).Length));
            AddPerkToList(temp_perk);
        }

        while (true)
        {
            float temp_levelUp = Random.Range(0.0f, 1.0f);
            if (temp_levelUp >= 0.05)
            {
                break;
            }
            LevelUp();
        }
        buyout = (writing + logic + survey) * 2;
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
}