using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/*
 * This page allows a person to find which electorates they live in,
 * and hence which MPs represent them.
 *
 * This is used in two possible places:
 * (1) if the person clicks on 'My MP' when setting question metadata,
 * we need to know who their MPs are. After this page,
 * there will be a list of MPs loaded for them to choose from.
 * This is indicated by setting LaunchMPsSelectionPageNext to true.
 * 
 * (2) if the person tries to vote or post a question.
 * In this case, they have generated a name via RegisterPage1
 * and can skip this step (so showSkip should be set to true).
 * And we don't follow with a list of MPs for them to chose from.
 */
namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage2 : ContentPage
    {
        private Address _address = new Address(); 
        
        private IndividualParticipant _thisParticipant;
        private bool _launchMPsSelectionPageNext;
        private ObservableCollection<MP> _alreadySelectedMPs; 

        // private ParliamentData.Chamber stateLCChamber=ParliamentData.Chamber.Vic_Legislative_Council;
        // private ParliamentData.Chamber stateLAChamber=ParliamentData.Chamber.Vic_Legislative_Assembly;

        private List<string> _allFederalElectorates = new List<string>();
        private List<string> allStateLAElectorates = new List<string>();
        private List<string> allStateLCElectorates = new List<string>();
        // alreadySelectedMPs are passed in if a Selection page is to be launched next.
        // If they're null/absent, no selection page is launched.
        public RegisterPage2(IndividualParticipant thisParticipant, bool showSkip, bool launchMPsSelectionPageNext, 
            ObservableCollection<MP>? alreadySelectedMPs = null)
        {
            InitializeComponent();
            BindingContext = thisParticipant;
            _thisParticipant = thisParticipant;
            _launchMPsSelectionPageNext = launchMPsSelectionPageNext;
            _alreadySelectedMPs = alreadySelectedMPs ?? new ObservableCollection<MP>();

            KnowElectoratesFrame.IsVisible = false;
            addressSavingStack.IsVisible = false;
            
            FindMPsButton.IsVisible = false;
            if (!showSkip)
            {
                SkipButton.IsVisible = false;
            }
            
            stateOrTerritoryPicker.ItemsSource = ParliamentData.StatesAndTerritories;
            string state = thisParticipant.RegistrationInfo.State;
            stateOrTerritoryPicker.Title = String.IsNullOrEmpty(state) ? "Choose State or Territory" : state;
            stateOrTerritoryPicker.BindingContext = thisParticipant.RegistrationInfo.State;
        }
        
        void OnStatePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            if (picker != null)
            {
                int selectedIndex = picker.SelectedIndex;
         
                if (selectedIndex != -1)
                {
                    string state = (string) picker.SelectedItem;
                    _thisParticipant.RegistrationInfo.State = state;
                    _thisParticipant.AddSenatorsFromState(state);
                    UpdateElectoratePickerSources(state);
                }
            }
        }

        private void UpdateElectoratePickerSources(string state)
        {
            _allFederalElectorates = ParliamentData.ListElectoratesInHouseOfReps(state);
            federalElectoratePicker.ItemsSource = _allFederalElectorates;
            
            allStateLAElectorates 
                = ParliamentData.ListElectoratesInStateLowerHouse(state);
            stateLAElectoratePicker.ItemsSource = allStateLAElectorates;

            if (ParliamentData.HasUpperHouse(state))
            {
                allStateLCElectorates
                    = ParliamentData.ListElectoratesInStateUpperHouse(state);
                stateLCElectoratePicker.ItemsSource = allStateLCElectorates;
            }
            else
            {
                allStateLCElectorates = new List<string>();
                stateLCElectoratePicker.Title = state + " has no Upper House";
            }
        }

        void OnStateLCElectoratePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker) sender;
            string region = ChooseElectorate(picker, allStateLCElectorates);
            if (!String.IsNullOrEmpty(region))
            {
                var state = _thisParticipant.RegistrationInfo.State;
                _thisParticipant.AddStateUpperHouseElectorate(state, region);
                RevealNextStepIfElectoratesKnown();
            }
        }
        void OnStateLAElectoratePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker) sender;
            string region = ChooseElectorate(picker, allStateLAElectorates);
            if (!String.IsNullOrEmpty(region))
            {
                var state = _thisParticipant.RegistrationInfo.State;
                _thisParticipant.AddStateElectoratesGivenOneRegion(state, region);
                RevealNextStepIfElectoratesKnown();
            }    
        }

        void OnFederalElectoratePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker) sender;
            string region = ChooseElectorate(picker, _allFederalElectorates);
            if (!String.IsNullOrEmpty(region))
            {
                _thisParticipant.AddHouseOfRepsElectorate(region);
                RevealNextStepIfElectoratesKnown();
            }
        }

        private string ChooseElectorate(Picker p, List<string> allElectorates)
        {
            int selectedIndex = p.SelectedIndex;

            if (selectedIndex >= 0 && selectedIndex < allElectorates.Count)
            {
                return allElectorates[selectedIndex];
            }

            return "";

        }
        
        // TODO: Add a check that at least some electorates have been 
        // chosen, plus a prompt to remind people to fill them all
        // if only some have been chosen.
        private void RevealNextStepIfElectoratesKnown()
        {
                    FindMPsButton.IsVisible = true;
                    _thisParticipant.MPsKnown = true;
        }
        
        // If we've been asked to push an MP-selecting page, go there and
        // remove this page, otherwise just pop. Note that SelectedAnsweringMPs
        // is empty/new because we didn't know this person's MPs until this page.
        private async void OnFindMPsButtonClicked(object sender, EventArgs e)
        {
            var currentPage = Navigation.NavigationStack.LastOrDefault();
        
            if (_launchMPsSelectionPageNext)
            {
                string message = "These are your MPs.  Select the one(s) who should answer the question";
                  
           	    var mpsExploringPage = new ExploringPage(_thisParticipant.GroupedMPs, _alreadySelectedMPs , message);
                await Navigation.PushAsync(mpsExploringPage);
                //await Shell.Current.GoToAsync($"{nameof(ExploringPage)}");
            }
            
            Navigation.RemovePage(currentPage); 
        }
                
        // At the moment this just chooses random electorates. 
        // TODO: We probably want this to give the person a chance to go back and fix it if wrong.
        // If we don't even know the person's state, we have no idea so they have to go back and pick;
        // If we know their state but not their Legislative Assembly or Council makeup, we can go on. 
        private async void OnSubmitAddressButton_Clicked(object sender, EventArgs e)
        {
            Result<GeoscapeAddressFeature> httpResponse;
            
            string state = _thisParticipant.RegistrationInfo.State;
            
            if (String.IsNullOrEmpty(state))
            {
                await DisplayAlert("Please choose a state", "", "OK");
                return;
            }

            Result<bool> addressValidation = _address.SeemsValid();
            if (!String.IsNullOrEmpty(addressValidation.Err))
            {
                await DisplayAlert(addressValidation.Err, "", "OK");
                return;
            }
            
            httpResponse = await GeoscapeClient.GetFirstAddressData(_address + " " + state);
            
            if (!String.IsNullOrEmpty(httpResponse.Err))
            {
                ReportLabel.Text = httpResponse.Err;
                return;
            } 
            
            // Now we know everything is good.
            var bestAddress = httpResponse.Ok;
            AddElectorates(bestAddress);
            FindMPsButton.IsVisible = true;
            ReportLabel.Text = "";

            bool saveThisAddress = await DisplayAlert("Electorates found!", 
                // "State Assembly Electorate: "+thisParticipant.SelectedLAStateElectorate+"\n"
                // +"State Legislative Council Electorate: "+thisParticipant.SelectedLCStateElectorate+"\n"
                "Federal electorate: "+_thisParticipant.CommonwealthElectorate+"\n"+
                "State lower house electorate: "+_thisParticipant.StateLowerHouseElectorate+"\n"+
                "Do you want to save your address on this device? Right To Ask will not learn your address.", 
                "OK - Save address on this device", "No thanks");
            if (saveThisAddress)
            {
                SaveAddress();
            }
            
            federalElectoratePicker.TextColor = Color.Black;
            stateLAElectoratePicker.TextColor = Color.Black;
            stateLCElectoratePicker.TextColor = Color.Black;

            SkipButton.IsVisible = false;
        }

        // TODO: At the moment, this does nothing, since there's no notion of not 
        // saving the address.
        private void SaveAddress()
        {
        }

        private void AddElectorates(GeoscapeAddressFeature addressData)
        {
            _thisParticipant.AddElectoratesFromGeoscapeAddress(addressData);
            _thisParticipant.MPsKnown = true;
        }

        // TODO Think about what should happen if the person has made 
        // some choices, then clicks 'skip'.  At the moment, it retains 
        // the choices they made and pops the page.
        private async void OnSkipButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void OnStreetNumberAndNameChanged(object sender, TextChangedEventArgs e)
        {
            _address.StreetNumberAndName = e.NewTextValue;
            mainScrollView.ScrollToAsync(addressSavingStack, ScrollToPosition.End, true); 
        }
        private void OnStreetNumberAndNameEntered(object sender, EventArgs e)
        {
            _address.StreetNumberAndName = ((Entry)sender).Text;
        }

        private void OnCityOrSuburbChanged(object sender, TextChangedEventArgs e)
        {
            _address.CityOrSuburb = e.NewTextValue;
        }
        private void OnCityOrSuburbEntered(object sender, EventArgs e)
        {
            _address.CityOrSuburb =  ((Entry)sender).Text;
        }

        private void OnPostcodeChanged(object sender, TextChangedEventArgs e)
        {
            _address.Postcode = e.NewTextValue;
        }
        
        private void OnPostcodeEntered(object sender, EventArgs e)
        {
            _address.Postcode =  ((Entry)sender).Text;
        }

        private void KnowElectorates_Tapped(object sender, EventArgs e)
        {
            KnowElectoratesFrame.IsVisible = true;
        }

        private void LookupElectorates_Tapped(object sender, EventArgs e)
        {
            addressSavingStack.IsVisible = true;
        }
    }
}