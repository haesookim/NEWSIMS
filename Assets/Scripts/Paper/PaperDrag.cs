using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaperDrag : Drag
{
    GameObject infoWindow;
    Text[] infoText;
    
    protected override void Start()
    {
        infoWindow = transform.GetChild(0).gameObject;
        SetInfoText();
        base.Start();
    }


    ///<param name = "infoText[0]"> 기사 제목 </param>
    ///<param name = "infoText[1]"> 기자,분야 등 </param>
    void SetInfoText()
    {
        infoText = GetComponentsInChildren<Text>();
        Paper paper = GetComponent<Paper>();
        infoText[0].text = paper.article.article_name;
        infoText[1].text = paper.reporter.name + "\n" + paper.article.article_field;
        infoWindow.SetActive(false);
    }


    private void OnMouseExit() 
    {
        if(!inClick)
         infoWindow.SetActive(false);   
    }

    private void OnMouseOver() //마우스 올리고 있을 때
    {
        if(!inClick && !GameManager.Instance.in_ReporterMenu)
            infoWindow.SetActive(true);  
        else 
            infoWindow.SetActive(false);  
    }   

 
    //지면 위에 오브젝트를 끌고 있을 때

   void OnTriggerStay2D(Collider2D other) {
        if(other.CompareTag("NewsPaper"))
        {
            NewsPaper.Instance.inPaper = true;
            NewsPaper.Instance.PreviewPaper(transform.position);
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("NewsPaper"))
        {
            NewsPaper.Instance.inPaper = false;
        }
    }

    

}
