using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Model;
using VesalCommon;

public class BookMark : MonoBehaviour
{



public GameObject bookmark;
    public BookMarkInfo markInfo;
    public Image thisSprite;

    //Sprite spriteScreenshot;

    public GameObject delete;
    public Button thisBtn;
    public InputField nameText;
    int id;
    //string bookmarkName;
    ////截图转化为比特流
    byte[] ImageMarkByte;
    SceneModelState  modelState;
    SceneBtnState  btnState;
    CameraParams cameraParams;
    public bool is_fix=false;

    PlayerCommand playRecord;
    //string  cameraParamsByte;


    //private Customer markData;

    //public Customer MarkData
    //{
    //    set
    //    {
    //        markData = value;
    //        _id = markData._id;
    //        nameText.text = markData.bookmarkName;
    //        // DebugLog.DebugLogInfo(markData.btnState+" btnState:"+btnState);
    //        // DebugLog.DebugLogInfo(" markData.cameraParams:"+markData.cameraParams);
    //        // DebugLog.DebugLogInfo("markData.BookmarkPicture :"+markData.BookmarkPicture.Length);
    //        //类对象反序列化
    //        btnState = (SceneBtnState)BookMarkManager.DeserializeObject<SceneBtnState>(markData.btnState);
    //        modelState = (SceneModelState)BookMarkManager.DeserializeObject<SceneModelState>(markData.modelState);
    //        cameraParams = (CameraParams)BookMarkManager.DeserializeObject<CameraParams>(markData.cameraParams);
    //        ImageMark = markData.BookmarkPicture;
    //        ShowUI();
    //    }
    //}

    //书签的构造函数
    //public BookMark(string _id,string _name,byte[] _ImageMark,string  _modelState,string  _btnState,string  _cameraParams)
    //{
    //    id = _id;
    //    ImageMarkByte = _ImageMark;
    //    modelStateByte = _modelState;
    //    btnStateByte = _btnState;
    //    cameraParamsByte = _cameraParams;

    //}


    public void SetMarkData(BookMarkInfo mark_data, bool pc_image)
    {
        markInfo = mark_data;
        id = markInfo.id;
        nameText.text = markInfo.bookmarkName;
        //类对象反序列化
        btnState = (SceneBtnState)Vesal_DirFiles.Bytes2Object(markInfo.btnState);
        modelState = (SceneModelState) Vesal_DirFiles.Bytes2Object(markInfo.modelState);
        cameraParams = (CameraParams)Vesal_DirFiles.Bytes2Object(markInfo.cameraParams);
        ImageMarkByte = markInfo.bookmarkPicture;
        is_fix = (markInfo.type=="0") ?true:false;
        playRecord = new PlayerCommand(modelState, btnState, cameraParams);
        ShowUI(pc_image);
    }

    

    //读取书签
    public void LoadBookMark()
    {
        DebugLog.DebugLogInfo("打开开关按钮");

        #region 

        //if (btnState.isMultiBtnUI== false)
        //{
        //    SceneModels.instance.set_Multi_Selection(false);
        //    // SceneModels.instance.CancleSelect();
        //    UIChangeTool.ShowOneObject(XT_AllButton.Instance.openMu, XT_AllButton.Instance.closeMu, false);
        //    // XT_AllButton.Instance.multiSelectBtn.GetComponent<Image>().color = Color.white;
        //}
        //else
        //{
        //    SceneModels.instance.set_Multi_Selection(true);
        //    // XT_AllButton.Instance.multiSelectBtn.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        //    UIChangeTool.ShowOneObject(XT_AllButton.Instance.openMu, XT_AllButton.Instance.closeMu, true);
        //}

        //DebugLog.DebugLogInfo("交互模块赋值");
        //Interaction.instance.gameObject.transform.localPosition=new Vector3(cameraParams.positionx, cameraParams.positiony,cameraParams.distance);

        //Interaction.instance.rotateAxis.transform.localPosition = new Vector3(cameraParams.rotateAxis.x, cameraParams.rotateAxis.y, cameraParams.rotateAxis.z);
        //Interaction.instance.y = cameraParams.rotation.x;
        //Interaction.instance.x = cameraParams.rotation.y;
        //Interaction.instance.rotateAxis.transform.rotation= Quaternion.Euler(-cameraParams.rotation.x, cameraParams.rotation.y,0);

        //DebugLog.DebugLogInfo("场景模型信息");
        //Model[] tempAllmodel = SceneModels.instance.Get_scope_models();
        //DebugLog.DebugLogInfo("场景模型长度 "+tempAllmodel.Length);

        //try
        //{
        //    //解析场景模型信息
        //    for (int i = 0; i < tempAllmodel.Length; i++)
        //    {
        //        modelState outState = modelState.SceneModelStateDict[tempAllmodel[i].name];
        //        if (outState.isActive)
        //        {

        //            tempAllmodel [i].gameObject.transform.position=new Vector3(outState.position.x,outState.position.y,outState.position.z);
        //            tempAllmodel[i].BecomeDisplay();
        //            if (outState.isSeleted)
        //            {
        //                SceneModels.instance.ChooseModel(tempAllmodel[i]);
        //            }
        //            else
        //            {
        //                tempAllmodel[i].BecomeNormal();
        //            }
        //            if (outState.isFade)
        //            {
        //                tempAllmodel[i].BecomeTranslucent();
        //            }
        //        }
        //        else
        //        {
        //            tempAllmodel[i].BecomeHide();
        //        }
        //    }
        //}
        //catch (System.Exception e)
        //{
        //    DebugLog.DebugLogInfo("Message  "+e.Message);
        //    DebugLog.DebugLogInfo("StackTrace  "+e.StackTrace);
        //}

        #endregion

        

        playRecord.ReadBookMark();
        
        DebugLog.DebugLogInfo("BookMarkManager.instance  "+(BookMarkManager.instance.BookmarkPanel == null));
        DebugLog.DebugLogInfo("BookMarkManager.instance  "+BookMarkManager.instance.BookmarkPanel.name);
        BookMarkManager.instance.SetBookmarkPanel(false);
        CommandManager.instance.ClearRecordStack();
        PublicClass.currentState = RunState.Playing;


    }

    //显示解析出的图片
    void ShowUI(bool pc_image)
    {
        if (pc_image)
        {
            int width = 1920;
            int height = 1080;
            Texture2D texture = new Texture2D(width, height);
            texture.LoadImage(ImageMarkByte);
            float sprite_width = texture.width / 2.3f;
            float offset_x = (texture.width - sprite_width) / 2f;
            thisSprite.sprite = Sprite.Create(texture, new Rect(offset_x, 0, sprite_width, texture.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            int width = 1080;
            int height = 1920;
            Texture2D texture = new Texture2D(width, height);
            texture.LoadImage(ImageMarkByte);
            thisSprite.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
    public void OpenDeleteUI(bool isopen)
    {
        delete.gameObject.SetActive(isopen);
        thisBtn.interactable = !isopen;
    }

    public void DeleteThis()
    {
        //请求本地数据库删除数据
        //传递编号
        DebugLog.DebugLogInfo("info  "+markInfo.bookmarkName);

        BookMarkManager.instance.DeleteBookmark(markInfo);
        DebugLog.DebugLogInfo("书签数据删除成功，接下来销毁书签");
        DestroyObject(this.gameObject);
    }

    public void ChangeName(InputField input)
    {
        if(!BookMarkManager.instance.isCreateBookmark)
            BookMarkManager.instance.UpdateBookmark(markInfo, (markInfo.type == "0") ? "FixCommand.db" : "Command.db",input.text);
    }
}
