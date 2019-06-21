using System;
using System.Collections.Generic;
using SQLite4Unity3d;

namespace Assets.Scripts.Model
{
    [Serializable]
    public class GetNounSubModel
    {
        [PrimaryKey]
        public string nounNo { get; set; }
        public int submodelId { get; set; }
        public float transparency { get; set; }

    }
}
