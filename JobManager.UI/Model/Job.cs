using JobManager.Service;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JobManager.UI
{
    public class Job : Validatable, IEditableObject
    {
        private Location office;
        private Location copyOffice;
        private Data currentData;
        private Data copyData;
        private DateTime? dateCompleted;

        public Job()
        {
            DateCreated = DateTime.Now;
            //Office = new Location();
            Place = new Location();
            JobItems = new JobItemList();
            DataClient = new DataClient();
            Map = new Map();
        }

        public event EventHandler LocationChanged;

        public int Id { get; set; }
        public bool IsCompleted
        {
            get => currentData.isCompleted;
            set
            {
                SetProperty(ref currentData.isCompleted, value);
            }
        }
        [Required]
        [StringLength(250, MinimumLength = 5)]
        public string Description
        {
            get => currentData.description;
            set
            {
                SetProperty(ref currentData.description, value);
            }
        }
        public DateTime DateCreated { get; set; }
        public DateTime? DateCompleted
        {
            get => dateCompleted;
            set => SetProperty(ref dateCompleted, value);
        }
        public IDataClient DataClient { get; }
        public JobItemList JobItems { get; set; }
        [Required]
        public Location Office
        {
            get => office;
            set
            {
                SetProperty(ref office, value);
            }
        }
        public Location Place { get; private set; }
        public Map Map { get; private set; }

        public async Task<Job> CreateAsync()
        {
            this.DateCreated = DateTime.Now;

            SJob createdSJob = (SJob)await DataClient.AddAsync(MapToSJob());

            if (createdSJob == null)
            {
                return null;
            }

            //namapuje PrimaryKey ktore priradil SQL server 
            Id = createdSJob.Id;
            for (var i = 0; i < JobItems.Count; i++)
            {
                JobItems[i].Id = createdSJob.SJobItems[i].Id;
            }
            Place.Id = createdSJob.WorkPlace.Id;

            return this;
        }
        public async Task<bool> UpdateAsync()
        {
            bool isObjectGraphValid;
            bool isObjectValid;
            bool isComplexPropertyValid;
            bool isCollectionPropertyValid;

            isObjectValid = TryValidateModel();
            isComplexPropertyValid = Place.LocationAdress.TryValidateModel();
            isCollectionPropertyValid = JobItems.TryValidateCollection();
            isObjectGraphValid = isObjectValid & isComplexPropertyValid & isCollectionPropertyValid;

            if (isComplexPropertyValid)
            {
                if(await Place.GetLocationPointAsync() == false)
                    return false;
                if(await Map.CreateMapAsync(630, 460, Office, Place) == false)
                    return false;
            }

            if (!isObjectGraphValid)
                return false;

            SJob updatedSJob = (SJob)await DataClient.UpdateAsync(MapToSJob());

            if (updatedSJob == null)
                return false;


            //namapuje PrimaryKey ktore priradil SQL server 
            for (var i = 0; i < JobItems.Count; i++)
            {
                JobItems[i].Id = updatedSJob.SJobItems[i].Id;
            }

            RaiseOnLocationChanged();

            return true;
        }
        public async Task<bool> DeleteAsync()
        {
            if (await DataClient.DeleteAsync(MapToSJob()))
            {
                return true;
            }
            else return false;
        }
        public async Task<bool> CompleteAsync()
        {
            IsCompleted = true;
            DateCompleted = DateTime.Now;

            if (!await UpdateAsync())
                return false;

            return true;
        }
        public SJob MapToSJob()
        {
            return new SJob
            {
                Id = this.Id,
                Description = this.Description,
                IsCompleted = this.IsCompleted,
                DateCreated = this.DateCreated,
                DateCompleted = this.DateCompleted,
                SJobItems = JobItems.MapToSJobItems(),
                WorkPlaceId = this.Place.Id,
                WorkPlace = this.Place.MapToSLocation(false),
                BranchOfficeId = this.Office.Id,
                MapData = Map.MapData
            };
        }
        public void RaiseOnLocationChanged()
        {
            LocationChanged?.Invoke(this, EventArgs.Empty);
        }
        public void BeginEdit()
        {
            //scalar
            copyData = currentData;

            //collection
            JobItems.BackupCollectionData();

            //complex
            Place.BackupData();
            copyOffice = Office;
            Map.BackupData();
        }
        public void EndEdit()
        {

        }
        public void CancelEdit()
        {
            //scalar
            Description = copyData.description;
            IsCompleted = copyData.isCompleted;

            //collection            
            JobItems.RestoreCollectionData();

            //complex
            Place.RestoreData();
            Office = copyOffice;
            Map.RestoreData();
        }

        struct Data
        {
            internal string description;
            internal bool isCompleted;
        }
    }
}
