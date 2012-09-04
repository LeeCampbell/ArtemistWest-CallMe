namespace ArtemisWest.CallMe.Contract
{
    public interface ILocalStore
    {
        string Get(string key);
        void Remove(string key);
        void Put(string key, string value);
    }
}
