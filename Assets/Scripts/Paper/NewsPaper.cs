using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid{
    public int x;
    public int y;

    public Grid()
    {
        x = 6;
        y = 5;
    } //지면 사이즈

    public Grid(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
}

public class NewsPaper : Singleton<NewsPaper>
{
    Grid grid = new Grid(); 
    Vector3[,] paperIndex; //인덱스에 따른 위치좌표를 저장한 배열
    GameObject[,] assignedPapers; //실제 인덱스에 들어있는 기사오브젝트
    GameObject preview; //미리보기 오브젝트

    Vector3 origin; //배치하려고 하는 오브젝트의 현재 위치
    Grid index; // 배치하려고 하는 오브젝트의 변환된 인덱스
   
    public GameObject assignedPaperPrefab; 
    

    
    void Start()
    {
        InitPaper();
    }

    void InitPaper() 
    {
        preview = transform.GetChild(0).gameObject;

        paperIndex = new Vector3[grid.x,grid.y];
        assignedPapers = new GameObject[grid.x,grid.y];

        for(int i = 0 ; i< grid.x; i++)
        {
            for(int j = 0; j< grid.y; j++)
            {
                paperIndex[i,j] = new Vector3( transform.position.x + (i*2*0.56f), transform.position.y + (-j * 1.5f * 0.80f),0);
                assignedPapers[i,j] = null;
            }
        }
    }

    public void CreatAssignedPaper(Paper paper) // 기사를 놓은 곳에 할당된 기사 배치
    {
        if(assignedPapers[index.x,index.y] != null)
        {
            Debug.Log("동일 위치에 이미 배정된 기사가 있습니다!");
        }
        else
        {
            GameObject assignedPaper = Instantiate(assignedPaperPrefab,paperIndex[index.x,index.y],Quaternion.identity,transform);
            assignedPapers[index.x,index.y] = assignedPaper;
            
            preview.SetActive(false);
            paper.gameObject.SetActive(false); 
        }
        
    }

    public Grid ConvertPositionToArray(float _x, float _y) //월드좌표를 인덱스값으로 변환 
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
           index = ConvertPositionToArray(origin.x,origin.y);

            if(paperIndex[index.x,index.y] != null)
            {   
                preview.SetActive(true);
                preview.transform.position = paperIndex[index.x,index.y];
            }
    }
}
