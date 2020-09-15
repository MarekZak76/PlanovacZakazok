using System.ComponentModel.DataAnnotations;

namespace JobManager.Service
{
    public class SLocation : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string City { get; set; }
        public string Street { get; set; }
        public int? Number { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsBranchOffice { get; set; }       
    }
}