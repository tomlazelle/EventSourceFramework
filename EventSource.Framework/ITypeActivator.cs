using StructureMap;

namespace EventSource.Framework
{
    public interface ITypeActivator
    {
        T Instance<T>();
    }

    public class TypeActivator : ITypeActivator
    {
        private readonly IContainer _container;

        public TypeActivator(IContainer container)
        {
            _container = container;
        }

        public T Instance<T>()
        {
            return _container.GetInstance<T>();
        }
    }
}