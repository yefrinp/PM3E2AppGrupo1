using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PM2E2Grupo1.ViewModels
{
    public class sitiosViewModel : INotifyPropertyChanged
    {
        private int _idItem;
        public int IdItem
        {
            get { return _idItem; }
            set
            {
                if (_idItem != value)
                {
                    _idItem = value;
                    OnPropertyChanged(nameof(IdItem));
                }
            }
        }

        private string? _descripcion;
        public string? Descripcion
        {
            get { return _descripcion; }
            set
            {
                if (_descripcion != value)
                {
                    _descripcion = value;
                    OnPropertyChanged(nameof(Descripcion));
                }
            }
        }

        private string _latitud;
        public string Latitud
        {
            get { return _latitud; }
            set
            {
                if (_latitud != value)
                {
                    _latitud = value;
                    OnPropertyChanged(nameof(Latitud));
                }
            }
        }

        private string _longitud;
        public string Longitud
        {
            get { return _longitud; }
            set
            {
                if (_longitud != value)
                {
                    _longitud = value;
                    OnPropertyChanged(nameof(Longitud));
                }
            }
        }

        private string? _fotografia;
        public string? Fotografia
        {
            get { return _fotografia; }
            set
            {
                if (_fotografia != value)
                {
                    _fotografia = value;
                    OnPropertyChanged(nameof(Fotografia));
                }
            }
        }

        private string? _audiofile;
        public string? Audiofile
        {
            get { return _audiofile; }
            set
            {
                if (_audiofile != value)
                {
                    _audiofile = value;
                    OnPropertyChanged(nameof(Audiofile));
                }
            }
        }

        private string? _lugar;
        public string? Lugar
        {
            get { return _lugar; }
            set
            {
                if (_lugar != value)
                {
                    _lugar = value;
                    OnPropertyChanged(nameof(Lugar));
                }
            }
        }

        public ICommand? VerMediaTappedCommand { get; set; }
        public ICommand? VerMapaTappedCommand { get; set; }
        public ICommand? EditarTappedCommand { get; set; }
        public ICommand? EliminarTappedCommand { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
