using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.DTOs.Files
{
    public record FileUpload(Stream Content, string FileName, string ContentType, long Length);

}
