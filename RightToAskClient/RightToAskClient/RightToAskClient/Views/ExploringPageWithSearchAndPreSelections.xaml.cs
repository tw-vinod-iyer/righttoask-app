using System.Collections.Generic;
using System.Collections.ObjectModel;
using RightToAskClient.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExploringPageWithSearchAndPreSelections  : RightToAskClient.Views.ExploringPageWithSearch 
    {
        public ExploringPageWithSearchAndPreSelections(ObservableCollection<MP> allEntities, 
            ObservableCollection<MP> selectedEntities, string? message=null) : base (allEntities, selectedEntities, message)
        {
            var selectionsListView = setUpPage();
            selectionsListView.ItemsSource = wrapInTags<MP>(selectedMPs, selectedMPs);
        }

        public ExploringPageWithSearchAndPreSelections(ObservableCollection<Authority> filtersSelectedAuthorities,
            string message)
            : base(filtersSelectedAuthorities, message)
        {
            var selectionsListView = setUpPage();
            selectionsListView.ItemsSource = wrapInTags<Authority>(selectedAuthorities, selectedAuthorities);
            
        }

        // Sets up the page, mostly by inserting a list for selected items at the beginning.
        // Returns a pointer to the new ListView, so that its ItemSource can be set according to
        // type of data.
        private ListView setUpPage()
        {
            Label testInsert = new Label() 
                { 
                    Text = "Already selected",
                };
                
            MainLayout.Children.Insert(1, testInsert);

            ListView selections = new ListView()
            {
                ItemTemplate=(DataTemplate)Application.Current.Resources["SelectableDataTemplate"],
            };
            
            MainLayout.Children.Insert(2,selections);
            return selections;
        }
    }
}