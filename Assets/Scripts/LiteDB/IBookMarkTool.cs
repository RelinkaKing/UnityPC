using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBookMarkTool{

    I insertOne<T, I>(T t,string id) where T : class where I : struct;        //返回值类型
    List<T> selectAll<T>() where T : class;
    O updateOne<T,O>(T t) where T : class where O : class;
    O deleteOne<T,O>(T t)where T : class where O : struct;
    void openDb(string dnName);
    void closeDb();
}
