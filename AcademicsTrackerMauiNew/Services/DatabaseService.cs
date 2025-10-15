using AcademicsTrackerMauiNew.Models;
using SQLite;

namespace AcademicsTrackerMauiNew.Services
{
    /// <summary>
    /// Handles CRUD operations between SQLite and the models.
    /// </summary>
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseService()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "Academics.db3");
            _database = new SQLiteAsyncConnection(dbPath);

        }

        public async Task InitializeAndSeedAsync()
        {
            // Create tables
            await _database.CreateTableAsync<Term>();
            await _database.CreateTableAsync<Course>();
            await _database.CreateTableAsync<Assessment>();
            await _database.CreateTableAsync<Notes>();

            // If empty, insert sample data
            await InsertSampleDataAsync();
        }

        private async Task InsertSampleDataAsync()
        {
            var termCount = await _database.Table<Term>().CountAsync();
            if (termCount == 0)
            {
                var sampleTerm = new Term
                {
                    Title = "Term 1",
                    StartDate = new DateTime(2025, 9, 5),
                    EndDate = new DateTime(2025, 12, 31)
                };
                await _database.InsertAsync(sampleTerm);

                var sampleCourse = new Course
                {
                    TermId = sampleTerm.Id,
                    Title = "Sample Course",
                    StartDate = sampleTerm.StartDate,
                    EndDate = sampleTerm.EndDate,
                    DueDate = sampleTerm.EndDate,
                    Status = CourseStatus.InProgress,
                    InstructorName = "Anika Patel",
                    InstructorPhone = "555-123-4567",
                    InstructorEmail = "anika.patel@strimeuniversity.edu",
                    Notes = "Sample note."
                };
                await _database.InsertAsync(sampleCourse);

                // Two assessments
                var pa = new Assessment
                {
                    CourseId = sampleCourse.Id,
                    Type = AssessmentType.Performance,
                    Title = "Performance Assessment 1",
                    StartDate = sampleCourse.StartDate.AddDays(30),
                    EndDate = sampleCourse.EndDate.AddDays(-30)
                };
                await _database.InsertAsync(pa);

                var oa = new Assessment
                {
                    CourseId = sampleCourse.Id,
                    Type = AssessmentType.Objective,
                    Title = "Objective Assessment 1",
                    StartDate = sampleCourse.StartDate.AddDays(60),
                    EndDate = sampleCourse.EndDate.AddDays(-15)
                };
                await _database.InsertAsync(oa);
            }
        }

        // Term CRUD Operations
        public Task<List<Term>> GetTermsAsync() => _database.Table<Term>().ToListAsync();
        public Task<Term> GetTermAsync(int id) => _database.GetAsync<Term>(id);
        public Task<int> SaveTermAsync(Term term) => term.Id == 0 ? _database.InsertAsync(term) : _database.UpdateAsync(term);
        public Task<int> DeleteTermAsync(Term term) => _database.DeleteAsync(term);

        // Course CRUD Operations
        public Task<List<Course>> GetCoursesAsync(int termId) => _database.Table<Course>().Where(c => c.TermId == termId).ToListAsync();
        public Task<Course> GetCourseAsync(int id) => _database.GetAsync<Course>(id);
        public Task<int> SaveCourseAsync(Course course) => course.Id == 0 ? _database.InsertAsync(course) : _database.UpdateAsync(course);
        public Task<int> DeleteCourseAsync(Course course) => _database.DeleteAsync(course);
        // Limit to 6
        public async Task<bool> CanAddCourseAsync(int termId)
        {
            var count = await GetCoursesAsync(termId).ContinueWith(t => t.Result.Count);
            return count < 6;
        }

        // Assessment CRUD Operations
        public Task<List<Assessment>> GetAssessmentsAsync(int courseId) => _database.Table<Assessment>().Where(a => a.CourseId == courseId).ToListAsync();
        public Task<Assessment> GetAssessmentAsync(int id) => _database.GetAsync<Assessment>(id);
        public Task<int> SaveAssessmentAsync(Assessment assessment) => assessment.Id == 0 ? _database.InsertAsync(assessment) : _database.UpdateAsync(assessment);
        public Task<int> DeleteAssessmentAsync(Assessment assessment) => _database.DeleteAsync(assessment);
        public async Task<bool> CanAddAssessmentAsync(int courseId)
        {
            var count = await GetAssessmentsAsync(courseId).ContinueWith(c => c.Result.Count);
            return count < 2;
        }

        // Notes CRUD Operations
        public Task<List<Notes>> GetNotesAsync(int courseId) => _database.Table<Notes>().Where(n => n.CourseId == courseId).ToListAsync();
        public Task<int> SaveNoteAsync(Notes note) => note.Id == 0 ? _database.InsertAsync(note) : _database.UpdateAsync(note);
        public Task<int> DeleteNoteAsync(Notes note) => _database.DeleteAsync(note);
    }
}
