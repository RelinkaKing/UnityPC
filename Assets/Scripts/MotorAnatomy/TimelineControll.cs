using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using XLua;
[Hotfix]
public class TimelineControll : MonoBehaviour
{
    bool isDragSlider = true;
    public Slider AnimationSlide;
    public PlayableDirector timeLine;
    public float speed = 1;
    float startTime = 0;
    float endtime;
    float timeLength;
    public CameraCtl cameraCtl;
    bool init = true;
    public float[] timeNode;
    public GameObject stagePrefab;
    public float stageFrame;
    public float stageTime;
    public Image[] stageButton;
    public Sprite[] stageButtonSprite;
    public bool isCycle;
    public Image cycleButton;
    public GameObject[] playAndPauseButton;
    public float frame;
    bool isover;
    public Transform SlideHandle;
    public static TimelineControll instance;

    private void Awake()
    {
        instance = this;
    }

    [LuaCallCSharp]
    public void Init()
    {
        endtime = (float)timeLine.duration;
        timeLength = (float)timeLine.duration;
        stageTime = stageFrame / frame;
        timeNode = new float[3];
        timeNode[0] = startTime;
        timeNode[1] = stageTime;
        timeNode[2] = endtime;
        for (int i = 1; i < timeNode.Length - 1; i++)
        {
            float x = (timeNode[i] - startTime) / timeLength * AnimationSlide.GetComponent<RectTransform>().sizeDelta.x - 0.5f * AnimationSlide.GetComponent<RectTransform>().sizeDelta.x;
            Debug.Log(x);
            GameObject go = Instantiate(stagePrefab, AnimationSlide.transform);
            go.GetComponent<RectTransform>().localPosition = new Vector3(x, 9, 0);
        }
        timeLine.time = startTime;
        timeLine.Evaluate();
        SlideHandle.SetAsLastSibling();
        stageTime = startTime;
        isDragSlider = false;
    }
    [LuaCallCSharp]
    void Update()
    {
        if (!isDragSlider)
        {
            if (timeLine.time >= endtime && isCycle && speed != 0)
            {
                timeLine.time = stageTime;
            }


            if (timeLine.time < endtime)
            {
                timeLine.time += Time.deltaTime * speed;
                isover = false;

            }
            else
            {
                timeLine.time = stageTime;
                isover = true;
                speed = 0;
                isshowplay(true);
            }

            timeLine.Evaluate();

            AnimationSlide.value = (float)(((timeLine.time - startTime)) / timeLength);
        }
    }
    float r = 0;
    [LuaCallCSharp]
    public void clickSliderDown()
    {
        Pauseplay();
        cameraCtl.isDragSlide = true;
        isDragSlider = true;
        float a = AnimationSlide.value;
        timeLine.time = a * timeLength + startTime;
        timeLine.Evaluate();
    }
    [LuaCallCSharp]
    public void clickSliderUP()
    {
        isDragSlider = false;
        cameraCtl.isDragSlide = false;
        speed = 0;
        isshowplay(true);

    }
    [LuaCallCSharp]
    public void SelectStage(int num)
    {
        if (stageTime == timeNode[num - 1] && endtime == timeNode[num])
        {
            stageTime = startTime;
            endtime = timeNode[timeNode.Length - 1];
            timeLine.time = stageTime;
            timeLine.Evaluate();
            for (int i = 0; i < stageButton.Length; i++)
            {
                stageButton[i].sprite = stageButtonSprite[i];
            }
        }
        else
        {
            stageTime = timeNode[num - 1];
            endtime = timeNode[num];
            timeLine.time = stageTime;
            timeLine.Evaluate();
            for (int i = 0; i < stageButton.Length; i++)
            {
                stageButton[i].sprite = stageButtonSprite[i];
            }
            stageButton[num - 1].sprite = stageButtonSprite[2];
        }
        speed = 1;
        isover = false;
        isshowplay(false);
    }
    [LuaCallCSharp]
    public void cycle()
    {
        if (isCycle)
        {
            cycleButton.enabled = false;
            isCycle = false;
        }
        else
        {
            cycleButton.enabled = true;
            isCycle = true;
        }
    }
    [LuaCallCSharp]
    public void SlideControll()
    {
        float a = AnimationSlide.value;
        timeLine.time = a * timeLength + startTime;
        timeLine.Evaluate();
    }
    [LuaCallCSharp]
    public void startplay()
    {
        speed = 1;
        isshowplay(false);
    }
    [LuaCallCSharp]
    public void Pauseplay()
    {
        isshowplay(true);
        speed = 0;
    }
    [LuaCallCSharp]
    public void isshowplay(bool isshow)
    {
        playAndPauseButton[0].SetActive(isshow);
        playAndPauseButton[1].SetActive(!isshow);
        if (!isshow && isover)
        {
            timeLine.time = stageTime;
            timeLine.Evaluate();
        }
    }

    public void close()
    {
        timeLine.time = 0;
        timeLine.Evaluate();
    }
}
