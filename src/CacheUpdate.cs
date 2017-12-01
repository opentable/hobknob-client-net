namespace HobknobClientNet
{
    public class CacheUpdate
    {
        public string Key { get; private set; }
        public bool? OldValue { get; private set; }
        public bool? NewValue { get; private set; }

        public CacheUpdate(string key, bool? oldValue, bool? newValue)
        {
            Key = key;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}