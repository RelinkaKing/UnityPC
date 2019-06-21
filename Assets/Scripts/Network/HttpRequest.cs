using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Infrastructure;

namespace Assets.Scripts.Network
{
    public class HttpRequest
    {
        public string Url { get; set; }
        public HttpMethod Method { get; set; }
        public string Parameters { get; set; }
    }
}
