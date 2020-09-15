using System.ComponentModel.DataAnnotations;

namespace JobManager.UI
{
    public class Adress : Validatable
    {
        private Data copyData;
        private Data currentData;

        [Required]
        public string City
        {
            get => currentData.city;
            set => SetProperty(ref currentData.city, value);
        }
        public string Street
        {
            get => currentData.street;
            set => SetProperty(ref currentData.street, value);
        }
        public int? Number
        {
            get => currentData.number;
            set => SetProperty(ref currentData.number, value);
        }

        public void BackupData()
        {
            copyData = currentData;
        }
        public void RestoreData()
        {
            //currentData = copyData;
            City = copyData.city;
            Street = copyData.street;
            Number = copyData.number;
        }

        private struct Data
        {
            internal string city;
            internal string street;
            internal int? number;
        }

    }

}
