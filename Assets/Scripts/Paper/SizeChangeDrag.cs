using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeChangeDrag : MonoBehaviour
{
    Grid mouseIndex ; //현재 마우스위치의 인덱스
    public AssignedPaperDrag assignedPaper; // 기사 오브젝트
    public Paper paperData ; //배분도 업데이트를 위해서... 일단은 급조. 나중에 Ratio를 쓰던가 해서 고칠것.

    ///// 미리보기 이미지 컨트롤
    Transform preview; 

    ///// 배정된 기사의 사이즈 조절에 필요
    Vector3 scale;
    Vector3 beforeScale = new Vector3(1,1,0);

    Vector3 originPosition;

    bool canSizeChange;

    private void Start() {
        assignedPaper = GetComponentInParent<AssignedPaperDrag>();
        paperData = assignedPaper.originData.GetComponent<Paper>();
        
        //급조된 부분.ㅋㅋ
        paperData.UpdateViewText("1");

        preview = NewsPaper.Instance.preview.GetComponent<Transform>();
        originPosition = assignedPaper.gameObject.transform.position;
    }

    private void OnMouseDown() {
       
    }

    private void OnMouseDrag() {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        
        mouseIndex = NewsPaper.Instance.ConvertPositionToArray(objPosition);
        PreviewSizeChange();
    }

    private void OnMouseUp() {
        preview.localScale = new Vector3(1,1,0);
        NewsPaper.Instance.inPaper = false;

        AssignedPaperData data = NewsPaper.Instance.assignedPapers[assignedPaper.index.y,assignedPaper.index.x];

        if(canSizeChange)
            if(beforeScale != scale)
            {
                //기존 범위의 데이터를 삭제하고 1*1사이즈 기준으로 되돌림.
                DeleteDatainPaper();
                NewsPaper.Instance.assignedPapers[assignedPaper.index.y,assignedPaper.index.x] = data;

                //새로운 범위로 저장
                for(int i = 0; i<scale.x; i++)
                {
                    for(int j =0; j<scale.y; j++)
                    {
                        NewsPaper.Instance.assignedPapers[assignedPaper.index.y+j,assignedPaper.index.x+i] = data;
                        assignedPaper.size = new Grid((int)beforeScale.x,(int)beforeScale.y);
                    }
                }
                beforeScale = scale;
                assignedPaper.gameObject.transform.localScale = scale;
                paperData.UpdateViewText((beforeScale.x * beforeScale.y).ToString());
            }
    }

    void PreviewSizeChange()
    {
        NewsPaper.Instance.inPaper = true;
        Vector3 adjustPosition = new Vector3(assignedPaper.transform.position.x+0.01f,assignedPaper.transform.position.y,0);
        NewsPaper.Instance.PreviewPaper(adjustPosition);
       
        //프리뷰의 확장 사이즈를 알기 위한 값
        int x = mouseIndex.x - assignedPaper.index.x + 1;
        int y = mouseIndex.y - assignedPaper.index.y + 1;

        if(x < 1) x = 1;
        if(y < 1) y = 1;
        scale = new Vector3(x,y,0);

        CheckingInside();

        preview.localScale = scale; 
    }

    //프리뷰 내의 지역을 탐색하여 확장 가능한지를 판정
    void CheckingInside()
    {
        bool temp = false;
        bool breakCheck = false;

        for(int i = 0 ; i < scale.x; i++ )
        {
            for(int j = 0; j < scale.y; j++)
            {
                if(i == 0 && j == 0) //1*1 일 때 예외처리
                {
                    temp = true;
                    continue;
                }

                if(NewsPaper.Instance.assignedPapers[assignedPaper.index.y+j,assignedPaper.index.x+i] != null)
                {
                    //주변 칸이 비어있지 않은데 그 칸에 자신의 데이터가 아니면 => 다른 무언가 차있을 경우
                    if(NewsPaper.Instance.assignedPapers[assignedPaper.index.y+j,assignedPaper.index.x+i] != 
                    NewsPaper.Instance.assignedPapers[assignedPaper.index.y,assignedPaper.index.x])
                    {
                        temp = false;
                        breakCheck = true;
                        break;
                    }
                    else
                        temp = true;
                }
                else
                    temp = true;
            }

            if(breakCheck) break;
        }

        canSizeChange = temp;

        if(!canSizeChange)
            NewsPaper.Instance.ChangePreviewColor(false);
        else
            NewsPaper.Instance.ChangePreviewColor(true);
    }



    public void DeleteDatainPaper()
    {
        for(int i = 0; i<beforeScale.x; i++)
        {
            for(int j =0; j<beforeScale.y; j++)
            {
                NewsPaper.Instance.assignedPapers[assignedPaper.index.y+j,assignedPaper.index.x+i] = null;
            }
        }    
        paperData.UpdateViewText("0");
    }

            /////DEBUG_전체 지면 훑기
    void DEBUG_DisplayAllPaperArray()
    {
        Debug.Log("---시작");
        for(int i =0; i < 6; i++)
        {
            for(int j =0; j<5; j++)
            {
                Debug.Log("(" + j + " , " + i + " )  " + NewsPaper.Instance.assignedPapers[j,i]);
            }
        }
        Debug.Log("---끝");
    }
}
