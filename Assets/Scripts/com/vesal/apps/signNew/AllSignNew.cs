using SQLite4Unity3d;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [Serializable]
    public class SignNewInfo
    {

        [PrimaryKey]
        public string id { get; set; }
        public string sm_name { get; set; }
        public string sm_ch_name { get; set; }
        public string sm_en_name { get; set; }
        //public string start_point { get; set; }
        //public string stop_point { get; set; }


        public string r { get; set; }
        public string g { get; set; }
        public string b { get; set; }




        //public string start_desc { get; set; }
        // public string stop_desc { get; set; }
        public string description { get; set; }
        public string camera_params { get; set; }
        public string high_r { get; set; }
        public string high_g { get; set; }
        public string high_b { get; set; }
        // [PrimaryKey]
        // public string signnew_id { get; set; }
        //public string CameraPos { get; set; }

    }

    [Serializable]
    public class SignNewTex
    {
        public string mod_name { get; set; }
        public string main_tex { get; set; }
        public string mask_tex { get; set; }



    }

    [Serializable]
    public class noun_no_info
    {
        [PrimaryKey]
        public string sn_id { get; set; }
        public string noun_no { get; set; }
        public string sign_new_ids { get; set; }
        public string text_pair_list { get; set; }
    }

    [Serializable]
    public class GetTextureModelList
    {
        [PrimaryKey]
        public string tex_name { get; set; }
        public string model_list { get; set; }
    }
}