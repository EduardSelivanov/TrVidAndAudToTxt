
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace TrVidAndAudToTxt.Services
{
    public class PathService
    {
        // create path for Youtube audiofile
        public async Task<string> PathForYoutube(string urlYt)
        {
            try
            {
                string pathhForAudio = "C:\\MyProjectsVisStu22\\TrVidAndAudToTxt\\TrVidAndAudToTxt\\AudioFiles\\result.wav";
                await ExtractAudio(urlYt, pathhForAudio);
                return pathhForAudio;
            }
            catch (Exception ex)
            {
                throw new Exception("an err:", ex);
            }


        }
        //YoutubeExplode 
        private async Task ExtractAudio(string ytUrl, string pathToSave)
        {
            YoutubeClient ytClient = new YoutubeClient();
            var video = await ytClient.Videos.GetAsync(ytUrl);

            var streamInfoSet = await ytClient.Videos.Streams.GetManifestAsync(video.Id);
            var streamInfo = streamInfoSet.GetAudioOnlyStreams().GetWithHighestBitrate();

            if (streamInfo != null)
            {
                await ytClient.Videos.Streams.DownloadAsync(streamInfo, pathToSave);
            }
            else
            {
                throw new Exception("No streams");
            }
        }


    }
}
