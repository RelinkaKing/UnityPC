using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections;

// Create a menu item that causes a new controller and statemachine to be created.

public class CreateAniController : MonoBehaviour
{
    [MenuItem("Ani/Create Controller")]
    static void CreateController()
    {
        // Creates the controller
        var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Assets/ani/SplitController.controller");

        // Add parameters
        controller.AddParameter("cat_index", AnimatorControllerParameterType.Int);
        
        var rootStateMachine = controller.layers[0].stateMachine;
        var stateA1 = rootStateMachine.AddState("idle");
        
        AnimatorStateTransition any = rootStateMachine.AddAnyStateTransition(stateA1);
        any.hasExitTime = false;
        any.AddCondition(AnimatorConditionMode.Equals, 0.0f, "cat_index");
        for (int i = 1;i<17;i++) {
            var cat_1 = rootStateMachine.AddState("cat_"+i);
            AnimatorStateTransition any2 = rootStateMachine.AddAnyStateTransition(cat_1);
            any2.hasExitTime = true;
            any2.AddCondition(AnimatorConditionMode.Equals,i, "cat_index");
        }
      
    }

    private static void AddStateTransition(string path, AnimatorControllerLayer layer)
	{
		// AnimatorStateMachine sm = layer.stateMachine;
		// AnimationClip[] clips = AssetDatabase.LoadAllAssetRepresentationsAtPath(path, typeof(AnimationClip)) as AnimationClip;
		// //根据动画文件读取它的AnimationClip对象
		// AnimationClip newClip = AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip)) as AnimationClip;
		// //取出动画名子 添加到state里面
		// AnimatorState state = sm.AddState(newClip.name);
		// state.SetAnimationClip(newClip,layer);
		// //把state添加在layer里面
		// Transition trans = sm.AddAnyStateTransition(state);
		// //把默认的时间条件删除
        //         trans.RemoveCondition(0);
	}

    static string Fbx_path="Assets/JS/胸部健身动画.FBX";

    [MenuItem("MyMenu/Test")]
	static void DoCreateAnimationAssets() 
	{
		//创建animationController文件，保存在Assets路径下
		AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath("Assets/ani/SplitController.controller");
		//得到它的Layer， 默认layer为base 你可以去拓展
		AnimatorControllerLayer layer = animatorController.layers[0];
		//把动画文件保存在我们创建的AnimationController中
		AddStateTransition(Fbx_path,layer);
	}
}