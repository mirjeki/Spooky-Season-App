using Plugin.Maui.Audio;
using System.Text.Json;

namespace SpookySeason;

public partial class FilmPage : ContentPage
{

    List<Film> films = new List<Film>();
    string filePath = "";
    private readonly AudioManager _audioManager;
    private bool isEditMode;

    public FilmPage()
    {
        InitializeComponent();
        filePath = Path.Combine(FileSystem.AppDataDirectory, "films.json");
        FilmImage.Source = ImageSource.FromFile("spooky");
        LoadData();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadData(); // Load or refresh the list of films
    }

    private async void LoadData()
    {
        try
        {
            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);
                films = JsonSerializer.Deserialize<List<Film>>(json);
                bool changesfound = false;

                foreach (var film in films)
                {
                    if (string.IsNullOrEmpty(film.WatchedDate))
                    {
                        changesfound = true;
                        film.WatchedDate = DateTime.Now.ToShortDateString();
                    }
                }

                if (changesfound)
                {
                    var updatedData = JsonSerializer.Serialize(films);
                    await File.WriteAllTextAsync(filePath, updatedData);
                }
            }
            else
            {
                // Load the default file from app package (for first time load)
                using var stream = await FileSystem.OpenAppPackageFileAsync("films.json");
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();
                films = JsonSerializer.Deserialize<List<Film>>(json);

                // Save a copy of this to the writable app data directory for future use
                SaveFilmData(); // Save the initial loaded data to local storage
            }

            List<Film> availableFilms = films.Where(w => w.Watched != true).ToList();
            if (availableFilms.Any())
            {
                FilmLabel.Text = $"There are {availableFilms.Count} scares left in your library!"; 
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Could not load films: " + ex.Message, "OK");
        }
    }

    private async void OnSelectFilmClicked(object sender, EventArgs e)
    {
        //save watched data, additionally, allow user to enter score and comments to be added to later 'review' page
        List<Film> availableFilms = films.Where(w => w.Watched != true).ToList();
        if (availableFilms.Any())
        {
            var random = new Random();
            int index = random.Next(availableFilms.Count);
            var selectedFilm = availableFilms[index];

            await Navigation.PushAsync(new FilmDetailPage(selectedFilm, SaveFilmData));
        }
        else
        {
            FilmLabel.Text = "No films available";
        }
    }

    private async void SaveFilmData()
    {
        var filePath = Path.Combine(FileSystem.AppDataDirectory, "films.json");
        var json = JsonSerializer.Serialize(films);
        await File.WriteAllTextAsync(filePath, json);
    }

    private async void OnResetClicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Reset Data", "Are you sure you want to reset all data to default?", "Yes", "No");
        if (answer)
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("films.json");
                using var reader = new StreamReader(stream);
                var defaultJson = await reader.ReadToEndAsync();

                // Overwrite the existing local JSON file with the default JSON
                await File.WriteAllTextAsync(filePath, defaultJson);

                films = JsonSerializer.Deserialize<List<Film>>(defaultJson);
                await DisplayAlert("Success", "Data has been reset to default.", "OK");

                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to reset data: {ex.Message}", "OK");
            }
        }
    }
}