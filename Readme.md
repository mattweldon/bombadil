Bombadil - Pseudo-static site generator for ASP.NET
================================

Takes the static file aspects of Jekyll and exposes them in an ASP.NET MVC friendly way. Text/Markdown files get uploaded to a directory on your server and are generated into HTML/JSON. 
When a file is requsted, this JSON is then parsed into a POCO which can be used in your MVC project.


Project Structure
--------------------------------

*Bombadil.Core*
 - Core DLL to be included in any project using Bombadil.

*Bombadil.Sandbox*
 - Sandbox MVC project used to test Core integration with MVC.

*Bombadil.Tests*
 - Unit tests for the Core project.

