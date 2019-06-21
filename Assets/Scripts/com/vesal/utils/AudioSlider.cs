using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour {
    private AudioSource audioSource;
    private int  currentMinute, currentSecond;
    private int  clipMinute, clipSecond;
    public Text audioTimeText;
    public Slider audioTimeSlider;
    public bool isStopping = true;

    public Button playButton;
    public Image buttonImage;
    public Sprite play, pause;
    public void OnMouseDrag()
    {
        isStopping = true;
        if (audioSource.isPlaying) {
            playButton.onClick.Invoke();
        }
    }
    // Use this for initialization
    void Start () {
        
        audioTimeSlider.onValueChanged.AddListener(delegate { SetAudioTimeValueChange(); });
	}
    public void init(AudioSource audioSource)
    {
        this.audioSource = audioSource;
        //clipMinute = (int)audioSource.clip.length / 3600;
        //clipSecond = (int)audioSource.clip.length - clipMinute * 3600 / 60;
        isStopping = false;
        audioTimeSlider.value = 0;
        currentMinute = 0;
        currentSecond = 0;
        clipMinute = 0;
        clipSecond = 0;
    }
    private void SetAudioTimeValueChange() {
        isStopping = false;
        if (audioSource!=null && audioSource.clip!=null) {
            audioSource.time = audioTimeSlider.value * audioSource.clip.length;
            if (audioSource.isPlaying != true) {
                audioSource.Play();
            }
        }
    }
	// Update is called once per frame
	void Update () {
        if (PPTGlobal.pptStatus == PPTGlobal.PPTStatus.pause)
        {
            this.transform.localScale = Vector3.zero;
            return;
        }
        else {
            this.transform.localScale = Vector3.one;
        }
        if (audioSource!=null && audioSource.clip!=null) {
            clipMinute = (int)audioSource.clip.length / 3600;
            clipSecond = (int)audioSource.clip.length - clipMinute * 3600 / 60;
            if (audioSource.isPlaying)
            {
                ShowAudioTime(audioSource.time);
            }
            //拖拽时
            if(isStopping) {
                ShowAudioTime((audioTimeSlider.value * Mathf.Max((audioSource.clip.length), 0.000001f)));
            }
            if (audioSource.isPlaying)
            {
                if (buttonImage.sprite != pause)
                {
                    buttonImage.sprite = pause;
                }
            }
            else {
                if (buttonImage.sprite == pause)
                {
                    buttonImage.sprite = play;
                }
            }
        }
    }
    private void ShowAudioTime(float time) {
        currentMinute = (int)time / 3600;
        currentSecond = (int)time - currentMinute * 3600 / 60;

        audioTimeText.text = string.Format("{0:D2}:{1:D2} / {2:D2}:{3:D2}",
            currentMinute, currentSecond, clipMinute, clipSecond);
        audioTimeSlider.value = time / Mathf.Max((audioSource.clip.length),0.000001f);
    }
}
