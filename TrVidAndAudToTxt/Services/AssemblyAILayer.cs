using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using TrVidAndAudToTxt.Models;

namespace TrVidAndAudToTxt.Services
{
    public class AssemblyAILayer
    {
        private readonly ResultService _resultService;
        private readonly IConfiguration _iconfig;

        public AssemblyAILayer(ResultService res,IConfiguration iconfig)
        {
            _resultService = res;
            _iconfig = iconfig;
        }
        
        public async Task<string?> ProcessHandler(string filePath)
        {
            string? api_key =_iconfig.GetSection("ApiKey")["apiKeyForAI"];
            HttpClient clientApi = new HttpClient();
            clientApi.BaseAddress = new Uri("https://api.assemblyai.com/v2/");
            clientApi.DefaultRequestHeaders.Add("authorization", api_key);

            string? downloadstr = await SendFile(clientApi, filePath);
            string? transcripedId;

            if (downloadstr != null)
            {
                transcripedId = await Transcript(downloadstr, clientApi);
            }
            else
            {
                return null;
            }
            
            DateTime startPoint= DateTime.Now;

            DateTime endPoint = startPoint.AddSeconds(50);


            if(!String.IsNullOrEmpty(transcripedId))
            {
                while (DateTime.Now<endPoint)
                {
                    string? status = await GetStatus(transcripedId, clientApi);
                  
                    if(!String.IsNullOrEmpty(status) && status.Equals("completed"))
                    {
                        TranscribeResponse? resp = await GetTransCriptionViaId(transcripedId, clientApi);
                        if (resp != null)
                        {
                            _resultService.SetRes(resp);
                            return resp.Text;
                        }
                        else
                        {
                            return "nothing to return";
                        }
                    }
                    await Task.Delay(3000);
                    
                   
                }
                return "Unable to reach the Server";
            }
            else
            {
                return "Id is null";
            }
        }
        // 
        private async Task<string?> SendFile(HttpClient cl, string filePath)
        {
            using (FileStream fileStream = File.OpenRead(filePath))
            using (StreamContent fileCOntent = new StreamContent(fileStream))
            {
                fileCOntent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                using (HttpResponseMessage response = await cl.PostAsync("https://api.assemblyai.com/v2/upload", fileCOntent))
                {
                    response.EnsureSuccessStatusCode();
                    var jsonDoc = await response.Content.ReadFromJsonAsync<JsonDocument>();

                    return jsonDoc?.RootElement.GetProperty("upload_url").GetString();
                }
            }
        }

        private async Task<string?> Transcript(string uplUrl, HttpClient client)
        {
            var jsn = new { audio_url = uplUrl };
            StringContent payload = new StringContent(JsonConvert.SerializeObject(jsn), Encoding.UTF8, "application/json");
            HttpResponseMessage responseMessage = await client.PostAsync("https://api.assemblyai.com/v2/transcript", payload);
            responseMessage.EnsureSuccessStatusCode();
            string ResponseString = await responseMessage.Content.ReadAsStringAsync();
            Smtbtw? someresult = JsonConvert.DeserializeObject<Smtbtw>(ResponseString);
            if (someresult != null)
            {
                string idTr = someresult.id;
                return idTr;
            }
            else
            {
                return null;
            }
        }
        private async Task<TranscribeResponse?> GetTransCriptionViaId(string idIssue, HttpClient client)
        {
            if (!String.IsNullOrEmpty(idIssue) && client != null)
            {
                HttpResponseMessage responseMess = await client.GetAsync("https://api.assemblyai.com/v2/transcript/" + idIssue);
                responseMess.EnsureSuccessStatusCode();
                var parsedResponse = await responseMess.Content.ReadAsStringAsync();
                TranscribeResponse? trRes = JsonConvert.DeserializeObject<TranscribeResponse>(parsedResponse);
                return trRes;
            }
            else
            {
                return null;
            }
        }

        private async Task<string?> GetStatus(string id, HttpClient client)
        {
            if (!String.IsNullOrEmpty(id) && client != null)
            {
                HttpResponseMessage responseMess = await client.GetAsync("https://api.assemblyai.com/v2/transcript/" + id);
                responseMess.EnsureSuccessStatusCode();

                string status = await responseMess.Content.ReadAsStringAsync();
                if (!String.IsNullOrEmpty(status))
                {
                    Smtbtw? someresult = JsonConvert.DeserializeObject<Smtbtw>(status);
                    return someresult?.status;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

        }

        //PBLC MTHDS
        public List<ConvertedWordResponse>? SearchTimeStamps(string req)
        {
            if (!string.IsNullOrEmpty(req))
            {
                TranscribeResponse resp = _resultService.GetRes();
                List<WordResp>? result = resp.Words.Where(w => w.text.Contains(req)).ToList();
                List<ConvertedWordResponse> convResp = new List<ConvertedWordResponse>();
                foreach (var wordResp in result)
                {
                    float sv = wordResp.start / 1000f;
                    float ev = wordResp.end / 1000f;
                    convResp.Add(new ConvertedWordResponse
                    {
                        start = (int)sv,
                        end = ev,
                        text = wordResp.text
                    });
                }
                return convResp;

            }
            else
            {
                return null;
            }

        }
    }
}
