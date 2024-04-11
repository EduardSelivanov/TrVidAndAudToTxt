using Microsoft.AspNetCore.Mvc;
using TrVidAndAudToTxt.Services;

namespace TrVidAndAudToTxt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HandlerController : ControllerBase
    {
        private readonly PathService _pathSer;
        private readonly AssemblyAILayer _asAiLa;

        public HandlerController(PathService pathSer, AssemblyAILayer asAiLa)
        {
            _pathSer = pathSer;
            _asAiLa = asAiLa;
        }

        //from Yt Url-> Repo return path -> service uses it, convert it to 
        //from Pc file->send to server -//-

        [HttpPost]
        [Route("TransYT")]
        public async Task<IActionResult> FromYtFileToPath([FromBody] string urlOfYT)
        {
            try
            {
                //https://www.youtube.com/shorts/X6IwqjvrZAA
                string p = await _pathSer.PathForYoutube(urlOfYT);
                string? transcribtion = await _asAiLa.ProcessHandler(p);

                //return audiofile path
                //use this audioFilePath to send it to AssemblyAI
                return Ok(transcribtion);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        [Route("SearchWord")]
        public IActionResult SearchForCertainWord([FromBody] string searchReq)
        {
            try
            {
                var aftersearch = _asAiLa.SearchTimeStamps(searchReq);
                return Ok(aftersearch);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //DUMMY
        [HttpPost]
        [Route("From_OwnFile")]
        public async Task<IActionResult> FromOwn()
        {
            string path = "C:\\MyProjectsVisStu22\\TrVidAndAudToTxt\\TrVidAndAudToTxt\\AudioFiles\\output.wav";
            var res = await _asAiLa.ProcessHandler(path);

            return Ok(res);
        }


    }
}
