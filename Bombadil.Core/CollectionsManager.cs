using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using Bombadil.Core.Models;

namespace Bombadil.Core.Managers
{
    public class CollectionsManager
    {


        //
        // Private implementation details
        //

        public CollectionsManager(CollectionType Type)
        {
            this.Type = Type;
        }

        private string AbsoluteFolderPath
        {
            get
            {

                // Work out which folder to use.
                string folderName = "";

                switch (this.Type)
                {
                    case CollectionType.Archives:
                        folderName = "Archives";
                        break;
                    case CollectionType.Categories:
                        folderName = "Categories";
                        break;
                    case CollectionType.Tags:
                        folderName = "Tags";
                        break;
                }

                // Construct the absolute path.
                return System.Web.HttpContext.Current.Server.MapPath("~/Content/Collections/" + folderName + "/");
            }
        }

        private CollectionType Type { get; set; }


        //
        // Public API methods
        //

        public Collection Get(string CollectionName)
        {

            // Get a reference to the collection.
            string reference = Directory.GetFiles(this.AbsoluteFolderPath)
                                        .Where(file => Path.GetFileNameWithoutExtension(file) == CollectionName)
                                        .FirstOrDefault();

            // Grab the raw data.
            string fileContents = Utilities.RawFileContents(reference);

            // Deserialise the data.
            return Utilities.DeserializeJson<Collection>(fileContents);

        }

        public void AddPost(Post post)
        {

            // Get any associated collections for this Post.
            List<string> collectionsToAmend = new List<string>();
            List<string> collectionsToCreate = new List<string>();

            switch (this.Type)
            { 
                case CollectionType.Archives:

                    string collectionsReference = Directory.GetFiles(this.AbsoluteFolderPath, post.DateCreated.ToString("yyyy-MM.json")).FirstOrDefault();

                    // If we have a reference, 
                    if (!string.IsNullOrEmpty(collectionsReference))
                    {
                        collectionsToAmend.Add(collectionsReference);
                    }
                    else
                    {
                        collectionsToCreate.Add(post.DateCreated.ToString("yyyy-MM"));
                    }

                    break;

                case CollectionType.Categories:

                    foreach (string category in post.Categories)
                    {
                        string collectionReference = Directory.GetFiles(this.AbsoluteFolderPath, category + ".json").FirstOrDefault();

                        if (!string.IsNullOrEmpty(collectionReference))
                        {
                            collectionsToAmend.Add(collectionReference);
                        }
                        else
                        {
                            collectionsToCreate.Add(category);
                        }
                    }

                    break;
                case CollectionType.Tags:

                    foreach (string tag in post.Tags)
                    {
                        string collectionReference = Directory.GetFiles(this.AbsoluteFolderPath, tag + ".json").FirstOrDefault();

                        if (!string.IsNullOrEmpty(collectionReference))
                        {
                            collectionsToAmend.Add(collectionReference);
                        }
                        else
                        {
                            collectionsToCreate.Add(tag);
                        }
                    }

                    break;
            }


            // Amend any collections that need amending.
            foreach (string collectionReference in collectionsToAmend)
            {
                Collection collection = new Collection();

                collection = this.Get(collectionReference);
                collection.References.Add(post.CompleteFilePath);

                this.Save(collection);
            }


            // Create any collections that need creating.
            foreach (string collectionName in collectionsToCreate)
            {
                Collection collection = new Collection();

                switch (this.Type)
                { 
                    case CollectionType.Archives:
                        collection.Type = "archive";
                        break;
                    case CollectionType.Tags:
                        collection.Type = "tag";
                        break;
                    case CollectionType.Categories:
                        collection.Type = "category";
                        break;
                }

                collection.Name = collectionName;
                collection.References.Add(post.CompleteFilePath);

                this.Save(collection);
            }

        }

        public void Save(Collection collection)
        {
            string fileData = JsonConvert.SerializeObject(collection);
            File.WriteAllText(this.AbsoluteFolderPath + collection.Name + ".json", fileData);
        }

    }
}