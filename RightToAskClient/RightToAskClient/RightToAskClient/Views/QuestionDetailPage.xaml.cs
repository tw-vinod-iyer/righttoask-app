using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RightToAskClient.HttpClients;
using RightToAskClient.CryptoUtils;
using RightToAskClient.Models;
using RightToAskClient.Models.ServerCommsData;
using RightToAskClient.ViewModels;
using Xamarin.Forms;
using RightToAskClient.Resx;

// This sets slightly different things to be visible depending on how we 
// arrived here.
// TODO: VT - at the moment, this is a write-up of what I'd like it to do...
// Either way, we should see:
// - the question text,
// - who should answer? Even if it's blank, it should be there

// If the question asker has been filled, or if we came via Option B,
// we should see the 'who should raise it' box (even if it's empty). If we
// came via Option A, or if we're reading someone else's question and the 
// question-asked is blank, it should be omitted.

// We should see questionWriter if it's present, even if it's this user.
// Omit if uninitialized/Anonymous.

// We should see upvotes if we're viewing a published question (including
// ours), but not if we're reviewing our own question (when it's always 0).

// Background-adding should be
// - enabled and visible for our own question (even if already published) 
// - edit-disabled but visible if present in published questions from others
// - invisible if not present in published questions from others

// Link or answer:
// - Omit when reviewing own question.
// - Include and enable when reading another person's question. (Later, we'll
//   enforce the rule that only MPs can answer and others can only add Hansard links.)
// - If this user wrote the question, and there is an answer, show the "Approve Answer"? label
//   We might want some nice icons like the thumbs-up icon on the website.

// TODO: Discuss how to express the "is follow up to" concept.

namespace RightToAskClient.Views
{
    public partial class QuestionDetailPage : ContentPage
    {
        public QuestionDetailPage()
        {
            InitializeComponent();
            BindingContext = QuestionViewModel.Instance;
            QuestionViewModel.Instance.PopupLabelText = AppResources.QuestionDetailPopupText;
            // default to false, then check if they should be true
            QuestionViewModel.Instance.CanEditBackground = false;
            QuestionViewModel.Instance.CanEditQuestion = false;
            
            // TODO (Github issue # ). This logic isn't quite right if you haven't yet registered and are in the process
            // of drafting a question. If you're in the process of drafting but haven't yet registered, you should
            // be able to edit the background (first).
            // I think we want the IsNewQuestion flag
            if (!string.IsNullOrEmpty(App.ReadingContext.ThisParticipant.RegistrationInfo.uid))
            {
                if (!string.IsNullOrEmpty(QuestionViewModel.Instance.Question.QuestionSuggester))
                {
                    if (QuestionViewModel.Instance.Question.QuestionSuggester == App.ReadingContext.ThisParticipant.RegistrationInfo.uid)
                    {
                        QuestionViewModel.Instance.CanEditBackground = true;
                        if (!QuestionViewModel.Instance.IsNewQuestion)
                        {
                            QuestionViewModel.Instance.CanEditQuestion = true;
                        }
                    }
                }
            }
            var vm = BindingContext as QuestionViewModel;
            vm.ReinitQuestionUpdates();
        }

        // TODO At the moment, this just interprets a single string as a single URL, but we should 
        // actually have two entry fields:
        // - One for MPs allows free-form answers, and we'll need a list because MPs can answer other
        // MPs' answers.
        // - One for other participants allows only Hansard links, which should also be allowed to be a list.
        // MPs should see both options; ordinary users can read the free-form answers but only add Hansard urls.
        private void Answer_Entered(object sender, EventArgs e)
        {
            QuestionViewModel.Instance.Question.HansardLink = new List<Uri>()
            {
                new Uri(((Editor)sender).Text)
            };
        }

        protected override bool OnBackButtonPressed()
        {
            App.Current.MainPage.Navigation.PopToRootAsync();
            return true;
        }
    }
}