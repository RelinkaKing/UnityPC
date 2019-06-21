using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateData : MonoBehaviour {
    public static StateData Instance;
    public List<string> StatePath = new List<string>();
    public bool isReturn;
    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }


}
