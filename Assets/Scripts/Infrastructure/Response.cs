using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Infrastructure
{
    [System.Serializable]
    public class Response<T>
    {
        public string msg;
        public int code;
        public List<T> List;
        public string maxVersion;


    }

}
