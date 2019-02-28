using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drag : MonoBehaviour
{
    bool inPaper = false; //책상 안에 존재하는가?
    bool inClick = false;
    float timer1; //눌렀을 때 시간
    float timer2; //뗐을 때 시간

    Vector3 offset; 

    Vector3 originPosition;
    Vector3 objPosition;

    SpriteRenderer spriteRender;
    public GameObject infoWindow;
    public Text[] infoText;


    private void Start()
    {
       spriteRender = GetComponent<SpriteRenderer>();
       infoWindow = transform.GetChild(0).gameObject;
       SetInfoText();
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

    private void OnMouseDown() //누를 때 클릭된 프리팹을 앞으로 옮김
    {
        inClick = true;
        if (!GameManager.Instance.in_PaperMenu && !GameManager.Instance.in_ReporterMenu)
        {
            timer1 = Time.realtimeSinceStartup;
            originPosition = transform.position;
            spriteRender.sortingOrder = 5;   
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            offset = originPosition - Camera.main.ScreenToWorldPoint(mousePosition);
        }
    }

    private void OnMouseDrag() //드래그했을 때 이동
    {
        inClick = true;
        if (!GameManager.Instance.in_PaperMenu && !GameManager.Instance.in_ReporterMenu) 
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            transform.position = objPosition + offset;
        }
    }


    private void OnMouseUp() //마우스를 뗐을 때
    {
        inClick = false;
        if (!GameManager.Instance.in_PaperMenu && !GameManager.Instance.in_ReporterMenu)
        {
            timer2 = Time.realtimeSinceStartup;
            
            if (!inPaper)
            {
                originPosition = transform.position;                
            }
            else
            {
                NewsPaper.Instance.CreatAssignedPaper(GetComponent<Paper>());
                transform.position = originPosition;
            }

            spriteRender.sortingOrder = 2;

            if (timer2 - timer1 < 0.13f) //클릭이라면
            {
                Paper paper = GetComponent<Paper>();
                bool advance = paper.article.advance;                

                if (!advance) //심화 취재를 지시하지 않은 기사라면
                {
                    GameManager.Instance.selectedPaper = paper;
                    GameManager.Instance.DisplayPaperMenu();
                }
                else //심화 취재를 지시한 기사라면
                {
                    GameManager.Instance.selectedPaper = paper;
                    GameManager.Instance.DisplayPaperMenu();
                }
            }
        }
    }

    
    private void OnMouseOver() //마우스 올리고 있을 때
    {
        if(!inClick && !GameManager.Instance.in_PaperMenu && !GameManager.Instance.in_ReporterMenu)
            infoWindow.SetActive(true);  
        else 
            infoWindow.SetActive(false);  
    }   

    private void OnMouseExit() 
    {
        if(!inClick)
         infoWindow.SetActive(false);   
    }
 
    //지면 위에 오브젝트를 끌고 있을 때

    private void OnTriggerStay2D(Collider2D other) {
        if(other.CompareTag("NewsPaper"))
        {
            inPaper = true;
            NewsPaper.Instance.PreviewPaper(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        inPaper = false;
    }
}
