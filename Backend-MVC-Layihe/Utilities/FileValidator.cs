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
            public static async Task<string> FileCreate(this IFormFile file, string root, string folder, DbSet<Slider> sliders = null, DbSet<ClothesImage> cImages = null)
            {
                string filename = string.Empty;
                //Bele ishleyir amma baglamamin sebebi eyni papkadan image goturende gedib override edir ve silende o shekil hemishelik gedir.

                //if (sliders != null)
                //{
                //    bool isSame = sliders.Any(s => s.Image == file.FileName);
                //    if (isSame)
                //    {
                //        filename = string.Concat(Guid.NewGuid(), file.FileName);
                //    }
                //    else
                //    {
                //        filename = file.FileName;
                //    }
                //}

                //if (cImages != null)
                //{
                //    bool isSame = cImages.Any(c => c.Name == file.FileName);
                //    if (isSame)
                //    {
                //        filename = string.Concat(Guid.NewGuid(), file.FileName);
                //    }
                //    else
                //    {
                //        filename = file.FileName;
                //    }
                //}
                filename = string.Concat(Guid.NewGuid(), file.FileName);

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

