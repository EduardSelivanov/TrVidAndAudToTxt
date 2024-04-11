namespace TrVidAndAudToTxt.Models
{
    public class TranscribeResponse
    {
        public string? Text { get; set; }
        public List<WordResp>? Words { get; set; }
    }
}
