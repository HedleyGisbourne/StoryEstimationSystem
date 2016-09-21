using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HG.SoftwareEstimationService.Shared;

namespace HG.SoftwareEstimationService.Dto.ViewModels
{
    public class ObservationsGrid
    {
        [PrimaryKey]
        public int StoryPartId { get; set; }
        
        [HideColumn]
        public int StoryId { get; set; }
        
        [HideColumn]
        public int? PartTypeId { get; set; }
        
        [Display(Name = "Description")]
        public string PartDescription { get; set; }

        [DisplayName("Type")]
        [Editable(false)]
        public string PartType { get; set; }
    }
}
