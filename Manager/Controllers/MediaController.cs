using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataAccess.Models;
using DataAccess.ViewModels;
using Manager.Repositories;
using Manager.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using static Manager.Classes.Utility;

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
            return Ok(await unitOfWork.Media.GetCollection<MediaViewModel>(x => x.MediaType == type));
        }





        [HttpPost, DisableRequestSizeLimit]
        [Route("Image")]
        public async Task<ActionResult> NewImage()
        {
            // Get the new image
            IFormFile imageFile = Request.Form.Files["image"];

            // Get the image name
            StringValues imageName;
            Request.Form.TryGetValue("name", out imageName);


            // Copy the image to the images folder
            string imageUrl = await CopyImage(imageFile);


            // Add the new image to the database
            Media media = new Media
            {
                Name = imageName,
                Image = imageUrl,
                MediaType = (int)MediaType.Image,
                Thumbnail = imageUrl
            };


            // Add the new image to the database
            unitOfWork.Media.Add(media);
            await unitOfWork.Save();

            return Ok(new
            {
                id = media.Id,
                Image = media.Image,
                name = media.Name,
                thumbnail = media.Thumbnail,
                mediaType = media.MediaType
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
            string wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string imagesFolder = Path.Combine(wwwroot, "images");
            string filePath = Path.Combine(imagesFolder, media.Image);
            System.IO.File.Delete(filePath);

            // Update the url
            media.Thumbnail = media.Image = imageUrl;

            // Update and save
            unitOfWork.Media.Update(media);
            await unitOfWork.Save();


            return Ok(new { src = imageUrl });
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
            string wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string imagesFolder = Path.Combine(wwwroot, "images");
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
        public async Task<ActionResult> NewVideo(MediaViewModel media)
        {
            var thumbnail = await GetVideoThumbnail(media);

            if (thumbnail == null) return Ok();

            Media newVideo = new Media
            {
                Name = media.Name,
                VideoId = media.VideoId,
                MediaType = media.Type,
                VideoType = media.VideoType,
                Thumbnail = thumbnail,
                Image = ""
            };

            // Add the new video
            unitOfWork.Media.Add(newVideo);
            await unitOfWork.Save();


            return Ok(new
            {
                name = newVideo.Name,
                videoId = newVideo.VideoId,
                mediaType = newVideo.MediaType,
                videoType = newVideo.VideoType,
                thumbnail = newVideo.Thumbnail
            });
        }






        [HttpPut]
        [Route("Video")]
        public async Task<ActionResult> UpdateVideo(ItemViewModel video)
        {
            // Get the current video
            //Media media = await unitOfWork.Media.Get(video.Id);

            //// Delete the old thumbnail
            //string wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            //string imagesFolder = Path.Combine(wwwroot, "images");
            //System.IO.File.Delete(Path.Combine(imagesFolder, media.VideoId));


            //// Get the updated video
            //Media updatedVideo = await SetVideo(video.Name);

            //// Update the new properties
            //media.Image = updatedVideo.Image;
            //media.VideoId = updatedVideo.VideoId;


            //// Update & save
            //unitOfWork.Media.Update(media);
            //await unitOfWork.Save();

            //return Ok(new
            //{
            //    url = media.Image,
            //    thumbnail = media.VideoId
            //});


            return Ok();
        }





        private async Task<string> GetVideoThumbnail(MediaViewModel media)
        {
            // Create a new unique name for the thumbnail
            string thumbnailName = Guid.NewGuid().ToString("N") + ".jpg";
            string thumbnailUrl = string.Empty;

            // YouTube
            if (media.VideoType == (int)VideoType.YouTube)
            {
                // Get the youtube thumbnail
                thumbnailUrl = "https://img.youtube.com/vi/" + media.VideoId + "/mqdefault.jpg";

                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.GetAsync(thumbnailUrl);

                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        thumbnailUrl = string.Empty;
                    }
                }

            }


            // Vimeo
            else if (media.VideoType == (int)VideoType.Vimeo)
            {
                // Get vimeo thumbnail
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.GetAsync("https://vimeo.com/api/oembed.json?url=https://vimeo.com/" + media.VideoId);

                    if (response.StatusCode != HttpStatusCode.NotFound)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var json = JsonSerializer.Deserialize<Vimeo>(result);
                        thumbnailUrl = json.thumbnail_url;
                    }
                }
            }


            // Wistia
            else if (media.VideoType == (int)VideoType.Wistia)
            {
                // Get the Wistia thumbnail
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.GetAsync("http://fast.wistia.net/oembed?url=http://home.wistia.com/medias/" + media.VideoId);

                    if (response.StatusCode != HttpStatusCode.NotFound)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var json = JsonSerializer.Deserialize<Wistia>(result);
                        thumbnailUrl = json.thumbnail_url;
                    }

                }
            }


            if (thumbnailUrl != string.Empty)
            {
                // Download the thumbnail to the images folder
                using (WebClient client = new WebClient())
                {

                    string wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    string imagesFolder = Path.Combine(wwwroot, "images");
                    string filePath = Path.Combine(imagesFolder, thumbnailName);

                    client.DownloadFileAsync(new Uri(thumbnailUrl), filePath);
                }

                return thumbnailName;
            }

            return null;
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
            return Ok(await unitOfWork.Media.GetCollection<MediaViewModel>(x => x.MediaType == type, searchWords));
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



    class Wistia
    {
        public string Version { get; set; }
        public string type { get; set; }
        public string html { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string provider_name { get; set; }
        public string provider_url { get; set; }
        public string title { get; set; }
        public string thumbnail_url { get; set; }
        public int thumbnail_width { get; set; }
        public int thumbnail_height { get; set; }
        public float duration { get; set; }
    }
}