using System.Collections.ObjectModel;
using System.Text.Json;

namespace SpookySeason;

public partial class WatchedArchive : ContentPage
{
    public ObservableCollection<Film> WatchedFilms { get; set; }
    private string filePath;
    private List<Film> films;

    public WatchedArchive()
    {
        InitializeComponent();

        filePath = Path.Combine(FileSystem.AppDataDirectory, "films.json");

        LoadData();

        WatchedFilmsCollectionView.SelectionChanged += WatchedFilmsCollectionView_SelectionChanged;
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
                var orderedFilmList = new List<Film>(films.OrderByDescending(film => DateTime.TryParse(film.WatchedDate, out var date) ? date : DateTime.MinValue));
                films = orderedFilmList;
            }
            else
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("films.json");
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();
                films = JsonSerializer.Deserialize<List<Film>>(json);

                SaveFilmData();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Could not load films: " + ex.Message, "OK");
        }

        WatchedFilms = new ObservableCollection<Film>(GetWatchedFilms());
        WatchedFilmsCollectionView.ItemsSource = WatchedFilms;
    }

    private async void SaveFilmData()
    {
        var filePath = Path.Combine(FileSystem.AppDataDirectory, "films.json");
        var json = JsonSerializer.Serialize(films);
        await File.WriteAllTextAsync(filePath, json);
    }

    private List<Film> GetWatchedFilms()
    {
        return films.Where(w => w.Watched == true).ToList();
    }

    private async void WatchedFilmsCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedFilm = e.CurrentSelection.FirstOrDefault() as Film;
        if (selectedFilm != null)
        {
            await Navigation.PushAsync(new FilmArchivePage(selectedFilm, SaveFilmData));
        }
    }

    private async void OnFilmTapped(object sender, EventArgs e)
    {
        var frame = (Frame)sender;
        var tappedFilm = (Film)frame.BindingContext;

        if (tappedFilm != null)
        {
            await Navigation.PushAsync(new FilmArchivePage(tappedFilm, SaveFilmData));
        }
    }
}