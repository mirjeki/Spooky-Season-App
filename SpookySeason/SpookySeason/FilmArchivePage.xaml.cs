using Microsoft.Maui.Controls.PlatformConfiguration;

namespace SpookySeason;

public partial class FilmArchivePage : ContentPage
{
    private Film film;

    public FilmArchivePage(Film selectedFilm)
    {
        InitializeComponent();
        film = selectedFilm;
        BindingContext = film;
        //FilmTitle.Text = film.Title;
        //Rating.Text = film.Rating.ToString();
        //Comments.Text = film.Comments;
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
                           $"Score: {film.Rating}\n" +
                           $"Comments: {film.Comments}";

        await Share.RequestAsync(new ShareTextRequest
        {
            Text = shareText,
            Title = "Share film details"
        });
    }
}