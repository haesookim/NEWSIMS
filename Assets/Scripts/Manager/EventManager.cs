using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    public delegate void DayHandler(Society society, Company company);
    public static event DayHandler DayEvent_Beginning = delegate{};
    public static event DayHandler DayEvent_End = delegate{};
     public static event DayHandler DayEvent_Publication = delegate{};

    ///<summary>
    ///하루가 시작할 때 호출하는 이벤트.   
    ///날짜 애니메이션이 출력되고 기자가 기사를 써낸다.
    ///</summary>
    ///<param name = "society"> 사회 클래스를 대입 </param>
    public void Do_BeginningofDay(Society society, Company company)
    {
        DayEvent_Beginning(society,company);
    }

    ///<summary>
    ///하루가 끝날 때 호출하는 이벤트.
    ///날짜가 지나고, 시민들은 신문을 읽은 후 행동하며, 오보발생 시의 처리를 한다.
    ///</summary>
    ///<param name = "society"> 사회 클래스를 대입 </param>
    public void Do_EndofDay(Society society, Company company)
    {
        DayEvent_Publication(society,company);
        DayEvent_End(society,company);
    }
}

