Azure.Storage
=============

**Speak to me:** [![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/cmatskas/Azure.Storage?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

**Build Status:** [![Build status](https://ci.appveyor.com/api/projects/status/6i7g6igga5ee8o3l)](https://ci.appveyor.com/project/cmatskas/azure)

**NuGet:** [![NuGet](https://img.shields.io/nuget/v/azure.storage.svg)](https://www.nuget.org/packages/Azure.Storage/)

**Browse through the code** http://sourcebrowser.io/Browse/cmatskas/Azure.Storage

###Introduction
Azure.Storage provides a wrapper around the native Azure Storage API and allows you to quickly and easily code against current Azure Storage solutions (blobs, queues, tables etc). The project will aim to target the latest Azure SDK and use best practices as advised by Microsoft.

To get started, you will need to grab the latest version from Nuget or, alternatively, you could download the source and build it locally. The second option gives you the ability to crack open the code and <del>have a laugh</del> check the underlying implementation

Azure.Storage provide 4 basic classes, each responsible for managing a separate Azure storage type:

- BlobStorage => for managing blob objects
- QueueStorage => for managing queues
- TableStorage => for managing table storage
- FileStorage => for managing folders and files on Azure-attached drives

I promise to add tutorials and walkthroughs soon, so apologize in advance for the luck of documentation.

###Portability
Not everyone is developing for the web or the desktop. If you are creating a mobile application that requires access to the Azure Storage service, then there is a portable version of library (i.e. a PCL). There is no NuGet package yet, but I plan to release one as soon as possible. In the meantime you can grab the code and use it as see fit. 

###Xamarin
For xamarin developers, the project will also be released in the Xamarin Components Store.

###Caveats
Please note that the unit tests are still being added along with the concrete implementation of the code. It's still early days so if you decide to use it you should now that some paths of the code have not been tested and some part of the functionality is not implemented across both the PCL and full version.

###Feedback
This is a project to help developers interact with Azure Storage. There is nothing stopping you from using the official Azure SDK and this project was built around it, but if there is any way I can improve this library or have any problems, please let me know by raising an issue or emailing me directly.


