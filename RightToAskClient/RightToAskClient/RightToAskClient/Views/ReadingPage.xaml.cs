using System;
using System.Collections.ObjectModel;
using RightToAskClient.Controls;
using RightToAskClient.Models;
using Xamarin.Forms;

namespace RightToAskClient.Views
{
	public partial class ReadingPage : ContentPage
	{
		private string draftQuestion;
		private readonly ReadingContext readingContext;
		private FilterDisplayTableView ttestableView;

		public ReadingPage(bool isReadingOnly, ReadingContext readingContext)
		{
			InitializeComponent();
			this.readingContext = readingContext;
			BindingContext = readingContext;

            ttestableView = new FilterDisplayTableView(readingContext.Filters);
            WholePage.Children.Insert(1,ttestableView);
            // ttestableView.IsVisible = true;
            hideFilters();
            
            /*
			FilterShower.GestureRecognizers.Add(new TapGestureRecognizer
			{
				Command = new Command(showFilters)
			});
			
			showFilters();
			*/
            
			if (isReadingOnly)
			{
				TitleBar.Title = "Read Questions";
				QuestionDraftingBox.IsVisible = false;
				KeepButton.IsVisible = false;
				DiscardButton.IsVisible = false;
			}
			else
			{
				TitleBar.Title = "Similar questions";
			}
		}

		private void showFilters(object sender, EventArgs e)
		{  
			ttestableView.IsVisible = true;
            FilterShower.IsVisible = false;
		}

		private void hideFilters()
		{
			ttestableView.IsVisible = false;
			FilterShower.IsVisible = true;
		}

		private void Questions_Scrolled(object sender, ScrolledEventArgs e)
		{
			hideFilters();
		}
		void Question_Entered(object sender, EventArgs e)
		{
			draftQuestion = ((Editor) sender).Text;
			((ReadingContext) BindingContext).DraftQuestion = draftQuestion;
		}

		// Note: it's possible that this would be better with an ItemTapped event instead.
		private async void Question_Selected(object sender, ItemTappedEventArgs e)
		{
			var questionDetailPage 
				= new QuestionDetailPage(false, (Question) e.Item, readingContext);
			await Navigation.PushAsync(questionDetailPage);
		}

		async void OnDiscardButtonClicked(object sender, EventArgs e)
		{
            readingContext.DraftQuestion = null;
            DraftEditor.IsVisible = false;
            DiscardButton.IsVisible = false; 
			KeepButton.IsVisible = false;
			
            bool goHome = await DisplayAlert("Draft discarded", "Save time and focus support by voting on a similar question", "Home", "Related questions");
            if (goHome)
            {
                await Navigation.PopToRootAsync();
            }

		}


		async void OnSaveButtonClicked(object sender, EventArgs e)
		{
			// Tag the new question with the authorities that have been selected.
			// ObservableCollection<Entity> questionAnswerers;
			var questionAnswerers =
				new ObservableCollection<Entity>(readingContext.Filters.SelectedAuthorities);

			foreach (var answeringMP in readingContext.Filters.SelectedAnsweringMPs)
			{
				questionAnswerers.Add(answeringMP);	
			}

			IndividualParticipant thisParticipant = readingContext.ThisParticipant;
			
			Question newQuestion = new Question
			{
				QuestionText = readingContext.DraftQuestion,
				// TODO: Enforce registration before question-suggesting.
				QuestionSuggester 
					= (thisParticipant.Is_Registered) ? thisParticipant.RegistrationInfo.display_name : "Anonymous user",
				QuestionAnswerers = questionAnswerers,
				DownVotes = 0,
				UpVotes = 0
			};


			var questionDetailPage = new QuestionDetailPage(true, newQuestion, readingContext);
			await Navigation.PushAsync(questionDetailPage);
		}
	}
}