using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowProgress : MonoBehaviour
{

    //下载文本
    public Text progress;
    public Text Title;
    //进度条
    public Slider progressSlider;
    public GameObject canvas_obj;

    public Button Back_btn;

    public bool StartLoad
    {
        get { return startload; }
        set
        {
            startload = value;
        }
    }
    private bool startload = false;
    public float current_progress = 0f;

    public bool no_progress = false;


    void Start() {
        
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        if (StartLoad && !no_progress)
        {
            progress.text = current_progress.ToString("0%");//string.Format("{0:00%}", current_progress); //
            progressSlider.value = current_progress;
            if (progressSlider.value >= 0.99f)
            {
                print("stop----------------------");
                startload = false;
            }
        }
    }

    public GameObject BackBtn;

    public void BackPlantForm()
    {
        InternalSocketMananger.instance.HideUnityWidows();
    }

    public void closeBtn()
    {
        BackBtn.SetActive(false);
    } 

    public void CloseProgress()
    {
        canvas_obj.SetActive(false);
    }

    public GameObject BtnGrop;

    public void CloseBtn(bool is_close)
    {
        //BtnGrop.SetActive(is_close);
    }

    public void StopLoad()
    {
        print("stop----------------------");
        startload = false;
        progressSlider.value = 0f;
        progress.text = progressSlider.value.ToString("0%");
        canvas_obj.SetActive(false);
    }

    /// <summary>
    /// 设置返回按钮监听函数
    /// </summary>
    public void Set_Progress(string title, UnityEngine.Events.UnityAction callBackbutton = null)
    {
        if (BackBtn == null)
        {
            BackBtn = new GameObject("backbtn(clone)");
        }
        //if (BtnGrop == null)
        //{
        //    BtnGrop = new GameObject("BtnGrop(clone)");
        //}
        try
        {
            
        Title.text = title;
        }
        catch (System.Exception)
        {
        }
        current_progress = 0f;
        startload = true;
        canvas_obj.SetActive(true);
        BackBtn.SetActive(true);
        // if (callBackbutton == null)
        // {
        //     print("回调为空--------------------");
        Back_btn.gameObject.SetActive(false);
        //     return;
        // }
        // else
        // {

        //     Back_btn.gameObject.SetActive(true);
        // }
        // Back_btn.onClick.AddListener(delegate () { this.StopLoad(); });
        // Back_btn.onClick.AddListener(delegate () { callBackbutton(); });
    }
}