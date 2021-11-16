

// This class represents a human, who might be 
// an MP or a non-MP participant.

using System.Collections.Generic;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Models
{
    public abstract class Person : Entity
    {
		protected string address;
        // public string StateOrTerritory { get; set; }
        
		protected string selectedStateOrTerritory;
		protected string selectedFederalElectorate; 
		protected string selectedLAStateElectorate;
		protected string selectedLCStateElectorate;
		// protected string userName;
		protected string userEmail;

		protected Registration registrationInfo = new Registration();
        // TODO add attributes for a nice profile, such as a photo.
        
		// Initially, when we don't know the state, it's only the Australian
		// Parliament.
		protected List<BackgroundElectorateAndMPData.Chamber> chambersRepresentedIn 
			= BackgroundElectorateAndMPData.FindChambers("");

		public void UpdateChambers(string state)
		{
			chambersRepresentedIn = BackgroundElectorateAndMPData.FindChambers(state);
		}
        public Registration RegistrationInfo 
        {
			get { return registrationInfo; }
			set
			{
				registrationInfo = value;
				OnPropertyChanged("RegistrationInfo");
			}
		}
        
        /*
        public string UpdateLCStateElectorate(BackgroundElectorateAndMPData.Chamber chamber, string region)
        {
		        // TODO (Issue #9) when this LC state electorate is
		        // chosen, add the representative(s) for that electorate to MyMPs.
		        // Note that you'll need to compare both the chamber and the
		        // Electorate Name.
		        registrationInfo.AddElectorate(chamber, region);
		        // OnPropertyChanged("SelectedLCStateElectorate");
        }

        public string UpdateLAStateElectorate
        {
	        get { return selectedLAStateElectorate; }
	        set
	        {
		        // TODO (Issue #9) Update MyMPs.
		        selectedLAStateElectorate = value;
		        OnPropertyChanged("SelectedLAStateElectorate");
	        }
        }

        public string SelectedFederalElectorate
        {
	        get { return selectedFederalElectorate; }
	        set
	        {
		        // TODO (Issue #9) Update MyMPs.
		        selectedFederalElectorate = value;
		        OnPropertyChanged("SelectedFederalElectorate");
	        }
        }

*/
        public string Address
        {
	        get { return address; }
	        set
	        {
		        address = value;
		        OnPropertyChanged("Address");
	        }
        }
		public string UserEmail
		{
			get { return userEmail; }
			set
			{
				userEmail = value;
				OnPropertyChanged("UserEmail");
			}
		}
    }

}