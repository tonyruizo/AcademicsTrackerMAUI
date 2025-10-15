using AcademicsTrackerMauiNew.Models;
using AcademicsTrackerMauiNew.Services;

namespace AcademicsTrackerMauiNew.Views;

public partial class AssessmentEditPage : ContentPage
{
    private readonly DatabaseService _dbService;
    private readonly NotificationService _notificationService;
    private readonly Assessment? _currentAssessment;
    private readonly int _courseId;

    public AssessmentEditPage(DatabaseService dbService, NotificationService notificationService, int courseId, Assessment? assessment = null, AssessmentType? defaultType = null)
    {
        InitializeComponent();
        _dbService = dbService;
        _notificationService = notificationService;
        _courseId = courseId;
        TypePicker.ItemsSource = Enum.GetNames(typeof(AssessmentType));
        if (assessment == null)
        {
            _currentAssessment = new Assessment { CourseId = _courseId, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(30), Type = defaultType ?? AssessmentType.Performance };
            TypePicker.SelectedIndex = (int)_currentAssessment.Type;
        }
        else
        {
            _currentAssessment = assessment;
            TypePicker.SelectedIndex = (int)_currentAssessment.Type;
            TitleEntry.Text = _currentAssessment.Title;
            StartDatePicker.Date = _currentAssessment.StartDate;
            EndDatePicker.Date = _currentAssessment.EndDate;
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleEntry.Text) || StartDatePicker.Date >= EndDatePicker.Date)
        {
            await DisplayAlert("Invalid Input", "Title required and start before due date.", "OK");
            return;
        }

        if (_currentAssessment == null) return;

        _currentAssessment.Type = (AssessmentType)TypePicker.SelectedIndex;
        _currentAssessment.Title = TitleEntry.Text;
        _currentAssessment.StartDate = StartDatePicker.Date;
        _currentAssessment.EndDate = EndDatePicker.Date;

        await _dbService.SaveAssessmentAsync(_currentAssessment);

        await Navigation.PopAsync();
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_currentAssessment != null && await DisplayAlert("Confirm", "Delete this assessment?", "Yes", "No"))
        {
            NotificationService.CancelAssessmentNotifications(_currentAssessment.Id);
            await _dbService.DeleteAssessmentAsync(_currentAssessment);
            await Navigation.PopAsync();
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}