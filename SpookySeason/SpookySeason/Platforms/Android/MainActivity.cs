using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.AppCompat.App;

namespace SpookySeason
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Force dark mode on Android
            AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightYes;
        }
    }
}
