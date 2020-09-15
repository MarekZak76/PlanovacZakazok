using JobManager.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace JobManager.UI
{
    public class OfficeList : ObservableCollection<Location>
    {
        private static OfficeList instance;

        private OfficeList() : base()
        {
            DataClient = new DataClient();
        }

        public event EventHandler LocationChanged;

        public static OfficeList Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new OfficeList();
                    return instance;
                }
                else return instance;
            }
        }
        public IDataClient DataClient { get; set; }

        public async Task LoadDataAsync()
        {
            IEnumerable<SLocation> loadedData = (await DataClient.GetAllAsync(typeof(SLocation)))?.Cast<SLocation>();

            if (loadedData != null)
            {
                foreach (var item in loadedData)
                {
                    if (item.IsBranchOffice)
                    {
                        Location location = new Location()
                        {
                            Id = item.Id,
                            Name = item.Name
                        };
                        location.LocationAdress.City = item.City;
                        location.LocationAdress.Street = item.Street;
                        location.LocationAdress.Number = item.Number;
                        location.LocationPoint[0] = item.Latitude;
                        location.LocationPoint[1] = item.Longitude;

                        location.LocationChanged += LocationChangedForward;

                        instance.Add(location);
                    }
                }
            }
        }
        public async Task<bool> AddAsync(Location location)
        {
            Location addedLocation;

            if (location.LocationAdress.TryValidateModel() == false | location.TryValidateModel() == false)
            {
                return false;
            }

            if (await location.GetLocationPointAsync() == false)
                return false;

            addedLocation = await location.CreateAsync();

            if (addedLocation == null)
                return false;

            //Add(addedLocation);   / pri ukonceni transakcie

            return true;
        }
        public async Task<bool> DeleteAsync(Location location)
        {
            if (!await location.DeleteAsync())
            {
                return false;
            }

            //Remove(location);  pri ukonceni transakcie
            return true;
        }
        private void LocationChangedForward(object sender, EventArgs e)
        {
            // preposle event zmeny Job.Office dalej do OfficeWithJobsViewModel
            LocationChanged?.Invoke(sender, e);
        }

    }
}
