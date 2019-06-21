using SQLite4Unity3d;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.Model
{
    [Serializable]
    public class BookMarkInfo
    {
        [PrimaryKey,AutoIncrement]
        public int id { get; set; }
        public string bookmarkName { get; set; }
        public string bookmarkType { get; set; }
        public byte[] cameraParams { get; set; }
        public byte[] modelState { get; set; }
        public byte[] btnState { get; set; }
        public byte[] bookmarkPicture { get; set; }
        public string type { get; set; }
    }

    public class frameMessage
    {
        public byte id { get; set; }
        public byte bookmarkType { get; set; }
        public CameraParams cameraParams { get; set; }
        public SceneModelState modelState { get; set; }
        public SceneBtnState btnState { get; set; }
    }
}