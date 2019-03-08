using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drag : MonoBehaviour
{
       
    protected float timer1; //눌렀을 때 시간
    protected float timer2; //뗐을 때 시간

    protected Vector3 offset; //클릭 시 위치 유지를 위해 필요한 거리

    protected Vector3 originPosition; 
    protected Vector3 objPosition;

    protected GameObject infoWindow; 
    protected Text[] infoText;

    public SpriteRenderer spriteRender;
    protected bool inClick = false;

    protected virtual void Start()
    {
       spriteRender = GetComponent<SpriteRenderer>();       
       infoWindow = transform.GetChild(0).gameObject;
       EventManager.DayEvent_End += EndDay;
    }

    ///<param name = "infoText[0]"> 기사 제목 </param>
    ///<param name = "infoText[1]"> 기자,분야 등 </param>
    protected void SetInfoText()
    {
        infoText = GetComponentsInChildren<Text>();
        Paper paper = GetComponent<Paper>();
        infoText[0].text = paper.article.article_name;
        infoText[1].text = paper.reporter_name + "\n" + paper.article.article_field;
        infoWindow.SetActive(false);
    }

    protected virtual void EndDay(Society society, Company company) 
    {
        if(this != null)
            Destroy(gameObject);
    }
    
    protected virtual void OnMouseDown() //누를 때 클릭된 프리팹을 앞으로 옮김
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

    protected virtual void OnMouseDrag() //드래그했을 때 이동
    {
        inClick = true;
        if (!GameManager.Instance.in_PaperMenu && !GameManager.Instance.in_ReporterMenu) 
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            transform.position = objPosition + offset;
        }
    }


    protected virtual void OnMouseUp()  {}  //마우스를 뗐을 때
   

    protected void OnMouseExit() 
    {
        if(!inClick)
         infoWindow.SetActive(false);   
    }

    protected void OnMouseOver() //마우스 올리고 있을 때
    {
        if(!inClick && !GameManager.Instance.in_ReporterMenu)
            infoWindow.SetActive(true);  
        else 
            infoWindow.SetActive(false);  
    }   

}
