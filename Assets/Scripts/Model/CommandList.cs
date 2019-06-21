using System;
using System.Collections;
using System.Collections.Generic;
using SQLite4Unity3d;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [Serializable]
    public class CommandList
    {
        [PrimaryKey]
        public int id { get; set; }
        public byte[] modelInfo { get; set; }
    }


}