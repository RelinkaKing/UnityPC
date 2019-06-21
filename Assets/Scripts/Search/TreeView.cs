using System;
using System.Collections.Generic;
using UnityEngine;

namespace SearchComponet
{
public class TreeView : MonoBehaviour
{

    [SerializeField] private Transform template;
    [SerializeField] private RectTransform itemParent;
    [SerializeField] private RectTransform conent;

    [Header("setting")]
    [SerializeField]
    private bool isChangedToggle = true;
    [SerializeField] private float indent;
    [SerializeField] private float space;
    [SerializeField] private Vector2 itemSize;
    [SerializeField] private Rect padding;

    public Action<TreeItemData, bool> onClick;
    public Action<TreeItemData> onCreateItem;
    public Action<TreeItemData> onHideItem;

    float currentHeight;
    TreeItemData root = new TreeItemData();
    List<TreeItemData> datas = new List<TreeItemData>();
    List<GameObject> items = new List<GameObject>();
    Dictionary<int, Vector2> customsSize = new Dictionary<int, Vector2>();
    Dictionary<int, float> customsSpace = new Dictionary<int, float>();

    /// <summary>
    /// 设置树的数据，需要配合Generate使用
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="id"></param>
    /// <param name="name"></param>
    public void AddData<T>(int parent, int id, string name)where T:TreeItemData , new()
    {
        var item = new T { id = id, title = name };
        var node = root.GetNode(parent);
        if (node == null)
        {
            Debug.LogException(new Exception("parent node is null!"));
            return;
        }
        node.Add(item);
        item.indentLevel = node.indentLevel + 1;
        datas.Add(item);
    }

    /// <summary>
    /// 设置树数据，并生成对象
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="id"></param>
    /// <param name="name"></param>
    public void AddItem<T>(int parent, int id, string name) where T : TreeItemData, new()
    {
        AddData<T>(parent, id, name);
        CreateItem(datas[datas.Count - 1]);
        UpdatePos();
        UpdateSize();
    }

    /// <summary>
    /// 移除节点
    /// </summary>
    /// <param name="id"></param>
    public void Remove(int id)
    {
        var node = root.GetNode(id);
        if (node == null)
        {
            Debug.LogException(new Exception("node is null!"));
            return;
        }
        DeleteItem(node);
    }

    /// <summary>
    /// 清空树
    /// </summary>
    public void Clear()
    {
        ClearItem();
        root = new TreeItemData();
        datas.Clear();
        conent.sizeDelta = Vector2.zero;
    }

    /// <summary>
    /// 设置节点名字
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    public void SetTitle(int id, string value)
    {
        var node = root.GetNode(id);
        if (node == null)
        {
            Debug.LogException(new Exception("node is null!"));
            return;
        }
        node.title = value;
        node.name.text = value;
    }

    /// <summary>
    /// 获得节点名字
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string GetTitle(int id)
    {
        var node = root.GetNode(id);
        if (node == null)
        {
            Debug.LogException(new Exception("node is null!"));
            return string.Empty;
        }
        return node.title;
    }

    /// <summary>
    /// 自定义ID下的子对象大小
    /// </summary>
    /// <param name="id"></param>
    /// <param name="size"></param>
    public void CustomsChildSize(int id, Vector2 size)
    {
        customsSize[id] = size;
    }

    /// <summary>
    /// 清空自定义大小
    /// </summary>
    public void ClearCustomsSize()
    {
        customsSize.Clear();
    }

    /// <summary>
    /// 自定义ID下的子对象间隔
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    public void CustomsSpace(int id, float value)
    {
        customsSpace[id] = value;
    }

    /// <summary>
    /// 清空自定义间隔
    /// </summary>
    public void ClearCustomsSpace()
    {
        customsSpace.Clear();
    }

    /// <summary>
    /// 打开或关闭节点
    /// </summary>
    /// <param name="node"></param>
    /// <param name="value"></param>
    public void OpenNode(TreeItemData node, bool value)
    {
        int count = node.childs.Count;
        for (int i = 0; i < count; i++)
        {
            node.childs[i].target.gameObject.SetActive(value);
            node.childs[i].isShow = value;
        }
        UpdatePos();
        UpdateSize();
    }

    /// <summary>
    /// 根据当前树数据生成对象
    /// </summary>
    public void Generate()
    {
        ClearItem();

        int count = datas.Count;
        for (int i = 0; i < count; i++)
        {
            CreateItem(datas[i]);
        }
        UpdatePos();
        UpdateSize();
        OpenNode(root, true);
    }
    
    private void UpdatePos()
    {
        currentHeight = 0;
        UpdatePos(root);
    }


    private void UpdatePos(TreeItemData data, int index = -1)
    {
        if (data != root)
        {
            if (data.target.gameObject.activeSelf)
            {
                if (!(index == 0 && data.parent.id == 0))
                {
                    if (index != 0) currentHeight += GetSpace(data);
                    else currentHeight += space;
                }
                data.target.localPosition = itemParent.localPosition - new Vector3(-data.indentLevel * indent - padding.x, currentHeight + padding.y);
                currentHeight += GetHeight(data);
            }
        }

        int count = data.childs.Count;
        for (int i = 0; i < count; i++)
        {
            UpdatePos(data.childs[i], i);
        }
    }

    private void UpdateSize()
    {
        int count = items.Count;
        Vector2 max = Vector2.zero;
        for (int i = 0; i < count; i++)
        {
            if (!datas[i].isShow) continue;
            var tmp = items[i].transform as RectTransform;

            var size = new Vector2(tmp.localPosition.x, -tmp.localPosition.y) + tmp.sizeDelta;

            max.x = Mathf.Max(max.x, size.x);
            max.y = Mathf.Max(max.y, size.y);
        }
        conent.sizeDelta = new Vector2(max.x + padding.width, max.y + padding.height);
    }
    


    private void DeleteItem(TreeItemData value)
    {
        int count = value.childs.Count;
        for (int i = 0; i < count; i++)
        {
            DeleteItem(value.childs[i]);
        }

        value.parent.childs.Remove(value);
        datas.Remove(value);
        items.Remove(value.target.gameObject);
        GameObject.Destroy(value.target.gameObject);
        UpdatePos();
        UpdateSize();
    }

    private int GetShowCount()
    {
        int tmp = 0;
        int count = datas.Count;
        for (int i = 0; i < count; i++)
        {
            if (datas[i].isShow) tmp++;
        }
        return tmp;
    }

    private int GetMaxIdent(TreeItemData value, int max = 0)
    {
        int count = value.childs.Count;
        for (int i = 0; i < count; i++)
        {
            if (value.childs[i].isShow) max = Mathf.Max(GetMaxIdent(value.childs[i], Mathf.Max(max, value.childs[i].indentLevel)), max);
        }
        return max;
    }

    private void ClearItem()
    {
        int count = items.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject.Destroy(items[i].gameObject);
        }
        items.Clear();
    }

    private void CreateItem(TreeItemData value)
    {
        var item = Instantiate(template);
        item.SetParent(itemParent, false);
        value.SetTarget(item.transform);

        if (customsSize.ContainsKey(value.parent.id))
        {
            value.target.sizeDelta = customsSize[value.parent.id];
        }
        else
        {
            value.target.sizeDelta = itemSize;
        }

        value.button.onClick.AddListener(() =>
        {
            value.isClick = !value.isClick;
            OnItemValueChanged(value, value.isClick);
            if ( onClick != null)
            {
                onClick(value, value.isClick);
            }
        });
        if (onCreateItem != null) onCreateItem(value);
        items.Add(item.gameObject);
    }

    private float GetHeight(TreeItemData data)
    {
        return customsSize.ContainsKey(data.parent.id) ? customsSize[data.parent.id].y : itemSize.y;
    }

    private float GetSpace(TreeItemData value)
    {
        return customsSpace.ContainsKey(value.parent.id) ? customsSpace[value.parent.id] : space;
    }

    private void OnItemValueChanged(TreeItemData value, bool isOn)
    {
        if (isOn)
        {
            OpenNode(value, true);

        }
        else
        {
            SetItemActive(value, false);
        }
        UpdatePos();
        UpdateSize();
    }

    private void SetItemActive(TreeItemData value, bool active)
    {
        int count = value.childs.Count;
        if (onHideItem != null && !active)
        {
            value.isClick = false;
            onHideItem(value);
        }
        for (int i = 0; i < count; i++)
        {
            value.childs[i].target.gameObject.SetActive(active);
            value.childs[i].isShow = active;
            SetItemActive(value.childs[i], active);
        }
    }
}

}