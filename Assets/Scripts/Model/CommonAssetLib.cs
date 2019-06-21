using SQLite4Unity3d;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.Model
{
	[Serializable]
	public class CommonAssetLib {
        [PrimaryKey]
        public int id{ get; set; }
        public string ab_name{ get; set; }
		public string url{ get; set; }
		public string version{ get; set; }
        public string type{ get; set; }
        public string platform{ get; set; }
    }
}