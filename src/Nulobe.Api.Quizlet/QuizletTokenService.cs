﻿using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Nulobe.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nulobe.Api.Quizlet
{
    public class QuizletTokenService : IQuizletTokenService
    {
        private readonly QuizletOptions _options;

        public QuizletTokenService(
            IOptions<QuizletOptions> options)
        {
            _options = options.Value;
        }

        public async Task<QuizletTokenResponse> GetTokenAsync(QuizletTokenRequest request)
        {
            var tokenFormDictionary = new Dictionary<string, string>()
            {
                { "grant_type", "authorization_code" },
                { "code", request.Code },
                { "redirect_uri", request.RedirectUri.ToString() }
            };

            using (var client = new HttpClient())
            {
                var auth = $"{_options.ClientId}:{_options.ClientSecret}";
                client.DefaultRequestHeaders.Add("Authorization", $"Basic {auth.Base64Encode()}");
                
                var response = await client.PostAsync(_options.TokenEndpoint, new FormUrlEncodedContent(tokenFormDictionary));
                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<QuizletTokenResponse>(content);
                }
                else
                {
                    var error = JsonConvert.DeserializeObject<QuizletError>(content);
                    throw new QuizletException(error);
                }
            }
        }
    }
}
