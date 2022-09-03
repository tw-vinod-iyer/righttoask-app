﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RightToAskClient.Models;
using RightToAskClient.Models.ServerCommsData;
using RightToAskClient.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OtherUserProfilePage : ContentPage
    {
        public OtherUserProfilePage(Registration user)
        {
            BindingContext = new RegistrationViewModel(); 
            InitializeComponent();
        }
    }
}