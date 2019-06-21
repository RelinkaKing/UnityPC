using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testPlayLoad : MonoBehaviour {

    [SerializeField]
    private RawImage image;
    [SerializeField]
    private List<Texture> animationSprites = new List<Texture>();

    public LoadAndInit loadAndInit;
    bool isClose;
    float alpha = 1;
    private int AnimationAmount { get { return animationSprites.Count; } }
    public void Start()
    {
        if (image == null) image = GetComponent<RawImage>();
        StartCoroutine(PlayAnimationForwardIEnum());
    }

    private IEnumerator PlayAnimationForwardIEnum()
    {
        int index = 0;//可以用来控制起始播放的动画帧索引
        gameObject.SetActive(true);
        while (true)
        {
            //当我们需要在整个动画播放完之后  重复播放后面的部分 就可以展现我们纯代码播放的自由性
            if (index > 30 - 1&&!isClose)
            {
                index = 0;
            }
            if (index >= AnimationAmount - 1)
            {
                Destroy(this.gameObject);
                loadAndInit.IsAutoRotation();
            }
            if (index == AnimationAmount)
                index = AnimationAmount - 1;
            image.texture = animationSprites[index];
            index++;
            yield return new WaitForSeconds(0.03f);//等待间隔  控制动画播放速度
        }
    }
    public void Close()
    {
       
        isClose = true;
     //   animationSprites.Clear();
      //  Destroy(this);
    }
}
