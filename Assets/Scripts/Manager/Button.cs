﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button : MonoBehaviour
{
    public enum function
    {
        DisplayDesk,
        DisplayOffice,
        DisappearPaperMenu,
        DividePoint,
        SetDeepenArticle

    }

    [Header("클릭 시 실행될 함수")]
    public function functionName;

    void Start()
    {
        EventTrigger eventTrigger = GetComponent<EventTrigger>();
        EventTrigger.Entry entry_PointerDown = new EventTrigger.Entry();
        entry_PointerDown.eventID = EventTriggerType.PointerDown;
        entry_PointerDown.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
        eventTrigger.triggers.Add(entry_PointerDown);
    }

    void OnPointerDown(PointerEventData data)
    {
        SelectFunction(); 
    }

    void SelectFunction()
    {
        switch(functionName)
        {
            case function.DisplayDesk :
                DisplayDesk();
                break;
            
            case function.DisplayOffice :
                DisplayOffice();
                break;

            case function.DisappearPaperMenu :
                DisappearPaperMenu();
                break;

            case function.DividePoint :
                DividePoint();
                break;

            case function.SetDeepenArticle :
                SetDeepenArticle();
                break;

        }
    }

    public void DisplayDesk() //책상 화면으로
   {
       if(!GameManager.Instance.in_DeskMenu)
       {
           GameManager.Instance.deskWindow.SetActive(true);
           GameManager.Instance.officeWindow.SetActive(false);
           GameManager.Instance.in_DeskMenu = true;
       }
   }

   public void DisplayOffice() // 메인 화면으로
   {
       if(GameManager.Instance.in_DeskMenu)
       {
           GameManager.Instance.officeWindow.SetActive(true);
           GameManager.Instance.deskWindow.SetActive(false);
           GameManager.Instance.in_DeskMenu = false;
       }
   }

   public void DisappearPaperMenu() //상세 기사 화면 없애기
   {
        GameManager.Instance.in_PaperMenu = false;
        GameManager.Instance.paperWindow.SetActive(false);
        GameManager.Instance.paperWindow.transform.GetChild(1).gameObject.SetActive(false);
        GameManager.Instance.paperWindow.transform.GetChild(2).gameObject.SetActive(false);
   }

    public void DividePoint() //확인 버튼 클릭시 포인트 분배 처리
    {
        GameManager.Instance.selectedPaper.article.assignedPoint = GameManager.Instance.temp_point;
        GameManager.Instance.point -= GameManager.Instance.temp_point;
        GameManager.Instance.temp_point = 0;
        DisappearPaperMenu();
    }
    
    public void SetDeepenArticle() //심화취재 버튼 클릭시
    {
        GameManager.Instance.selectedPaper.article.advance = true;
        GameManager.Instance.temp_point = 0;
        DisappearPaperMenu();
    }
}
