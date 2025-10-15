using AcademicsTrackerMauiNew.Models;
using AcademicsTrackerMauiNew.Services;

namespace AcademicsTrackerMauiNew.Views;

public partial class CourseAlertPage : ContentPage
{
    private readonly DatabaseService _dbService;
    private readonly NotificationService _notificationService;
    private readonly Course _currentCourse;

    public CourseAlertPage(DatabaseService dbService, NotificationService notificationService, Course course)
    {
        InitializeComponent();
        _dbService = dbService;
        _notificationService = notificationService;
        _currentCourse = course;

        // Load UI with defaults from course
        CourseTitleLabel.Text = $"Alerts for {_currentCourse.Title}";
        StartDatePicker.Date = _currentCourse.StartDate;
        EndDatePicker.Date = _currentCourse.EndDate;
        NotifyStartCheck.IsChecked = _currentCourse.NotifyStart;
        NotifyEndCheck.IsChecked = _currentCourse.NotifyEnd;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (StartDatePicker.Date >= EndDatePicker.Date)
        {
            await DisplayAlert("Invalid Input", "Start date must be before end date.", "OK");
            return;
        }
#if ANDROID
    var status = await Permissions.RequestAsync<Permissions.PostNotifications>();
    if (status != PermissionStatus.Granted)
    {
        await DisplayAlert("Permission Denied", "Notifications require permission to schedule alerts.", "OK");
        return;
    }
#endif

        _currentCourse.StartDate = StartDatePicker.Date;
        _currentCourse.EndDate = EndDatePicker.Date;
        _currentCourse.NotifyStart = NotifyStartCheck.IsChecked;
        _currentCourse.NotifyEnd = NotifyEndCheck.IsChecked;

        await _dbService.SaveCourseAsync(_currentCourse);


        NotificationService.CancelCourseNotifications(_currentCourse.Id);
        NotificationService.ScheduleCourseNotification(_currentCourse);

        await DisplayAlert("Saved", "Course alerts updated.", "OK");
        await Navigation.PopAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}