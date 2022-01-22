namespace MVC.Runtime.Bind.Bindings
{
    public interface IBinding
    {
        public object Key { get; }
        public object Value { get;}

        void To<TValueType>();
    }
}