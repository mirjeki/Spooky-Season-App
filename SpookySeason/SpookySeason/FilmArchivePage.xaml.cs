using Microsoft.Maui.Controls.PlatformConfiguration;
using SpookySeason.ViewModels;

namespace SpookySeason;

public partial class FilmArchivePage : ContentPage
{
    List<Film> films = new List<Film>();
    private Film film;
    private ToolbarItem editToolbarItem;
    private ToolbarItem saveToolbarItem;

    public FilmArchivePage(Film selectedFilm, Action saveFilmData)
    {
        InitializeComponent();
        film = selectedFilm;
        var viewModel = new FilmDetailViewModel(film, saveFilmData);
        BindingContext = viewModel;

        // Create toolbar items
        editToolbarItem = new ToolbarItem { Text = "Edit" };
        saveToolbarItem = new ToolbarItem { Text = "Save" };

        // Bind commands
        editToolbarItem.Command = viewModel.ToggleEditModeCommand;
        saveToolbarItem.Command = viewModel.SaveChangesCommand;

        // Add the initial toolbar item
        ToolbarItems.Add(editToolbarItem);

        // Listen for changes to update toolbar items
        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(viewModel.IsEditMode))
            {
                UpdateToolbarItems(viewModel.IsEditMode);
            }
        };
    }

    private void UpdateToolbarItems(bool isEditMode)
    {
        ToolbarItems.Clear();
        if (isEditMode)
        {
            ToolbarItems.Add(saveToolbarItem);
        }
        else
        {
            ToolbarItems.Add(editToolbarItem);
        }
    }

    private async void OnShareClicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Share", "Would you like to share your comments?", "Yes", "No");

        if (answer)
        {
            await ShareClicked();
        }

        await Navigation.PopAsync();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }


    private async Task ShareClicked()
    {
        string shareText = $"Film: {film.Title}\n" +
                           $"Watched On: {film.WatchedDate}\n" +
                           $"Score: {film.Rating}\n" +
                           $"Comments: {film.Comments}\n";

        await Share.RequestAsync(new ShareTextRequest
        {
            Text = shareText,
            Title = "Share film details"
        });
    }
}