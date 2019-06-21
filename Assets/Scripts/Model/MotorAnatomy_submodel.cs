using SQLite4Unity3d;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Assets.Scripts.Model
{
    class MotorAnatomy_submodel
    {
        [PrimaryKey]
        public string ID { get; set; }

        public string ChineseName { get; set; }
        public string ModelName { get; set; }
        public string EnglishName { get; set; }
    }
    class acu
    {
        [PrimaryKey]
        public string no { get; set; }

        public string cn_name { get; set; }
        public string guanlian { get; set; }
    }
    class AcuNounSubmodel
    {

        public string no { get; set; }
        [PrimaryKey]
        public string id { get; set; }
        public string qiu { get; set; }
        public string zhen { get; set; }

    }
    public class AcuSubmodel
    {

        public string no { get; set; }

        public string guijing { get; set; }
        public string texing { get; set; }
        public string code { get; set; }
        public string zhouwei { get; set; }
        public string dingwei { get; set; }
        public string name { get; set; }
        public string caozuo { get; set; }
        public string zhuzhi { get; set; }
        public string qiu { get; set; }
        public string yincang_muscle { get; set; }
        [PrimaryKey]
        public string zhen { get; set; }
        public string yincang { get; set; }
        public string pai_location { get; set; }
        public string acu_camera { get; set; }
    }
    public class AcuLine
    {
        [PrimaryKey]
        public string id { get; set; }
        public string line { get; set; }
        public string no { get; set; }

    }

    // "mark_noun_no":"JG_egu","fy_name":"结构","map_name":"egu","mark_id":2,"sm_name_list":"DongGu","camera_params":"","nail_model_name":"egud"}

    public class MarkNoun
    {
        [PrimaryKey]
        public string mark_noun_no { get; set; }
        public string fy_name { get; set; }
        public string map_name { get; set; }
        public string mark_id { get; set; }
        public string sm_name_list { get; set; }
        public string camera_params { get; set; }
        public string nail_model_name { get; set; }

    }
  //  {"trigger_id":1,"en_name":"TP_trapezius 1","ch_name":"斜方肌1号触发点","tenton_quyu":"c6,c15,c21,c26,c33","camera_params":"","trigger_no":"CFD001","noun_no":"QUYU0001"
    public class TriggerSubmodel
    {
        [PrimaryKey]
        public string trigger_id { get; set; }
        public string en_name { get; set; }
        public string ch_name { get; set; }
        public string tenton_quyu { get; set; }
        public string camera_params { get; set; }
        public string trigger_no { get; set; }
        public string noun_no { get; set; }
        public string hide_muscle { get; set; }
        public string height_muscle { get; set; }


    }
    public class Sheet2
    {
        [PrimaryKey]
        public string nail_id { get; set; }
        public string nail_no { get; set; }
        public string camera_params { get; set; }
        public string nail_name { get; set; }

    }
}