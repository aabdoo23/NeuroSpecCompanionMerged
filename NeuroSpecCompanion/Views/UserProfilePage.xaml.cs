using Microsoft.Maui.Storage;
using NeuroSpec.Shared.Services.Firebase_Service;

namespace NeuroSpecCompanion.Views
{
    public partial class UserProfilePage : ContentPage
    {
        private Stream _fileStream;

        public UserProfilePage()
        {
            InitializeComponent();
        }

        private async void OnUploadPhotoClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select a photo"
                });

                if (result != null)
                {
                    _fileStream = await result.OpenReadAsync();

                    FirebaseService firebaseService = new FirebaseService();
                    var downloadUrl = await firebaseService.UploadFile(_fileStream);
                    //TODO: Save the download URL to the user's profile'

                    Console.WriteLine("Url: " + downloadUrl);
                    // Display the selected photo
                    userImageButton.Source = ImageSource.FromStream(() => _fileStream);
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

    }
}
