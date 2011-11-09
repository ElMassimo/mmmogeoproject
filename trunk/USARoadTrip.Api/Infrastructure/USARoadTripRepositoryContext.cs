using System.Data.Objects;

namespace USARoadTrip.Api.Infrastructure
{
    public class HerbertRepositoryContext : IRepositoryContext
    {
        private const string OBJECT_CONTEXT_KEY = "USARoadTrip.Api.EntityModels.USARoadTripContainer";
        public IObjectSet<T> GetObjectSet<T>() 
            where T : class
        {
            return ContextManager.GetObjectContext(OBJECT_CONTEXT_KEY).CreateObjectSet<T>();
        }

        /// <summary>
        /// Returns the active object context
        /// </summary>
        public ObjectContext ObjectContext
        {
            get
            {
                return ContextManager.GetObjectContext(OBJECT_CONTEXT_KEY);
            }
        }

        public int SaveChanges()
        {
            return this.ObjectContext.SaveChanges();
        }

        public void Terminate()
        {
            ContextManager.SetRepositoryContext(null, OBJECT_CONTEXT_KEY);
        }
    }
}
