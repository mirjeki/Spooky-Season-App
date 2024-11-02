using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpookySeason.ViewModels
{
    public class FilmDetailViewModel : INotifyPropertyChanged
    {
        private bool _isEditMode;
        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                _isEditMode = value;
                OnPropertyChanged(nameof(IsEditMode));
            }
        }

        public Film CurrentFilm { get; private set; }

        public Command ToggleEditModeCommand { get; }
        public Command SaveChangesCommand { get; }

        private Action SaveFilmData;

        public FilmDetailViewModel(Film film, Action saveFilmData)
        {
            CurrentFilm = film;
            SaveFilmData = saveFilmData;
            ToggleEditModeCommand = new Command(ToggleEditMode);
            SaveChangesCommand = new Command(SaveChanges);
            IsEditMode = false; // Initially read-only mode
        }

        public DateTime WatchDate
        {
            get => DateTime.TryParse(CurrentFilm.WatchedDate, out var date) ? date : DateTime.Today;
            set
            {
                CurrentFilm.WatchedDate = value.ToString("yyyy-MM-dd");
                OnPropertyChanged(nameof(WatchDate));
            }
        }

        private void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;
        }

        private void SaveChanges()
        {
            // Save the film data back to the JSON file
            SaveFilmData?.Invoke();

            IsEditMode = false; // Exit edit mode after saving
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
