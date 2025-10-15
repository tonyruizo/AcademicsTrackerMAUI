using AcademicsTrackerMauiNew.Models;
using AcademicsTrackerMauiNew.Services;

namespace AcademicsTrackerMauiNew.Views;

public partial class HomePage : ContentPage
{
    private readonly DatabaseService _dbService;
    private readonly NotificationService _notificationService;

    public HomePage(NotificationService notificationService, DatabaseService dbService)
    {
        InitializeComponent();
        _notificationService = notificationService;
        _dbService = dbService;

        // Load Terms
        _ = LoadTerms();
    }

    private async Task LoadTerms()
    {
        var terms = await _dbService.GetTermsAsync();
        TermsView.ItemsSource = terms;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadTerms();
    }

    private async void OnAddTerm(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TermEditPage(_dbService));
    }

    private async void OnEdit(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Term term)
        {
            await Navigation.PushAsync(new TermEditPage(_dbService, term));
        }
    }

    private async void OnView(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Term term)
        {
            await Navigation.PushAsync(new TermDetailPage(_dbService, _notificationService, term));
        }
    }

    private async void OnDelete(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Term term)
        {
            if (await DisplayAlert("Confirm Delete", $"Delete term '{term.Title}' and all its courses/assessments?", "Yes", "No"))
            {
                var courses = await _dbService.GetCoursesAsync(term.Id);
                foreach (var course in courses)
                {
                    NotificationService.CancelCourseNotifications(course.Id);
                    var assessments = await _dbService.GetAssessmentsAsync(course.Id);
                    foreach (var assessment in assessments)
                    {
                        NotificationService.CancelAssessmentNotifications(assessment.Id);
                        await _dbService.DeleteAssessmentAsync(assessment);
                    }
                    await _dbService.DeleteCourseAsync(course);
                }
                await _dbService.DeleteTermAsync(term);
                await LoadTerms();
            }
        }
    }


}