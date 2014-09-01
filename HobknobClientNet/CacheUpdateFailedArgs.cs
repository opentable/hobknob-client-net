using System;

namespace HobknobClientNet
{
    public class CacheUpdateFailedArgs : EventArgs
    {
        public Exception Exception { get; private set; }

        public CacheUpdateFailedArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}
