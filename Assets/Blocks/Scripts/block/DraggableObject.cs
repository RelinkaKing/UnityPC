//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections;

/// <summary>
/// Allows dragging of the specified target object by mouse or touch, optionally limiting it to be within the UIPanel's clipped rectangle.
/// </summary>

[ExecuteInEditMode]

public class DraggableObject : MonoBehaviour
{
	
	
	public Transform target;

	
	public Vector3 dragMovement = Vector3.one;

	

	
	Plane mPlane;
	Vector3 mTargetPos;
	Vector3 mLastPos;
	Vector3 mMomentum = Vector3.zero;
	Vector3 mScroll = Vector3.zero;
	int mTouchID = 0;
	bool mStarted = false;
	bool mPressed = false;

	
	void OnDisable () 
	{ 
		mStarted = false; 
	}

	

	/// <summary>
	/// Create a plane on which we will be performing the dragging.
	/// </summary>
	
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
	
	/// <summary>
	/// Drag the object along the plane.
	/// </summary>
	
	void OnDrag (Vector2 delta)
	{
		if (SuperGameManager.Instance.transformType != TransformTools.Type.Translate) 
		{
			return;
		}
		if (mPressed && mTouchID == UICamera.currentTouchID && enabled && NGUITools.GetActive(gameObject) && target != null)
		{
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
			
			Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);
			float dist = 0f;
			
			if (mPlane.Raycast(ray, out dist))
			{
				Vector3 currentPos = ray.GetPoint(dist);
				Vector3 offset = currentPos - mLastPos;
				mLastPos = currentPos;
				
				if (!mStarted)
				{
					mStarted = true;
					offset = Vector3.zero;
				}
				
				if (offset.x != 0f || offset.y != 0f)
				{
					offset = target.InverseTransformDirection(offset);
					offset.Scale(dragMovement);
					offset = target.TransformDirection(offset);
				}
				
				
				// Adjust the position and bounds
				Vector3 before = target.localPosition;
				Move(offset);
			}
		}
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
	
	/// <summary>
	/// Cancel all movement.
	/// </summary>
	
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

}
