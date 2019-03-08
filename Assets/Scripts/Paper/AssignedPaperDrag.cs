using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignedPaperDrag : Drag
{
    public GameObject originData; //배정되기 전의 원본 기사 오브젝트
    public Article article;
    public Grid index; //배정된 위치인덱스
    public Grid size = new Grid(1,1);  //기사의 사이즈


    bool inWorkDesk;

    public void SetPaper(GameObject _originData, Grid _index) 
    {
        originData = _originData;
        article = _originData.GetComponent<Paper>().article;
        index = _index;
        originData.SetActive(false);
    }



    protected override void OnMouseUp() 
    {
        if (!GameManager.Instance.in_PaperMenu)
        {
            timer2 = Time.realtimeSinceStartup;
            
            if (!inWorkDesk)
            {             
                transform.position = originPosition;
            }
            else
            {
                ReturnArticle();
                Destroy(gameObject);
            }

            spriteRender.sortingOrder = 2;

            if (timer2 - timer1 < 0.13f) //클릭이라면
            {
              
            }
        }
    }

    void ReturnArticle() //지면에서 빼기 
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        originData.transform.position = objPosition;
        originData.SetActive(true); //배정 시 숨겨 둔 기사 오브젝트를 복원


        //인덱스 내의 데이터를 삭제
        
        NewsPaper.Instance.ArticlesInAssignedPaper.Remove(this);
        GetComponentInChildren<SizeChangeDrag>().DeleteDatainPaper();
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.CompareTag("WorkDesk"))
        {
                inWorkDesk = true;
         }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("WorkDesk"))
        {
                inWorkDesk = false;
         }
    }

}
