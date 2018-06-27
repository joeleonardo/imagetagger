using MetadataExtractor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ImageFileSorter
{
    class Program
    {
        public enum VideoFileExtensions
        {
            flv = 1,
            mp4 = 2,
            avi = 3,
            mov = 4,
            webm = 5,
            wmv = 6
        }

        static void Main(string[] args)
        {
            DirectoryInfo imageDirInfoOrganized;
            DirectoryInfo videoDirInfoOrganized;
            string path;
            string targetPath;

            path = @"C:\Users\conta\Downloads\Telegram Desktop";
            targetPath = @"Z:\Porn";

            string[] files = System.IO.Directory.GetFiles(path);

            if (!System.IO.Directory.Exists(String.Format(@"{0}\{1}", new DirectoryInfo(targetPath).FullName, "Organized")))
            {
                imageDirInfoOrganized = System.IO.Directory.CreateDirectory(String.Format(@"{0}\{1}", new DirectoryInfo(targetPath).FullName, "Organized"));                
            }
            else
            {
                imageDirInfoOrganized = new DirectoryInfo(String.Format(@"{0}\{1}", new DirectoryInfo(targetPath).FullName, "Organized"));
            }
            
            if (!System.IO.Directory.Exists(String.Format(@"{0}\{1}", new DirectoryInfo(targetPath).FullName, "Videos")))
            {
                videoDirInfoOrganized = System.IO.Directory.CreateDirectory(String.Format(@"{0}\{1}", new DirectoryInfo(targetPath).FullName, "Videos"));
            }
            else
            {
                videoDirInfoOrganized = new DirectoryInfo(String.Format(@"{0}\{1}", new DirectoryInfo(targetPath).FullName, "Videos"));
            }

            var x = 0;
            foreach (string file in files)
            {
                Console.WriteLine("{1}: Processing file: {0}", file, ++x);

                try
                {
                    var fileInfo = new FileInfo(file);
                    var fileExtension = fileInfo.Extension.Replace(".", "");
                    
                    // Video File Check
                    if (Enum.IsDefined(typeof(VideoFileExtensions), fileExtension))
                    {
                        ProcessVideoFile(fileInfo, videoDirInfoOrganized);
                    }
                    else
                    {
                        // Image File Check
                        using (var stream = new FileStream(file, FileMode.Open))
                        {
                            var filetype = MetadataExtractor.Util.FileTypeDetector.DetectFileType(stream);
                            if (filetype == MetadataExtractor.Util.FileType.Unknown)
                            {
                                throw new Exception(String.Format("{0} is not a supported file type.", filetype.ToString()));
                            }
                        }

                        ProcessImageFile(fileInfo, imageDirInfoOrganized);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("There was an error opening file: {0}", file);
                    Console.WriteLine("{0}", e.Message);
                    continue;
                }                                        
            }

            Console.WriteLine("Finished processing {0} files.", x);
            Console.ReadKey();
        }

        private static void ProcessImageFile(FileInfo fileInfo, DirectoryInfo imageDirInfoOrganized)
        {
            var file = fileInfo.FullName;

            IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(file);

            var provider = CultureInfo.InvariantCulture;
            var filetags = directories.Single(t => t.Name == "File");
            var filedatetimetag = filetags.Tags.Single(t => t.Name == "File Modified Date");

            try
            {
                var filedatetime = DateTime.ParseExact(filedatetimetag.Description, "ddd MMM dd HH:mm:ss zzz yyyy", provider);
                var year = filedatetime.Year;
                var month = filedatetime.Month;
                DirectoryInfo targetDir;

                if (!System.IO.Directory.Exists(String.Format(@"{0}\{1}\{2}", imageDirInfoOrganized.FullName, year, month)))
                {
                    targetDir = System.IO.Directory.CreateDirectory(String.Format(@"{0}\{1}\{2}", imageDirInfoOrganized.FullName, year, month));
                }
                else
                {
                    targetDir = new System.IO.DirectoryInfo(String.Format(@"{0}\{1}\{2}", imageDirInfoOrganized.FullName, year, month));
                }

                var targetFile = new FileInfo(file);
                targetFile.MoveTo(String.Format(@"{0}\{1}", targetDir.FullName, targetFile.Name));
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error copying the file: {0}", file);
                Console.WriteLine("{0}", e.Message);
            }
        }

        private static void ProcessVideoFile(FileInfo fileInfo, DirectoryInfo videoDirInfoOrganized)
        {
            try
            {
                fileInfo.MoveTo(String.Format(@"{0}\{1}", videoDirInfoOrganized.FullName, fileInfo.Name));
            }
            catch
            {
                throw;
            }
        }
    }
}
