using UnityEngine;
using System.Collections;

public class CameraPoint : MonoBehaviour
{
    public Vector3 m_rot = new Vector3(0f, 180f, 0f);//焦点的角度
    public static CameraPoint Instance = null;

    void Awake()
    {
        Instance = this;
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "CameraPoint.tif");
    }
    public void SetDestination(Vector3 destination)
	{
		if (SuperGameManager.Instance.autoLocate) 
		{//如果出于自动定位模式
			//TargetSpawnController.instance.SpawnTarget(m_transform, UICamera.lastWorldPosition);//最新的关注点！！！！！！！！！！！！！！！！！！！！！！！
			UITweener tween1 = TweenPosition.Begin(gameObject, 0.7f, destination);
			tween1.method = UITweener.Method.EaseOut;
			tween1.SetOnFinished(SuperGameManager.Instance.ControlRegionTwo);
			SuperGameManager.Instance.ControlRegion();//这个就是关键
		}

	}
}
