using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto.Operators;
using RightToAskClient.HttpClients;
using static RightToAskClient.HttpClients.RTAClient;

namespace RightToAskClient.Models
{
    // This class reads in information about electorates, MPs, etc, from static files.
    public static class ParliamentData
    {
	    private static readonly List<MP> FederalMPs 
		    = readMPsFromCSV(Chamber.Australian_House_Of_Representatives, "StateRepsCSV.csv");
	    private static readonly List<MP> Senators 
		    = readMPsFromCSV(Chamber.Australian_Senate, "allsenstate.csv");
	    // TODO - at the moment, we only have Vic MPs. Add other states.
	    private static readonly List<MP> VicLA_MPs 
		    = readMPsFromCSV(Chamber.Vic_Legislative_Assembly, "VicLegislativeAssemblymembers.csv");
	    private static readonly List<MP> VicLC_MPs 
		    =  readMPsFromCSV(Chamber.Vic_Legislative_Council, "VicLegislativeCouncilmembers.csv");
	    
	    public static readonly MPData MPs = new MPData();

	    /*
	    public static readonly ObservableCollection<string> StatesAndTerritories = new ObservableCollection<string>()
	    {
		    "ACT",
			"NSW",
			"NT",
			"QLD",
			"SA",
			"TAS",
			"VIC",
			"WA"
	    };
	    */

	    public static readonly List<string> StatesAndTerritories
		    = StatesAsStringList(); 

	    private static List<string> StatesAsStringList()
	    {
		    var stateList = new List<string>();
		    FieldInfo[] fields = (typeof(State).GetFields());
		    
		    for(int i = 0; i < fields.Length; i++)
		    {
			    stateList.Add(fields[i].GetValue(typeof(State)).ToString());
		    }
		    
		    return stateList;
	    }
	    
	    public static class State
	    {
		    public const string ACT = "ACT";
		    public const string NSW = "NSW";
		    public const string NT = "NT";
		    public const string QLD = "QLD";
		    public const string SA = "SA";
		    public const string TAS = "TAS";
		    public const string VIC ="VIC";
		    public const string WA = "WA";
	    }

	    public static readonly Dictionary<string, List<Chamber>> Parliaments = new Dictionary<string, List<Chamber>>
	    {
		    { State.ACT, new List<Chamber> { Chamber.ACT_Legislative_Assembly} },
		    { State.NSW, new List<Chamber> { Chamber.NSW_Legislative_Assembly, Chamber.NSW_Legislative_Council} },
		    { State.NT, new List<Chamber> { Chamber.NT_Legislative_Assembly } },
		    { State.QLD, new List<Chamber> { Chamber.Qld_Legislative_Assembly} },
		    { State.SA, new List<Chamber> { Chamber.SA_House_Of_Assembly, Chamber.SA_Legislative_Council} },
		    { State.TAS, new List<Chamber> { Chamber.Tas_House_Of_Assembly, Chamber.Tas_Legislative_Council} },
		    { State.VIC, new List<Chamber> { Chamber.Vic_Legislative_Assembly, Chamber.Vic_Legislative_Council } },
		    { State.WA, new List<Chamber> { Chamber.WA_Legislative_Assembly, Chamber.WA_Legislative_Council} },
	    };
	    
	    public enum Chamber
	    {
			ACT_Legislative_Assembly,
			Australian_House_Of_Representatives, 
		    Australian_Senate, 
		    NSW_Legislative_Assembly,
			NSW_Legislative_Council,
			NT_Legislative_Assembly,
		    Qld_Legislative_Assembly, 
		    SA_House_Of_Assembly,
			SA_Legislative_Council,
			Tas_House_Of_Assembly,
			Tas_Legislative_Council,
		    Vic_Legislative_Assembly, 
			Vic_Legislative_Council,
			WA_Legislative_Assembly,
			WA_Legislative_Council
	    }
        
	    /*
	    public static class Chamber 
	    {
			public const string ACT_Legislative_Assembly = "ACT_Legislative_Assembly";
			public const string Australian_House_Of_Representatives = "Australian_House_Of_Representatives";
		    public const string Australian_Senate = "Australian_Senate";
		    public const string NSW_Legislative_Assembly = "NSW_Legislative_Assembly" ;
			public const string NSW_Legislative_Council ="NSW_Legislative_Council" ;
			public const string NT_Legislative_Assembly = "NT_Legislative_Assembly" ;
		    public const string Qld_Legislative_Assembly ="Qld_Legislative_Assembly" ;
		    public const string SA_Legislative_Assembly = "SA_Legislative_Assembly" ;
			public const string SA_Legislative_Council = "SA_Legislative_Council" ;
			public const string Tas_House_Of_Assembly ="Tas_House_Of_Assembly" ;
			public const string Tas_Legislative_Council ="Tas_Legislative_Council" ;
		    public const string Vic_Legislative_Assembly = "Vic_Legislative_Assembly" ;
			public const string Vic_Legislative_Council = "Vic_Legislative_Council" ;
			public const string WA_Legislative_Assembly = "WA_Legislative_Assembly";
			public const string WA_Legislative_Council = "WA_Legislative_Council";
	    } */
	    
		public static readonly ObservableCollection<Authority> AllAuthorities =
			new ObservableCollection<Authority>(readAuthoritiesFromFiles());

		private static List<Chamber> StateLowerHouseChambers = new List<Chamber>
		{
			Chamber.ACT_Legislative_Assembly,
			Chamber.NSW_Legislative_Assembly,
			Chamber.NT_Legislative_Assembly,
			Chamber.Qld_Legislative_Assembly,
			Chamber.SA_House_Of_Assembly,
			Chamber.Vic_Legislative_Assembly,
			Chamber.Tas_House_Of_Assembly,
			Chamber.WA_Legislative_Assembly
		};

		private static List<Chamber> StateUpperHouseChambers = new List<Chamber>
		{
			Chamber.NSW_Legislative_Council,
			Chamber.SA_Legislative_Council,
			Chamber.Vic_Legislative_Council,
			Chamber.Tas_Legislative_Council,
			Chamber.WA_Legislative_Council
		};
		public static bool IsLowerHouseChamber(Chamber chamber)
		{
			return StateLowerHouseChambers.Contains(chamber);
		}

		public static bool IsUpperHouseChamber(Chamber chamber)
		{
			return StateUpperHouseChambers.Contains(chamber);
		}

		private static List<MP> readMPsFromCSV(Chamber chamber, string filename)
		{
			var MPs = new List<MP>();
			FileIO.readDataFromCSV(filename, MPs, (string line) =>  parseCSVLineAsMP(chamber,line) );
			return MPs;
		}
		
       private static List<Authority> readAuthoritiesFromFiles()
       {
		    var AllAuthorities = new List<Authority>();
		    FileIO.readDataFromCSV("all-authorities.csv",AllAuthorities,parseCSVLineAsAuthority);
		    return AllAuthorities;
       }
        
		
		private static MP parseCSVLineAsMP(Chamber chamberExpected, string line)
		{
			string[] words = line?.Split(',');
			if (words?.Length >= 5)
			{
					var electorate = new ElectorateWithChamber(chamberExpected, words[3]);
						
					var first_name = words[2]; 
					var surname = words[1];

					MP newMP = new MP(first_name, surname, electorate);
					return newMP;
			}
			
			return null;
		}	
		
		// This parses a line from Right To Know's CSV file as an Authority.
		// It is, obviously, very specific to the expected file format.
		// Ignore any line that doesn't produce at least 3 words.
		private static Authority parseCSVLineAsAuthority(string line)
		{
			string[] words = line.Split(',');
			if (words.Length >= 3)
			{
				
				Authority newAuthority = new Authority()
				{
					AuthorityName = words[0],
					NickName = words[1],
					RightToKnowURLSuffix = words[2]
				};
				return newAuthority;
			}
			else
			{
				return null;
			}
		}


		/* Finds all the chambers in which a citizen of this state is represented,
		 * including the House of Representatives and the Senate.
		 * If the string input doesn't match any states, it simply
		 * returns the federal chambers.
		 */
		public static List<Chamber> FindChambers(string state)
		{
			var chambersForTheState = new List<Chamber>()
			{
				Chamber.Australian_House_Of_Representatives,
				Chamber.Australian_Senate
			};

			switch (state.ToUpper())
			{
				case (State.ACT):
					chambersForTheState.Add(Chamber.ACT_Legislative_Assembly);
					break;
				case (State.NSW):
					chambersForTheState.Add(Chamber.NSW_Legislative_Assembly);
					chambersForTheState.Add(Chamber.NSW_Legislative_Council);
					break;
				case (State.NT):
					chambersForTheState.Add(Chamber.NT_Legislative_Assembly);
					break;
				case (State.QLD):
					chambersForTheState.Add(Chamber.Qld_Legislative_Assembly);
					break;
				case (State.SA):
					chambersForTheState.Add(Chamber.SA_House_Of_Assembly);
					chambersForTheState.Add(Chamber.SA_Legislative_Council);
					break;
				case (State.VIC):
					chambersForTheState.Add(Chamber.Vic_Legislative_Assembly);
					chambersForTheState.Add(Chamber.Vic_Legislative_Council);
					break;
				case (State.TAS):
					chambersForTheState.Add(Chamber.Tas_House_Of_Assembly);
					chambersForTheState.Add(Chamber.Tas_Legislative_Council);
					break;
				case (State.WA):
					chambersForTheState.Add(Chamber.WA_Legislative_Assembly);
					chambersForTheState.Add(Chamber.WA_Legislative_Council);
					break;
			}

			return chambersForTheState;
		}

		// TODO: add logic for inferred other houses.
		public static List<ElectorateWithChamber> GetStateElectoratesGivenOneRegion(string state, string region)
		{
			Result<Chamber> chamber = GetLowerHouseChamber(state);
			if(!String.IsNullOrEmpty(chamber.Err))
			{
				Debug.WriteLine("Error: Couldn't find lower house chamber for "+state);	
				return new List<ElectorateWithChamber>();
			}

			return new List<ElectorateWithChamber>(){new ElectorateWithChamber(chamber.Ok, region)};
		}
		public static List<string> ListElectoratesInHouseOfReps(string state)
		{
			if (MPs.IsInitialised)
			{
				return MPs.ListElectoratesInChamber(Chamber.Australian_House_Of_Representatives);
			}

			return new List<string>();
		}

		public static List<string> ListElectoratesInStateLowerHouse(string state)
		{
			Result<Chamber> possibleChamber = GetLowerHouseChamber(state);
			if (MPs.IsInitialised && possibleChamber.Err.IsNullOrEmpty())
			{
				return MPs.ListElectoratesInChamber(possibleChamber.Ok);
			}

			return new List<string>();
		}

		public static Result<Chamber> GetLowerHouseChamber(string state)
		{
			List<Chamber> chamberList = FindChambers(state).Where(p => IsLowerHouseChamber(p)).ToList();
			if (chamberList.IsNullOrEmpty() || chamberList.Count() > 1 )
			{
				Debug.WriteLine("Error: "+chamberList.Count()+" lower house chambers in "+state);
				return new Result<Chamber>() { Err = "Can't get lower house chamber." };
			}
			return new Result<Chamber>() {Ok = chamberList[0]};
		}
		// Not every state has an upper house, so failing to find one is not necessarily an error.
		// So we return empty but don't log an error. 
		// After that, it's just like the lower-house lookup.
		public static List<string> ListElectoratesInStateUpperHouse(string state)
		{
			if (!HasUpperHouse(state))
			{
				return new List<string>();
			}
			
			Result<Chamber> possibleChamber = GetUpperHouseChamber(state);
			if (MPs.IsInitialised && possibleChamber.Err.IsNullOrEmpty())
			{
				return MPs.ListElectoratesInChamber(possibleChamber.Ok);
			}

			return new List<string>();
		}

		public static Result<Chamber> GetUpperHouseChamber(string state)
		{
			if (HasUpperHouse(state))
			{
				List<Chamber> chamberList = FindChambers(state).Where(c => IsUpperHouseChamber(c)).ToList();
				if (chamberList.IsNullOrEmpty() || chamberList.Count > 1)
				{
					Debug.WriteLine("Error: "+chamberList.Count()+" lower house chambers in "+state);
					return new Result<Chamber>() {Err = "Error: wrong number of lower houses for "+state };
				}
				
				return new Result<Chamber>() {Ok = chamberList[0]};
			}

			return new Result<Chamber>() { Err = "This state has no upper house." };
		}
		public static bool HasUpperHouse(string state)
		{
			return Parliaments[state].Count() == 2;
		}
    }
}