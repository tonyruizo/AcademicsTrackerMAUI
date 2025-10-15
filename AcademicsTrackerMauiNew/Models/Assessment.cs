using SQLite;

namespace AcademicsTrackerMauiNew.Models
{
    public enum AssessmentType { Objective, Performance }
    public class Assessment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int CourseId { get; set; }
        public AssessmentType Type { get; set; }
        [NotNull]
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool NotifyStart { get; set; }
        public bool NotifyEnd { get; set; }
    }
}
