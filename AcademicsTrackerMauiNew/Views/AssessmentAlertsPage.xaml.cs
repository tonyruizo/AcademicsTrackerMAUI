using AcademicsTrackerMauiNew.Models;
using AcademicsTrackerMauiNew.Services;

namespace AcademicsTrackerMauiNew.Views;

public partial class AssessmentAlertsPage : ContentPage
{
    private readonly DatabaseService _dbService;
    private readonly NotificationService _notificationService;
    private readonly Assessment _currentAssessment;

    public AssessmentAlertsPage(DatabaseService dbService, NotificationService notificationService, Assessment assessment)
    {
        InitializeComponent();
        _dbService = dbService;
        _notificationService = notificationService;
        _currentAssessment = assessment;

        AssessmentTitleLabel.Text = $"Alerts for {_currentAssessment.Title}";
        StartDatePicker.Date = _currentAssessment.StartDate;
        EndDatePicker.Date = _currentAssessment.EndDate;
        NotifyStartCheck.IsChecked = _currentAssessment.NotifyStart;
        NotifyEndCheck.IsChecked = _currentAssessment.NotifyEnd;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (StartDatePicker.Date >= EndDatePicker.Date)
        {
            await DisplayAlert("Invalid Input", "Start date must be before due date.", "OK");
            return;
        }

        // Update model from UI
        _currentAssessment.StartDate = StartDatePicker.Date;
        _currentAssessment.EndDate = EndDatePicker.Date;
        _currentAssessment.NotifyStart = NotifyStartCheck.IsChecked;
        _currentAssessment.NotifyEnd = NotifyEndCheck.IsChecked;

        await _dbService.SaveAssessmentAsync(_currentAssessment);

        // Request notification permission (Android 13+)
#if ANDROID
        var status = await Permissions.RequestAsync<Permissions.PostNotifications>();
        if (status != PermissionStatus.Granted)
        {
            await DisplayAlert("Permission Denied", "Notifications require permission to schedule alerts.", "OK");
            return;
        }
#endif

        // Cancel old, schedule new notifications
        NotificationService.CancelAssessmentNotifications(_currentAssessment.Id);
        if (_currentAssessment.NotifyStart || _currentAssessment.NotifyEnd)
        {
            NotificationService.ScheduleAssessmentNotification(_currentAssessment);
        }

        await DisplayAlert("Saved", "Assessment alerts updated.", "OK");
        await Navigation.PopAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}