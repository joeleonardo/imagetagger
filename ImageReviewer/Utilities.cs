using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace ImageReviewer
{
    public class Utilities
    {
        private SchemaContainer dbContext;

        public Utilities()
        {
            this.dbContext = new SchemaContainer();
        }

        public enum LoggingLevels
        {
            Information = 0,
            Debug = 1,
            Error = 2
        }

        internal void Log(string message, LoggingLevels level)
        {
            using (var log = new LoggerConfiguration()
                                    .MinimumLevel.Information()
                                    .WriteTo.Console()
                                    .WriteTo.File("log.txt",
                                        rollingInterval: RollingInterval.Day,
                                        rollOnFileSizeLimit: true)
                                    .CreateLogger())
            {
                switch (level)
                {
                    case LoggingLevels.Debug:
                        log.Debug(message);
                        break;
                    case LoggingLevels.Error:
                        log.Error(message);
                        break;
                    case LoggingLevels.Information:
                        log.Information(message);
                        break;
                }
            }

        }

        internal void CheckDatabaseForFile(string name, string selectedPath)
        {
            Image image;
            try
            {
                image = dbContext.Image.Single(t => t.FileName == name);
            }
            catch
            {
                Image newImage = new Image();
                newImage.FileName = name;
                newImage.FullPath = String.Format(@"{0}\{1}", selectedPath, name);

                dbContext.Image.Add(newImage);
                dbContext.SaveChanges();
            }
        }

        internal IEnumerable<FileInfo> GetFileList(string selectedPath)
        {
            var dir = new System.IO.DirectoryInfo(selectedPath);
            return dir.EnumerateFiles();
        }

        internal int CreateNewTag(string text)
        {
            Tag tag = new Tag { TagName = text };
            dbContext.Tag.Add(tag);
            dbContext.SaveChanges();

            this.Log(String.Format("Created tag #{0}: {1}", tag.Id, text), Utilities.LoggingLevels.Information);

            return tag.Id;
        }

        internal int GetExistingImageId(string text)
        {
            try
            {
                var image = dbContext.Image.Single(t => t.FullPath == text);
                this.Log(String.Format("Loaded image ID #{0}: {1}", image.Id, image.FileName), Utilities.LoggingLevels.Information);
                return image.Id;
            }
            catch (Exception e)
            {
                this.Log(String.Format("No image found: {0}", text), Utilities.LoggingLevels.Error);
                throw new Exception(String.Format("No image found: {0}", text), e);
            }
        }

        internal bool AddImageTag(int tagId, int imageId)
        {
            Image image; 
            Tag tag; 

            try
            {
                image = dbContext.Image.Single(t => t.Id == imageId);
                tag = dbContext.Tag.Single(t => t.Id == tagId);

                if (image.ImageTags.Select(t => t.Tag.TagName).Contains(tag.TagName) && image.ImageTags.Select(t => t.Image.FileName).Contains(image.FileName))
                {
                    throw new Exception("The image tag already exists.");
                }
                else
                {
                    image.ImageTags.Add(new ImageTag { Image = image, Tag = tag });

                    dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                this.Log(e.Message, LoggingLevels.Error);
                return false;
            }
        }

        internal List<Tag> GetTags()
        {
            return dbContext.Tag.OrderBy(t => t.TagName).ToList();
        }

        internal List<Tag> GetTags(int imageID)
        {
            List<ImageTag> imageTags = dbContext.ImageTag.Where(t => t.Image.Id == imageID).ToList();
            List<Tag> tags = new List<Tag>();

            foreach (var imageTag in imageTags)
            {
                tags.Add(imageTag.Tag);
            }

            return tags;
        }

        internal bool CheckTagExists(string text)
        {
            return dbContext.Tag.Select(t => t.TagName).Contains(text);
        }

        internal int GetExistingTagId(string tagName)
        {
            try
            {
                return dbContext.Tag.Single(t => t.TagName == tagName).Id;
            }
            catch (Exception e)
            {
                Log(String.Format("The tag {0} does not exist in the system.", tagName), LoggingLevels.Error);
                Log(e.Message, LoggingLevels.Error);
                return -1;
            }
        }

        internal bool RemoveImageTag(int tagId, int imageId)
        {
            try
            {
                dbContext.ImageTag.Remove(dbContext.ImageTag.Single(t => t.Image.Id == imageId && t.Tag.Id == tagId));
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                this.Log(e.Message, LoggingLevels.Error);
                return false;
            }
        }

        internal string GetFirstTagText(string text)
        {
            try
            {
                return dbContext.Tag.First(t => t.TagName.StartsWith(text)).TagName;
            }
            catch
            {
                return String.Empty;
            }
        }

        internal List<string> GetMatchingFileList(ItemCollection items, ItemCollection ignoreitems)
        {
            IEnumerable<string> set = new List<string>();           

            foreach (string item in items)
            {
                var tag = dbContext.Tag.Single(x => x.TagName == item);
                var results = dbContext.ImageTag.Where(x => x.Tag.Id == tag.Id).Select(y => y.Image.FullPath).ToList();

                if (set.Count() == 0)
                {
                    set = results;
                }
                else
                {
                    set = results.Intersect(set).ToList();
                }
            }

            if (ignoreitems.Count > 0)
            {
                foreach (string item in ignoreitems)
                {
                    var tag = dbContext.Tag.Single(x => x.TagName == item);
                    var results = dbContext.ImageTag.Where(x => x.Tag.Id == tag.Id).Select(y => y.Image.FullPath).ToList();

                    if (results.Count > 0)
                    {
                        foreach (var result in results)
                        {
                            if (set.Contains(result))
                            {
                                ((List<string>)set).Remove(result);
                            }
                        }
                    }
                }
            }

            return set.ToList();
        }

        internal void DeleteExistingTag(int tagId)
        {
            var imageTags = dbContext.ImageTag.Where(t => t.Tag.Id == tagId);
            
            foreach (var imageTag in imageTags)
            {
                dbContext.ImageTag.Remove(imageTag);
            }

            dbContext.Tag.Remove(dbContext.Tag.Single(t => t.Id == tagId));
            dbContext.SaveChanges();
        }
    }
}
