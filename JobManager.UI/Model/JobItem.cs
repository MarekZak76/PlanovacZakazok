using JobManager.Service;
using System.ComponentModel.DataAnnotations;

namespace JobManager.UI
{
    public class JobItem : Validatable
    {
        private string description;
        private Data copyData;

        public int Id { get; set; }
        [Required]
        [StringLength(250, MinimumLength = 2)]
        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }
        public int JobId { get; set; }

        public void BackupData()
        {
            copyData.id = Id;
            copyData.description = Description;
            copyData.jobId = JobId;
        }
        public void RestoreData()
        {
            Id = copyData.id;
            Description = copyData.description;
            JobId = copyData.jobId;
        }
        public SJobItem MapToSJobItem()
        {
            return new SJobItem
            {
                Id = this.Id,
                Description = this.Description,
                SJobId = this.JobId
            };
        }

        private struct Data
        {
            internal int id;
            internal string description;
            internal int jobId;
        }
    }
}
