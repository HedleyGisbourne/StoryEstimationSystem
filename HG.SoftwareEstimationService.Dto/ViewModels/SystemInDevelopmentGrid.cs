using System.ComponentModel.DataAnnotations;
using HG.SoftwareEstimationService.Shared;

namespace HG.SoftwareEstimationService.Dto.ViewModels
{
    public class SystemInDevelopmentHomeGrid
    {
        [PrimaryKey]
        public int SystemId { get; set; }

        [ColumnWidth(30)]
        public string Owner { get; set; }

        [Display(Name = "System Name")]
        public string Name { get; set; }
    }
}
