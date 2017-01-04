using System;
using Raven.Client;
using StructureMap;

namespace EventSource.Framework
{
    public class RavenDBEventStore : IEventStore
    {
        private readonly IDocumentStore documentStore;
        private readonly ITypeActivator _typeActivator;

        public RavenDBEventStore(IDocumentStore documentStore, ITypeActivator typeActivator)
        {
            this.documentStore = documentStore;
            _typeActivator = typeActivator;
        }

        public bool Write<T>(T aggregate)
        {
            using (var session = documentStore.OpenSession())
            {
                session.Store(aggregate);

                session.SaveChanges();
            }

            return true;
        }

        public T Get<T>(Guid id)
        {
            T result;

            using (var session = documentStore.OpenSession())
            {
                result = session.Load<T>(typeof(T).Name + "/" + id);
            }

            return result;
        }

        public TEventType AddEvent<TEventType>(Guid id, IVersionedEvent<Guid> eventItem) where TEventType : EventContainer
        {

            EventContainer result;

            using (var session = documentStore.OpenSession())
            {

                result = session.Load<TEventType>(typeof(TEventType).Name + "/" + id);

                if (result == null)
                {
                    result =  _typeActivator.Instance<TEventType>();

                    result.Init(id);
                    result.AddEvent(eventItem);
                    session.Store(result);
                }
                else
                {
                    result.AddEvent(eventItem);
                }                

                session.SaveChanges();
            }

            return (TEventType)result;
        }
    }

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