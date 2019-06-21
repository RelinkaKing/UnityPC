using System;
using System.Collections.Generic;
using SQLite4Unity3d;

namespace Assets.Scripts.Model
{
    [Serializable]
    public class TableVersions
    {
        [PrimaryKey]
        public string table_name { get; set; }
        public string version { get; set; }
       
    }
}
