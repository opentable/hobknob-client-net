using System;
using System.Collections.Generic;

namespace HobknobClientNet
{
    public class CacheUpdatedArgs : EventArgs
    {
        public IEnumerable<CacheUpdate> Updates { get; private set; }

        public CacheUpdatedArgs(IEnumerable<CacheUpdate> updates)
        {
            Updates = updates;
        }
    }
}