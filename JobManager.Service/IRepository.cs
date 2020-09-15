using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace JobManager.Service
{
    [ServiceKnownType(typeof(SJob))]
    [ServiceKnownType(typeof(SLocation))]
    [ServiceContract]
    public interface IRepository
    {
        [OperationContract]
        Task<IEnumerable<IEntity>> GetAllAsync(string typeName);

        [OperationContract]
        Task<IEntity> AddAsync(IEntity entity);

        [OperationContract]
        Task<IEntity> UpdateAsync(IEntity entity);

        [OperationContract]
        Task<IEntity> DeleteAsync(IEntity entity);
    }
}
