using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace JobManager.UI
{
    public class OfficeWithJobsViewModel
    {
        private Location selectedOffice;

        public OfficeWithJobsViewModel()
        {
            Offices = OfficeList.Instance;
            Jobs = JobList.Instance;

            FilteredJobs = new ListCollectionView(Jobs);
            FilteredJobs.Filter = FilterJobsByOffice;

            Jobs.CollectionChanged += OnCollectionChanged;
            Jobs.LocationChanged += OnLocationChanged;
            Offices.CollectionChanged += OnCollectionChanged;
            Offices.LocationChanged += OnLocationChanged;

            Map = new Map();            
        }

        public OfficeList Offices { get; set; }
        public JobList Jobs { get; set; }
        public Location SelectedOffice
        {
            get => selectedOffice;
            set
            {
                selectedOffice = value;
                FilteredJobs.Refresh();
                UpdateMapAsync();
            }
        }
        public ListCollectionView FilteredJobs { get; set; }
        public Map Map { get; set; }

        private bool FilterJobsByOffice(object item)
        {
            Job itemJob = item as Job;

            if (SelectedOffice == null || itemJob.Office == SelectedOffice)
                return true;
            else return false;
        }
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (SelectedOffice != null)
            {
                FilteredJobs.Refresh();
                UpdateMapAsync();
            }
        }
        private void OnLocationChanged(object sender, EventArgs e)
        {
            if (SelectedOffice != null)
            {
                FilteredJobs.Refresh();
                UpdateMapAsync();
            }
        }
        public async Task UpdateMapAsync()
        {
            List<Location> places = new List<Location>();

            foreach (var item in FilteredJobs.Cast<Job>())
            {
                places.Add(item.Place);
            }

            await Map.CreateMapAsync(1290, 460, SelectedOffice, places.ToArray());


        }

    }
}
