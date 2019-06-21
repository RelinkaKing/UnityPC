using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Infrastructure;

namespace Assets.Scripts.Network
{
    public class HttpTool : Singleton<HttpTool>
    {
        // 无法在外界使用构造函数，确保Singleton
        protected HttpTool() { }
    }
}
