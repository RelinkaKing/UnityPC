using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class BlockManager : MonoBehaviour
{

    public static BlockManager Instance;
    public DifficultyController difficultyControl;
    public int availableBaseCount = 3;
    public int finishedCount = 0;
    public int blocksNumber;

    private void Start()
    {
        Instance = this;

    }
    public enum Difficulties
    {
        Easy,
        Normal,
        Hard
    }

    public Difficulties Difficulty;
    public float easy_Pos, easy_Ang, normal_Pos, normal_Ang, hard_Pos, hard_Ang;

    public void InitTargetPosition()
    {
        Block[] blocks = GameObject.FindObjectsOfType<Block>();
        blocksNumber = blocks.Length;
        for (int i = 0; i < blocks.Length; i++)
        {
            Block block = blocks[i];
            block.targetPosition = block.transform.localPosition;
            block.targetAngles = block.transform.localEulerAngles;
            block.targetRotation = block.transform.localRotation;
            block.targetRight = block.transform.right;
            block.targetUp = block.transform.up;
            block.targetForward = block.transform.forward;
        }
    }
    public TransformToggleController translate_control;
    public TransformToggleController rotation_control;
    public void SetToggleModel(bool isTranslate)
    {
        if (isTranslate)
        {
            translate_control.SetCurrentType();
        }
        else
        {
            rotation_control.SetCurrentType();
        }
    }

    //难度选择
    public void OnDifficultyChange(int index)
    {
        Difficulty = Difficulties.Easy;
        switch (index)
        {
            case 0:
                DebugLog.DebugLogInfo("简单模式");
                Difficulty = Difficulties.Easy;
                availableBaseCount = 15;
                break;
            case 1:
                DebugLog.DebugLogInfo("普通模式");
                Difficulty = Difficulties.Normal;
                availableBaseCount = 10;
                break;
            case 2:
                DebugLog.DebugLogInfo("困难模式");
                Difficulty = Difficulties.Hard;
                availableBaseCount = 5;
                break;
        }
        difficultyControl.OperatEnableObject(true);
        Show_info_test();
    }

    public void Show_info_test()
    {
        StartCoroutine(ShowTooltipAtStart(2));
    }

    IEnumerator ShowTooltipAtStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        //NotificationManager.instance.Show("请先任意双击选择一个结构作为基准", 5f);
        NotificationManager.instance.Show("旋转模型，单指（击）旋转，双指或左右键同时按住进行平移。", 5f);
    }

    public void AddScore()
    {
        finishedCount++;
        if (finishedCount == blocksNumber)//如果完成数和积木数量一样，说明全部搭建完成
        {
            NotificationManager.instance.Show("你真棒，所有结构都正确拼接完成了！", 2.5f);
        }
    }
}
