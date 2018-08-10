namespace LedMusic2.Reactive
{
    interface ISerializable
    {
        string Serialize();
        void Deserialize(string s);
    }
}
