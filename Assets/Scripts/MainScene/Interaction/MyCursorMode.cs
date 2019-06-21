using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MyCursorMode : MonoBehaviour {


    Model LastModel = null;
    Model currentModel;
    // Use this for initialization

    Vector3 offset=new Vector3(0,150);
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


        transform.position = Input.mousePosition;
          SelectModel();
            
        

    }


    public void SelectModel()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition+offset);
        //Debug.Log(Input.mousePosition + offset);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit))
        {
            Model rayTarget = hit.transform.GetComponent<Model>();
            if (rayTarget != null)
            {
                currentModel = rayTarget;

                //currentModel.BecomeMouseColor();
                if (LastModel == null)
                {
                    LastModel = rayTarget;
                }
                else
                {
                    if (LastModel.isSeleted)
                        LastModel.BecomeHight();
                    else if (LastModel.isTranslucent)
                        LastModel.BecomeTranslucent();
                    else
                        LastModel.BecomeNormal();
                    LastModel = currentModel;
                }

                rayTarget.BecomeMouseColor();

                ////根据射线目标选择模型
                //SceneModels.instance.ChooseModel(currentModel);
                ////UI界面显示相关信息
                XT_TouchContorl.Instance.GiveUiValue_new(rayTarget.GetComponent<Model>());

                // modelName.text=LoadModel.instance.notes[rayTarget.name].chinese;
            }
        }
        else
        {
            if (currentModel != null)
            {
                if (currentModel.isSeleted)
                    currentModel.BecomeHight();
                else if (currentModel.isTranslucent)
                    currentModel.BecomeTranslucent();
                else
                    currentModel.BecomeNormal();
            }
        }
    }

    public  void Select()
    {
        if (currentModel != null) {
        //根据射线目标选择模型
        SceneModels.instance.ChooseModel(currentModel);
        //UI界面显示相关信息
        XT_TouchContorl.Instance.GiveUiValue_new(currentModel.GetComponent<Model>());
        }
    }

    private void OnDestroy()
    {
        if (currentModel != null)
        {
            currentModel.BecomeNormal();
        }
        if (LastModel != null)
        {
            LastModel.BecomeNormal();
        }
        
    }



}
