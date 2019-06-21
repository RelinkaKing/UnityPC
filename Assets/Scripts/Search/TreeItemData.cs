using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeItemData
{
    public bool isShow = false;
    public bool isClick = false;
    public int indentLevel = -1;
    public int id = 0;
    public string title;

    public TreeItemData parent;
    public RectTransform target;
    public Text name;
    public Button button;

    public List<TreeItemData> childs = new List<TreeItemData>();

    public TreeItemData GetNode(int id)
    {
        if (id == this.id) return this;
        TreeItemData node = null;
        for (int i = 0; i < childs.Count; i++)
        {
            node = childs[i].GetNode(id);
            if (node != null)
            {
                break;
            }
        }
        return node;
    }

    public void SetTarget(Transform value)
    {
        target = value as RectTransform;
        name = target.GetComponentInChildren<Text>();
        button = target.GetComponentInChildren<Button>();
        name.text = title;
        Init(value);
    }


    public void Add(TreeItemData value)
    {
        childs.Add(value);
        value.parent = this;
    }

    protected virtual void Init(Transform value) { }
}
