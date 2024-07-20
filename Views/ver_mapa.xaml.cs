using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Graphics;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Maps;
using PM2E2Grupo1.Models;
using System.Text.Json;

namespace PM2E2Grupo1.Views;


public partial class ver_mapa : ContentPage
{
    private double Latitud;
    private double Longitud;
    private string? Titulo;
    private string? Lugar;



    public ver_mapa(Sitios sitio, string lugar)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        double latitud = Convert.ToDouble(sitio.Latitud);
        double longitud = Convert.ToDouble(sitio.Longitud);

        Latitud = latitud;
        Longitud = longitud;
        Lugar = lugar;
        Titulo = sitio.Descripcion;
        labelSitio.Text = Titulo;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await colocarSitio(Latitud, Longitud);

    }



    public async Task colocarSitio(double latitude, double longitude)
    {
        // Agrega el Marcador en la ubicacion
        var marker = new Pin
        {
            Type = PinType.Place,
            Location = new Location(latitude, longitude),
            Label = Titulo,
            Address = Lugar
        };

        mapa.Pins.Add(marker);

        await Task.Delay(5000);

        await MoveMapToRegion(Latitud, Longitud);
    }

    public async Task MoveMapToRegion(double latitude, double longitude)
    {
        // Se mueve a la ubicacion del pin
        Location location = new Location(latitude, longitude);
        MapSpan mapSpan = new MapSpan(location, 0.01, 0.01);
        mapa.MoveToRegion(mapSpan);
    }

  

    private async void btnSalir_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PageList());
    }

    private async void btnDirecciones_Clicked(object sender, EventArgs e)
    {
        var result = await DisplayAlert("Abrir Mapas", "�Desea abrir a aplicaci�n de mapas para navegaci�n?", "Si", "No");

        if (result)
        {
            Location location = new Location(Latitud, Longitud);

            var uri = new Uri($"https://www.google.com/maps/dir/?api=1&travelmode=driving&dir_action=navigate&destination={Latitud},{Longitud}");

            await Launcher.TryOpenAsync(uri);
        }


    }
}