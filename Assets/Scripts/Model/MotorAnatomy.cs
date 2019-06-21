using SQLite4Unity3d;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Model
{
    class MotorAnatomy
    {
        [PrimaryKey]
        public string ID { get; set; }

        public string NameEng { get; set; }
        public string NameCng { get; set; }
        public string Details { get; set; }
        public string ModelName { get; set; }    
        public string MuscleName { get; set; }
        public string MuscleNameCng { get; set; }
        public string SecondaryMuscleName { get; set; }
        public string StageFrame { get; set; }
        public string ModelPosition { get; set; }
        public string MainMuscleName { get; set; }
        public string JointName { get; set; }
        public string MaxAngle { get; set; }
    }
}
