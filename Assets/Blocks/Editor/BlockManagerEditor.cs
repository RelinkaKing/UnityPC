using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BlockManager))]
public class BlockManagerEditor : Editor {

    protected BlockManager blockManager;
    bool isInited = false;

    void OnEnable()
    {
        blockManager = (BlockManager)target;
    }

    public override void OnInspectorGUI()
    {
        if (!isInited)
        {
            if (GUILayout.Button("初始化"))
            {
                blockManager.InitTargetPosition();
                isInited = true;
            }
        }
        
        DrawDefaultInspector();
    }
}
