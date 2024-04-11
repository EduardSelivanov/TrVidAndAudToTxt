namespace FrontEndAudToText.Services
{
    public class DataSaver
    {
        private string ParsedText;
        public void SetText(string text)
        {
            ParsedText = text;
        }
        public string GetText()
        {
            return ParsedText;
        }
    }
}
