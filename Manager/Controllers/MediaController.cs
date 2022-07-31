using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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



        private async Task<Image> GetImageFromFile(IFormFile imageFile)
        {
            Image image;

            using (var memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                using (var img = Image.FromStream(memoryStream))
                {
                    image = (Image)img.Clone();
                }
            }

            return image;
        }



        [HttpPost, DisableRequestSizeLimit]
        [Route("Image")]
        public async Task<ActionResult> NewImage()
        {
            // Get the new image
            IFormFile imageFile = Request.Form.Files["image"];
            Image image = await GetImageFromFile(imageFile);


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
            string imageSrc = SetImageSizes(imageSize, image, media);



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




        private ScaledImage ScaleImage(Image image, float targetSize)
        {
            float maxSize = Math.Max(image.Width, image.Height);
            float multiplier = targetSize / maxSize;
            int scaledWidth = (int)Math.Round(image.Width * multiplier);
            int scaledHeight = (int)Math.Round(image.Height * multiplier);

            //Scale
            Bitmap scaledBitmap = new Bitmap(scaledWidth, scaledHeight);
            Graphics graphics = Graphics.FromImage(scaledBitmap);
            graphics.InterpolationMode = InterpolationMode.High;
            graphics.DrawImage(image, 0, 0, scaledWidth, scaledHeight);

            // Get the path
            string wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string imagesFolder = Path.Combine(wwwroot, "images");

            //Create the new image
            string ext = ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.FormatID == image.RawFormat.Guid).FilenameExtension.Split(";").First().Trim('*').ToLower();
            string imageFile = Guid.NewGuid().ToString("N") + ext;
            string newImage = Path.Combine(imagesFolder, imageFile);
            scaledBitmap.Save(newImage);

            return new ScaledImage
            {
                Src = imageFile,
                Width = scaledWidth,
                Height = scaledHeight
            };
        }


        private void SetLargeImageSize(Image image, Media media)
        {
            const float large = (float)ImageSize.Large;
            const float medium = (float)ImageSize.Medium;
            float width = image.Width;
            float height = image.Height;

            if (width > large || height > large)
            {
                ScaledImage scaledImage = ScaleImage(image, large);

                media.ImageLg = scaledImage.Src;
                media.ImageLgWidth = scaledImage.Width;
                media.ImageLgHeight = scaledImage.Height;
            }
            else if (width == large || height == large || width > medium || height > medium)
            {
                if (media.ImageAnySize == null || (media.ImageAnySizeWidth != width && media.ImageAnySizeHeight != height))
                {
                    media.ImageLg = CopyImage(image);
                    media.ImageLgWidth = (int)width;
                    media.ImageLgHeight = (int)height;
                }
                else
                {
                    media.ImageLg = media.ImageAnySize;
                    media.ImageLgWidth = media.ImageAnySizeWidth;
                    media.ImageLgHeight = media.ImageAnySizeHeight;
                }

            }
        }


        private string SetMediumImageSize(Image image, Media media)
        {
            float width = image.Width;
            float height = image.Height;
            const float medium = (float)ImageSize.Medium;
            string imageSrc;

            if (width > medium || height > medium)
            {
                ScaledImage scaledImage = ScaleImage(image, medium);

                media.ImageMd = imageSrc = scaledImage.Src;
                media.ImageMdWidth = scaledImage.Width;
                media.ImageMdHeight = scaledImage.Height;
            }
            else
            {
                media.ImageMd = imageSrc = CopyImage(image);
                media.ImageMdWidth = (int)width;
                media.ImageMdHeight = (int)height;
            }

            return imageSrc;
        }



        private string SetSmallImageSize(Image image, Media media)
        {
            float width = image.Width;
            float height = image.Height;
            const float small = (float)ImageSize.Small;
            string imageSrc;

            if (width > small || height > small)
            {
                ScaledImage scaledImage = ScaleImage(image, small);

                media.ImageSm = imageSrc = scaledImage.Src;
                media.ImageSmWidth = scaledImage.Width;
                media.ImageSmHeight = scaledImage.Height;
            }
            else
            {
                media.ImageSm = imageSrc = CopyImage(image);
                media.ImageSmWidth = (int)width;
                media.ImageSmHeight = (int)height;
            }

            return imageSrc;
        }




        private void SetThumbnailSize(Image image, Media media)
        {
            float width = image.Width;
            float height = image.Height;
            const float thumbnail = (float)ImageSize.Thumbnail;

            if (width > thumbnail || height > thumbnail)
            {
                ScaledImage scaledImage = ScaleImage(image, thumbnail);

                media.Thumbnail = scaledImage.Src;
                media.ThumbnailWidth = scaledImage.Width;
                media.ThumbnailHeight = scaledImage.Height;
            }
            else
            {
                media.Thumbnail = CopyImage(image);
                media.ThumbnailWidth = (int)width;
                media.ThumbnailHeight = (int)height;
            }
        }




        private string SetImageAnySize(Image image, Media media)
        {
            float width = image.Width;
            float height = image.Height;
            string imageSrc;

            media.ImageAnySize = imageSrc = CopyImage(image);
            media.ImageAnySizeWidth = (int)width;
            media.ImageAnySizeHeight = (int)height;

            return imageSrc;
        }


        private string SetImageSizes(ImageSize imageSize, Image image, Media media)
        {
            string imageSrc = string.Empty;
            int imageAnySizeMax = Math.Max(media.ImageAnySizeWidth, media.ImageAnySizeHeight);

            // Medium
            if (imageSize == ImageSize.Medium)
            {


                // Set the large image size
                if (media.ImageAnySize != null && imageAnySizeMax == (int)ImageSize.Large)
                {
                    media.ImageLg = media.ImageAnySize;
                    media.ImageLgWidth = media.ImageAnySizeWidth;
                    media.ImageLgHeight = media.ImageAnySizeHeight;
                }
                else
                {
                    SetLargeImageSize(image, media);
                }




                // Set the medium image
                if (media.ImageMd == null)
                {
                    if (media.ImageAnySize != null && imageAnySizeMax == (int)ImageSize.Medium)
                    {
                        media.ImageMd = imageSrc = media.ImageAnySize;
                        media.ImageMdWidth = media.ImageAnySizeWidth;
                        media.ImageMdHeight = media.ImageAnySizeHeight;
                    }
                    else
                    {
                        imageSrc = SetMediumImageSize(image, media);
                    }

                    if (media.ImageLg == null)
                    {
                        media.ImageLg = media.ImageMd;
                        media.ImageLgWidth = media.ImageMdWidth;
                        media.ImageLgHeight = media.ImageMdHeight;
                    }
                }




                // Set the small image
                if (media.ImageSm == null)
                {
                    if (image.Width > (int)ImageSize.Small || image.Height > (int)ImageSize.Small)
                    {
                        SetSmallImageSize(image, media);
                    }
                    else
                    {
                        media.ImageSm = media.ImageMd;
                        media.ImageSmWidth = media.ImageMdWidth;
                        media.ImageSmHeight = media.ImageMdHeight;
                    }
                }


                // Set the thumbnail
                if (media.Thumbnail == null)
                {
                    if (image.Width > (int)ImageSize.Thumbnail || image.Height > (int)ImageSize.Thumbnail)
                    {
                        SetThumbnailSize(image, media);
                    }
                    else
                    {
                        media.Thumbnail = media.ImageSm;
                        media.ThumbnailWidth = media.ImageSmWidth;
                        media.ThumbnailHeight = media.ImageSmHeight;
                    }
                }

            }

            // Small
            else if (imageSize == ImageSize.Small)
            {
                // Set the small image
                if (media.ImageSm == null)
                {
                    if (media.ImageAnySize != null && imageAnySizeMax == (int)ImageSize.Small)
                    {
                        media.ImageSm = imageSrc = media.ImageAnySize;
                        media.ImageSmWidth = media.ImageAnySizeWidth;
                        media.ImageSmHeight = media.ImageAnySizeHeight;
                    }
                    else
                    {
                        imageSrc = SetSmallImageSize(image, media);
                    }
                }



                // Set the thumbnail
                if (media.Thumbnail == null)
                {
                    if (image.Width > (int)ImageSize.Thumbnail || image.Height > (int)ImageSize.Thumbnail)
                    {
                        SetThumbnailSize(image, media);
                    }
                    else
                    {
                        media.Thumbnail = media.ImageSm;
                        media.ThumbnailWidth = media.ImageSmWidth;
                        media.ThumbnailHeight = media.ImageSmHeight;
                    }
                }
            }


            // Any Size
            else if (imageSize == ImageSize.AnySize)
            {
                if (media.ImageSmWidth == image.Width && media.ImageSmHeight == image.Height)
                {
                    media.ImageAnySize = media.ImageSm;
                    media.ImageAnySizeWidth = media.ImageSmWidth;
                    media.ImageAnySizeHeight = media.ImageSmHeight;
                }
                else
                {
                    imageSrc = SetImageAnySize(image, media);
                }




                // Set the thumbnail
                if (media.Thumbnail == null)
                {
                    if (image.Width > (int)ImageSize.Thumbnail || image.Height > (int)ImageSize.Thumbnail)
                    {
                        SetThumbnailSize(image, media);
                    }
                }




            }

            return imageSrc;
        }




        [HttpGet]
        [Route("Image")]
        public async Task<ActionResult> AddImageSize(int imageId, ImageSize imageSize)
        {
            Media media = await unitOfWork.Media.Get(imageId);
            string src = media.ImageAnySize;
            string wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string imagesFolder = Path.Combine(wwwroot, "images");
            string imagePath = Path.Combine(imagesFolder, src);
            Image image = Image.FromFile(imagePath);

            // Set the image sizes for this image
            string imageSrc = SetImageSizes(imageSize, image, media);

            // Update and save
            unitOfWork.Media.Update(media);
            await unitOfWork.Save();

            return Ok(new { src = imageSrc });
        }




        [HttpPost, DisableRequestSizeLimit]
        [Route("UpdateImage")]
        public async Task<ActionResult> UpdateImage()
        {
            string imageSrc = string.Empty;

            // Get the image
            IFormFile imageFile = Request.Form.Files["image"];
            Image updatedImage = await GetImageFromFile(imageFile);


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



            string wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string imagesFolder = Path.Combine(wwwroot, "images");


            //if (imageSize == ImageSize.Medium || imageSize == ImageSize.Small)
            //{
            //    string image;

            //    // Reset thumbnail
            //    if (media.Thumbnail != null)
            //    {
            //        image = Path.Combine(imagesFolder, media.Thumbnail);
            //        System.IO.File.Delete(image);

            //        media.Thumbnail = null;
            //        media.ThumbnailWidth = 0;
            //        media.ThumbnailHeight = 0;
            //    }


            //    // Reset small
            //    if (media.ImageSm != null)
            //    {
            //        image = Path.Combine(imagesFolder, media.ImageSm);
            //        System.IO.File.Delete(image);

            //        media.ImageSm = null;
            //        media.ImageSmWidth = 0;
            //        media.ImageSmHeight = 0;
            //    }



            //    // Reset medium
            //    if (media.ImageMd != null)
            //    {
            //        image = Path.Combine(imagesFolder, media.ImageMd);
            //        System.IO.File.Delete(image);

            //        media.ImageMd = null;
            //        media.ImageMdWidth = 0;
            //        media.ImageMdHeight = 0;
            //    }


            //    // Reset large
            //    if (media.ImageLg != null)
            //    {
            //        image = Path.Combine(imagesFolder, media.ImageLg);
            //        System.IO.File.Delete(image);

            //        media.ImageLg = null;
            //        media.ImageLgWidth = 0;
            //        media.ImageLgHeight = 0;
            //    }


            //    if (media.ImageAnySize != null)
            //    {
            //        image = Path.Combine(imagesFolder, media.ImageAnySize);
            //        System.IO.File.Delete(image);

            //        int updatedImageMaxSize = Math.Max(updatedImage.Width, updatedImage.Height);
            //        int imageAnySizeMaxSize = Math.Max(media.ImageAnySizeWidth, media.ImageAnySizeHeight);

            //        if (updatedImageMaxSize > imageAnySizeMaxSize)
            //        {
            //            ScaledImage scaledImage = ScaleImage(updatedImage, imageAnySizeMaxSize);

            //            media.ImageAnySize = scaledImage.Src;
            //            media.ImageAnySizeWidth = scaledImage.Width;
            //            media.ImageAnySizeHeight = scaledImage.Height;
            //        }
            //        else
            //        {
            //            SetImageAnySize(updatedImage, media);
            //        }
            //    }

            //    imageSrc = SetImageSizes(imageSize, updatedImage, media);

            //    if (imageSize == ImageSize.Small && (updatedImage.Width >= (int)ImageSize.Medium || updatedImage.Height >= (int)ImageSize.Medium)) // && builder ==  product && location == product || location == media
            //    {
            //        SetImageSizes(ImageSize.Medium, updatedImage, media);
            //    }
            //}






















            //// Reset any size
            //if (media.ImageAnySize != null)
            //{
            //    image = Path.Combine(imagesFolder, media.ImageAnySize);
            //    System.IO.File.Delete(image);

            //    media.ImageAnySize = null;
            //    media.ImageAnySizeWidth = 0;
            //    media.ImageAnySizeHeight = 0;
            //}







            // Update and save
            unitOfWork.Media.Update(media);
            await unitOfWork.Save();

            return Ok(new { src = imageSrc });
        }







        private string CopyImage(Image image)
        {
            // Create a new unique name for the image
            string ext = ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.FormatID == image.RawFormat.Guid).FilenameExtension.Split(";").First().Trim('*').ToLower();
            string imageFile = Guid.NewGuid().ToString("N") + ext;

            // Get the file path
            string wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string imagesFolder = Path.Combine(wwwroot, "images");
            string filePath = Path.Combine(imagesFolder, imageFile);


            // Save the image to the images folder
            Bitmap bitmap = new Bitmap(image);
            bitmap.Save(filePath);

            return imageFile;
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
            return Ok(await unitOfWork.Media.GetCollection(x => x.MediaType == type, searchWords, x => new SearchedMedia
            {
                Id = x.Id,
                Name = x.Name,
                Thumbnail = x.Thumbnail,
                ThumbnailWidth = x.ThumbnailWidth,
                ThumbnailHeight = x.ThumbnailHeight,
                ImageSm = x.ImageSm,
                ImageSmWidth = x.ImageSmWidth,
                ImageSmHeight = x.ImageSmHeight,
                ImageMd = x.ImageMd,
                ImageMdWidth = x.ImageMdWidth,
                ImageMdHeight = x.ImageMdHeight,
                ImageLg = x.ImageLg,
                ImageLgWidth = x.ImageLgWidth,
                ImageLgHeight = x.ImageLgHeight,
                ImageAnySize = x.ImageAnySize,
                ImageAnySizeWidth = x.ImageAnySizeWidth,
                ImageAnySizeHeight = x.ImageAnySizeHeight
            }));
        }




        [HttpPost]
        [Route("ImageReference")]
        public async Task<ActionResult> AddImageReference(ImageReferenceViewModel imageReferenceViewModel)
        {
            ImageReference imageReference = new ImageReference
            {
                ImageId = imageReferenceViewModel.ImageId,
                ImageSize = imageReferenceViewModel.ImageSize,
                Builder = imageReferenceViewModel.Builder,
                Host = imageReferenceViewModel.Host,
                Location = imageReferenceViewModel.Location
            };

            unitOfWork.ImageReferences.Add(imageReference);
            await unitOfWork.Save();

            return Ok();
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