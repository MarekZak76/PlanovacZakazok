using JobManager.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace JobManager.UI
{
    public class JobList : ObservableCollection<Job>
    {
        private static JobList instance;

        private JobList() : base()
        {
            DataClient = new DataClient();
            Offices = OfficeList.Instance;
        }

        public event EventHandler LocationChanged;

        public static JobList Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new JobList();
                    return instance;
                }
                else return instance;
            }
        }
        public IDataClient DataClient { get; set; }
        public OfficeList Offices { get; }

        public async Task LoadDataAsync()
        {
            IEnumerable<SJob> loadedData = (await DataClient.GetAllAsync(typeof(SJob)))?.Cast<SJob>();

            if (loadedData != null)
            {
                foreach (var item in loadedData)
                {
                    Job job = new Job()
                    {
                        Id = item.Id,
                        Description = item.Description,
                        IsCompleted = item.IsCompleted,
                        DateCreated = item.DateCreated,
                        DateCompleted = item.DateCompleted
                    };

                    job.Office = Offices.Single(office => office.Id == item.BranchOfficeId);

                    job.Place.Id = item.WorkPlace.Id;
                    job.Place.Name = item.WorkPlace.Name;
                    job.Place.LocationAdress.City = item.WorkPlace.City;
                    job.Place.LocationAdress.Street = item.WorkPlace.Street;
                    job.Place.LocationAdress.Number = item.WorkPlace.Number;
                    job.Place.LocationPoint[0] = item.WorkPlace.Latitude;
                    job.Place.LocationPoint[1] = item.WorkPlace.Longitude;

                    job.Map.MapData = item.MapData;
                    if (item.MapData != null)
                        job.Map.ConvertDataToImagery();

                    foreach (var sJobItem in item.SJobItems)
                    {
                        job.JobItems.Add(new JobItem() { Id = sJobItem.Id, Description = sJobItem.Description, JobId = sJobItem.SJobId });
                    }

                    job.LocationChanged += LocationChangedForward;

                    instance.Add(job);
                }
            }
        }
        public async Task<bool> AddAsync(Job job)
        {
            Job addedJob;
            bool isObjectGraphValid;
            bool isObjectValid;
            bool isComplexPropertyValid;
            bool isCollectionPropertyValid;

            if (job == null)
                return false;

            isObjectValid = job.TryValidateModel();
            isComplexPropertyValid = job.Place.LocationAdress.TryValidateModel();
            isCollectionPropertyValid = job.JobItems.TryValidateCollection();
            isObjectGraphValid = isObjectValid & isComplexPropertyValid & isCollectionPropertyValid;

            if (isComplexPropertyValid & (job.Office != null))
            {
                if(await job.Place.GetLocationPointAsync() == false)
                    return false;

                if(await job.Map.CreateMapAsync(630, 460, job.Office, job.Place) == false)
                    return false;
            }

            if (!isObjectGraphValid)
                return false;

            addedJob = await job.CreateAsync();

            if (addedJob == null)
                return false;

            //Insert(0, addedJob);  //ulozi sa pri ukonceni Add transakcie
            return true;
        }
        public async Task<bool> DeleteAsync(Job job)
        {
            if (!await job.DeleteAsync())
            {
                return false;
            }

            //Remove(job); vymaze sa pri ukonceni transakcie
            return true;
        }
        private void LocationChangedForward(object sender, EventArgs e)
        {
            // preposle event zmeny Job.Office dalej do OfficeWithJobsViewModel
            LocationChanged?.Invoke(sender, e);
        }

    }
}
