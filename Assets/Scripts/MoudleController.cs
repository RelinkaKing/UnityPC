using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class MoudleController : MonoBehaviour {

    public static MoudleController Instance;
    public XT_TouchContorl touchControl;

    public GameObject UI_canvas;
    public GameObject UI_canvas_rot;

    void Awake () {

    }

    void Start () {
        //UIBtnFlag.InitRightMenuFlag ();
        //get_process_hwnd();

    }
    void Update () {
        
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Home))
{
        if(BookMarkManager.instance.BookmarkPanel.activeSelf)
        {
            BookMarkManager.instance.SetBookmarkPanel(false);
        }
        else
        {
            if(XT_AllButton.Instance.isExitPanelDown)
            {
            XT_AllButton.Instance.BackCancel();

            }
            else{

            XT_AllButton.Instance.ExitButtonClick();
            }
        }
}
#elif UNITY_STANDALONE_WIN

#else 
//移动端
        if(Input.GetKeyDown(KeyCode.Home)||Input.GetKeyDown(KeyCode.Escape))
        {
            if(BookMarkManager.instance.BookmarkPanel.activeSelf)
            {
                BookMarkManager.instance.SetBookmarkPanel(false);
            }
            else
            {
                if(XT_AllButton.Instance.isExitPanelDown)
                {
                    XT_AllButton.Instance.BackCancel();
                }
                else{
                    XT_AllButton.Instance.ExitButtonClick();
                }
            }
        }
#endif
    }

    IntPtr _hwnd;
    public IntPtr get_process_hwnd (int micro_seconds = 10000) {
        long num = DateTime.Now.Ticks / 0x2710L;
        while (true) {
            this._hwnd = Process.GetCurrentProcess ().MainWindowHandle;
            if (this._hwnd != IntPtr.Zero) {
                return this._hwnd;
            }
            long num2 = DateTime.Now.Ticks / 0x2710L;
            if (num2 >= (num + micro_seconds)) {
                this._hwnd = IntPtr.Zero;
                return IntPtr.Zero;
            }
            //Thread.Sleep(1);
        }
    }
    public void set_win_max () {
        ShowWindow (this._hwnd, 3);
    }

    [DllImport ("user32.dll", SetLastError = true)]
    private static extern bool ShowWindow (IntPtr hWnd, uint nCmdShow);

    public GameObject littleMan;

    public GridLayoutGroup left_list; 
    public GridLayoutGroup right_list; 

    public void ChangeLayoutGroupCount(bool isLS)
    {
        if(left_list!=null && right_list!=null)
        {

        if(isLS)
        {
            left_list.constraintCount=2;
            right_list.constraintCount=2;
            left_list.padding.left=0;
            right_list.padding.left=0;
        }
        else
        {

            left_list.constraintCount=1;
            right_list.constraintCount=1;
            left_list.padding.left=-90;
            right_list.padding.left=120;
        
        }

        }
    }


    bool is_switch = false;
    public static bool isLanScaple = true;
    public void RotateUI () {
        if (is_switch) {
            Display.Rotate (1, Orientations.DEGREES_CW_0);
            isLanScaple = true;
            if (PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.plugin) {
                Screen.SetResolution (1920, 1080, false);
            }
            littleMan.transform.localScale = Vector3.one;
        } else {
            littleMan.transform.localScale = 0.5f * Vector3.one;
            Display.Rotate (1, Orientations.DEGREES_CW_270);
            isLanScaple = false;
            ChangeLayoutGroupCount(false);
            if (PPTGlobal.PPTEnv != PPTGlobal.PPTEnvironment.plugin) {
                Screen.SetResolution (1080, 1920, false);
            }
        }

        DebugLog.DebugLogInfo ("width " + Screen.width + "  height " + Screen.height);
        is_switch = !is_switch;
    }

    private void OnDestroy () {
        Destroy (this);
    }
}