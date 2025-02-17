using System.Text.Json;
using System.Threading.Tasks;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using RightToAskClient.Models.ServerCommsData;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class MPRegistrationVerificationViewModel : BaseViewModel
    {
        #region Properties
        
        // If the person has stated that they are an MP or staffer, return that one (there should be only one).
        // Otherwise, a new/blank one.
        private MP _mpRepresenting = new MP();
        public MP MPRepresenting
        {
            get => _mpRepresenting;
            private set => SetProperty(ref _mpRepresenting, value);
        }

        // Bool to distinguish MPs from staffers. True if MP; false if staffer.
        private bool _isStaffer;
        public bool IsStaffer
        {
            get => _isStaffer;
            set => SetProperty(ref _isStaffer, value);
        }

        // Bool to distinguish MPs from staffers. True if MP; false if staffer.
        private string _emailUsername;
        public string EmailUsername
        {
            get => _emailUsername;
            set => SetProperty(ref _emailUsername, value);
        }

        // Index of the picker for choosing email domains
        private int _parliamentaryDomainIndex = -1;
        public int ParliamentaryDomainIndex
        {
            get => _parliamentaryDomainIndex;
            set => SetProperty(ref _parliamentaryDomainIndex, value);
        }

        // The hash returned from the server that allows us to link the PIN response
        // with the initial email verification request.
        private string _mpVerificationHash = "";
        
        // The PIN entered by the user to verify that they read the email.
        private int _mpRegistrationPin;

        public int MpRegistrationPin
        {
            get => _mpRegistrationPin;
            set => SetProperty(ref _mpRegistrationPin, value);
        }

        public bool ReturnToAccountPage;

        // The domain of the email, one of 9 hardcoded valid parliamentary email domains.
        private string _domain = "";
        
        #endregion

        // constructor
        public MPRegistrationVerificationViewModel()
        {
            MessagingCenter.Subscribe<SelectableListViewModel, MP>(this, "ReturnToAccountPage", (sender, arg) =>
            {
                MPRepresenting = arg;
                ReturnToAccountPage = true;
                MessagingCenter.Unsubscribe<SelectableListViewModel>(this, "ReturnToAccountPage");
            });

            SendMPVerificationEmailCommand = new Command(() => { SendMPRegistrationToServer(); });
            SubmitMPRegistrationPinCommand = new AsyncCommand(async () =>
            {
                var success = await SendMPRegistrationPINToServer();
                if (success)
                {
                    StoreMPRegistration();
                    SaveMPRegistrationToPreferences();
                    MessagingCenter.Send(this, Constants.IsVerifiedMPAccount);
                    // navigate back to account page
                    if (ReturnToAccountPage)
                    {
                        await Application.Current.MainPage.Navigation.PopToRootAsync();
                    }
                }
            });
        }
        
        // commands
        public Command SendMPVerificationEmailCommand { get; }
        public IAsyncCommand SubmitMPRegistrationPinCommand { get; }
        
        // methods
        public string RegisterMPReportLabelText { get; } = "";

        private bool _showRegisterMPReportLabel;
        public bool ShowRegisterMPReportLabel
        {
            get => _showRegisterMPReportLabel;
            set => SetProperty(ref _showRegisterMPReportLabel, value);
        }
        
        private async void SendMPRegistrationToServer()
        {
            _domain = _parliamentaryDomainIndex >= 0  && _parliamentaryDomainIndex < ParliamentData.Domains.Count
                ? ParliamentData.Domains[_parliamentaryDomainIndex] : "";
            var message = new RequestEmailValidationMessage()
            {
                why = new EmailValidationReason() { AsMP = !IsStaffer },
                // name = MPRepresenting.first_name + " " + MPRepresenting.surname +" @"+domain
                name = Badge.WriteBadgeName(MPRepresenting, _domain)
            };
            var httpResponse = await RTAClient.RequestEmailValidation(message, EmailUsername + "@" + _domain);
            (bool isValid, string errorMsg, string hash) validation = RTAClient.ValidateHttpResponse(httpResponse, "Email Validation Request");
            if (validation.isValid)
            {
                _mpVerificationHash = validation.hash;
            }
            else
            {
                // TODO - deal properly with errors e.g. email not known.
                ShowRegisterMPReportLabel = true;
                ReportLabelText = validation.errorMsg;
            }
        }

        // TODO Note it's unclear that we really need both MPRegisteredAs and RegistrationInfo.Badges - perhaps
        // cleaner to remove the former.
        private void SaveMPRegistrationToPreferences()
        {
            // Note that a staffer has booth of these flags set to true.
            Preferences.Set(Constants.IsVerifiedMPAccount, true);
            Preferences.Set(Constants.IsVerifiedMPStafferAccount, _isStaffer); 
            Preferences.Set(Constants.MPRegisteredAs,JsonSerializer.Serialize(MPRepresenting));
            var registrationObjectToSave = new ServerUser(App.ReadingContext.ThisParticipant.RegistrationInfo);
            Preferences.Set(Constants.RegistrationInfo, JsonSerializer.Serialize(registrationObjectToSave));
        }

        private void StoreMPRegistration()
        {
            App.ReadingContext.ThisParticipant.IsVerifiedMPAccount = true;
            App.ReadingContext.ThisParticipant.MPRegisteredAs = MPRepresenting;
            App.ReadingContext.ThisParticipant.IsVerifiedMPStafferAccount = _isStaffer;
            App.ReadingContext.ThisParticipant.RegistrationInfo.Badges.Add(
                    new Badge()
                    {
                        badge = IsStaffer ? BadgeType.MPStaff : BadgeType.MP,
                        name = Badge.WriteBadgeName(MPRepresenting, _domain)
                    });
        }

        private async Task<bool> SendMPRegistrationPINToServer()
        {
            var msg = new EmailValidationPIN()
            {
                hash = _mpVerificationHash,
                code = _mpRegistrationPin
            };
            var httpResponse = await RTAClient.SendEmailValidationPin(msg);
            (bool isValid, string errorMsg, string hash) validation = RTAClient.ValidateHttpResponse(httpResponse, "Email Validation PIN");

                // TODO - deal properly with errors e.g. email not known.
                // also display a nice success message if applicable.
            if (!validation.isValid)
            {
                ShowRegisterMPReportLabel = true;
                ReportLabelText = validation.errorMsg;
            }

            return validation.isValid;
        }
    }
}