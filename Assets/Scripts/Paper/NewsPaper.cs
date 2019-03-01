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
    Vector3[,] paperIndex; //인덱스에 따른 위치좌표를 저장한 배열
    AssignedPaperData[,] assignedPapers; //실제 인덱스에 들어있는 기사오브젝트 정보

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
        

        //배열 내부 위치값 설정
        for(int i = 1 ; i< grid.x -1; i++)
        {
            for(int j = 1; j< grid.y -1; j++)
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

        x = Mathf.Clamp(x,0,grid.x - 1);
        y = Mathf.Clamp(y,0,grid.y - 1);

        Grid index= new Grid(x,y);
        return index;
    }

    public void PreviewPaper(Transform paper)
    {
        origin = paper.gameObject.transform.position;
        index = ConvertPositionToArray(origin.y,origin.x);

        if(paperIndex[index.y,index.x] != null)
        {   
            preview.transform.position = paperIndex[index.y,index.x];
        }
    }

    /////////////////////// 배치한 기사의 이동, 확장(탐색), 제외
    public void MovePaperAToB(Grid _A, Grid _B) //이동
    {
        assignedPapers[_B.y,_B.x] = assignedPapers[_A.y,_A.x];
        assignedPapers[_A.y,_A.x] = null;
    }

    public void ExcludePaper(Grid paper) // 제외
    {
        assignedPapers[paper.y,paper.x] = null;
    }

    public void ExpandPaper() //확장
    {

    }

    public struct EmptyDirection
    {
        public bool north; public bool south; public bool east; public bool west;
    }

    public void SearchAround(AssignedPaperDrag paper)
    {
        EmptyDirection emptyDirection = new EmptyDirection();;
        Grid size = paper.size;

        Grid paperIndex = paper.index;
        Grid searchRange = new Grid(paperIndex.x + (size.x - 1), paperIndex.y + (size.y-1)); //탐색 범위
        for(int i = paperIndex.x; i <searchRange.x; i++ )
        {
            for(int j = paperIndex.y; j<searchRange.y; j++)
            {
                emptyDirection = ConditionCheck(paperIndex);
            }
        }

       DEBUG_Direction(emptyDirection);

    }

    void DEBUG_Direction(EmptyDirection direction)
    {
         if(direction.north) Debug.Log("위가 비었습니다");
         if(direction.south) Debug.Log("아래가 비었습니다");
         if(direction.west) Debug.Log("서쪽이 비었습니다");
         if(direction.east) Debug.Log("동쪽이 비었습니다");
    }


    EmptyDirection ConditionCheck(Grid originIndex)
    {
        EmptyDirection emptyDirection = new EmptyDirection();
        AssignedPaperData origin = assignedPapers[index.y,index.x];

        //1칸짜리 기사일 경우
        if(Direction(-1,0) != origin && Direction(1,0) != origin && Direction(0,-1) != origin && Direction(0,1) != origin)
        {
            if(Direction(1,0) == null) emptyDirection.north = true;
            if(Direction(-1,0) == null) emptyDirection.south = true;
            if(Direction(0,1) == null) emptyDirection.east = true;
            if(Direction(0,-1) == null) emptyDirection.west = true;
        }
        else //2칸 이상의 기사일 경우
        {
            //상단
            if(Direction(-1,0) == origin && Direction(1,0) != origin) 
            {
                if(Direction(1,0) == null) emptyDirection.north = true;

                if(Direction(0,1) == origin && Direction(0,-1) != origin) 
                    if(Direction(0,-1) == null) emptyDirection.west = true;
                
                if(Direction(0,1) == origin && Direction(0,-1) == origin) 
                    //아무것도 하지 않음
               
                if(Direction(0,1) != origin && Direction(0,-1) == origin) 
                    if(Direction(0,1) == null) emptyDirection.east = true;
                
            }

            //중단
            else if(Direction(-1,0) == origin && Direction(1,0) == origin) 
            {
                if(Direction(0,1) == origin && Direction(0,-1) != origin) 
                    if(Direction(0,-1) == null) emptyDirection.west = true;
                
                if(Direction(0,1) == origin && Direction(0,-1) == origin) 
                    //아무것도 하지 않음
               
                if(Direction(0,1) != origin && Direction(0,-1) == origin) 
                    if(Direction(0,1) == null) emptyDirection.east = true;
            }

            //하단
            else if(Direction(-1,0) != origin && Direction(1,0) == origin) 
            {
                if(Direction(-1,0) == null) emptyDirection.south = true;

                if(Direction(0,1) == origin && Direction(0,-1) != origin) 
                    if(Direction(0,-1) == null) emptyDirection.west = true;
                
                if(Direction(0,1) == origin && Direction(0,-1) == origin) 
                    //아무것도 하지 않음
               
                if(Direction(0,1) != origin && Direction(0,-1) == origin) 
                    if(Direction(0,1) == null) emptyDirection.east = true;
            }    
        }
        
        return emptyDirection;
    }

    AssignedPaperData Direction(int _y, int _x)
    {
        return assignedPapers[index.y + _y,index.x + _x];
    }
}
