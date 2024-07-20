using System.Collections.ObjectModel;
using PM2E2Grupo1.Models;
using PM2E2Grupo1.ViewModels;
using PM2E2Grupo1.Controllers;

namespace PM2E2Grupo1.Views
{
    public partial class PageList : ContentPage
    {
        private ApiService _apiService;
        private readonly GeocodingService _geocodingService;
        private Sitios sitio;

        public ObservableCollection<sitiosViewModel> SitioItems { get; set; }
        public PageList()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = this;
            _apiService = new ApiService();
            _geocodingService = new GeocodingService(Config.Config.GoogleApiKey);

            ShowLoadingDialog();
            LoadData();

        }

        private void listasitioaudio_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            sitio = (Sitios)e.Item;
        }


        private async void LoadData()
        {
            var locations = await _apiService.GetLocationsAsync();
            listasitios.ItemsSource = locations;
        }




        private async void btneditar_Clicked(object sender, EventArgs e)
        {
            //try
            //{

                if (listasitios.SelectedItem is Sitios selectedSitio)
                {
                    bool confirm = await Application.Current.MainPage.DisplayAlert("Confirmacion", "Desea modificar el sito seleccionado? "+ selectedSitio.Descripcion, "Si", "Cancelar");

                    if (confirm)
                    {
                        await Navigation.PushAsync(new Views.PageEdit(selectedSitio));

                    }
                }

           // }
           // catch
           // {
            //    await DisplayAlert("Advertencia", "Favor seleccione el sitio donde desea editar", "Ok");
           // }
        }

        private async void btnvermapa_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (listasitios.SelectedItem is Sitios selectedSitio)
                {

                    await Navigation.PushAsync(new ver_mapa(selectedSitio, selectedSitio.Descripcion));

                }

            }
            catch
            {
                await DisplayAlert("Advertencia", "Favor seleccione el sitio donde desea ver en el mapa", "Ok");
            }
        }

        private async void btndetalle_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (listasitios.SelectedItem is Sitios selectedSitio)
                {

                    await Navigation.PushAsync(new siteView(selectedSitio, selectedSitio.Descripcion));

                }

            }
            catch
            {
                await DisplayAlert("Advertencia", "Favor seleccione el sitio donde desea ver.", "Ok");
            }
        }


        private async void btneliminarregistro_Clicked(object sender, EventArgs e)
         {
            try
            {
                if (listasitios.SelectedItem is Sitios selectedSitio)
                {
                    bool confirm = await Application.Current.MainPage.DisplayAlert("Confirmaci�n", "�Est�s seguro de querer eliminar el sito seleccionado?", "S�", "Cancelar");

                    if (confirm)
                    {
                        int item = Convert.ToInt16(selectedSitio.id);

                        bool isDeleted = await _apiService.DeleteDataAsync("/api/sitio", item);
                        if (isDeleted)
                        {
                            await DisplayAlert("Alerta", "El sitio ha sido eliminado", "OK");
                            //SitioItems.Remove(selectedSitio);
                            //listasitios.ItemsSource = SitioItems;
                            LoadData();

                        }
                        else
                        {
                            await DisplayAlert("Advertencia", "Ha ocurrido un error", "Aceptar");
                        }
                    }
                }
            }
            catch
            {
                await DisplayAlert("Advertencia", "Favor seleccione que sitio desea eliminar", "Aceptar");
            }

        }



        private async void btnOnShareImage_Clicked(object sender, EventArgs e)
        {
            List<ShareFile> shareFiles = new List<ShareFile>();

            byte[] imageBytes = Convert.FromBase64String(sitio.Fotografia);

            // Recorrer los elementos seleccionados y guardar cada imagen en almacenamiento temporal
            var tempImagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"{sitio.id}_sharedimage.png");
            File.WriteAllBytes(tempImagePath, imageBytes);
            shareFiles.Add(new ShareFile(tempImagePath));


            // Compartir las im�genes usando .NET MAUI
            await Share.RequestAsync(new ShareMultipleFilesRequest
            {
                Title = "Compartir im�gen",
                Files = shareFiles
            });
        }

        private async void btnPlayAudio_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (listasitios.SelectedItem is Sitios selectedSitio)
                {

                   // await Navigation.PushAsync(new siteView(selectedSitio, selectedSitio.Descripcion));

                }

            }
            catch
            {
                await DisplayAlert("Advertencia", "Favor seleccione el sitio que desea reproducir audio.", "Ok");
            }
        }



        private async void HandleVerMediaTapped(Sitios item, string lugar)
        {
            var verMedia = new siteView(item, lugar);

            await Navigation.PushModalAsync(new NavigationPage(verMedia));
        }
        private async void HandleVerMapaTapped(Sitios item, string lugar)
        {
            await Navigation.PushAsync(new Views.ver_mapa(item, lugar));
        }

        private async void HandleEditarTapped(Sitios item)
        {
            //await Navigation.PushAsync(new Views.editarSitio(item));
        }

        private async void HandleEliminarTapped(Sitios sitio)
        {
            var tappedItem = SitioItems.FirstOrDefault(item => item.IdItem == sitio.id);

            if (tappedItem != null)
            {
                bool userConfirmed = await DisplayAlert("Confirmaci�n", "�Est� seguro de que desea eliminar este sitio?", "Si", "No");

                if (userConfirmed)
                {
                    bool isDeleted = await _apiService.DeleteDataAsync("sitios", sitio.id);
                    if (isDeleted)
                    {
                        await DisplayAlert("Alerta", "El sitio ha sido eliminado", "OK");
                        SitioItems.Remove(tappedItem);
                        listasitios.ItemsSource = SitioItems;
                    }
                }
            }
        }

        private void btnRegresar_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PageHome());
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Limpia los archivos temporales
            //CleanUpTempFiles();
        }

        public void CleanUpTempFiles()
        {
            try
            {
                // Get all files in the LocalApplicationData directory with the specified prefix
                string[] tempVideoFiles = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "tempFoto_*");
                string[] tempAudioFiles = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "tempAudio_*");

                // Delete each temporary file
                foreach (string file in tempVideoFiles)
                {
                    File.Delete(file);
                }

                foreach (string file in tempAudioFiles)
                {
                    File.Delete(file);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
            }
        }

        private void ShowLoadingDialog()
        {
            // Show your loading dialog here
            //activityIndicator.IsRunning = true;
            // activityIndicator.IsVisible = true;
            //btnRegresar.IsEnabled = false;
        }

        private void HideLoadingDialog()
        {
            // Hide your loading dialog here
            //activityIndicator.IsRunning = false;
            //activityIndicator.IsVisible = false;
           // btnRegresar.IsEnabled = true;
        }
    }
}