using System;
using System.Collections.Generic;
using System.Text;
using ECommerce.Application.DTOs.Files;

namespace ECommerce.Application.Interfaces
{
    public interface IImageService
    {
      
            Task<string> SaveAsync(FileUpload file, string folder);  
            void Delete(string fileName, string folder);
        
    }
}
