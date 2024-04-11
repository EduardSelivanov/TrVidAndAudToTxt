using TrVidAndAudToTxt.Models;

namespace TrVidAndAudToTxt.Services
{
    public class ResultService
    {
        private TranscribeResponse _transcribe;
        public void SetRes(TranscribeResponse res)
        {
            _transcribe = res;
        }
        public TranscribeResponse GetRes()
        {
            return _transcribe;
        }
    }
}
