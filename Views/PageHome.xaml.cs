using PM2E2Grupo1.Models;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Maui.ApplicationModel.Permissions;
using Plugin.AudioRecorder;
using System.Net.NetworkInformation;
using PM2E2Grupo1.Controllers;
using PM2E2Grupo1.Extensions;
using Microsoft.Maui;
using CommunityToolkit.Maui.Views;
using SkiaSharp;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Diagnostics;

namespace PM2E2Grupo1.Views
{
    public partial class PageHome : ContentPage
    {
        private ApiService _apiService;

        //audio
        private AudioRecorderService recorder;
        private string audioFilePath;
        private bool isRecording;
        private byte[] audioBytes;
        private string base64Audio;
        private byte[] fotoBytes;
        private string base64Foto;
        private double Latitude = 00.00;
        private double Longitude = 00.00;

        byte[] imageToSave;


        public PageHome()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            _apiService = new ApiService();
            recorder = new AudioRecorderService
            {
                StopRecordingAfterTimeout = false,
                TotalAudioTimeout = TimeSpan.FromSeconds(30) // Ajusta el tiempo seg�n tus necesidades
            };
            InitializePage();
        }

        private async void InitializePage()
        {
            bool isInternet = await IsInternetAvailable();

            if (isInternet)
            {
                try
                {
                    // Revisa si el permiso de ubicacion ha sido concedido
                    var locationPermissionStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                    if (locationPermissionStatus == PermissionStatus.Granted)
                    {
                        // Obtiene la ubicacion
                        var location = await Geolocation.GetLocationAsync(new GeolocationRequest
                        {
                            DesiredAccuracy = GeolocationAccuracy.Default,
                            Timeout = TimeSpan.FromSeconds(10)
                        });

                        if (location != null)
                        {
                            Latitude = location.Latitude;
                            Longitude = location.Longitude;
                            // Coloca la latitude y longitud en los labels
                            labelLatitude.Text = $"{location.Latitude}";
                            labelLongitude.Text = $"{location.Longitude}";
                        }
                        else if (labelLatitude.Text.Equals("00.00") || labelLongitude.Text.Equals("00.00"))
                        {
                            // Cuando la ubicacion es nula
                            await DisplayAlert("Alerta", "El GPS se encuentra desactivado. Porfavor active su GPS y abra la aplicaci�n de nuevo!", "Ok");
                        }
                    }
                    else
                    {
                        // Cuando el permiso no es otorgado
                        await DisplayAlert("Error", "Permiso de Ubicaci�n no otorgado. El Permiso es necesario para utilizar la aplicacion.", "OK");
                        Application.Current.Quit();
                    }
                }
                catch (FeatureNotEnabledException)
                {
                    try
                    {
                        await Application.Current.MainPage.DisplayAlert("Alerta", "El GPS se encuentra desactivado. Porfavor active su GPS y abra la aplicaci�n de nuevo!", "Ok");
                        Application.Current.Quit();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in DisplayGpsNotEnabledAlert: {ex.Message}");
                    }

                }
            }
            else
            {
                await DisplayAlert("Alerta", "Su Dispositivo no tiene acceso a internet!", "Ok");
                await Navigation.PopAsync();
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }

        }

        public async Task<bool> IsInternetAvailable()
        {
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                // Connection to internet is available, perform ping test
                try
                {
                    var ping = new Ping();
                    var response = await ping.SendPingAsync("www.google.com", 1000); // Ping Google's server
                    return response.Status == IPStatus.Success;
                }
                catch (PingException)
                {
                    // Ping failed
                    return false;
                }
            }
            else
            {
                // No network available
                return false;
            }
        }




        //void para tomar la foto
        private async void btnfoto_Clicked(object sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();

                if (photo != null)
                {
                    imageToSave = null;

                    // Obtener la ruta local para guardar la imagen
                    string localizacion = System.IO.Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                    using (Stream sourceStream = await photo.OpenReadAsync())
                    {
                        // Redimensionar la imagen
                        var resizedStream = ResizeImage(sourceStream, 200, 200);

                        // Convertir el flujo redimensionado a una cadena Base64
                        base64Foto = ConvertToBase64(resizedStream);

                        // Guardar la imagen redimensionada localmente
                        using (FileStream imagenLocal = File.OpenWrite(localizacion))
                        {
                            await resizedStream.CopyToAsync(imagenLocal);
                        }

                        // Posicionar el flujo redimensionado al inicio para su posterior uso
                        resizedStream.Position = 0;

                        // Leer la imagen redimensionada en un arreglo de bytes
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            await resizedStream.CopyToAsync(memoryStream);
                            imageToSave = memoryStream.ToArray();
                        }

                        // Mostrar la imagen en el UI
                        img.Source = ImageSource.FromStream(() => new MemoryStream(imageToSave));
                    }

                    entryDescripcion.Focus();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Se ha generado el siguiente error al agregar la imagen: " + ex.Message, "Aceptar");
            }
        }

        private string ConvertToBase64(Stream stream)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    var imageBytes = memoryStream.ToArray();

                    if (imageBytes.Length == 0)
                    {
                        throw new Exception("La imagen est� vac�a.");
                    }

                    return Convert.ToBase64String(imageBytes);
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine($"Error al convertir la imagen a Base64: {ex.Message}");
                throw;
            }
        }

        private Stream ResizeImage(Stream inputStream, int width, int height)
        {
            using (var original = SKBitmap.Decode(inputStream))
            {
                var resized = original.Resize(new SKImageInfo(width, height), SKFilterQuality.Medium);

                if (resized == null)
                {
                    throw new Exception("Error al redimensionar la imagen.");
                }

                var image = SKImage.FromBitmap(resized);
                var data = image.Encode(SKEncodedImageFormat.Jpeg, 80); // Ajusta la calidad seg�n tus necesidades

                var resizedStream = new MemoryStream();
                data.SaveTo(resizedStream);
                resizedStream.Seek(0, SeekOrigin.Begin);

                return resizedStream;
            }
        }




        private async void btnAgregar_Clicked(object sender, EventArgs e)
        {
            ShowLoadingDialog();

            Console.WriteLine($"RutaAudio: {base64Audio}");
            if (string.IsNullOrEmpty(base64Foto))
            {
                await DisplayAlert("Alerta", "Por favor agregar una foto.", "OK");
                HideLoadingDialog();
                return;
            }

            if (Latitude == 00.00 || Longitude == 00.00)
            {
                await DisplayAlert("Alerta", "No existen datos de geolocalizaci�n", "OK");
                HideLoadingDialog();
                return;
            }

            /*if (string.IsNullOrEmpty(base64Audio))
            {
                await DisplayAlert("Alerta", "Por favor grabe un audio con la descripci�n de la ubicaci�n (mantenga presionado el icono del micr�fono para grabar)", "OK");
                HideLoadingDialog();
                return;
            }*/

            if (string.IsNullOrEmpty(entryDescripcion.Text))
            {
                await DisplayAlert("Alerta", "Por favor ingrese un t�tulo para el lugar", "OK");
                HideLoadingDialog();
                return;
            }

            string sanitizedInput = entryDescripcion.Text;

            //string sanitizedInput = InputSanitizer.SanitizeInput(entryDescripcion.Text);

            var data = new
            {
                descripcion = sanitizedInput,
                latitud = Latitude,
                longitud = Longitude,
                fotografia = base64Foto,
                audiofile = base64Audio
            };

            bool isSuccess = await _apiService.PostSuccessAsync("/api/sitio/create", data);


            if (isSuccess)
            {
                HideLoadingDialog();
                await DisplayAlert("�Guardado!", "El sitio ha sido guardado", "OK");
                //mediaElementAudio.Source = null;
                //mediaElementVideo.Source = null;
                base64Audio = string.Empty;
                base64Foto = string.Empty;
                entryDescripcion.Text = string.Empty;
                img.Source = "anadir.png";
                imageToSave = null;
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Hubo un error al subir la foto", "OK");
            }
        }



        private async void btnStart_Clicked(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "Estado: Grabando...";
                btnStart.IsEnabled = false;
                btnStop.IsEnabled = true;

                var status = await Permissions.RequestAsync<Permissions.Microphone>();

                if (status == PermissionStatus.Granted)
                {
                    await recorder.StartRecording();
                }
                else
                {
                    lblStatus.Text = "Estado: Permiso denegado";
                    btnStart.IsEnabled = true;
                    btnStop.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Se ha generado el siguiente error al iniciar la grabaci�n: {ex.Message}", "Aceptar");
                lblStatus.Text = "Estado: Inactivo";
                btnStart.IsEnabled = true;
                btnStop.IsEnabled = false;
            }
        }

        private async void btnStop_Clicked(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "Estado: Procesando...";
                btnStart.IsEnabled = true;
                btnStop.IsEnabled = false;

                await recorder.StopRecording();

                if (File.Exists(recorder.FilePath))
                {
                    var optimizedFile = recorder.FilePath;
                     base64Audio = ConvertToBase64Audio(optimizedFile);

                    lblStatus.Text = "Estado: Inactivo";
                }
                else
                {
                    lblStatus.Text = "Estado: Error en la grabaci�n";
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Se ha generado el siguiente error al detener la grabaci�n: {ex.Message}", "Aceptar");
                lblStatus.Text = "Estado: Inactivo";
                btnStart.IsEnabled = true;
                btnStop.IsEnabled = false;
            }
        }

        private string ConvertToBase64Audio(string filePath)
        {
            try
            {
                byte[] audioBytes = File.ReadAllBytes(filePath);

                if (audioBytes.Length == 0)
                {
                    throw new Exception("El archivo de audio est� vac�o.");
                }

                return Convert.ToBase64String(audioBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al convertir el audio a Base64: {ex.Message}");
                throw;
            }
        }

        private string OptimizeAudio(string filePath)
        {
            string optimizedFilePath = Path.Combine(Path.GetDirectoryName(filePath), "optimized_" + Path.GetFileName(filePath));

            try
            {
                // Proporciona la ruta completa a FFmpeg si no est� en el PATH
                string ffmpegPath = "ffmpeg"; // Cambia esto a la ruta completa de FFmpeg si es necesario

                string arguments = $"-i \"{filePath}\" -b:a 128k \"{optimizedFilePath}\"";
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = ffmpegPath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = new Process { StartInfo = processStartInfo })
                {
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"FFmpeg error: {error}");
                    }
                }

                return optimizedFilePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al optimizar el audio: {ex.Message}");
                throw;
            }
        }




        private void ShowLoadingDialog()
        {
            // Show your loading dialog here
           // activityIndicator.IsRunning = true;
            //activityIndicator.IsVisible = true;
            //framePrincipal.IsVisible = false;
            btnAgregar.IsEnabled = false;
        }

        private void HideLoadingDialog()
        {
            // Hide your loading dialog here
            //activityIndicator.IsRunning = false;
            //activityIndicator.IsVisible = false;
            //framePrincipal.IsVisible = true;
            btnAgregar.IsEnabled = true;
        }

        private async void OnListaClicked(object sender, EventArgs e)
        {

            await Navigation.PushAsync(new PageList());

        }


        private async void OnSalirClicked(object sender, EventArgs e)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert("Confirmaci�n", "�Desea cerrar la App?", "S�", "Cancelar");

            if (confirm)
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }
    }
}
