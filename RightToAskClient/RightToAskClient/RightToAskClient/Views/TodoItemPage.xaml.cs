﻿using System;
using RightToAskClient.Models;

namespace RightToAskClient.Views
{
	public partial class TodoItemPage
	{
		public TodoItemPage ()
		{
			InitializeComponent ();
		}
		public TodoItemPage (Registration reg)
		{
			InitializeComponent ();
			BindingContext = reg;
		}

		async void OnSaveButtonClicked (object sender, EventArgs e)
		{
			var newRegistration = (Registration)BindingContext;
			Result<bool> httpResponse = await App.RegItemManager.SaveTaskAsync (newRegistration);
			if(String.IsNullOrEmpty(httpResponse.Err))
			{
				if (httpResponse.Ok)
				{
					reportLabel.Text = "Server signature successfully verified.";
				}
				else
				{
					reportLabel.Text = "Server signature verification failed";
				}
			}
			else
			{
				reportLabel.Text = "Server connection error" + httpResponse.Err;
			}
		}

		async void OnCancelButtonClicked (object sender, EventArgs e)
		{
			await Navigation.PopAsync ();
		}
	}
}
