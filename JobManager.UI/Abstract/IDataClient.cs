using JobManager.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobManager.UI
{
    public interface IDataClient
    {       
        IRepository Channel { get;}
        string ErrorMessage { get; set; }
        Task<IEnumerable<IEntity>> GetAllAsync(Type type);
        Task<IEntity> AddAsync(IEntity entity);
        Task<IEntity> UpdateAsync(IEntity entity);
        Task<bool> DeleteAsync(IEntity entity);
    }
}
