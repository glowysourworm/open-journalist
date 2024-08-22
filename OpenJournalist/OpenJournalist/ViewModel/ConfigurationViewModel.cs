
using WpfCustomUtilities.Extensions;

namespace OpenJournalist.ViewModel
{
    public class ConfigurationViewModel : ViewModelBase
    {
        private string _apiKey;
        private string _clientID;// OAuth 2.0 (Google)
        private string _clientSecret;// OAuth 2.0 (Google)
        private string _localDatabaseConnectionString;

        public string ApiKey
        {
            get { return _apiKey; }
            set { this.RaiseAndSetIfChanged(ref _apiKey, value); }
        }
        public string ClientID
        {
            get { return _clientID; }
            set { this.RaiseAndSetIfChanged(ref _clientID, value); }
        }
        public string ClientSecret
        {
            get { return _clientSecret; }
            set { this.RaiseAndSetIfChanged(ref _clientSecret, value); }
        }
        public string LocalDatabaseConnectionString
        {
            get { return _localDatabaseConnectionString; }
            set { this.RaiseAndSetIfChanged(ref _localDatabaseConnectionString, value); }
        }

        public ConfigurationViewModel()
        {
            this.ApiKey = "AIzaSyANk6jYB8BkD0idtuMVShGfeUeIjhJ2xJs";
            this.ClientID = "88533911009-eonq4qpolnrnrjfbfptmipjn0mb4d0jp.apps.googleusercontent.com";
            this.ClientSecret = "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";

            // TODO: Fix this
            this.LocalDatabaseConnectionString = "metadata=res://*/OpenJournalistEntityModel.csdl|res://*/OpenJournalistEntityModel.ssdl|res://*/OpenJournalistEntityModel.msl;provider=System.Data.SqlClient;provider connection string='data source=LAPTOP-JG4V86VG\\LOCALDB;initial catalog=OpenJournalist;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework'";
        }
    }
}
