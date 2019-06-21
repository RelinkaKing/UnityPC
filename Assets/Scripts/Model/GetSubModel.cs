using System;
using System.Collections.Generic;
using SQLite4Unity3d;

namespace Assets.Scripts.Model
{
    [Serializable]
    public class GetSubModel
    {
        [PrimaryKey]
        public string name { get; set; }
        public string enName { get; set; }
        public string description { get; set; }
        public string submodelId { get; set; }
        public string chName { get; set; }
        public string partitionName { get; set; }
    }
}
