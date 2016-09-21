using System.ComponentModel.DataAnnotations;
using HG.SoftwareEstimationService.Shared;

namespace HG.SoftwareEstimationService.Dto.ViewModels
{
    public class StoryGrid
    {
        [PrimaryKey]
        public int StoryId { get; set; }
        
        [ColumnWidth(15)]
        [Required]
        [MaxLength(100)]
        [Display(Name = "Ticket")]
        public string TicketName { get; set; }

        [Required]
        [Display(Name = "Story Description")]
        [MaxLength(100)]
        public string StoryTitle { get; set; }

        [Editable(false)]
        [Display(Name = "Observations")]
        [ColumnWidth(12)]
        public int Observations { get; set; }

        [Editable(false)]
        [Display(Name = "Estimated Duration")]
        [ColumnWidth(15)]
        public string EstimatedCompletionDuration { get; set; }

        [Editable(false)]
        [Display(Name = "Actual Duration")]
        [ColumnWidth(12)]
        public string ActualCompletionDuration { get; set; }

        [Editable(false)]
        [Display(Name = "Completed")]
        [ColumnWidth(12)]
        public string CompletionDate { get; set; }
    }
}

