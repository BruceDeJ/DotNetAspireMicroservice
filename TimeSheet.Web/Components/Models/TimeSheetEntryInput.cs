using System.ComponentModel.DataAnnotations;

namespace TimeSheet.Web.Components.Models
{
    public class TimeSheetEntryInput
    {
        [Required]
        public int DurationInMinutes { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
