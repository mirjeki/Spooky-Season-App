using Plugin.Maui.Audio;

namespace SpookySeason
{
    public class AudioService
    {
        private readonly IAudioManager _audioManager;
        private IAudioPlayer _audioPlayer;

        public AudioService(IAudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        public async Task PlayAudio(string audioName)
        {
            if (_audioPlayer == null)
            {
                _audioPlayer = _audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync($"{audioName}.mp3"));
            }

            _audioPlayer.Play();
        }

        public void StopAudio()
        {
            _audioPlayer?.Stop();
        }
    }
}


