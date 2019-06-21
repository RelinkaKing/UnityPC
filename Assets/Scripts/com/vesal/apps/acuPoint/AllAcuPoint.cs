using SQLite4Unity3d;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [Serializable]
    public class AcupointInfo
    {
        public string sm_type { get; set; }
        public string transparency { get; set; }
        public string sm_em_name { get; set; }
        public string handle { get; set; }
        [PrimaryKey]
        public string id { get; set; }
        public string sm_name { get; set; }
        public string position { get; set; }
        public string cure { get; set; }
        public string iso_code { get; set; }
        public string partition_name { get; set; }
        public string sm_ch_name { get; set; }
        public string camera_params { get; set; }

    }

    [Serializable]
    public class AcuListInfo
    {
        public string noun_no { get; set; }
        public string line_list { get; set; }
        public string camera_params { get; set; }
        public string ids { get; set; }
    }
}