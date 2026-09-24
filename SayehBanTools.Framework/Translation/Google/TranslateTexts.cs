using Newtonsoft.Json.Linq;
using SayehBanTools.Framework.Translation.Google.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SayehBanTools.Framework.Translation.Google
{
    /// <summary>
    /// این کلاس برای ترجمه متن به زبان های مختلف استفاده میشود.
    /// </summary>
    public class TranslateTexts
    {
        private readonly HttpClient _httpClient;
        /// <summary>
        /// این متد برای ترجمه متن به زبان های مختلف استفاده میشود.
        /// </summary>
        /// <param name="httpClient"></param>
        public TranslateTexts(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        /// <summary>
        /// این متد برای ترجمه متن به زبان های مختلف استفاده میشود.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<TranslationResponse> TranslateTextAsync(TranslationRequest request)
        {
            string url = $"https://translate.google.com/translate_a/single?client=gtx&sl={request.InputLanguage}&tl={request.OutputLanguage}&dt=t&q={request.OriginalText}";

            using (var response = await _httpClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    string responseText = await response.Content.ReadAsStringAsync();
                    JArray jsonResponse = JArray.Parse(responseText);

                    JArray translationParts = (JArray)jsonResponse[0];
                    List<string> translatedParts = new List<string>();

                    foreach (JArray part in translationParts)
                    {
                        string translatedText = part[0]?.ToString(); // Safely handle potential null values
                        if (translatedText != null)
                        {
                            translatedParts.Add(translatedText);
                        }
                    }

                    return new TranslationResponse { Translations = translatedParts };
                }
                else
                {
                    throw new Exception("Translation request failed. Status code: " + response.StatusCode);
                }
            }
        }
    }

    /*
     طریقه استفاده از دستور
    using API.Model;
    using Microsoft.AspNetCore.Mvc;
    using SayehBanTools.Translation.Google;
    using SayehBanTools.Translation.Google.Model;

    namespace API.Controllers;

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TranslateController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<TranslationResponse>> TranslateText([FromQuery] TranslationRequest request)
        {
            var translator = new TranslateTexts(new HttpClient());

            var translationResponse = await translator.TranslateTextAsync(request); // Use the parameter directly

            if (translationResponse != null)
            {
                Console.WriteLine("Translated text:");
                foreach (string translatedPart in translationResponse.Translations)
                {
                    Console.WriteLine(translatedPart);
                }
            }
            else
            {
                Console.WriteLine("Translation failed.");
            }

            return translationResponse;
        }
    }


     */
}