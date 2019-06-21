using UnityEngine;
using System.Collections;

public class RotatableObject : MonoBehaviour {
    public Block block;
	public enum RotateEffect
	{
		None,
		Momentum,
		MomentumAndSpring,
	}
	
	/// <summary>
	/// Target object that will be dragged.
	/// </summary>
	
	public Transform target;
	
	
	/// <summary>
	/// Scale value applied to the drag delta. Set X or Y to 0 to disallow dragging in that direction.
	/// </summary>
	
	public float rotateMovement = 1f;
	
	/// <summary>
	/// Effect to apply when dragging.
	/// </summary>
	
	//public RotateEffect rotateEffect = RotateEffect.None;
	
	/// <summary>
	/// How much momentum gets applied when the press is released after dragging.
	/// </summary>
	
	//public float momentumAmount = 35f;
	
	
	//Plane mPlane;
	//Vector3 mTargetPos;
	//Vector3 mLastPos;
	//Vector3 mMomentum = Vector3.zero;
	//Vector3 mScroll = Vector3.zero;
	//int mTouchID = 0;
	//bool mStarted = false;
	//bool mPressed = false;
	
	
	//void OnDisable () 
	//{ 
		//mStarted = false; 
	//}
    void OnStart()
    {
        //
    }
	
	
	/// <summary>
	/// Create a plane on which we will be performing the dragging.
	/// </summary>
	/**
	void OnPress (bool pressed)
	{
		if (enabled && NGUITools.GetActive(gameObject) && target != null)
		{
			if (pressed)
			{
				if (!mPressed)
				{
					// Remove all momentum on press
					mTouchID = UICamera.currentTouchID;
					mPressed = true;
					mStarted = false;
					
					// Create the plane to drag along
					Transform trans = UICamera.currentCamera.transform;
					mPlane = new Plane(trans.rotation * Vector3.back, UICamera.lastWorldPosition);
				}
			}
			else if (mPressed && mTouchID == UICamera.currentTouchID)
			{
				mPressed = false;
			}
		}
	}
	**/

	public bool isRotating=false;

	/// <summary>
	/// Drag the object along the plane.
	/// </summary>
	
	void OnDrag (Vector2 delta)
	{
		isRotating=true;
        block = GetComponent<Block>();
        if (block.isFinished)
        {
            return;
        }
		if (SuperGameManager.Instance.transformType != TransformTools.Type.Rotate || target == null) 
		{
			return;
		}
		//Debug.Log ("旋转有执行吗？？？？？？？？？？？？？？？？？？？？？");
		//x方向的滑动角度
		float distanceX = delta.x * rotateMovement;
		//y方向的滑动角度
		float distanceY = delta.y * rotateMovement;

		//Quaternion tmpQuaternionX = new Quaternion (Camera.main.transform.up, distanceX);
		Quaternion tmpQuaternionX = Quaternion.AngleAxis (-distanceX, Camera.main.transform.up);
		Quaternion tmpQuaternionY = Quaternion.AngleAxis (distanceY, Camera.main.transform.right);

		//获取自身的四元数
		Quaternion myQuaternion = target.rotation;

		tmpQuaternionX = tmpQuaternionX * tmpQuaternionY;
		tmpQuaternionX = tmpQuaternionX * myQuaternion;

		target.rotation = tmpQuaternionX;
	}
	
	/// <summary>
	/// Move the dragged object by the specified amount.
	/// </summary>
	
	void Move (Vector3 worldDelta)
	{
		target.position += worldDelta;
	}
	
	/// <summary>
	/// Apply the dragging momentum.
	/// </summary>
	/**
	void LateUpdate ()
	{
		#if UNITY_EDITOR
		if (!Application.isPlaying) return;
		#endif
		if (target == null) return;
		float delta = RealTime.deltaTime;
		
		mMomentum -= mScroll;
		mScroll = NGUIMath.SpringLerp(mScroll, Vector3.zero, 20f, delta);
		
		// No momentum? Exit.
		if (mMomentum.magnitude < 0.0001f) return;
		
		if (!mPressed)
		{
			
			Move(NGUIMath.SpringDampen(ref mMomentum, 9f, delta));
			
			// Dampen the momentum
			NGUIMath.SpringDampen(ref mMomentum, 9f, delta);
			
			// Cancel all movement (and snap to pixels) at the end
			if (mMomentum.magnitude < 0.0001f) CancelMovement();
		}
		else NGUIMath.SpringDampen(ref mMomentum, 9f, delta);
	}
	**/
	/// <summary>
	/// Cancel all movement.
	/// </summary>
	/**
	public void CancelMovement ()
	{
		if (target != null)
		{
			Vector3 pos = target.localPosition;
			pos.x = Mathf.RoundToInt(pos.x);
			pos.y = Mathf.RoundToInt(pos.y);
			pos.z = Mathf.RoundToInt(pos.z);
			target.localPosition = pos;
		}
		mTargetPos = (target != null) ? target.position : Vector3.zero;
		mMomentum = Vector3.zero;
		mScroll = Vector3.zero;
	}
    **/
}
