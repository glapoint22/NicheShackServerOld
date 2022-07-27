using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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


        //[HttpGet]
        //public async Task<ActionResult> Get(int type)
        //{
        //    return Ok(await unitOfWork.Media.GetCollection<MediaViewModel>(x => x.MediaType == type));
        //}





        [HttpPost, DisableRequestSizeLimit]
        [Route("Image")]
        public async Task<ActionResult> NewImage()
        {
            // Get the new image
            IFormFile imageFile = Request.Form.Files["image"];


            // Get the image name
            StringValues imageName;
            Request.Form.TryGetValue("name", out imageName);



            // Create a new media object
            Media media = new Media
            {
                Name = imageName,
                MediaType = (int)MediaType.Image,
            };




            // Get the image size
            StringValues imageSizeString;
            Request.Form.TryGetValue("imageSize", out imageSizeString);
            ImageSize imageSize = (ImageSize)int.Parse(imageSizeString);

            // Set the image sizes for this image
            string imageSrc = await SetImageSizes(imageSize, imageFile, media);



            // Add the new image to the database
            unitOfWork.Media.Add(media);
            await unitOfWork.Save();

            return Ok(new
            {
                id = media.Id,
                src = imageSrc,
                name = media.Name,
                thumbnail = media.Thumbnail
            });
        }




        private async Task<string> ScaleImage(IFormFile imageFile, float size)
        {
            float originalWidth;
            float originalHeight;

            using (var image = Image.FromStream(imageFile.OpenReadStream()))
            {
                originalWidth = image.Width;
                originalHeight = image.Height;
            }


            float maxSize = Math.Max(originalWidth, originalHeight);

            if (maxSize <= size) return null;


            float multiplier = size / maxSize;
            int imageWidth = (int)Math.Round(originalWidth * multiplier);
            int imageHeight = (int)Math.Round(originalHeight * multiplier);

            //Convert from image file to bitmap
            Bitmap bitmap;
            using (var memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);

                using (var tempImage = Image.FromStream(memoryStream))
                {
                    bitmap = new Bitmap(tempImage);
                }
            }



            //Scale
            Bitmap scaledBitmap = new Bitmap(imageWidth, imageHeight);
            Graphics graph = Graphics.FromImage(scaledBitmap);
            graph.InterpolationMode = InterpolationMode.High;
            graph.DrawImage(bitmap, 0, 0, imageWidth, imageHeight);


            string wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string imagesFolder = Path.Combine(wwwroot, "images");

            //Create the new image
            string name = Guid.NewGuid().ToString("N") + ".png";
            string newImage = Path.Combine(imagesFolder, name);
            scaledBitmap.Save(newImage, ImageFormat.Png);

            return name;
        }



        private async Task<string> SetImageSizes(ImageSize imageSize, IFormFile imageFile, Media media)
        {
            const int thumbnailSize = 100;
            string imageSrc = string.Empty;

            // 500
            if (imageSize == ImageSize.Medium)
            {
                media.ImageLg = await ScaleImage(imageFile, 675);
                media.ImageMd = imageSrc = await ScaleImage(imageFile, 500);

                // Image is less or equal to 500
                if (imageSrc == null)
                {
                    media.ImageLg = media.ImageMd = imageSrc = await CopyImage(imageFile);
                }
                else
                {
                    // If image is less or equal to 675
                    if (media.ImageLg == null) media.ImageLg = media.ImageMd;
                }

                media.ImageSm = await ScaleImage(imageFile, 200);

                // Image is less or equal to 200
                if (media.ImageSm == null)
                {
                    media.ImageSm = media.ImageMd;
                }



                media.Thumbnail = await ScaleImage(imageFile, thumbnailSize);

                // Image is less or equal to the thumbnail size
                if (media.Thumbnail == null)
                {
                    media.Thumbnail = media.ImageSm;
                }
            }

            // 200
            else if (imageSize == ImageSize.Small)
            {
                media.ImageSm = imageSrc = await ScaleImage(imageFile, 200);

                // Image is less or equal to 200
                if (imageSrc == null)
                {
                    media.ImageSm = imageSrc = await CopyImage(imageFile);
                }


                media.Thumbnail = await ScaleImage(imageFile, thumbnailSize);

                // Image is less or equal to the thumbnail size
                if (media.Thumbnail == null)
                {
                    media.Thumbnail = media.ImageSm;
                }
            }


            // Any Size
            else if (imageSize == ImageSize.AnySize)
            {
                media.ImageAnySize = imageSrc = await CopyImage(imageFile);
                media.Thumbnail = await ScaleImage(imageFile, thumbnailSize);

                // Image is less or equal to the thumbnail size
                if (media.Thumbnail == null)
                {
                    media.Thumbnail = media.ImageAnySize;
                }
            }

            return imageSrc;
        }




        [HttpPost, DisableRequestSizeLimit]
        [Route("UpdateImage")]
        public async Task<ActionResult> UpdateImage()
        {
            // Get the new image
            IFormFile imageFile = Request.Form.Files["image"];


            // Get the Image id
            StringValues idValue;
            Request.Form.TryGetValue("id", out idValue);
            int id = Convert.ToInt32(idValue);



            // Get the image size
            StringValues imageSizeString;
            Request.Form.TryGetValue("imageSize", out imageSizeString);
            ImageSize imageSize = (ImageSize)int.Parse(imageSizeString);
            

            // Get the id of the image
            Media media = await unitOfWork.Media.Get(id);


            // Delete the old images
            string wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string imagesFolder = Path.Combine(wwwroot, "images");

            // Thumbnail
            if (media.Thumbnail != null)
            {
                string image = Path.Combine(imagesFolder, media.Thumbnail);
                System.IO.File.Delete(image);
                media.Thumbnail = null;
            }


            // 200
            if (media.ImageSm != null)
            {
                string image = Path.Combine(imagesFolder, media.ImageSm);
                System.IO.File.Delete(image);
                media.ImageSm = null;
            }

            // 500
            if (media.ImageMd != null)
            {
                string image = Path.Combine(imagesFolder, media.ImageMd);
                System.IO.File.Delete(image);
                media.ImageMd = null;
            }

            // 675
            if (media.ImageLg != null)
            {
                string image = Path.Combine(imagesFolder, media.ImageLg);
                System.IO.File.Delete(image);
                media.ImageLg = null;
            }


            // Any Size
            if (media.ImageAnySize != null)
            {
                string image = Path.Combine(imagesFolder, media.ImageAnySize);
                System.IO.File.Delete(image);
                media.ImageAnySize = null;
            }


            // Set the image sizes for this image
            string imageSrc = await SetImageSizes(imageSize, imageFile, media);


            

            // Update and save
            unitOfWork.Media.Update(media);
            await unitOfWork.Save();

            return Ok(new { src = imageSrc });
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
        public async Task<ActionResult> NewVideo(MediaViewModel video)
        {
            var thumbnail = await GetVideoThumbnail(video);

            if (thumbnail == null) return Ok();

            Media newVideo = new Media
            {
                Name = video.Name,
                VideoId = video.VideoId,
                MediaType = (int)MediaType.Video,
                VideoType = video.VideoType,
                Thumbnail = thumbnail,
                ImageAnySize = ""
            };

            // Add the new video
            unitOfWork.Media.Add(newVideo);
            await unitOfWork.Save();


            return Ok(new
            {
                id = newVideo.Id,
                thumbnail = newVideo.Thumbnail
            });
        }






        [HttpPut]
        [Route("UpdateVideo")]
        public async Task<ActionResult> UpdateVideo(MediaViewModel video)
        {
            // Get the updated video
            var thumbnail = await GetVideoThumbnail(video);

            if (thumbnail == null) return Ok();

            // Get the current video
            Media media = await unitOfWork.Media.Get(video.Id);

            // Delete the old thumbnail
            string wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string imagesFolder = Path.Combine(wwwroot, "images");
            System.IO.File.Delete(Path.Combine(imagesFolder, media.Thumbnail));


            // Update the new properties
            media.Thumbnail = thumbnail;
            media.VideoId = video.VideoId;


            // Update & save
            unitOfWork.Media.Update(media);
            await unitOfWork.Save();

            return Ok(new
            {
                thumbnail = media.Thumbnail
            });
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


            // Delete thumbnail and image
            string wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string imagesFolder = Path.Combine(wwwroot, "images");

            if (media.Thumbnail != null && media.Thumbnail != string.Empty) System.IO.File.Delete(Path.Combine(imagesFolder, media.Thumbnail));
            if (media.ImageAnySize != null && media.ImageAnySize != string.Empty) System.IO.File.Delete(Path.Combine(imagesFolder, media.ImageAnySize));



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