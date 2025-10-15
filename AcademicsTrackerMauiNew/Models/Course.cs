using SQLite;

namespace AcademicsTrackerMauiNew.Models
{
    public enum CourseStatus { InProgress, Completed, Dropped, PlanToTake }
    public class Course
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int TermId { get; set; }
        [NotNull]
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DueDate { get; set; }
        public CourseStatus Status { get; set; }
        public string InstructorName { get; set; } = string.Empty;
        public string InstructorPhone { get; set; } = string.Empty;
        public string InstructorEmail { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public bool NotifyStart { get; set; }
        public bool NotifyEnd { get; set; }
    }
}
