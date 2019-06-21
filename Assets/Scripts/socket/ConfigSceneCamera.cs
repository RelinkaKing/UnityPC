using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigSceneCamera : MonoBehaviour {

	public bool is_set_camera=true;
	public GameObject bg_image;
	bool _set=false;
	bool _is_start=false;

	void Start () {
	}

	/// <summary>
	/// is_start 为true 表示进入录制模式
	/// </summary>
	/// <param name="is_start"></param>
	void startSet(bool is_start)
	{
		_is_start=is_start;
		_set=true;
	}

	void OpeareTargetTexture()
	{
		bg_image.SetActive(_is_start);
	}

	void Update()
	{
		// if(_set)
		// {
		// 	OpeareTargetTexture();
		// 	_set=false;
		// }
	}
}
