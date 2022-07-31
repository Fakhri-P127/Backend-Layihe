using Backend_MVC_Layihe.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Utilities
{
    public static class FileValidator
    {
            public static async Task<string> FileCreate(this IFormFile file, string root, string folder)
            {
              
               string filename = string.Concat(Guid.NewGuid(), file.FileName);

                string path = Path.Combine(root, folder);
                string filePath = Path.Combine(path, filename);

                try
                {
                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                catch (Exception)
                {
                    throw new FileLoadException();
                }

                return filename;
            }

            public static void FileDelete(string root, string folder, string image)
            {
                string filePath = Path.Combine(root, folder, image);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            public static bool IsImageOkay(this IFormFile file, int mb)
            {
                return file.Length / 1024 / 1024 < mb && file.ContentType.Contains("image/");
            }
        }

    }

