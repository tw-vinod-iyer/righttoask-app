using RightToAskClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.CommunityToolkit.ObjectModel;

namespace RightToAskClient.Models
{
    public class Question : ObservableObject
    {
        private int _upVotes;
        private int _downVotes;

        private string _questionText = "";
        public string QuestionText
        {
            get => _questionText;
            set
            {
                SetProperty(ref _questionText, value);
                QuestionViewModel.Instance._serverQuestionUpdates.question_text = _questionText;
            }
        }
        public DateTime UploadTimestamp { get; set; }
        private string _background = "";
        public string Background
        {
            get => _background;
            set
            {
                SetProperty(ref _background, value);
                QuestionViewModel.Instance._serverQuestionUpdates.background = _background;
            }
        }
        public bool AnswerAccepted { get; set; }
        public string IsFollowupTo { get; set; } = "";
        public List<string> Keywords { get; set; } = new List<string>();
        public List<string> Category { get; set; } = new List<string>();
        public DateTime ExpiryDate { get; set; }
        private string _questionId = "";
        public string QuestionId
        {
            get => _questionId;
            set
            {
                SetProperty(ref _questionId, value);
                QuestionViewModel.Instance._serverQuestionUpdates.question_id = _questionId;
            }
        }
        private string _version = "";
        public string Version
        {
            get => _version;
            set
            {
                SetProperty(ref _version, value);
                QuestionViewModel.Instance._serverQuestionUpdates.version = _version;
            }
        }

        // The person who suggested the question
        private string _questionSuggester = "";
        public string QuestionSuggester 
        { 
            get => _questionSuggester; 
            set => SetProperty(ref _questionSuggester, value); 
        }

        // The Authority, department, MPs, who are meant to answer 
        public ObservableCollection<Entity> QuestionAnswerers { get; set; } = new ObservableCollection<Entity>();
        
        // The MPs or committee who are meant to ask the question
        public string QuestionAsker { get; set; } = "";

        public string LinkOrAnswer { get; set; } = "";

        public int UpVotes
        {
            get => _upVotes;
            set => SetProperty(ref _upVotes, value);
        }
        public int DownVotes 
        {
            get
            {
                return _downVotes;
            }
            set
            {
                _downVotes = value;
                OnPropertyChanged();
            }
        }
        
        public bool AlreadyUpvoted { get; set; }

        public override string ToString ()
        {
            
            List<string> questionAnswerersList 
                = QuestionAnswerers.Select(ans => ans.GetName()).ToList();
            // view.Select(f => return new { Food = f, Selected = selectedFood.Contains(f)});
            return QuestionText+ "\n" +
                   "Suggested by: " + QuestionSuggester + '\n' +
                   "To be asked by: " + QuestionAsker + '\n' +
                   // var readablePhrase = string.Join(" ", words); 
                   "To be answered by: " + string.Join(", ", questionAnswerersList) + '\n' +
                   "UpVotes: " + UpVotes+ '\n' +
                   // "DownVotes: " + DownVotes + '\n' +
                   "Link/Answer: " + LinkOrAnswer;
        }
    }
}