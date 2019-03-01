using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeChangeDrag : MonoBehaviour
{
    Grid mouseIndex ; //현재 마우스위치의 인덱스
    public AssignedPaperDrag assignedPaper; // 기사 오브젝트

    Transform preview;
    Vector3 scale;

    Vector3 originPosition;

    private void Start() {
        assignedPaper = GetComponentInParent<AssignedPaperDrag>();
        preview = NewsPaper.Instance.preview.GetComponent<Transform>();
        originPosition = assignedPaper.gameObject.transform.position;
    }

    private void OnMouseDown() {
        Debug.Log("ㅗ");
    }

    private void OnMouseDrag() {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        
        mouseIndex = NewsPaper.Instance.ConvertPositionToArray(objPosition.y,objPosition.x);
        PreviewSizeChange();
    }

    private void OnMouseUp() {
        preview.localScale = new Vector3(1,1,0);
        NewsPaper.Instance.inPaper = false;

        Transform ass = assignedPaper.gameObject.GetComponent<Transform>();
        ass.localScale = scale;

    }

    void PreviewSizeChange()
    {
        NewsPaper.Instance.inPaper = true;
        NewsPaper.Instance.PreviewPaper(new Vector3(assignedPaper.transform.position.x+0.01f,assignedPaper.transform.position.y,0));
        int x = mouseIndex.x - assignedPaper.index.x +1;
        int y = mouseIndex.y - assignedPaper.index.y +1;

        if(x < 1) x = 1;
        if(y < 1) y = 1;
        scale = new Vector3(x,y,0);

        preview.localScale = scale;

        for(int i = 0 ; i < x; i++ )
        {
            for(int j = 0; j < y; j++)
            {
                if(NewsPaper.Instance.assignedPapers[y,x] != 
                NewsPaper.Instance.assignedPapers[assignedPaper.index.y,assignedPaper.index.x])
                {
                    assignedPaper.gameObject.transform.position = originPosition;

                }
            }
        }

    }
}
