using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.DbDao
{
    public interface IDbDao<T> where T : class, new()
    {
        void setCache(string r);
    }
}
