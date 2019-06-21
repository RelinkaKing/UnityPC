using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IbaseClass{

    T selectOneById<T,I>(I i) where T:class where I:struct;
    I insertOne<T, I>(T t) where T : class where I : struct;
    List<T> selectAll<T>() where T : class;
    O updateOne<T,O>(T t) where T : class where O : class;
    O deleteOne<T, O>(T t) where T : class where O : class;
}
