using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobManager.Service
{
    public class SJob : IEntity
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(250)]
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateCompleted { get; set; }
        public int BranchOfficeId { get; set; }
        public int WorkPlaceId { get; set; }
        public byte[] MapData { get; set; }

        [ForeignKey(nameof(BranchOfficeId))]
        public SLocation BranchOffice { get; set; }

        [ForeignKey(nameof(WorkPlaceId))]
        public SLocation WorkPlace { get; set; }

        public IList<SJobItem> SJobItems { get; set; }
    }
}