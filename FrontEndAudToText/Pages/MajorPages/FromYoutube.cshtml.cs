using FrontEndAudToText.Models;
using FrontEndAudToText.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FrontEndAudToText.Pages.MajorPages
{
    public class FromYoutubeModel : PageModel
    {
        private readonly CommunicationServiceWithApi _commTool;
        private readonly DataSaver _saveSer;

        public FromYoutubeModel(CommunicationServiceWithApi communication, DataSaver saveser)
        {
            _commTool = communication;
            _saveSer = saveser;
        }
        [BindProperty]
        public string? YtURL { get; set; }
        [BindProperty]
        public string? Result { get; set; }

        [BindProperty]
        public List<WordResult>? SearchResults { get; set; }

        public void OnGet()
        {
            Result = _saveSer.GetText();
            if (Result == null)
            {
                Result = "here will be your result";
            }
            else
            {
                Result = _saveSer.GetText();
            }
        }
        public async Task OnPostTranscribe()
        {
            if (!String.IsNullOrEmpty(YtURL))
            {
                Result = await _commTool.GetTextFromYouTube(YtURL);
                _saveSer.SetText(Result);
            }
        }
        public async Task OnPostSearchWord(string word)
        {
            if (word != null)
            {
                SearchResults = await _commTool.GetSearchResults(word);
                Result = _saveSer.GetText();
            }
        }
    }
}
