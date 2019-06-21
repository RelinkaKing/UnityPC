using UnityEngine;
using System.Collections;

public class TransformToggleController : MonoBehaviour {

	private UIToggle toggle;
	public TransformTools.Type type;
    public TransformTools.Type Oppeartype;
    public UILabel label;

	private void Awake()
	{
		toggle = GetComponent<UIToggle> ();
	}
	public void SetCurrentTool()
	{
		//如果该开关被选中
		if (toggle != null && toggle.value ) 
		{
			SuperGameManager.Instance.transformType = type;
		}
	}

    public void SetCurrentType()
    {
        SuperGameManager.Instance.transformType = type;
    }

    public void SetToggle()
    {
        if (toggle != null && toggle.value)
        {
            Block_loadmodel.Instance.SetWireFrameActive(Oppeartype != TransformTools.Type.Normal);
        }
    }
}
