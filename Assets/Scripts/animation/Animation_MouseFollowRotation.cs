using UnityEngine;
using System.Collections;
using UnityEngine.UI;

    public class Animation_MouseFollowRotation : MonoBehaviour
    {
        public float distance = 20, minDis = 1, maxDis = 40;
        public GameObject emptyBox;     //旋转轴
        float positionX;
        float positionY;
        float positionZ;
        float distanceSpeed = 100;
        public float xSpeed = 2; //滑动系数
        public float ySpeed = 2;
        public float x, x1; //摄像头位置
        public float y, y1;
        //记录上一次手指触摸位置判断用户时放大还是缩小手势
        Vector2 oldPosition1;
        Vector2 oldPosition2;
        Vector3 oldPosition;
        Vector3 direction;
        public float zoom = 5f;
        public float translation = 0.05f;

    private void OnEnable()
    {
            emptyBox = Camera.main.transform.parent.gameObject;
        
    }
    // Use this for initialization
    void Start()
        {
            right = false;
        }

        bool right = true;

        private void Update()
        {
            ////是否处于旋转状态
            //if (right)
            //{
            //    //根据旋转时间改变控制参数
            //    x += Time.deltaTime * 30;
            //    emptyBox.transform.rotation = Quaternion.Euler(y, x, 0);
            //    //鼠标左键点击时，并且没有点击在缩放滑动条上并且没有处于帮助状态
            //    if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began && !gameObject.GetComponent<AnimationControl>().onUISilder)
            //    {
            //        AutoRotate();
            //    }
            //}
            //if (!gameObject.GetComponent<AnimationControl>().onUISilder)
            //{
            //    if (Input.GetMouseButton(0))
            //    {
            //        x += Input.GetAxis("Mouse X") * xSpeed;
            //        y -= Input.GetAxis("Mouse Y") * ySpeed;
            //    }
            //    distance -= Input.GetAxis("Mouse ScrollWheel") * distance;
            //    distance = Mathf.Clamp(distance, minDis, maxDis);
            //    Quaternion rotation = Quaternion.Euler(y, x, 0.0f);
            //    Vector3 disVector = new Vector3(0, 0, -distance);
            //    emptyBox.transform.rotation = rotation;
            //    transform.localPosition = disVector;
            //}
        }
        public Image tod, lor, bof;
        bool isd = false, isr = false, isb = false;
        public Sprite[] sprite;
        public Sprite[] sprite0;
        public Sprite[] sprite1;
        public void TOD()
        {
            if (isd)
            {
                ChangeDirection("Up");
                tod.sprite = sprite[4];
                isd = false;
            }
            else
            {
                ChangeDirection("Down");
                tod.sprite = sprite[5];
                isd = true;
            }
        }
        public void LOR()
        {
            if (isr)
            {
                ChangeDirection("Left");
                lor.sprite = sprite[3];
                isr = false;
            }
            else
            {
                ChangeDirection("Right");
                lor.sprite = sprite[2];
                isr = true;
            }
        }
        public void BOF()
        {
            if (isb)
            {
                ChangeDirection("Forward");
                bof.sprite = sprite[1];
                isb = false;
            }
            else
            {
                ChangeDirection("Back");
                bof.sprite = sprite[0];
                isb = true;
            }
        }

        float xTemp = 0, yTemp = 0;
        //方向切换New
        void ChangeDirection(string direction)
        {
            //判断方向
            switch (direction)
            {
                case "Up":
                    xTemp = 0; yTemp = 90;
                    break;
                case "Down":
                    xTemp = 0; yTemp = -90;
                    break;
                case "Left":
                    xTemp = 90; yTemp = 0;
                    break;
                case "Right":
                    xTemp = -90; yTemp = 0;
                    break;
                case "Forward":
                    xTemp = 0; yTemp = 0;
                    break;
                case "Back":
                    xTemp = 180; yTemp = 0;
                    break;
                default:
                    xTemp = 0; yTemp = 0;
                    break;
            }
            //旋转动画
            iTween.RotateTo(emptyBox, iTween.Hash("rotation", new Vector3(yTemp, xTemp, 0), "time", 1));
            //1s后改变控制参数
            Invoke("TimeSleep", 1.0f);
        }

        void TimeSleep()
        {
            x = xTemp;
            y = yTemp;
        }
        //360旋转
        public Image autoRotate;
        public Sprite[] sprites;
        public void AutoRotate()
        {
        //Camera.main.GetComponent<XT_MouseFollowRotation>().To_360();
        }
    }
