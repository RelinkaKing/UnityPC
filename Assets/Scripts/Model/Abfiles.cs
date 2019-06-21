using System;
using System.Collections.Generic;
using SQLite4Unity3d;

namespace Assets.Scripts.Model
{
    [Serializable]
    public class Abfiles
    {
        [PrimaryKey]
        public string file_name { get; set; }
        public string head { get; set; }
       
    }

    [Serializable]
    public class FixTabs
    {
        [PrimaryKey]
        public string tab_name { get; set; }
    }
}
