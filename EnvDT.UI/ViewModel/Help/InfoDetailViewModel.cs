namespace EnvDT.UI.ViewModel
{
    class InfoDetailViewModel : ViewModelBase, IInfoDetailViewModel
    {

        public InfoDetailViewModel()
        {
            Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public string Version { get; set; }
    }
}
