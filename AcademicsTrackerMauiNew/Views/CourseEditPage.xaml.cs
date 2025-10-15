using AcademicsTrackerMauiNew.Models;
using AcademicsTrackerMauiNew.Services;
using System.Text.RegularExpressions;

namespace AcademicsTrackerMauiNew.Views;

public partial class CourseEditPage : ContentPage
{
    private readonly DatabaseService _dbService;
    private readonly NotificationService _notificationService;
    private Course? _currentCourse;
    private readonly int _termId;
    //private readonly bool _isNew;

    // Constructor for new course
    public CourseEditPage(DatabaseService dbService, NotificationService notificationService, int termId)
    {
        InitializeComponent();
        _dbService = dbService;
        _notificationService = notificationService;
        _termId = termId;
        InstructorPhoneEntry.TextChanged += OnPhoneTextChanged;
        SetupForNew();
    }

    // Constructor for edit course
    public CourseEditPage(DatabaseService dbService, NotificationService notificationService, int termId, Course course)
    {
        InitializeComponent();
        _dbService = dbService;
        _notificationService = notificationService;
        _termId = termId;
        _currentCourse = course;
        InstructorPhoneEntry.TextChanged += OnPhoneTextChanged;
        SetupForEdit();
    }

    private void SetupForNew()
    {
        StatusPicker.ItemsSource = Enum.GetNames(typeof(CourseStatus));
        _currentCourse = new Course { TermId = _termId, StartDate = DateTime.Today, EndDate = DateTime.Today.AddMonths(3) };
        StartDatePicker.Date = _currentCourse.StartDate;
        EndDatePicker.Date = _currentCourse.EndDate;
    }

    private void SetupForEdit()
    {
        StatusPicker.ItemsSource = Enum.GetNames(typeof(CourseStatus));
        if (_currentCourse != null)
        {
            TitleEntry.Text = _currentCourse.Title;
            StartDatePicker.Date = _currentCourse.StartDate;
            EndDatePicker.Date = _currentCourse.EndDate;
            StatusPicker.SelectedIndex = (int)_currentCourse.Status;
            InstructorNameEntry.Text = _currentCourse.InstructorName;
            InstructorPhoneEntry.Text = _currentCourse.InstructorPhone;
            InstructorEmailEntry.Text = _currentCourse.InstructorEmail;
        }
    }

    private void OnPhoneTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is Entry entry)
        {
            var text = entry.Text.Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", "");
            if (text.Length > 0)
            {
                if (text.Length <= 3)
                    entry.Text = text;
                else if (text.Length <= 6)
                    entry.Text = $"{text.Substring(0, 3)}-{text.Substring(3)}";
                else
                    entry.Text = $"{text.Substring(0, 3)}-{text.Substring(3, 3)}-{text.Substring(6)}";
                entry.CursorPosition = entry.Text.Length;
            }
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(TitleEntry.Text) || !IsValidEmail(InstructorEmailEntry.Text))
        {
            await DisplayAlert("Invalid Input", "Title and valid email required.", "OK");
            return;
        }

        // Phone validation (min 10 digits & regex for US format)
        var phone = InstructorPhoneEntry.Text;
        if (string.IsNullOrWhiteSpace(phone) || phone.Length < 10 || !Regex.IsMatch(phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""), @"^\d{10}$"))
        {
            await DisplayAlert("Invalid Input", "Phone must be 10 digits.", "OK");
            return;
        }

        if (_currentCourse == null) return;

        _currentCourse.Title = TitleEntry.Text;
        _currentCourse.StartDate = StartDatePicker.Date;
        _currentCourse.EndDate = EndDatePicker.Date;
        _currentCourse.Status = (CourseStatus)StatusPicker.SelectedIndex;
        _currentCourse.InstructorName = InstructorNameEntry.Text;
        _currentCourse.InstructorPhone = InstructorPhoneEntry.Text;
        _currentCourse.InstructorEmail = InstructorEmailEntry.Text;

        await _dbService.SaveCourseAsync(_currentCourse);
        await Navigation.PopAsync();
    }

    private static bool IsValidEmail(string email) => !string.IsNullOrWhiteSpace(email) && email.Contains('@') && email.Contains('.');



    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}