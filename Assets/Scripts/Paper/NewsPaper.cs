using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid{
    public int x;
    public int y;

    public Grid()
    {
        x = 6 +2; 
        y = 5 +2;
    } //지면 사이즈 + 외곽금지영역

    public Grid(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
}

public class AssignedPaperData{

    public GameObject originData;
    public Grid paperIndex;

    public AssignedPaperData( GameObject data, Grid index)
    {
        originData = data;
        paperIndex = index;
    }
}

public class NewsPaper : Singleton<NewsPaper>
{
    public bool inPaper;
    Grid grid = new Grid(); 
    public Vector3[,] paperIndex; //인덱스에 따른 위치좌표를 저장한 배열
    public AssignedPaperData[,] assignedPapers; //실제 인덱스에 들어있는 기사오브젝트 정보

    AssignedPaperData outerNULL; //배열의 외곽 체크용 NULL 데이터
   

    Vector3 origin; //배치하려고 하는 오브젝트의 현재 위치
    Grid index; // 배치하려고 하는 오브젝트의 변환된 인덱스
   
    public GameObject assignedPaperPrefab; 
    public GameObject preview; //미리보기 오브젝트
    

    
    void Start()
    {
        InitPaper();
    }

    private void Update() {
        if(inPaper)
            preview.SetActive(true);
        else
            preview.SetActive(false);
    }

    void InitPaper() 
    {
        preview = transform.GetChild(0).gameObject;

        paperIndex = new Vector3[grid.y+1,grid.x+1];
        assignedPapers = new AssignedPaperData[grid.y+1,grid.x+1];
        outerNULL = new AssignedPaperData(null,null);

        //배열 외곽 금지구역 설정
        for(int i = 0; i<grid.x; i++)
            assignedPapers[0,i] = outerNULL;
        for(int i = 0; i<grid.x; i++)
            assignedPapers[grid.y,i] = outerNULL;
        for(int i = 0; i<grid.y; i++)
            assignedPapers[i,0] = outerNULL;
        for(int i = 0; i<grid.y; i++)
            assignedPapers[i,grid.x] = outerNULL;
        

        //배열 내부 인덱스당 위치값 설정
        for(int i = 0 ; i< grid.x -1; i++)
        {
            for(int j = 0; j< grid.y -1; j++)
            {
                paperIndex[j,i] = new Vector3( transform.position.x + (i*2*0.56f), transform.position.y + (-j * 1.5f * 0.80f),0);
                assignedPapers[j,i] = null;
            }
        }
    }

    public void CreatAssignedPaper(GameObject paper) // 기사를 놓은 곳에 할당된 기사오브젝트를 생성해 배치
    {
        if(assignedPapers[index.y,index.x] != null)
        {
            Debug.Log("동일 위치에 이미 배정된 기사가 있습니다!");
        }
        else
        {
            GameObject paperObject = Instantiate(assignedPaperPrefab,paperIndex[index.y,index.x],Quaternion.identity,transform);
            AssignedPaperData assignedPaper = new AssignedPaperData(paper,index);
            paperObject.GetComponent<AssignedPaperDrag>().SetPaper(paper,index);

            assignedPapers[index.y,index.x] = assignedPaper;
        }
        inPaper = false;
    }

    public Grid ConvertPositionToArray(float _y, float _x) //월드좌표를 인덱스값으로 변환 
    {
        int x = Mathf.FloorToInt( (_x - transform.position.x) / (2f * 0.56f)); //위치값 - 기준값 / x스케일 * 기사프리팹크기
        int y = Mathf.FloorToInt( (_y - transform.position.y) / (1.5f * -0.8f));

        x = Mathf.Clamp(x,0,grid.x - 3);
        y = Mathf.Clamp(y,0,grid.y - 3);

        Grid index= new Grid(x,y);
        return index;
    }

    public void PreviewPaper(Vector3 paper)
    {
        origin = paper;
        index = ConvertPositionToArray(origin.y,origin.x);

        if(assignedPapers[index.y,index.x] != outerNULL)
        {   
            preview.transform.position = paperIndex[index.y,index.x];
        }
    }


    /////////////////////// 배치한 기사의 이동, (혹시 구현하게 될 경우를 대비해 남겨둠)
    public void MovePaperAToB(Grid _A, Grid _B) //이동
    {
        assignedPapers[_B.y,_B.x] = assignedPapers[_A.y,_A.x];
        assignedPapers[_A.y,_A.x] = null;
    }


}
