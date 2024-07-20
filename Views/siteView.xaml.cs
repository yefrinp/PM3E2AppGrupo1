using PM2E2Grupo1.Models;
namespace PM2E2Grupo1.Views;

public partial class siteView : ContentPage
{
	private string? base64foto;
	private string? base64audio;
    private string? Titulo;
    private string? Lugar;

	public siteView(Sitios item, string lugar)
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        base64foto = item.Fotografia;
		base64audio = item.Audiofile;
        Titulo = item.Descripcion;
        Lugar = lugar;
    }



    protected override void OnAppearing()
    {
        base.OnAppearing();


        byte[] fotoBytes = Convert.FromBase64String(base64foto);

        string fotoFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "tempFoto.jpg");

        //Crea ubicaciones temporales para guardar los archivos y poder verlos en los mediaElement
        File.WriteAllBytes(fotoFilePath, fotoBytes);

        mediaElementFoto.Source = fotoFilePath;


        //Convierte a byte array
        byte[] audioBytes = Convert.FromBase64String(base64audio);

        string audioFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "tempAudio.mp3");

        //Crea ubicaciones temporales para guardar los archivos y poder verlos en los mediaElement
        File.WriteAllBytes(audioFilePath, audioBytes);

        labelTitulo.Text = Titulo;
        labelLugar.Text = Lugar;
        mediaElementAudio.Source = audioFilePath;
    }

    private async void OnSalirClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PageList());
    }
}