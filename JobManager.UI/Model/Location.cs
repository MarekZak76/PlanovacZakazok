using JobManager.Service;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading.Tasks;

namespace JobManager.UI
{
    public class Location : Validatable, IEditableObject
    {
        private readonly IGeolocator locator;
        private string currentName;
        private string copyName;

        public Location()
        {
            locator = new BingLocator();
            DataClient = new DataClient();
            LocationAdress = new Adress();
            LocationPoint = new double[2];
            OfficeMap = new Map();
        }

        public event EventHandler LocationChanged;

        public int Id { get; set; }
        [Required]
        public string Name
        {
            get => currentName;
            set => SetProperty(ref currentName, value);
        }
        public Adress LocationAdress { get; set; }
        public double[] LocationPoint { get; set; }
        public IDataClient DataClient { get; set; }
        public Map OfficeMap { get; set; }
        public IGeolocator Geolocator { get => locator; }

        public async Task<bool> GetLocationPointAsync()
        {
            Tuple<string, double[]> loadedData;

            loadedData = await locator.GetPointAsync(LocationAdress);

            if (loadedData == null)
            {
                return false;
            }

            LocationPoint = loadedData.Item2;
            Name = loadedData.Item1;
            this.ClearValidationErrors();
            return true;
        }
        public async Task<Location> CreateAsync()
        {
            SLocation createdSLocation = (SLocation)await DataClient.AddAsync(MapToSLocation(true));

            if (createdSLocation == null)
            {
                return null;
            }

            Id = createdSLocation.Id;

            return this;
        }
        public async Task<bool> UpdateAsync()
        {
            if (this.LocationAdress.TryValidateModel() == false | this.TryValidateModel() == false)
                return false;

            if (await this.GetLocationPointAsync() == false)
                return false;

            SLocation updatedSLocation = (SLocation)await DataClient.UpdateAsync(MapToSLocation(true));

            if (updatedSLocation == null)
                return false;

            RaiseOnLocationChanged();

            return true;
        }
        public async Task<bool> DeleteAsync()
        {
            SLocation location = MapToSLocation(true);

            if (await DataClient.DeleteAsync(location))
            {
                return true;
            }

            return false;
        }

        public void BackupData()
        {
            copyName = currentName;

            LocationAdress.BackupData();
        }
        public void RestoreData()
        {
            Name = copyName;

            LocationAdress.RestoreData();
        }
        public SLocation MapToSLocation(bool isOffice)
        {
            return new SLocation
            {
                Id = this.Id,
                City = this.LocationAdress.City,
                IsBranchOffice = isOffice,
                Latitude = this.LocationPoint[0],
                Longitude = this.LocationPoint[1],
                Number = this.LocationAdress.Number,
                Street = this.LocationAdress.Street,
                Name = this.Name
            };
        }

        public void RaiseOnLocationChanged()
        {
            LocationChanged?.Invoke(this, EventArgs.Empty);
        }

        public void BeginEdit()
        {
            BackupData();
        }

        public void EndEdit()
        {

        }

        public void CancelEdit()
        {
            RestoreData();
        }
    }
}
