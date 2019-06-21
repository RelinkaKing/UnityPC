using SQLite4Unity3d;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [Serializable]
    public class StartStopInfo
    {

        [PrimaryKey]
        public string mod_name { get; set; }
        public string ch_name { get; set; }
        public string en_name { get; set; }
        //public int ss_id { get; set; }
        //public string start_point { get; set; }
        //public string stop_point { get; set; }


        public string qr { get; set; }
        public string qb { get; set; }
        
        
        public string qg { get; set; }
        public string zr { get; set; }
        public string zb { get; set; }
        public string zg { get; set; }

        public string start_desc { get; set; }
        public string stop_desc { get; set; }
       
        public string camera_params { get; set; }
      
    }
    [Serializable]
    public class camera_params
    {
        public vector3 camera_pos;
        public vector3 camera_parent_rot;
        public vector3 camera_parent_pos;
    }

    [Serializable]
    public class StartStopTex
    {
        public string bone_name { get; set; }
        public string main_tex { get; set; }
        public string mask_tex { get; set; }

    

    }


        public class StartStopPoint
    {

        //StartStopInfo startStop;
        public Vector3 startCol;
        public Vector3 stopCol;
        //public string ch_name;
        //public string start_desc;
        //public string stop_desc;
        public StartStopPoint(StartStopInfo startStop)
        {
            try
            {
            startCol = new Vector3(float.Parse(startStop.qr) / 255.0f, float.Parse(startStop.qg) / 255.0f, float.Parse(startStop.qb) / 255.0f);
            stopCol=new Vector3(float.Parse(startStop.zr) / 255.0f, float.Parse(startStop.zg) / 255.0f, float.Parse(startStop.zb) / 255.0f);
            }
            catch (System.Exception)
            {

            }
        }

        public StartStopPoint()
        {

        }
    }
}