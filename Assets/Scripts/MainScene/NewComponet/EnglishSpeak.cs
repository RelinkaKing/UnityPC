using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using VesalCommon;
public class EnglishSpeak : MonoBehaviour {

	public static EnglishSpeak instance;
	public AudioSource curAudioSound;
	public GameObject speakBtn;
	public Text english;
	public Text signenglish;
	AssetBundle voiceAB;
    void Awake () {
		instance=this;
	}

    void UnZip(string filePath,Action call)
	{
		if (File.Exists (filePath))
		{
			StartCoroutine(Vesal_DirFiles.unzipInThread(filePath, Vesal_DirFiles.get_dir_from_full_path(filePath),()=>{
				call();
				Vesal_DirFiles.DelFile (filePath);}));
		}
		else
		{
			call();
		}
	}

    private void Start() {
		//LoadAudio();
	}

    //音频bundle包
    void LoadAudio()
	{
		try
		{
			voiceAB= AssetBundle.LoadFromFile(PublicClass.filePath+"mp3.assetbundle");		
		}
		catch (System.Exception)
		{				
			DebugLog.DebugLogInfo( "mp3.assetbundle null");
		}	
	}

    //播放音频按钮调用
    public void PrefLoadAudio()
	{
		PlayAssetClip(english.text);
		// StartCoroutine(PlayEnglishClip(english.text));
	}
	public void PlaySignAudio()
	{
		PlayAssetClip(signenglish.text);
		// StartCoroutine(PlayEnglishClip(english.text));
	}
	IEnumerator PlayEnglishClip(string english)
	{	
		using (var uwr = UnityWebRequestMultimedia.GetAudioClip(english, AudioType.MPEG))
		{
			yield return uwr.Send();
			if (uwr.isNetworkError)
			{
				Debug.LogError(uwr.error);
				yield break;
			}
			curAudioSound.clip = DownloadHandlerAudioClip.GetContent(uwr);
			curAudioSound.Play();
		}
	}

	public void PlayAssetClip(string _name)
	{
		string name=_name;
		if(_name.IndexOf("_")!=-1)
			name=_name.Remove(_name.IndexOf("_"),2);
		DebugLog.DebugLogInfo(name);
		AudioClip clip;
		
		if(voiceAB.Contains(name))
		{
			clip=voiceAB.LoadAsset<AudioClip>(name);
			curAudioSound.clip=clip;
			curAudioSound.Play();
		}
		else
		{		
			// speakBtn.SetActive(false);
			DebugLog.DebugLogInfo("音频不存在");
		}	
	}

	public void CheckHasAudioClip(string english)
	{
		speakBtn.SetActive (File.Exists(PublicClass.filePath+english+".mp3"));
	}

	private void OnDestroy() {
		if(voiceAB!=null)
			voiceAB.Unload(false);
	}
}
