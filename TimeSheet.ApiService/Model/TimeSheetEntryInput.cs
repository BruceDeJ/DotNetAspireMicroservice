using System.ComponentModel.DataAnnotations;

namespace TimeSheet.ApiService.Model
{
    public class TimeSheetEntryInput
    {
        [Required]
        public int DurationInMinutes { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
