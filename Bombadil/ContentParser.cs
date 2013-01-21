using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Bombadil.Core.Models;
using Newtonsoft.Json;
using MarkdownSharp;

namespace Bombadil.Core.Managers
{
    internal class ContentParser
    {

        private string _AbsoluteRootPath { get; set; }
        private string _AbsolutePublishedPath { get; set; }
        private ContentType _ContentType { get; set; }

        public ContentParser(string RootPath, string PublishedPath, ContentType ContentType)
        {
            this._AbsolutePublishedPath = PublishedPath;
            this._AbsoluteRootPath = RootPath;
            this._ContentType = ContentType;
        }

        /// <summary>
        /// Creates a published JSON file for a given post title.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        internal string CreatePublishedFile(string title)
        {

            string publishedPostReference = null;

            // Get the posted filename.
            string postReference = Directory.GetFiles(this._AbsoluteRootPath)
                                            .Where(file => Path.GetFileNameWithoutExtension(file).Right(title.Length) == title)
                                            .FirstOrDefault();

            // If we have an unpublished file, generate it.
            if (!string.IsNullOrEmpty(postReference))
            {

                IContent publishedPost = this.ParseInputFile(postReference);
                publishedPostReference = this._AbsolutePublishedPath + Path.GetFileNameWithoutExtension(postReference) + ".json";

                string postJson = JsonConvert.SerializeObject(publishedPost);
                File.WriteAllText(publishedPostReference, postJson);
            }

            return publishedPostReference;
        }

        /// <summary>
        /// Parses the given human-readable Input file to a Post object.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        internal IContent ParseInputFile(string reference)
        {

            IContent content = null;

            switch (this._ContentType)
            { 
                case ContentType.Post:
                    content = new Post();
                    break;
                case ContentType.Page:
                    content = new Page();
                    break;
            }

            string rawText = Utilities.RawFileContents(reference);

            // ------------------------------------------------------------------------------------------
            // Generate a new Post object from the contents of the Input file.
            // ------------------------------------------------------------------------------------------

            bool processingMeta = false;
            bool processingContent = false;

            // Iterate over each line of the file.
            foreach (string line in rawText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                // Work out what's going on.
                if (line == "---" && processingMeta && !processingContent) { processingMeta = false; processingContent = true; }
                if (line == "---" && !processingMeta && !processingContent) { processingMeta = true; }

                // Now do the processing.
                if (processingMeta)
                {
                    if (line != "---")
                    {
                        switch (this._ContentType)
                        {
                            case Managers.ContentType.Post:
                                this.ProcessMeta(line, (Post)content);
                                break;
                            case Managers.ContentType.Page:
                                this.ProcessMeta(line, (Page)content);
                                break;
                        }
                    }

                }
                else if (processingContent && line != "---")
                {
                    content.Content += line + Environment.NewLine;
                }
            }


            // ------------------------------------------------------------------------------------------
            // Carry out any other processing of the Post that may be required.
            // ------------------------------------------------------------------------------------------

            Markdown contentMarkdown = new Markdown();
            content.Content = contentMarkdown.Transform(content.Content);


            return content;
        }

        /// <summary>
        /// Processes the meta information specific to a Post.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="post"></param>
        /// <returns></returns>
        private Post ProcessMeta(string line, Post post)
        {
            if (line.StartsWith("title: "))
            {
                post.Title = line.Replace("title: ", "");
            }

            if (line.StartsWith("description: "))
            {
                post.Description = line.Replace("description: ", "");
            }

            if (line.StartsWith("author: "))
            {
                post.Author = line.Replace("author: ", "");
            }

            if (line.StartsWith("date: "))
            {
                post.DateCreated = Convert.ToDateTime(line.Replace("date: ", ""));
            }

            if (line.StartsWith("tags: "))
            {
                string tagCsv = line.Replace("tags: ", "");

                post.Tags = tagCsv.Split(',').ToList<string>();
            }

            if (line.StartsWith("categories: "))
            {
                string tagCsv = line.Replace("categories: ", "");

                post.Tags = tagCsv.Split(',').ToList<string>();
            }

            return post;
        }

        /// <summary>
        /// Processes the meta infomation specific to a Page.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        internal Page ProcessMeta(string line, Page page)
        {

            bool isCustomContent = true;

            if (line.StartsWith("title: "))
            {
                page.Title = line.Replace("title: ", "");
                isCustomContent = false;
            }

            if (line.StartsWith("description: "))
            {
                page.Description = line.Replace("description: ", "");
                isCustomContent = false;
            }

            if (isCustomContent)
            {
                string contentKey = line.Split(':')[0];
                page.CustomContent.Add(contentKey, line.Replace(contentKey + ": ", ""));
            }

            return page;
        }

        /// <summary>
        /// Parses the given JSON file into a Post object.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        internal IContent ParseJsonFile(string reference)
        {

            IContent content = null;

            switch (this._ContentType)
            {
                case Managers.ContentType.Page:
                    content = new Page();
                    break;
                case Managers.ContentType.Post:
                    content = new Post();
                    break;
            }

            // Load the post if we have a reference.
            if (!string.IsNullOrEmpty(reference))
            {
                // Grab the raw data.
                string postJson = Utilities.RawFileContents(reference);

                // Deserialise the data.
                switch (this._ContentType)
                {
                    case Managers.ContentType.Page:
                        content = Utilities.DeserializeJson<Page>(postJson);
                        break;
                    case Managers.ContentType.Post:
                        content = Utilities.DeserializeJson<Post>(postJson);
                        break;
                }


                content.CompleteFilePath = reference;

                // Process the file if necessary.
                if (!content.IsProcessed)
                {
                    switch (this._ContentType)
                    {
                        case Managers.ContentType.Page:
                            this.Preprocess((Page)content);
                            break;
                        case Managers.ContentType.Post:
                            this.Preprocess((Post)content);
                            break;
                    }
                }
            }

            return content;
        }

        /// <summary>
        /// Carries out necessary preprocessing of a post such as adding to the archive, collating tags etc.
        /// </summary>
        /// <param name="post"></param>
        internal void Preprocess(Post post)
        {

            // Add to archive (i.e. by Date).
            CollectionsManager collectionsManager = new CollectionsManager(CollectionType.Archives);

            collectionsManager.AddPost(post);

            // Any tags? Update them.
            if (post.Tags != null)
            {
                collectionsManager = new CollectionsManager(CollectionType.Tags);

                collectionsManager.AddPost(post);
            }


            // Any categories? Add this post to them.
            if (post.Categories != null)
            {
                collectionsManager = new CollectionsManager(CollectionType.Categories);

                collectionsManager.AddPost(post);
            }


            // Flag the post as being processed and write to disk.
            post.IsProcessed = true;

            string postJson = JsonConvert.SerializeObject(post);
            File.WriteAllText(post.CompleteFilePath, postJson);

        }

        internal void Preprocess(Page page)
        {
            page.IsProcessed = true;

            string pageJson = JsonConvert.SerializeObject(page);
            File.WriteAllText(page.CompleteFilePath, pageJson);
        }

    }
}
