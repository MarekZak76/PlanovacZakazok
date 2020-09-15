using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace JobManager.Service
{

    public class Repository : IRepository
    {
        public Repository()
        {
            DatabaseContext = new DatabaseContext(DatabaseContext.DATABASE_CONNECTION_STRING);
        }

        public DatabaseContext DatabaseContext { get; }

        public Task<IEnumerable<IEntity>> GetAllAsync(string typeName)
        {
            IEnumerable<IEntity> result;

            if (string.IsNullOrEmpty(typeName)) return Task.FromException<IEnumerable<IEntity>>(new ArgumentNullException("Parameter 'typeName' nesmie byt null alebo empty"));

            Type type = Type.GetType(typeName);

            if (type == null) return Task.FromException<IEnumerable<IEntity>>(new ArgumentException("Typ 'typeName' je neznamy"));

            if (type.GetInterface(nameof(IEntity)) == null) return Task.FromException<IEnumerable<IEntity>>(new NotSupportedException("Typ 'typeName' musi implementovat IData interface"));

            switch (type.Name)
            {
                case nameof(SJob):
                    result = DatabaseContext.SJob
                        .Include(nameof(SJob.SJobItems))
                        //.Include(nameof(SJob.BranchOffice))
                        .Include(nameof(SJob.WorkPlace))
                        .ToList();
                    break;

                case nameof(SLocation):
                    result = DatabaseContext.SLocation.ToList();
                    break;

                default:
                    throw new ArgumentException();
            }

            DatabaseContext.Dispose();

            return Task.FromResult(result);
        }

        public Task<IEntity> AddAsync(IEntity entity)
        {
            if (entity == null)
                return Task.FromException<IEntity>(new ArgumentNullException());

            try
            {
                var addedEntity = DatabaseContext.Set(entity.GetType()).Add(entity) as IEntity;

                DatabaseContext.SaveChanges();

                DatabaseContext.Dispose();

                return Task.FromResult(addedEntity);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                return Task.FromException<IEntity>(ex);
            }

        }

        public Task<IEntity> UpdateAsync(IEntity entity)
        {
            if (entity == null)
                return Task.FromException<IEntity>(new ArgumentNullException());

            switch (entity.GetType().Name)
            {
                case nameof(SJob):

                    // scalar
                    SJob existingJob = DatabaseContext.Set<SJob>()
                        .Include(nameof(SJob.SJobItems))
                        //.Include(nameof(SJob.BranchOffice))
                        //.Include(nameof(SJob.WorkPlace))
                        .Single(job => job.Id == entity.Id);
                    DatabaseContext.Entry(existingJob).CurrentValues.SetValues(entity);

                    // collection
                    foreach (var jobItem in (entity as SJob).SJobItems)
                    {
                        if (jobItem.Id == 0)
                        {
                            DatabaseContext.Entry(jobItem).State = EntityState.Added;
                        }
                        else
                        {
                            var existingJobItem = existingJob.SJobItems.SingleOrDefault(i => i.Id == jobItem.Id);
                            DatabaseContext.Entry(existingJobItem).CurrentValues.SetValues(jobItem);
                        }
                    }

                    List<SJobItem> deletedJobItems = new List<SJobItem>(); // pomocna premenna pretoze nie je mozne iterovat kolekciou a zaroven menit state polozky na entitystate.deleted
                    foreach (var item in existingJob.SJobItems)
                    {
                        if (!(entity as SJob).SJobItems.Any(i => i.Id == item.Id))
                        {
                            deletedJobItems.Add(item);
                        }
                    }
                    deletedJobItems.ForEach(i => DatabaseContext.Entry(i).State = EntityState.Deleted);

                    //complex
                    DatabaseContext.Entry((entity as SJob).WorkPlace).State = EntityState.Modified;
                    break;

                case nameof(SLocation):
                    //scalar
                    SLocation existingLocation = DatabaseContext.Set<SLocation>()
                       .Single(location => location.Id == entity.Id);
                    DatabaseContext.Entry(existingLocation).CurrentValues.SetValues(entity);
                    break;

                default:
                    return Task.FromException<IEntity>(new ArgumentException());
            }

            try
            {
                DatabaseContext.SaveChanges();

                DatabaseContext.Dispose();

                return Task.FromResult(entity);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                return Task.FromException<IEntity>(ex);
            }
        }

        public Task<IEntity> DeleteAsync(IEntity entity)
        {
            if (entity == null)
                return Task.FromException<IEntity>(new ArgumentNullException());

            try
            {
                var originalEntity = DatabaseContext.Set(entity.GetType()).Find(entity.Id);

                DatabaseContext.Set(entity.GetType()).Remove(originalEntity);

                DatabaseContext.SaveChanges();

                DatabaseContext.Dispose();

                return Task.FromResult(entity);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                return Task.FromException<IEntity>(ex);
            }

        }

    }
}
