using AcademicsTrackerMauiNew.Models;
using AcademicsTrackerMauiNew.Services;
using System.Runtime.Versioning;

namespace AcademicsTrackerMauiNew.Views;

public partial class CourseDetailPage : ContentPage
{
    private readonly DatabaseService _dbService;
    private readonly NotificationService _notificationService;
    private readonly Course _currentCourse;

    [SupportedOSPlatform("windows10.0.17763.0")]
    public CourseDetailPage(DatabaseService dbService, NotificationService notificationService, Course course)
    {
        InitializeComponent();
        _dbService = dbService;
        _notificationService = notificationService;
        _currentCourse = course;
        CourseTitleLabel.Text = _currentCourse.Title;
        CourseDatesLabel.Text = $"Start: {_currentCourse.StartDate:MM/dd/yyyy} - End: {_currentCourse.EndDate:MM/dd/yyyy}";
        StatusLabel.Text = $"Status: {_currentCourse.Status}";
        InstructorLabel.Text = $"Instructor: {_currentCourse.InstructorName}\n" +
                       $"                    {_currentCourse.InstructorPhone}\n" +
                       $"                    {_currentCourse.InstructorEmail}";
        AssessmentCountLabel.Text = $"Assessments: 0/2";
        _ = LoadAssessments();
    }

    [SupportedOSPlatform("windows10.0.17763.0")]
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAssessments();
    }

    [SupportedOSPlatform("windows10.0.17763.0")]
    private async Task LoadAssessments()
    {
        var assessments = await _dbService.GetAssessmentsAsync(_currentCourse.Id);
        AssessmentsView.ItemsSource = assessments;
        AssessmentCountLabel.Text = $"Assessments: {assessments.Count}/2";
        AddAssessmentButton.IsEnabled = await _dbService.CanAddAssessmentAsync(_currentCourse.Id);
    }

    [SupportedOSPlatform("windows10.0.17763.0")]
    private async void OnEditAssessment(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Assessment assessment)
        {
            await Navigation.PushAsync(new AssessmentEditPage(_dbService, _notificationService, _currentCourse.Id, assessment));
            await LoadAssessments();
        }
    }

    [SupportedOSPlatform("windows10.0.17763.0")]
    private async void OnAssessmentAlerts(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Assessment assessment)
        {
            await Navigation.PushAsync(new AssessmentAlertsPage(_dbService, _notificationService, assessment));
        }
    }

    [SupportedOSPlatform("windows10.0.17763.0")]
    private async void OnDeleteAssessment(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Assessment assessment)
        {
            if (await DisplayAlert("Confirm", $"Delete '{assessment.Title}'?", "Yes", "No"))
            {
                NotificationService.CancelAssessmentNotifications(assessment.Id);
                await _dbService.DeleteAssessmentAsync(assessment);
                await LoadAssessments();
            }
        }
    }

    // Bottom page buttons
    [SupportedOSPlatform("windows10.0.17763.0")]
    private async void OnAddAssessmentClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AssessmentEditPage(_dbService, _notificationService, _currentCourse.Id, null, AssessmentType.Performance));
        await LoadAssessments();
    }

    [SupportedOSPlatform("windows10.0.17763.0")]
    private async void OnNotesClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NotesPage(_dbService, _currentCourse));
    }

    [SupportedOSPlatform("windows10.0.17763.0")]
    private async void OnAlertsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CourseAlertPage(_dbService, _notificationService, _currentCourse));
    }


}