using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drag : MonoBehaviour
{
       
    protected float timer1; //눌렀을 때 시간
    protected float timer2; //뗐을 때 시간

    protected Vector3 offset; 

    protected Vector3 originPosition;
    protected Vector3 objPosition;

    protected SpriteRenderer spriteRender;
    protected bool inClick = false;



    protected virtual void Start()
    {
       spriteRender = GetComponent<SpriteRenderer>();       
       EventManager.DayEvent_End += EndDay;
    }

    protected virtual void EndDay(Society society, Company company) { Destroy(gameObject);}
    
    protected virtual void OnMouseDown() //누를 때 클릭된 프리팹을 앞으로 옮김
    {
        inClick = true;
        if (!GameManager.Instance.in_PaperMenu)
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
        if (!GameManager.Instance.in_PaperMenu) 
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            transform.position = objPosition + offset;

        }
    }


    protected virtual void OnMouseUp() //마우스를 뗐을 때
    {
        inClick = false;
        if (!GameManager.Instance.in_PaperMenu)
        {
            timer2 = Time.realtimeSinceStartup;
            
            if (!NewsPaper.Instance.inPaper)
            {
                originPosition = transform.position;                
            }
            else
            {
                NewsPaper.Instance.CreatAssignedPaper(gameObject);
                transform.position = originPosition;
            }

            spriteRender.sortingOrder = 2;

            if (timer2 - timer1 < 0.13f) //클릭이라면
            {
                
            }
        }
    }

}
