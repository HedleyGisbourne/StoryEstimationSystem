using System.ComponentModel.DataAnnotations;
using HG.SoftwareEstimationService.Shared;

namespace HG.SoftwareEstimationService.Dto.ViewModels
{
    public class SystemInDevelopmentGrid
    {
        [PrimaryKey]
        public int SystemId { get; set; }

        [Required]
        [ColumnWidth(30)]
        public string Owner { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "System Name")]
        public string Name { get; set; }
    }
}
