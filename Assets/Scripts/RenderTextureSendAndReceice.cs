using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class RenderTextureSendAndReceice : MonoBehaviour
{
    public RenderTexture recordTexture;
    public RawImage rawImg;
    public byte[] bytes;
    public byte[] oldbytes;
    public Texture2D texture;
    float timer = 0;
    public Texture2D png;

    public GameObject record_bg;
    // Use this for initialization
    void Start()
    {
        texture = new Texture2D(1920, 1080);
        // record_bg=GameObject.Find("bg_Image");
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.W))
        {
            SaveRenderTextureToBytes();
            //SaveRenderTextureToBytes(renderTexture, path, Time.time.ToString());
            // FileStream file = File.Open( "D:/2222.png", FileMode.Create);
            // BinaryWriter writer = new BinaryWriter(file);
            // writer.Write(SaveRenderTextureToBytes());
            // file.Close();
        }
        if (bytes.Length != 0 && timer >= 0.04f)
        {
            ReceiveBytesToTexture2D(bytes);
            oldbytes = bytes;
            timer = 0;
        }

        if(_fade)
        {
            SetRawImage(false);
            _fade=false;
        }
        if(_show)
        {
            SetRawImage(true);
            _show=false;
        }

    }

    public void SetFade()
    {
        _fade=true;
    }
    public void SetShow()
    {
        _show=true;
    }
    bool _fade=false;
    bool _show=false;
    public byte[] SaveRenderTextureToBytes()
    {
        RenderTexture rt = recordTexture;
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;   
        png = new Texture2D(1920,1080, TextureFormat.RGBA32, false);    
        png.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        png.Apply();
        byte[] bytes = png.EncodeToPNG();
        // byte[] bytes = renderTexture.EncodeToPNG();
        //DebugLog.DebugLogInfo("ff0000", bytes.Length + "");
        Texture2D.DestroyImmediate(png);
        //RenderTexture.active = prev;
        //Receive(bytes);
        return bytes;
    }


    public void ReceiveBytesToTexture2D(byte[] bytes)
    {
        texture.LoadImage(bytes);
        // Debug.Log(texture);
        rawImg.texture = texture;
    }

    public void SetRawImage(bool i)
    {
        record_bg.SetActive(i);
        rawImg.gameObject.SetActive(i);
        rawImg.raycastTarget=i;
    }
}
