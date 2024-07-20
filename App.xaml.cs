namespace PM2E2Grupo1
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Views.PageHome());
        }

    }
}
