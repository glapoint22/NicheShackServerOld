using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataAccess.Models;
using DataAccess.ViewModels;
using Manager.Classes;
using Manager.Repositories;
using Manager.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Manager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public MediaController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        [HttpGet]
        public async Task<ActionResult> Get(int type)
        {
            return Ok(await unitOfWork.Media.GetCollection<MediaViewModel>(x => x.Type == type));
        }





        [HttpPost, DisableRequestSizeLimit]
        [Route("Image")]
        public async Task<ActionResult> NewImage()
        {
            StringValues typeValue;

            // Get the new image
            IFormFile imageFile = Request.Form.Files["image"];


            // Get the media type
            Request.Form.TryGetValue("type", out typeValue);
            int type = Convert.ToInt32(typeValue);


            // Copy the image to the images folder
            string imageUrl = await CopyImage(imageFile);


            // Add the new image to the database
            Media media = new Media
            {
                Name = "",
                Url = imageUrl,
                Type = type
            };


            // Add the new image to the database
            unitOfWork.Media.Add(media);
            await unitOfWork.Save();

            return Ok(new
            {
                id = media.Id,
                url = media.Url
            });
        }









        [HttpPost, DisableRequestSizeLimit]
        [Route("UpdateImage")]
        public async Task<ActionResult> UpdateImage()
        {

            // Get the new image
            IFormFile imageFile = Request.Form.Files["image"];


            StringValues idValue;

            // Get the media type
            Request.Form.TryGetValue("id", out idValue);
            int id = Convert.ToInt32(idValue);



            // Copy the image to the images folder
            string imageUrl = await CopyImage(imageFile);



            // Get the id of the image
            Media media = await unitOfWork.Media.Get(id);

            


            // Delete the old image
            System.IO.File.Delete(Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "images"), media.Url));

            // Update the url
            media.Url = imageUrl;

            // Update and save
            unitOfWork.Media.Update(media);
            await unitOfWork.Save();


            return Ok(imageUrl);
        }





        private async Task<string> CopyImage(IFormFile imageFile)
        {
            // This will get the file extension
            Regex regex = new Regex(@"\.(jpg|jpeg|gif|png|bmp|tiff|tga|svg|webp)$", RegexOptions.IgnoreCase);
            Match match = regex.Match(imageFile.FileName);
            string fileExtension = match.Value;


            // Create a new unique name for the image
            string imageUrl = Guid.NewGuid().ToString("N") + fileExtension;

            // Place the new image into the images folder
            string imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "images");
            string filePath = Path.Combine(imagesFolder, imageUrl);

            // Create the file stream
            var fileStream = new FileStream(filePath, FileMode.Create);

            // Copy to image to the images folder
            await imageFile.CopyToAsync(fileStream);


            // Close the file stream
            fileStream.Close();


            return imageUrl;
        }









        [HttpPost]
        [Route("Video")]
        public async Task<ActionResult> NewVideo(ItemViewModel videoLink)
        {
            Media video = await GetVideo(videoLink.Name);

            // Add the new video to the database
            Media media = new Media
            {
                Name = "",
                Url = video.Url,
                Thumbnail = video.Thumbnail,
                Type = 7
            };

            unitOfWork.Media.Add(media);

            await unitOfWork.Save();

            return Ok(new
            {
                id = media.Id,
                url = media.Url,
                thumbnail = media.Thumbnail
            });
        }






        [HttpPut]
        [Route("Video")]
        public async Task<ActionResult> UpdateVideo(ItemViewModel video)
        {
            // Get the current video
            Media media = await unitOfWork.Media.Get(video.Id);

            // Delete the old thumbnail
            System.IO.File.Delete(Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "images"), media.Thumbnail));


            // Get the updated video
            Media updatedVideo = await GetVideo(video.Name);

            // Update the new properties
            media.Url = updatedVideo.Url;
            media.Thumbnail = updatedVideo.Thumbnail;


            // Update & save
            unitOfWork.Media.Update(media);
            await unitOfWork.Save();

            return Ok(new
            {
                url = media.Url,
                thumbnail = media.Thumbnail
            });
        }





        private async Task<Media> GetVideo(string videoLink)
        {
            // Create a new unique name for the thumbnail
            string thumbnail = Guid.NewGuid().ToString("N") + ".jpg";
            string thumbnailUrl;
            string videoUrl;


            // Is this youtube or vimeo 
            Regex youtubeRegex = new Regex("youtube");
            Match youtubeMatch = youtubeRegex.Match(videoLink);



            if (youtubeMatch.Success)
            {
                // Get youtube video Id
                Regex youtubeIdRegex = new Regex(@"(?:https?:\/\/)?(?:(?:(?:www\.?)?youtube\.com(?:\/(?:(?:watch\?.*?(?:v=([^&\s]+)).*)|(?:v\/(.*))|(?:embed\/(.+))))?)|(?:youtu\.be\/(.*)?))");
                Match youtubeIdMatch = youtubeIdRegex.Match(videoLink);


                string videoId = null;

                for (int i = 1; i < youtubeIdMatch.Groups.Count; i++)
                {
                    if (youtubeIdMatch.Groups[i].Value != string.Empty)
                    {
                        videoId = youtubeIdMatch.Groups[i].Value;
                        break;
                    }
                }



                // Get youtube thumbnail url
                thumbnailUrl = "https://img.youtube.com/vi/" + videoId + "/mqdefault.jpg";


                // Get youtube url
                videoUrl = "https://www.youtube.com/embed/" + videoId;
            }
            else
            {

                // Get vimeo video id
                Regex vimeoIdRegex = new Regex(@"(?:(?:https?:)?\/\/(?:[\w]+\.)*vimeo\.com(?:[\/\w:]*(?:\/videos)?)?\/([0-9]+)[^\s]*)");
                Match vimeoIdMatch = vimeoIdRegex.Match(videoLink);


                string videoId = vimeoIdMatch.Groups[1].Value;

                // Get vimeo thumbnail
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.GetAsync("https://vimeo.com/api/oembed.json?url=https://vimeo.com/" + videoId);
                    var result = await response.Content.ReadAsStringAsync();
                    var json = JsonSerializer.Deserialize<Vimeo>(result);
                    thumbnailUrl = json.thumbnail_url;
                }



                // Get vimeo url
                videoUrl = "https://player.vimeo.com/video/" + videoId;
            }



            // Download the thumbnail to the images folder
            using (WebClient client = new WebClient())
            {
                client.DownloadFileAsync(new Uri(thumbnailUrl), "images/" + thumbnail);
            }


            Media media = new Media
            {
                Url = videoUrl,
                Thumbnail = thumbnail,
                Type = 7
            };


            return media;
        }





        [HttpPut]
        [Route("Name")]
        public async Task<ActionResult> UpdateMediaName(ItemViewModel media)
        {
            Media updatedMedia = await unitOfWork.Media.Get(media.Id);

            updatedMedia.Name = media.Name;

            // Update and save
            unitOfWork.Media.Update(updatedMedia);
            await unitOfWork.Save();

            return Ok();
        }






        [HttpDelete]
        public async Task<ActionResult> DeleteMedia(int id)
        {
            Media media = await unitOfWork.Media.Get(id);
            unitOfWork.Media.Remove(media);

            await unitOfWork.Save();

            return Ok();
        }







        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult> Search(int type, string searchWords)
        {
            return Ok(await unitOfWork.Media.GetCollection<MediaViewModel>(x => x.Type == type, searchWords));
        }

    }


    class Vimeo
    {
        public string type { get; set; }
        public string version { get; set; }
        public string provider_name { get; set; }
        public string provider_url { get; set; }
        public string title { get; set; }
        public string author_name { get; set; }
        public string author_url { get; set; }
        public string account_type { get; set; }
        public string html { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int duration { get; set; }
        public string description { get; set; }
        public string thumbnail_url { get; set; }
        public int thumbnail_width { get; set; }
        public int thumbnail_height { get; set; }
        public string thumbnail_url_with_play_button { get; set; }
        public string upload_date { get; set; }
        public int video_id { get; set; }
        public string uri { get; set; }
    }
}