﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RightToAskClient.Models;

namespace RightToAskClient.Data
{
    public class RestService : IRestService
    {
        HttpClient client;
        JsonSerializerOptions serializerOptions;

        public Result<List<string>> Items { get; private set; }

        public RestService()
        {
            client = new HttpClient();
            serializerOptions = new JsonSerializerOptions
            {
                // PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        public async Task<Result<List<string>>> RefreshDataAsync()
        {
            Items = new Result<List<string>>();

            Uri uri = new Uri(string.Format(Constants.UserListUrl, string.Empty));
            try
            {
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Items = JsonSerializer.Deserialize<Result<List<string>>>(content, serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return Items;
        }

        public async Task SaveTodoItemAsync(Registration item, bool isNewItem = false)
        {
            Uri uri = new Uri(string.Format(Constants.RegUrl, string.Empty));

            try
            {
                string json = JsonSerializer.Serialize<Registration>(item, serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;
                if (isNewItem)
                {
                    response = await client.PostAsync(uri, content);
                }
                else
                {
                    // TODO: Never use put. Get rid of this.
                    response = await client.PutAsync(uri, content);
                }

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Result<string> httpResponse =
                        JsonSerializer.Deserialize<Result<string>>(responseContent, serializerOptions);

                    if (String.IsNullOrEmpty(httpResponse.Err))
                    {
                        Debug.WriteLine(@"\tTodoItem successfully saved.");
                    }
                    else
                    {
                        // TODO: Give the user a sensible message - this is where we'll learn, for example,
                        // if the UID is already taken.
                        Debug.WriteLine(@"\tError saving TodoItem:"+httpResponse.Err);
                    }
                    
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }

    }
}
