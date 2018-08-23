namespace LedMusic2.Reactive.Binding
{
    public interface IBindable<T> where T : IBound
    {
        T Bind();
        void Unbind(T bound);
    }
}
