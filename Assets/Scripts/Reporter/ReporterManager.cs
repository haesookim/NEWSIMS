using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReporterManager : Singleton<ReporterManager>
{
    public List<VisualizeReporter> vrs = new List<VisualizeReporter>(); //리스트
    public Reporter myReporter; //직원 목록 디테일에서 사용하는 리포터를 저장하는 변수
    public Reporter employReporter; //고용 디테일에서 사용하는 리포터를 저장하는 변수

    public void AddVrsToList(VisualizeReporter _vrs) //VisualizeReporter를 리스트에 추가하는 함수
    {
        vrs.Add(_vrs);
    }

    public void RemoveVrsToList(VisualizeReporter _vrs) //VisualizeReporter를 리스트에서 지우는 함수
    {
        vrs.Remove(_vrs);
    }
}
