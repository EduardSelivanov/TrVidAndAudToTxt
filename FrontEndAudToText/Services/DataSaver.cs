namespace FrontEndAudToText.Services
{
    public class DataSaver
    {
        private string ParsedText;
        private string _url;
        public void SetText(string text)
        {
            ParsedText = text;
        }
        public string GetText()
        {
            return ParsedText;
        }
        public void SetUrl(string url)
        {
            _url = url;
        }
        public string GetUrl() 
        {
            return _url;
        }


    }
}
