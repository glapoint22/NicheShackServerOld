using System;
using System.IO;
using System.Linq;
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
        [Route("NewImage")]
        public async Task<ActionResult> NewImage()
        {
            StringValues typeValue;

            // Get the new image
            IFormFile imageFile = Request.Form.Files["image"];

            
            // Get the media type
            Request.Form.TryGetValue("type", out typeValue);
            int type = Convert.ToInt32(typeValue);

            // This will get the file extension
            Regex regex = new Regex(@"\.(jpg|jpeg|gif|png|bmp|tiff|tga|svg|webp)$", RegexOptions.IgnoreCase);
            Match match = regex.Match(imageFile.FileName);
            string fileExtension = match.Value;


            // Create a new unique name for the image
            string imageName = Guid.NewGuid().ToString("N") + fileExtension;

            // Place the new image into the images folder
            string imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "images");
            string filePath = Path.Combine(imagesFolder, imageName);
            await imageFile.CopyToAsync(new FileStream(filePath, FileMode.Create));


            // Add the new image to the database
            Media media = new Media
            {
                Name = "",
                Url = imageName,
                Type = type
            };

            unitOfWork.Media.Add(media);

            await unitOfWork.Save();

            return Ok(new { 
                id = media.Id,
                url = media.Url
            });
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




        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult> Search(int type, string searchWords)
        {
            return Ok(await unitOfWork.Media.GetCollection<MediaViewModel>(x => x.Type == type, searchWords));
        }
    }
}