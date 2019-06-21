using System.Collections.Generic;

public interface IQaDao
{
    void CloseSQLConnection();
    void CreateDataBase();
    void getSignInfo(string id, out string mapAbName, out string mapModelName);
    Dictionary<string, int> getTopErrorQa(int limit);
    List<string> getTopErrorQaId(int limit, string bankName, string libName);
    void InsertData(string id, string bankName, string libName, bool isError);
    void OpenDb();
    void recordDb();
    void recordErrorQa(string id, string bankName, string libName, bool isError);
    void Start();
}