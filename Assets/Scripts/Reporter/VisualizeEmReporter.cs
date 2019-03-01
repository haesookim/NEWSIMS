using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualizeEmReporter : MonoBehaviour
{
    [Header("기자 정보 받는 칸")]
    public SpriteRenderer reporterImage;
    public Text name;
    public Text mainField;
    public Text level;
    public Text writing;
    public Text logic;
    public Text survey;
    public Text buyout;
    public EmReporter emreporter;

    public string mf = "";

    // Start is called before the first frame update
    void Start()
    {
        ReporterManager.Instance.AddeVrsToList(this);

        reporterImage.sprite = emreporter.reporterImage;

        name.text = "이름 : " + emreporter.name;
        float temp_max = 0;
        for (int i = 0; i < System.Enum.GetValues(typeof(Setting.Fields)).Length; i++)
        {
            if (i == 0)
            {
                mf = System.Enum.GetName(typeof(Setting.Fields), i);
                temp_max = emreporter.interests[(Setting.Fields)i];
            }
            else
            {
                if (emreporter.interests[(Setting.Fields)i] > temp_max)
                {
                    mf = System.Enum.GetName(typeof(Setting.Fields), i);
                    temp_max = emreporter.interests[(Setting.Fields)i];
                }
            }
        }
        mainField.text = "주 분야 : " + mf;
        level.text = "레벨 : " + emreporter.level.ToString();
        writing.text = "필력 : " + emreporter.writing.ToString();
        logic.text = "논리력 : " + emreporter.logic.ToString();
        survey.text = "조사력 : " + emreporter.survey.ToString();

        buyout.text = emreporter.buyout.ToString();
    }

    public void UpdateStatus()
    {
        for (int i = 0; i < GameManager.Instance.company.em_reporters.Count; i++)
        {
            if (emreporter.reporter_index == GameManager.Instance.company.em_reporters[i].reporter_index)
            {
                emreporter = GameManager.Instance.company.em_reporters[i];
                break;
            }
        }
    }

    public void DisplayFireIcon()
    {
        if (!emreporter.is_employed)
        {
            transform.GetChild(7).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(7).gameObject.SetActive(true);
        }
    }
}
