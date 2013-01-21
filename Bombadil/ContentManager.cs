using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bombadil.Core.Models;
using System.IO;
using Newtonsoft.Json;
using MarkdownSharp;

namespace Bombadil.Core.Managers
{

    public enum ContentType
    {
        Post,
        Page
    }

    public class ContentManager
    {


        //
        // Private implementation details
        //

        public ContentManager(ContentType Type)
        {
            this.ContentType = Type;
        }

        private ContentType ContentType { get; set; }

        private string AbsoluteRootPath
        {
            get
            {

                // Work out which folder to use.
                string folderName = "";

                switch (this.ContentType)
                {
                    case ContentType.Post:
                        folderName = "Posts";
                        break;
                    case ContentType.Page:
                        folderName = "Pages";
                        break;
                }

                // Construct the absolute path.
                return System.Web.HttpContext.Current.Server.MapPath("~/Content/" + folderName + "/");
            }
        }

        private string AbsolutePublishedPath
        {
            get
            {
                return this.AbsoluteRootPath + "Published/";
            }
        }


        //
        // Public API methods
        //

        public T Get<T>(string title)
        {
            // Grab a reference to the post.
            string postReference = Directory.GetFiles(this.AbsolutePublishedPath)
                                            .Where(file => Path.GetFileNameWithoutExtension(file).Right(title.Length) == title)
                                            .FirstOrDefault();

            // Get ready for parse the file.
            ContentParser parser = new ContentParser(this.AbsoluteRootPath, this.AbsolutePublishedPath, this.ContentType);

            // If we don't have a reference, try to generate a file for it.
            if (string.IsNullOrEmpty(postReference))
            {
                postReference = parser.CreatePublishedFile(title);
            }

            return (T)parser.ParseJsonFile(postReference);
        }

    }
}