using System;
using System.Collections.Generic;
using SQLite4Unity3d;

namespace Assets.Scripts.Model
{
    [Serializable]
    public class GetStructList
    {
        [PrimaryKey]
        public string nounNo { get; set; }
        public string camPos { get; set; }
        public string camParentRot { get; set; }
        public string camParentPos { get; set; }
        public string nounName { get; set; }
        public string rm_list { get; set; }
        public string collection_b { get; set; }
        public string nounType { get; set; }
        public string submodelList { get; set; }
        
        //public string X { get; set; }
        //public string Y { get; set; }
        //public string Z { get; set; }
        //public string cameraDistance { get; set; }



    }
}
