using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaperDrag : Drag
{

    protected override void Start() {
        
        base.Start();
        SetInfoText();
    }

    protected override void OnMouseUp() //마우스를 뗐을 때
    {
        inClick = false;
        if (!GameManager.Instance.in_PaperMenu && !GameManager.Instance.in_ReporterMenu)
        {
            timer2 = Time.realtimeSinceStartup;
            
            if(NewsPaper.Instance.inAdvance)
            {
                Paper paper = GetComponent<Paper>();

                gameObject.SetActive(false);
                NewsPaper.Instance.inAdvance = false;

                //심화취재하면 기사정보를 기자의 adn으로 넘김
                foreach (Reporter reporter in GameManager.Instance.company.reporters)
                {
                    if(reporter.reporter_index == paper.article.write_reporter_index)
                    {
                        reporter.adn = paper.article;
                        reporter.advance_news = true;
                    }
                }
                Debug.Log(("심화취재를 지시합니다"));
            }


            if (!NewsPaper.Instance.inPaper) //지면 배정 
                originPosition = transform.position;                
            else
                NewsPaper.Instance.CreatAssignedPaper(gameObject);

            spriteRender.sortingOrder = 2;

            if (timer2 - timer1 < 0.13f) //클릭이라면
            {
                
            }
        }
    }
   
    //지면,심화취재박스 위에 오브젝트를 끌고 있을 때
   void OnTriggerStay2D(Collider2D other) {
        if(other.CompareTag("NewsPaper"))
        {
            NewsPaper.Instance.inPaper = true;
            NewsPaper.Instance.PreviewPaper(transform.position);
        }
        else if(other.CompareTag("AdvanceBox"))
        {
            NewsPaper.Instance.inAdvance = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("NewsPaper"))
        {
            NewsPaper.Instance.inPaper = false;
        }

        else if(other.CompareTag("AdvanceBox"))
        {
            NewsPaper.Instance.inAdvance = false;
        }
    }
}
