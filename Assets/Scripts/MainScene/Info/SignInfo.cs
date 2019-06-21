using UnityEngine;
using System.Collections;

public class SignInfo {

    private string modelName;
    private string chinese;
    private string signID;        //哪个钉子
    private string english;
    private string explain;



    public string ModelName { get { return modelName; } set { modelName = value; } }
    public string Chinese { get { return chinese; } set { chinese = value; } }
    public string SignID { get { return signID; } set { signID = value; } }

    public string English
    {
        get { return english; }
        set { english = value; }
    }

    public string Explain { get { return explain; } set { explain = value; } }
}
