using UnityEngine;
using System.Collections;

    public class SegmentedButton : MonoBehaviour
    {

        
        public int index;
        public void GoTo()
        {
        AnimationControl.instance.SegmentedButton(index);
        }
    }
