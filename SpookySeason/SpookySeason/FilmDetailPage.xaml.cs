using Microsoft.Maui.Controls.PlatformConfiguration;

namespace SpookySeason;

public partial class FilmDetailPage : ContentPage
{
    private Film _film;
    private string _filmImageFilePath;
    private Action _saveFilmData;

    public FilmDetailPage(Film film, Action saveFilmData)
    {
        InitializeComponent();
        _film = film;
        _saveFilmData = saveFilmData;

        var convertedFilmTitle = film.Title.Replace(' ', '_').ToLower();
        // Set data bindings
        _filmImageFilePath = Path.Combine(FileSystem.AppDataDirectory, "Assets", $"{convertedFilmTitle}");
        TitleLabel.Text = _film.Title;
        RatingPicker.SelectedIndex = _film.Rating;
        CommentsEditor.Text = _film.Comments;
    }

    private void OnRatingSelected(object sender, EventArgs e)
    {
        _film.Rating = RatingPicker.SelectedIndex;
    }

    // Event handler for when the user clicks the save button
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // Update the film object with the user's input
        _film.Watched = true;
        _film.Rating = RatingPicker.SelectedIndex;
        _film.Comments = CommentsEditor.Text;

        // Save the film data back to the JSON file
        _saveFilmData?.Invoke();

        bool answer = await DisplayAlert("Share", "Would you like to share your comments?", "Yes", "No");

        if (answer)
        {
            await ShareClicked();
            //var filmImageService = new FilmImageService();
            //var imagePath = await filmImageService.CreateFilmImageWithText(_film, _filmImageFilePath);

            //// Share the generated image to Instagram Story
            //await ShareToInstagramStory(imagePath);
        }

        // Go back to the previous page
        await Navigation.PopAsync();
    }

    private async void OnVetoClicked(object sender, EventArgs e)
    {
        // Go back to the previous page
        await Navigation.PopAsync();
    }

    private async Task ShareToInstagramStory(string imagePath)
    {
        var storyUri = new Uri($"instagram://story");

        // Check if Instagram is installed
        if (await Launcher.CanOpenAsync(storyUri))
        {
            // Use Instagram's story sharing feature
            var file = new ShareFile(imagePath);
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Share to Instagram Story",
                File = file
            });
        }
        else
        {
            await DisplayAlert("Instagram Not Available", "Instagram is not installed on this device.", "OK");
        }
    }

    private async Task ShareClicked()
    {
        // Prepare film data for sharing
        string shareText = $"Film: {_film.Title}\n" +
                           $"Score: {_film.Rating}\n" +
                           $"Comments: {_film.Comments}";

        // Use Share API to share the text
        await Share.RequestAsync(new ShareTextRequest
        {
            Text = shareText,
            Title = "Share film details"
        });
    }
}