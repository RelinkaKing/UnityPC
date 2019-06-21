using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XLua;

namespace Scripts
{
    [Hotfix]
    public class LoadLua : MonoBehaviour
    {

        public Text text;
        // Use this for initialization
        void Start()
        {
            
            
        }
        public void changeScene() {
            SceneManager.LoadScene("xluaTest2");
        }
        // Update is called once per frame
        void Update()
        {
            Show();
            if (Input.GetKeyDown(KeyCode.F1)) {
                SceneManager.LoadScene("xluaTest2");
            }
        }
        
        public void Show()
        {
            Debug.Log("Show!!!");
            text.text = "Show";
        }
    }
}

