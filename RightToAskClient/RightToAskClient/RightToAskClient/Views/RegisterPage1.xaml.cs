using System;
using System.Linq;
using RightToAskClient.CryptoUtils;
using RightToAskClient.Models;
using RightToAskClient.HttpClients;
using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage1 : ContentPage
    {
        public RegisterPage1()
        {
            InitializeComponent();

            var reg = BindingContext as Registration1ViewModel;
            reg.ReinitRegistrationUpdates();
        }

        void OnStatePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            Picker picker = (Picker)sender;

            if (picker.SelectedIndex != -1)
            {
                string state = (string)picker.SelectedItem;
                App.ReadingContext.ThisParticipant.RegistrationInfo.SelectedStateAsIndex = picker.SelectedIndex;
                App.ReadingContext.ThisParticipant.UpdateChambers(state);
            }
        }

        private void OnRegisterEmailFieldCompleted(object sender, EventArgs e)
        {
            App.ReadingContext.ThisParticipant.UserEmail = ((Editor)sender).Text;
        }
    }
}