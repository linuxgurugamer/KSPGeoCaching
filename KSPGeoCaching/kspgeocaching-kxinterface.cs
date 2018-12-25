//using KXAPI;
using System.Collections.Generic;
using UnityEngine;

//Created by Benjamin Cronin for the mod KSP GeoCaching, 2018


namespace KeoCaching
{
    public class Tuple<T1, T2>
    {
        public T1 item1 { get;  set; }
        public T2 item2 { get;  set; }
       
        internal Tuple(T1 first, T2 second)
        {
            item1 = first;
            item2 = second;
        }
    }

    public static class Tuple
    {
        public static Tuple<T1, T2> New<T1, T2>(T1 first, T2 second)
        {
            var tuple = new Tuple<T1, T2>(first, second);
            return tuple;
        }
    }

    public enum SearchSortBy { New, Old, MostPopular, LeastPopular }
	public enum EditMode { CreateNew, Edit }
	
	internal class KerbalXInterface
	{
		#region singleton
		private static KerbalXInterface instance = null;
		private static readonly object padlock = new object();
		KerbalXInterface() { }
		public static KerbalXInterface Instance
		{
			get
			{
				lock (padlock)
				{
					if (instance == null)
					{
						instance = new KerbalXInterface();
					}
					return instance;
				}
			}
		}
		#endregion
		//Creates the API for us to use, per https://github.com/Sujimichi/KXAPI/wiki
		//internal KerbalXAPI api = new KerbalXAPI("KSPGeoCaching", "1.0.0"); //TODO load version from config file or something for easy deployment
		#region fields
		int toolbarSelectedButton = 0;
		string[] toolbarStrings = new string[2] { "Export a geocache", "Import a geocache" };
		string searchTerms = "";
		List<Tuple<KeoCacheCollection, bool>> retrievedCaches = new List<Tuple<KeoCacheCollection, bool>>();
		string[] difficulties = new string[4] { "Easy", "Medium", "Difficult", "Insanity" };
		int difficultySelected = 0;
		string[] sortBy = new string[4] { "New", "Old", "Most Popular", "Least Popular" };
		int sortBySelected = 0;
		bool hasMods = false;
		string searchTermsMods = "";
		bool downloadAllToggle = false;
		KeoCacheCollection exportCollection;
		string title = "", description = "";
		Difficulty difficulty = Difficulty.Easy;
		bool addingHint = false;
		List<Hint> hintList = new List<Hint>();
		Hint editingHint = null;
		EditMode editingMode = EditMode.CreateNew;
		string editingHintDistance = "";
		Color originalColor = new Color(GUI.contentColor.r, GUI.contentColor.g, GUI.contentColor.b, GUI.contentColor.a);
		string[] scales = new string[2] { "m", "Km" };
		int scalesIndex = 0;
#endregion

		private void Start()
		{

		}

		//Call this from an OnGUI() method to draw the Import/Export display window. 
		//This method draws window internals and is meant to be the argument for a GUI.Window() or GUILayout.Window() method
		public void IODisplay(int id)
		{
			//Toolbar up top to draw the two different views in the I/O window
			GUILayout.BeginHorizontal();
			toolbarSelectedButton = GUILayout.Toolbar(toolbarSelectedButton, toolbarStrings);
			GUILayout.EndHorizontal();
			switch(toolbarSelectedButton)
			{
				case 0:
					DrawExportScreen();
					break;
				case 1:
					DrawImportScreen();
					break;
				default:
					DrawExportScreen();
					break;
			}
            GUI.DragWindow();
		}
		//Draws the export screen, complete with a hint editor
		internal void DrawExportScreen()
		{
			//User has to log in to export anything
			//api.login();
			GUILayout.BeginHorizontal();
			CenteredLabel("Export the current vessel");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();//creates vertically aligned column
			GUILayout.BeginVertical();//first column
			GUILayout.Label("Title:");
			GUILayout.Label("Description:");
			GUILayout.Label("Difficulty:");
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical();//second column
			title = GUILayout.TextField(title);
			description = GUILayout.TextArea(description);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("<<"))
			{
				int currDiff = (int)difficulty;
				if (currDiff == 0)
				{
					difficulty = Difficulty.Insane;
				}
				else
				{
					currDiff--;
					difficulty = (Difficulty)currDiff;
				}
			}
			CenteredLabel(difficulty.ToString());
			if (GUILayout.Button(">>"))
			{
				int currDiff = (int)difficulty;
				if (currDiff == 3)
				{
					difficulty = Difficulty.Easy;
				}
				else
				{
					currDiff++;
					difficulty = (Difficulty)currDiff;
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.Space(20.0f * (Screen.height / 1920f));//space between overview and hints
			GUILayout.BeginHorizontal();
			CenteredLabel("Hints");
			GUILayout.EndHorizontal();
			if(!addingHint)
			{
				GUILayout.BeginHorizontal();
				GUILayout.BeginVertical();
				GUILayout.Label("Title");
				foreach(Hint h in hintList)
				{
					GUILayout.Label(h.hintTitle);
				}
				GUILayout.EndVertical();
				GUILayout.BeginVertical();
				GUILayout.Label("Hint");
				foreach(Hint h in hintList)
				{
					GUILayout.Label(h.hint, GUILayout.MaxWidth(100 * (Screen.width / 1920)));
				}
				GUILayout.EndVertical();
				GUILayout.BeginVertical();
				GUILayout.Label("Distance");
				foreach(Hint h in hintList)
				{
					//Should print the distance to 2 decimal places with the scale after it
					GUILayout.Label(string.Format("{0:0.00}", h.hintDistance) + h.scale.ToString());
				}
				GUILayout.EndVertical();
				GUILayout.BeginVertical();
				GUILayout.Label("Edit");
				for(int i = 0; i < hintList.Count; i++)
				{
					if(GUILayout.Button("", GUILayout.MaxWidth(20 * (Screen.width / 1920))))
					{
						editingHint = hintList[i];
						addingHint = true;
						editingMode = EditMode.Edit;
						break;
					}
				}
				GUILayout.EndVertical();
				GUILayout.BeginVertical();
				GUILayout.Label("Delete");
				short ih = 0;
				while (ih < hintList.Count)
				{
					if (GUILayout.Button("", GUILayout.MaxWidth(20 * (Screen.width / 1920))))
					{
						hintList.RemoveAt(ih);
					}
					else
					{
						ih++;
					}
				}
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
				GUILayout.FlexibleSpace();
				if(GUILayout.Button("Add new hint"))
				{
					addingHint = !addingHint;
					editingMode = EditMode.CreateNew;
				}
				if (GUILayout.Button("Export geocache"))
				{

				}
                if (GUILayout.Button("Close", GUILayout.Width(60)))
                {
                    KeoCacheDriver.Instance.showKerbalX = false;
                }
            }
			else
			{
				if(editingHint == null)
				{
					editingHint = new Hint();
				}
				GUILayout.BeginHorizontal();
				CenteredLabel("Hint editor");
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Title");
				GUILayout.FlexibleSpace();
				editingHint.hintTitle = GUILayout.TextField(editingHint.hintTitle, GUILayout.MinWidth(250 * (Screen.width / 1080)));
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Hint");
				GUILayout.FlexibleSpace();
				editingHint.hint = GUILayout.TextArea(editingHint.hint, GUILayout.MinWidth(250 * (Screen.width / 1080)));
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Distance");
				GUILayout.FlexibleSpace();
                double d = 0;
                bool b = double.TryParse(editingHintDistance, out d);
               
                if (!b)
				{
					GUI.contentColor = Color.red;
				} else
                    editingHint.hintDistance = d;
                editingHintDistance = GUILayout.TextField(editingHintDistance, GUILayout.MinWidth(225 * (Screen.width / 1080)));
				GUI.contentColor = originalColor;
				scalesIndex = GUILayout.Toolbar(scalesIndex, scales);
				if(scalesIndex == 0)
				{
					editingHint.scale = Scale.m;
				}
				else
				{
					editingHint.scale = Scale.km;
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				if(GUILayout.Button("Save hint"))
				{
					if (editingMode == EditMode.CreateNew)
					{
						hintList.Add(editingHint);
					}
					editingHint = null;
					addingHint = !addingHint;
				}
				if(GUILayout.Button("Cancel"))
				{
					editingHint = null;
					addingHint = !addingHint;
				}
				GUILayout.EndHorizontal();
			}
			
			GUILayout.Space(20.0f * (Screen.height / 1920f));//space at the bottom
		}
		
		//Draws the import screen, which will fetch the keocaches from the remote server based off of the search criteria specified
		internal void DrawImportScreen()
		{
			//api.login(); //commented out for testing
			//search term bar
			GUILayout.BeginHorizontal();
			CenteredLabel("Import keocaches");
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Keywords");
			GUILayout.FlexibleSpace();
			searchTerms = GUILayout.TextField(searchTerms, GUILayout.MinWidth(250 * (Screen.width/1080)));
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Difficulty");
			difficultySelected = GUILayout.Toolbar(difficultySelected, difficulties);
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			hasMods = GUILayout.Toggle(hasMods, "Modded keocaches");
			GUILayout.FlexibleSpace();
			if (hasMods)
			{
				searchTermsMods = GUILayout.TextField(searchTermsMods, GUILayout.MinWidth(250 * (Screen.width / 1080)));
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			GUILayout.Label("Sort by:", GUILayout.MinWidth(50 * (Screen.width / 1080)));
			sortBySelected = GUILayout.Toolbar(sortBySelected, sortBy);
			GUILayout.EndHorizontal();
			GUILayout.Space(40 * (Screen.width / 1080));
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			bool prev = downloadAllToggle;
			downloadAllToggle = GUILayout.Toggle(downloadAllToggle, "Download");
			GUILayout.Space(20 * (Screen.height / 1980));
			if (downloadAllToggle != prev && downloadAllToggle == true)
			{
				for (int i = 0; i < retrievedCaches.Count; i++)
				{
					retrievedCaches[i].item2 = true;
				}
			}
			else if (downloadAllToggle != prev && downloadAllToggle == false)
			{
				for (int i = 0; i < retrievedCaches.Count; i++)
				{
					retrievedCaches[i].item2 = false;
				}
			}
			for (int i = 0; i < retrievedCaches.Count; i++)
			{
				retrievedCaches[i].item2 = GUILayout.Toggle(retrievedCaches[i].item2, retrievedCaches[i].item1.keocacheCollectionData.title);
			}
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical();
			GUILayout.Label("Creator");
			GUILayout.Space(20 * (Screen.height / 1980));
			for (int i = 0; i < retrievedCaches.Count; i++)
			{
				GUILayout.Label(retrievedCaches[i].item1.keocacheCollectionData.author);
			}
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical();
			GUILayout.Label("Difficulty");
			GUILayout.Space(20 * (Screen.height / 1980));
			for (int i = 0; i < retrievedCaches.Count; i++)
			{
				GUILayout.Label(retrievedCaches[i].item1.keocacheCollectionData.difficulty.ToString());
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Download selected"))
			{
				DownloadSelectedCaches();
			}
			if (GUILayout.Button("Update list"))
			{
				UpdateRetrievedCaches(true);//changeme to have no params to actually access the server
			}
            if (GUILayout.Button("Close", GUILayout.Width(60)))
            {
                KeoCacheDriver.Instance.showKerbalX = false;
            }
			GUILayout.EndHorizontal();
		}


		void UpdateRetrievedCaches(bool testing = false)
		{
			int switch_ = new System.Random().Next() % 3;
			retrievedCaches.Clear();
			if(switch_ == 0)
			{
				KeoCacheCollection coll = new KeoCacheCollection();
				KeoCacheCollectionData cd = new KeoCacheCollectionData();
				cd.name = "Hard boi 1";
				cd.title = "Hello There";
				cd.author = "Benjamin Kerman";
				cd.description = "This is a tester boi";
				cd.difficulty = Difficulty.Insane;
				coll.keocacheCollectionData = cd; retrievedCaches.Add(new Tuple<KeoCacheCollection, bool>(coll, false));
			}
			else if(switch_ == 1)
			{
				KeoCacheCollection coll = new KeoCacheCollection();
				KeoCacheCollectionData cd = new KeoCacheCollectionData();
				cd.name = "Hard boi 2";
				cd.title = "What the...";
				cd.author = "Linuxboi";
				cd.description = "woweee this is cool";
				cd.difficulty = Difficulty.Normal;
				coll.keocacheCollectionData = cd;
				retrievedCaches.Add(new Tuple<KeoCacheCollection, bool>(coll, false));
			}
			else
			{
				KeoCacheCollection coll = new KeoCacheCollection();
				KeoCacheCollectionData cd = new KeoCacheCollectionData();
				cd.name = "Easy man";
				cd.title = "dun dun DUN!";
				cd.author = "Kataochi";
				cd.description = "boi oh boy oh boph";
				cd.difficulty = Difficulty.Easy;
				coll.keocacheCollectionData = cd;
				retrievedCaches.Add(new Tuple<KeoCacheCollection, bool>(coll, false));
			}
		}

		void UpdateRetrievedCaches()
		{
			Debug.Log("[KGC] Updating retrieved cache list...");
			retrievedCaches.Clear();
			WWWForm searchForm = new WWWForm();
			//Keyword data
			searchForm.AddField("searchTerms", searchTerms); //FIXME INCORRECT VALUE FOR FIELD
			//Difficulty data
			byte difficulty;
			switch(difficultySelected)
			{
				case 0:
					difficulty = 0b0001;
					break;
				case 1:
					difficulty = 0b0010;
					break;
				case 2:
					difficulty = 0b0100;
					break;
				case 3:
					difficulty = 0b1000;
					break;
				default:
					difficulty = 0b0001;
					break;
			}
			searchForm.AddBinaryData("difficulty", new byte[] { difficulty });
			byte mods;
			switch(hasMods)
			{
				case true:
					mods = 0b0001;
					break;
				case false://both false and default go to 0b0000
				default:
					mods = 0b0000;
					break;
			}
			searchForm.AddBinaryData("moddedCaches", new byte[] { mods });
			searchForm.AddField("mods", searchTermsMods);
		}
		List<KeoCacheCollection> DownloadSelectedCaches()
		{
			return null;
		}
		void ExportGeoCache()
		{
			WWWForm geoCache = new WWWForm();
		}


		public static void CenteredLabel(string text)
		{
			GUILayout.FlexibleSpace();
			GUILayout.Label(text);
			GUILayout.FlexibleSpace();
		}
	}
}