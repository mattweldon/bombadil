using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bombadil.Core.Models
{
    public interface IContent
    {

        string CompleteFilePath { get; set; }

        string FileName { get; }

        DateTime DateCreated { get; }

        bool IsProcessed { get; set; }

        string Title { get; set; }

        string Description { get; set; }

        string Content { get; set; }

    }
}
