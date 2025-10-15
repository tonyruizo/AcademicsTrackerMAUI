using AcademicsTrackerMauiNew.Models;
using AcademicsTrackerMauiNew.Services;

namespace AcademicsTrackerMauiNew.Views;

public partial class TermDetailPage : ContentPage
{
    private readonly DatabaseService _dbService;
    private readonly Term _currentTerm;
    private readonly int _termId;
    private readonly NotificationService _notificationService;

    public TermDetailPage(DatabaseService dbService, NotificationService notificationService, Term term)
    {
        InitializeComponent();
        _dbService = dbService;
        _notificationService = notificationService;
        _currentTerm = term;
        _termId = term.Id;
        TermTitleLabel.Text = $"{term.Title}";
        TermDatesLabel.Text = $"{term.StartDate:MM/dd/yyyy} - {term.EndDate:MM/dd/yyyy}";
        CourseCountLabel.Text = $"Courses: 0/6";
        _ = LoadCourses();  // async load
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCourses();
    }

    private async Task LoadCourses()
    {
        var courses = await _dbService.GetCoursesAsync(_termId);
        CoursesView.ItemsSource = courses;
        CourseCountLabel.Text = $"Courses: {courses.Count}/6";  // Update count
        AddCourseButton.IsEnabled = await _dbService.CanAddCourseAsync(_termId);
    }

    private async void OnAddCourseClicked(object sender, EventArgs e)
    {
        if (await _dbService.CanAddCourseAsync(_termId))
        {
            await Navigation.PushAsync(new CourseEditPage(_dbService, _notificationService, _termId));
        }
        else
        {
            await DisplayAlert("Limit Reached", "Maximum 6 courses per term.", "OK");
        }
    }

    private async void OnView(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Course course)
        {
            await Navigation.PushAsync(new CourseDetailPage(_dbService, _notificationService, course));
        }
    }

    private async void OnEdit(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Course course)
        {
            await Navigation.PushAsync(new CourseEditPage(_dbService, _notificationService, _termId, course));
            await LoadCourses();
        }
    }

    private async void OnDelete(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Course course)
        {
            if (await DisplayAlert("Confirm Delete", $"Delete course '{course.Title}' and all its assessments/alerts/notes?", "Yes", "No"))
            {
                NotificationService.CancelCourseNotifications(course.Id);
                var assessments = await _dbService.GetAssessmentsAsync(course.Id);
                foreach (var assessment in assessments)
                {
                    NotificationService.CancelAssessmentNotifications(assessment.Id);
                    await _dbService.DeleteAssessmentAsync(assessment);
                }
                await _dbService.DeleteCourseAsync(course);
                await LoadCourses();  // Refresh courses list
            }
        }
    }

}