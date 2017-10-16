using System;
using Microsoft.Extensions.DependencyInjection;
using Raven.Client;

namespace EventSource.Framework.EventStores
{
    public class RavenDBEventStore : IEventStore
    {
        
        private readonly IDocumentStore documentStore;
        private readonly IServiceProvider _serviceProvider;

        public RavenDBEventStore(IDocumentStore documentStore, IServiceProvider serviceProvider)
        {
            this.documentStore = documentStore;
            _serviceProvider = serviceProvider;
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
                    result = _serviceProvider.GetService<TEventType>();

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
}