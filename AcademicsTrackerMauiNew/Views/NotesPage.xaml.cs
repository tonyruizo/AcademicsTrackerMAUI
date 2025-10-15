using AcademicsTrackerMauiNew.Models;
using AcademicsTrackerMauiNew.Services;

namespace AcademicsTrackerMauiNew.Views;

public partial class NotesPage : ContentPage
{
    private readonly DatabaseService _dbService;
    private readonly Course _currentCourse;

    public NotesPage(DatabaseService dbService, Course course)
    {
        InitializeComponent();
        _dbService = dbService;
        _currentCourse = course;
        CourseTitleLabel.Text = $"{_currentCourse.Title}";
        NotesEditor.Text = _currentCourse.Notes;

        _ = LoadNotes();
    }

    private async Task LoadNotes()
    {
        _ = await _dbService.GetNotesAsync(_currentCourse.Id);
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        _currentCourse.Notes = NotesEditor.Text;
        await _dbService.SaveCourseAsync(_currentCourse);
        await DisplayAlert("Saved", "Notes updated.", "OK");

        await Navigation.PopAsync();
    }

    private async void OnShareClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(_currentCourse.Notes))
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = _currentCourse.Notes,
                Title = $"Share Notes: {_currentCourse.Title}"
            });
        }
        else
        {
            await DisplayAlert("No Notes", "Add notes to share.", "OK");
        }
    }

}