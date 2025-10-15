using AcademicsTrackerMauiNew.Models;
using AcademicsTrackerMauiNew.Services;

namespace AcademicsTrackerMauiNew.Views;

public partial class TermEditPage : ContentPage
{
    private readonly DatabaseService _dbService;
    private readonly Term? _currentTerm;

    // Constructor for new term
    public TermEditPage(DatabaseService dbService)
    {
        InitializeComponent();
        _dbService = dbService;
        SetupForNew();
    }

    // Constructor for edit term
    public TermEditPage(DatabaseService dbService, Term term)
    {
        InitializeComponent();
        _dbService = dbService;
        _currentTerm = term;
        TitleEntry.Text = term.Title;
        StartDatePicker.Date = term.StartDate;
        EndDatePicker.Date = term.EndDate;
    }

    private void SetupForNew()
    {
        TitleEntry.Text = string.Empty;
        StartDatePicker.Date = DateTime.Today;
        // Default 6 months for a term
        EndDatePicker.Date = DateTime.Today.AddMonths(6);
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(TitleEntry.Text) || StartDatePicker.Date >= EndDatePicker.Date)
        {
            await DisplayAlert("Invalid Input", "Please enter a title and ensure start date is before end date.", "OK");
            return;
        }

        var term = _currentTerm ?? new Term();
        term.Title = TitleEntry.Text;
        term.StartDate = StartDatePicker.Date;
        term.EndDate = EndDatePicker.Date;

        await _dbService.SaveTermAsync(term);
        // Back to home
        await Navigation.PopAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}