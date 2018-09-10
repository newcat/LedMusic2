namespace LedMusic2.Reactive
{
    interface ISerializable
    {
        string Serialize();
        object Deserialize(string s);
    }
}
