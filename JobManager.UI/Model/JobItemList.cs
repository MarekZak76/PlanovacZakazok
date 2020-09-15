using JobManager.Service;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace JobManager.UI
{
    public class JobItemList : ObservableCollection<JobItem>
    {
        private Collection<JobItem> copyJobItems;
        Dictionary<JobItem, bool> jobItemsValidationResults;

        public JobItemList()
        {
            copyJobItems = new Collection<JobItem>();
            AddJobItemCommand = new Command(AddJobItem, () => true);
            RemoveJobItemCommand = new Command(RemoveJobItem, () => true);
            jobItemsValidationResults = new Dictionary<JobItem, bool>();
        }

        public ICommand AddJobItemCommand { get; }
        public ICommand RemoveJobItemCommand { get; }

        public void AddJobItem(object parameter)
        {
            Add(new JobItem() { JobId = (int)parameter });
        }        
        public void RemoveJobItem(object parameter)
        {
            Remove(parameter as JobItem);
        }
        public void BackupCollectionData()
        {            
            this.copyJobItems.Clear();
            for (int i = 0; i < Count; i++)
            {
                this[i].BackupData();
                this.copyJobItems.Add(this[i]);
            }
        }
        public void RestoreCollectionData()
        {           
            this.Clear();
            for (int i = 0; i < this.copyJobItems.Count; i++)
            {
                copyJobItems[i].RestoreData();
                this.Add(copyJobItems[i]);
            }
        }
        public bool TryValidateCollection()
        {
            jobItemsValidationResults.Clear();
            foreach (var item in this)
            {
                jobItemsValidationResults[item] = item.TryValidateModel();
            }
            return jobItemsValidationResults.Values.All(validationResult => validationResult == true);
        }
        public void ClearCollectionValidationErrors()
        {
            foreach (var item in this)
            {
                item.ClearValidationErrors();
            }
        }
        public IList<SJobItem> MapToSJobItems()
        {
            return new List<SJobItem>(this.Select(jobItem => jobItem.MapToSJobItem()));            
        }
    }
}
