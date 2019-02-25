using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{
    bool inDesk = true; //책상 안에 존재하는가?
    float timer1; //눌렀을 때 시간
    float timer2; //뗐을 때 시간

    Vector3 originPosition;
    Vector3 objPosition;

    SpriteRenderer spriteRender;

    private void Start()
    {
       spriteRender = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown() //누를 때 클릭된 프리팹을 앞으로 옮김
    {
       
        if (!GameManager.Instance.in_PaperMenu)
        {
            timer1 = Time.realtimeSinceStartup;
            originPosition = transform.position;
            spriteRender.sortingOrder = 10;   
        }
    }

    private void OnMouseDrag() //드래그했을 때 이동
    {
        if (!GameManager.Instance.in_PaperMenu) 
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            transform.position = objPosition;
        }
    }

    private void OnMouseUp() //마우스를 뗐을 때
    {
        if (!GameManager.Instance.in_PaperMenu)
        {
            timer2 = Time.realtimeSinceStartup;
            if (inDesk)
            {
                originPosition = transform.position;
            }
            else
            {
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
}
