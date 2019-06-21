using System;
using System.ComponentModel;
using UnityEngine.Networking;

public interface IWebServicesBase
{
    UnityWebRequest Get();
    UnityWebRequest Post();
    UnityWebRequest PostWithParams(servelets servelet = servelets.none, params KeyValue[] postParams);
    
}
public enum servelets
{
    
    [Description("studentClass/joinClass")] joinClass = 0,
    [Description("studentClass/getClasses")] getClasses = 1,
    [Description("studentClass/studentRegister")] studentRegister = 2,
    [Description("studentClass/isStudentExists")] isStudentExists = 3,
    [Description("studentTestpaper/studentTestpaperTitle")] studentTestpaperTitle = 4,
    [Description("studentClass/isClassesExists")] isClassesExists = 5,
    [Description("wrongTitle/selectStudentWrongTitlteNo")] selectStudentWrongTitlteNo = 6,
    [Description("studentTestpaper/commitScore")] commitScore = 7,
    [Description("studentTestpaper/queryScore")] queryScore = 8,
    [Description("wrongTitle/deleteWrongTitle")] deleteWrongTitle = 9,
    [Description("")] none = -1
}
public enum stateCode {
    [Description("200")] access = 0,
}
[Serializable]
public class ResponseJson {
    public string stateCode;
    public string message;
    public StuInfo[] stuInfo;
    public ClassInfo[] classesList;
    public string result;
    public Object data;
    public schoolInfo[] school;
}
[Serializable]
public class schoolInfo {
    public string code;
    public string name;
}


