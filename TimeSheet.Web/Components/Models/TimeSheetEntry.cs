namespace TimeSheet.Web.Components.Models
{
    public class TimeSheetEntry
    {
        public int Id { get; set; }
        public int DurationInMinutes { get; set; }
        public string Description { get; set; }
        public DateTime DateLogged { get; set; }
        public string LoggedBy { get; set; }
    }
}
