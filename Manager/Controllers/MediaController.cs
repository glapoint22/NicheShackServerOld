using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DataAccess.Models;
using DataAccess.ViewModels;
using Manager.Classes;
using Manager.Repositories;
using Manager.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Services.Classes;
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



        // ------------------------------------------------------------------------ New Image ------------------------------------------------------------------------
        [HttpPost, DisableRequestSizeLimit]
        [Route("Image")]
        public async Task<ActionResult> NewImage()
        {
            // Get the new image
            IFormFile imageFile = Request.Form.Files["image"];
            System.Drawing.Image image = await GetImageFromFile(imageFile);


            // Get the image name
            StringValues imageName;
            Request.Form.TryGetValue("name", out imageName);



            // Create a new media object
            Media media = new Media
            {
                Name = imageName.ToString().Trim(),
                MediaType = (int)MediaType.Image
            };



            // Get the image size
            StringValues imageSizeString;
            Request.Form.TryGetValue("imageSize", out imageSizeString);
            ImageSizeType imageSize = (ImageSizeType)int.Parse(imageSizeString);

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






        // ------------------------------------------------------------------------ Update Image ------------------------------------------------------------------------
        [HttpPost, DisableRequestSizeLimit]
        [Route("UpdateImage")]
        public async Task<ActionResult> UpdateImage()
        {
            string imageSrc = string.Empty;

            // Get the image
            IFormFile imageFile = Request.Form.Files["image"];
            System.Drawing.Image updatedImage = await GetImageFromFile(imageFile);


            // Get the Image id
            StringValues idValue;
            Request.Form.TryGetValue("id", out idValue);
            int id = Convert.ToInt32(idValue);



            // Get the image size
            StringValues imageSizeString;
            Request.Form.TryGetValue("imageSize", out imageSizeString);
            ImageSizeType imageSize = (ImageSizeType)int.Parse(imageSizeString);



            // Get media
            Media media = await unitOfWork.Media.Get(id);

            List<ImageSizeType> imageSizes = new List<ImageSizeType>();

            imageSizes.Add(imageSize);



            string wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string imagesFolder = Path.Combine(wwwroot, "images");

            string image;

            // Reset thumbnail
            if (media.Thumbnail != null)
            {
                imageSizes.Add(ImageSizeType.Thumbnail);

                image = Path.Combine(imagesFolder, media.Thumbnail);
                System.IO.File.Delete(image);

                media.Thumbnail = null;
                media.ThumbnailWidth = 0;
                media.ThumbnailHeight = 0;
            }


            // Reset small
            if (media.ImageSm != null)
            {
                imageSizes.Add(ImageSizeType.Small);

                image = Path.Combine(imagesFolder, media.ImageSm);
                System.IO.File.Delete(image);

                media.ImageSm = null;
                media.ImageSmWidth = 0;
                media.ImageSmHeight = 0;
            }



            // Reset medium
            if (media.ImageMd != null)
            {
                imageSizes.Add(ImageSizeType.Medium);

                image = Path.Combine(imagesFolder, media.ImageMd);
                System.IO.File.Delete(image);

                media.ImageMd = null;
                media.ImageMdWidth = 0;
                media.ImageMdHeight = 0;
            }


            // Reset large
            if (media.ImageLg != null)
            {
                imageSizes.Add(ImageSizeType.Large);

                image = Path.Combine(imagesFolder, media.ImageLg);
                System.IO.File.Delete(image);

                media.ImageLg = null;
                media.ImageLgWidth = 0;
                media.ImageLgHeight = 0;
            }





            // Reset any size
            if (media.ImageAnySize != null)
            {
                imageSizes.Add(ImageSizeType.AnySize);

                image = Path.Combine(imagesFolder, media.ImageAnySize);
                System.IO.File.Delete(image);

                media.ImageAnySize = null;
                media.ImageAnySizeWidth = 0;
                media.ImageAnySizeHeight = 0;
            }


            imageSizes = imageSizes.Distinct().OrderBy(x => x).ToList();

            foreach (ImageSizeType imageSizeType in imageSizes)
            {
                SetImageSizes(imageSizeType, updatedImage, media);
            }



            // Set the image src
            if (imageSize == ImageSizeType.AnySize)
            {
                imageSrc = media.ImageAnySize;
            }
            else if (imageSize == ImageSizeType.Small)
            {
                imageSrc = media.ImageSm;
            }
            else if (imageSize == ImageSizeType.Medium)
            {
                imageSrc = media.ImageMd;
            }

            // Update and save
            unitOfWork.Media.Update(media);
            await unitOfWork.Save();

            return Ok(new { src = imageSrc });
        }




        // ------------------------------------------------------------------------ Add Image Size ------------------------------------------------------------------------
        [HttpGet]
        [Route("Image")]
        public async Task<ActionResult> AddImageSize(int imageId, ImageSizeType imageSizeType, string src)
        {
            Media media = await unitOfWork.Media.Get(imageId);
            string wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string imagesFolder = Path.Combine(wwwroot, "images");
            string imagePath = Path.Combine(imagesFolder, src);
            System.Drawing.Image image = System.Drawing.Image.FromFile(imagePath);

            // Set the image sizes for this image
            string imageSrc = SetImageSizes(imageSizeType, image, media);

            // Update and save
            unitOfWork.Media.Update(media);
            await unitOfWork.Save();

            return Ok(new { src = imageSrc });
        }










        // --------------------------------------------------------------------------- New Video -------------------------------------------------------------------------
        [HttpPost]
        [Route("Video")]
        public async Task<ActionResult> NewVideo(MediaViewModel video)
        {
            var thumbnail = await GetVideoThumbnail(video);

            if (thumbnail == null) return Ok();

            Media newVideo = new Media
            {
                Name = video.Name.Trim(),
                VideoId = video.VideoId,
                MediaType = (int)MediaType.Video,
                VideoType = video.VideoType,
                Thumbnail = thumbnail
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



        public async Task<Item> TempNewVideo(MediaViewModel video)
        {
            var thumbnail = await GetVideoThumbnail(video);

            if (thumbnail == null) return null;

            Media newVideo = new Media
            {
                Name = video.Name.Trim(),
                VideoId = video.VideoId,
                MediaType = (int)MediaType.Video,
                VideoType = video.VideoType,
                Thumbnail = thumbnail
            };

            // Add the new video
            unitOfWork.Media.Add(newVideo);
            await unitOfWork.Save();


            return new Item
            {
                Id = newVideo.Id
            };
        }






        // --------------------------------------------------------------------------- Update Video -------------------------------------------------------------------------
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





        // --------------------------------------------------------------------------- Update Media Name -------------------------------------------------------------------------
        [HttpPut]
        [Route("Name")]
        public async Task<ActionResult> UpdateMediaName(ItemViewModel media)
        {
            Media updatedMedia = await unitOfWork.Media.Get(media.Id);

            updatedMedia.Name = media.Name.Trim();

            // Update and save
            unitOfWork.Media.Update(updatedMedia);
            await unitOfWork.Save();

            return Ok();
        }











        // --------------------------------------------------------------------------- Delete Image -------------------------------------------------------------------------
        [HttpDelete]
        public async Task<ActionResult> DeleteImage(int id)
        {

            Media media = await unitOfWork.Media.Get(id);
            string wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string imagesFolder = Path.Combine(wwwroot, "images");
            string image;

            // Remove thumbnail
            if (media.Thumbnail != null)
            {
                image = Path.Combine(imagesFolder, media.Thumbnail);
                System.IO.File.Delete(image);
            }


            // Remove small
            if (media.ImageSm != null)
            {
                image = Path.Combine(imagesFolder, media.ImageSm);
                System.IO.File.Delete(image);
            }



            // Remove medium
            if (media.ImageMd != null)
            {
                image = Path.Combine(imagesFolder, media.ImageMd);
                System.IO.File.Delete(image);
            }


            // Remove large
            if (media.ImageLg != null)
            {
                image = Path.Combine(imagesFolder, media.ImageLg);
                System.IO.File.Delete(image);
            }



            // Remove any size
            if (media.ImageAnySize != null)
            {
                image = Path.Combine(imagesFolder, media.ImageAnySize);
                System.IO.File.Delete(image);
            }



            unitOfWork.Media.Remove(media);
            await unitOfWork.Save();

            return Ok();
        }










        // ------------------------------------------------------------------------------ Search ----------------------------------------------------------------------------
        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult> SearchMedia(int type, string searchWords)
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
                ImageAnySizeHeight = x.ImageAnySizeHeight,
                VideoId = x.VideoId,
                VideoType = x.VideoType
            }));
        }








        // ------------------------------------------------------------------------ Get Image FromFile --------------------------------------------------------------------------
        private async Task<System.Drawing.Image> GetImageFromFile(IFormFile imageFile)
        {
            System.Drawing.Image image;

            using (var memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                using (var img = System.Drawing.Image.FromStream(memoryStream))
                {
                    image = (System.Drawing.Image)img.Clone();
                }
            }

            return image;
        }












        // ----------------------------------------------------------------------------- Scale Image -----------------------------------------------------------------------------
        private ImageSize ScaleImage(System.Drawing.Image image, float targetSize)
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

            return new ImageSize
            {
                Src = imageFile,
                Width = scaledWidth,
                Height = scaledHeight
            };
        }













        // ------------------------------------------------------------------------- Set Large Image Size --------------------------------------------------------------------------
        private void SetLargeImageSize(System.Drawing.Image image, Media media)
        {
            const float large = (float)ImageSizeType.Large;
            const float medium = (float)ImageSizeType.Medium;

            ImageSize imageSize = GetImageSize((int)ImageSizeType.Large, media);

            if (imageSize != null)
            {
                media.ImageLg = imageSize.Src;
                media.ImageLgWidth = imageSize.Width;
                media.ImageLgHeight = imageSize.Height;
            }
            else
            {
                if (image.Width > large || image.Height > large)
                {
                    imageSize = ScaleImage(image, large);
                    media.ImageLg = imageSize.Src;
                    media.ImageLgWidth = imageSize.Width;
                    media.ImageLgHeight = imageSize.Height;
                }
                else if (image.Width == large || image.Height == large || image.Width > medium || image.Height > medium)
                {
                    imageSize = GetImageSize(Math.Max(image.Width, image.Height), media);

                    if (imageSize != null)
                    {
                        media.ImageLg = imageSize.Src;
                        media.ImageLgWidth = imageSize.Width;
                        media.ImageLgHeight = imageSize.Height;
                    }
                    else
                    {
                        media.ImageLg = CopyImage(image);
                        media.ImageLgWidth = image.Width;
                        media.ImageLgHeight = image.Height;
                    }
                }
            }
        }













        // ------------------------------------------------------------------------- Set Medium Image Size --------------------------------------------------------------------------
        private string SetMediumImageSize(System.Drawing.Image image, Media media)
        {
            const float medium = (float)ImageSizeType.Medium;
            string imageSrc;

            ImageSize imageSize = GetImageSize((int)ImageSizeType.Medium, media);

            if (imageSize != null)
            {
                media.ImageMd = imageSrc = imageSize.Src;
                media.ImageMdWidth = imageSize.Width;
                media.ImageMdHeight = imageSize.Height;
            }
            else
            {
                if (image.Width > medium || image.Height > medium)
                {
                    imageSize = ScaleImage(image, medium);
                    media.ImageMd = imageSrc = imageSize.Src;
                    media.ImageMdWidth = imageSize.Width;
                    media.ImageMdHeight = imageSize.Height;
                }
                else
                {
                    imageSize = GetImageSize(Math.Max(image.Width, image.Height), media);

                    if (imageSize != null)
                    {
                        media.ImageMd = imageSrc = imageSize.Src;
                        media.ImageMdWidth = imageSize.Width;
                        media.ImageMdHeight = imageSize.Height;
                    }
                    else
                    {
                        media.ImageMd = imageSrc = CopyImage(image);
                        media.ImageMdWidth = image.Width;
                        media.ImageMdHeight = image.Height;
                    }
                }
            }

            if (media.ImageLg == null)
            {
                media.ImageLg = media.ImageMd;
                media.ImageLgWidth = media.ImageMdWidth;
                media.ImageLgHeight = media.ImageMdHeight;
            }

            return imageSrc;
        }










        // ------------------------------------------------------------------------- Set Small Image Size --------------------------------------------------------------------------
        private string SetSmallImageSize(System.Drawing.Image image, Media media)
        {
            const float small = (float)ImageSizeType.Small;
            string imageSrc;

            ImageSize imageSize = GetImageSize((int)ImageSizeType.Small, media);

            if (imageSize != null)
            {
                media.ImageSm = imageSrc = imageSize.Src;
                media.ImageSmWidth = imageSize.Width;
                media.ImageSmHeight = imageSize.Height;
            }
            else
            {
                if (image.Width > small || image.Height > small)
                {
                    imageSize = ScaleImage(image, small);
                    media.ImageSm = imageSrc = imageSize.Src;
                    media.ImageSmWidth = imageSize.Width;
                    media.ImageSmHeight = imageSize.Height;
                }
                else
                {
                    imageSize = GetImageSize(Math.Max(image.Width, image.Height), media);

                    if (imageSize != null)
                    {
                        media.ImageSm = imageSrc = imageSize.Src;
                        media.ImageSmWidth = imageSize.Width;
                        media.ImageSmHeight = imageSize.Height;
                    }
                    else
                    {
                        media.ImageSm = imageSrc = CopyImage(image);
                        media.ImageSmWidth = image.Width;
                        media.ImageSmHeight = image.Height;
                    }
                }
            }

            return imageSrc;
        }










        // ------------------------------------------------------------------------- Set Thumbnail Size --------------------------------------------------------------------------
        private void SetThumbnailSize(System.Drawing.Image image, Media media)
        {
            const float thumbnail = (float)ImageSizeType.Thumbnail;

            ImageSize imageSize = GetImageSize((int)ImageSizeType.Thumbnail, media);

            if (imageSize != null)
            {
                media.Thumbnail = imageSize.Src;
                media.ThumbnailWidth = imageSize.Width;
                media.ThumbnailHeight = imageSize.Height;
            }
            else
            {
                imageSize = ScaleImage(image, thumbnail);
                media.Thumbnail = imageSize.Src;
                media.ThumbnailWidth = imageSize.Width;
                media.ThumbnailHeight = imageSize.Height;
            }
        }











        // ------------------------------------------------------------------------- Set Image Any Size --------------------------------------------------------------------------
        private string SetImageAnySize(System.Drawing.Image image, Media media)
        {
            string imageSrc;

            ImageSize imageSize = GetImageSize(Math.Max(image.Width, image.Height), media);

            if (imageSize != null)
            {
                media.ImageAnySize = imageSrc = imageSize.Src;
                media.ImageAnySizeWidth = imageSize.Width;
                media.ImageAnySizeHeight = imageSize.Height;
            }
            else
            {
                imageSize = GetImageSize(Math.Max(image.Width, image.Height), media);

                if (imageSize != null)
                {
                    media.ImageAnySize = imageSrc = imageSize.Src;
                    media.ImageAnySizeWidth = imageSize.Width;
                    media.ImageAnySizeHeight = imageSize.Height;
                }
                else
                {
                    media.ImageAnySize = imageSrc = CopyImage(image);
                    media.ImageAnySizeWidth = image.Width;
                    media.ImageAnySizeHeight = image.Height;
                }
            }

            return imageSrc;
        }










        // ------------------------------------------------------------------------- Get Image Size --------------------------------------------------------------------------
        private ImageSize GetImageSize(int imageSize, Media media)
        {
            int maxSize;

            // Thumbnail
            maxSize = Math.Max(media.ThumbnailWidth, media.ThumbnailHeight);

            if (imageSize == maxSize)
            {
                return new ImageSize
                {
                    Src = media.Thumbnail,
                    Width = media.ThumbnailWidth,
                    Height = media.ThumbnailHeight
                };
            }



            // Small
            maxSize = Math.Max(media.ImageSmWidth, media.ImageSmHeight);

            if (imageSize == maxSize)
            {
                return new ImageSize
                {
                    Src = media.ImageSm,
                    Width = media.ImageSmWidth,
                    Height = media.ImageSmHeight
                };
            }



            // Medium
            maxSize = Math.Max(media.ImageMdWidth, media.ImageMdHeight);

            if (imageSize == maxSize)
            {
                return new ImageSize
                {
                    Src = media.ImageMd,
                    Width = media.ImageMdWidth,
                    Height = media.ImageMdHeight
                };
            }



            // Large
            maxSize = Math.Max(media.ImageLgWidth, media.ImageLgHeight);

            if (imageSize == maxSize)
            {
                return new ImageSize
                {
                    Src = media.ImageLg,
                    Width = media.ImageLgWidth,
                    Height = media.ImageLgHeight
                };
            }




            // Any Size
            maxSize = Math.Max(media.ImageAnySizeWidth, media.ImageAnySizeHeight);

            if (imageSize == maxSize)
            {
                return new ImageSize
                {
                    Src = media.ImageAnySize,
                    Width = media.ImageAnySizeWidth,
                    Height = media.ImageAnySizeHeight
                };
            }


            return null;
        }










        // ------------------------------------------------------------------------- Set Image Sizes --------------------------------------------------------------------------
        private string SetImageSizes(ImageSizeType imageSizeType, System.Drawing.Image image, Media media)
        {
            string imageSrc = string.Empty;

            // Medium
            if (imageSizeType == ImageSizeType.Medium)
            {
                // Set the large image size
                if (media.ImageLg == null)
                {
                    SetLargeImageSize(image, media);
                }



                // Set the medium image
                if (media.ImageMd == null)
                {
                    imageSrc = SetMediumImageSize(image, media);
                }





                // Set the small image
                if (media.ImageSm == null)
                {
                    if (image.Width > (int)ImageSizeType.Small || image.Height > (int)ImageSizeType.Small)
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
                    if (image.Width > (int)ImageSizeType.Thumbnail || image.Height > (int)ImageSizeType.Thumbnail)
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
            else if (imageSizeType == ImageSizeType.Small)
            {
                // Set the small image
                if (media.ImageSm == null)
                {
                    imageSrc = SetSmallImageSize(image, media);
                }



                // Set the thumbnail
                if (media.Thumbnail == null)
                {
                    if (image.Width > (int)ImageSizeType.Thumbnail || image.Height > (int)ImageSizeType.Thumbnail)
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
            else if (imageSizeType == ImageSizeType.AnySize)
            {
                // Set the any size image
                imageSrc = SetImageAnySize(image, media);

                // Set the thumbnail
                if (media.Thumbnail == null)
                {
                    if (image.Width > (int)ImageSizeType.Thumbnail || image.Height > (int)ImageSizeType.Thumbnail)
                    {
                        SetThumbnailSize(image, media);
                    }
                    else
                    {
                        media.Thumbnail = media.ImageAnySize;
                        media.ThumbnailWidth = media.ImageAnySizeWidth;
                        media.ThumbnailHeight = media.ImageAnySizeHeight;
                    }
                }
            }

            return imageSrc;
        }











        // ------------------------------------------------------------------------- Copy Image --------------------------------------------------------------------------
        private string CopyImage(System.Drawing.Image image)
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














        // ------------------------------------------------------------------------- Get Video Thumbnail --------------------------------------------------------------------------
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


            if (thumbnailUrl != string.Empty && thumbnailUrl != null)
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