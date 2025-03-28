namespace TimeSheet.ApiService.Domain
{
    public class TimeSheetEntry
    {
        public int Id { get; set; }
        public int DurationInMinutes { get; set; }
        public string Description { get; set; }
        public DateTime DateLogged { get; set; }
        public string LoggedBy { get; set; }

        public TimeSheetEntry()
        {
                
        }

        public TimeSheetEntry(int durationInMinutes, string description, string loggedBy)
        {
            DurationInMinutes = durationInMinutes;
            Description = description;
            DateLogged = DateTime.UtcNow;
            LoggedBy = loggedBy;
        }
    }
}
