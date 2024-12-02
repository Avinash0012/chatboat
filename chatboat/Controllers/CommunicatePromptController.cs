using chatboat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Matching;
using Newtonsoft.Json;
using System.Text;

namespace chatboat.Controllers
{
    public class CommunicatePromptController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public CommunicatePromptController(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> SendRequest(string prompt)
        {
            var apiUrl = _configuration["ApiSettings:ApiUrl"];
            var apiKey = _configuration["ApiSettings:ApiKey"];
            var requestPayload = new 
            {
                contents = new[]
            {
                new
                {
                    role = "user",
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
            };
            var res = JsonConvert.SerializeObject(requestPayload);
            var content = new StringContent(res, Encoding.UTF8, "application/json");
            var fullUrl = $"{apiUrl}?key={apiKey}";
            var response = await _httpClient.PostAsync(fullUrl, content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(responseContent))
                {
                    var contentList = JsonConvert.DeserializeObject<Response>(responseContent);
                    if (contentList?.Candidates != null && contentList.Candidates.Count > 0)
                    {
                        var firstCandidate = contentList.Candidates[0];
                        if (firstCandidate?.Content?.Parts != null && firstCandidate.Content.Parts.Count > 0)
                        {
                            var firstPart = firstCandidate.Content.Parts[0];
                            return Json( firstPart.Text);
                        }
                    }
                }
            }
            return StatusCode((int)response.StatusCode, response.ReasonPhrase);
        }
    }
}
