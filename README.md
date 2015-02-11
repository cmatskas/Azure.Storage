Azure.Storage
=============

**Speak to me:** [![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/cmatskas/Azure.Storage?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

**Build Status:** [![Build status](https://ci.appveyor.com/api/projects/status/6i7g6igga5ee8o3l)](https://ci.appveyor.com/project/cmatskas/azure)

**NuGet:** [![NuGet](https://img.shields.io/nuget/v/azure.storage.svg)](https://www.nuget.org/packages/Azure.Storage/)

Azure.Storage provides a wrapper around the native Azure Storage API and allows you to quickly and easily code against current Azure Storage solutions (blobs, queues, tables etc). The project will aim to target the latest Azure SDK and use best practices as advised by 
Microsoft.

To get started, you will need to grab the latest version from Nuget

You can still dowload the source code and run it locally if you wish to know how things work under the covers.

Azure.Storage provide 4 basic classes, each responsible for a separate Azure storage type:

- BlobStorage --> for managing blob objects
- QueueStorage --> for managing queues
- TableStorage --> for managing table storage
- FileStorage --> for managing folders and files on Azure-attached drives

The library will also be released as a PCL to enable cross platform mobile application development. For xamarin developers, the
project will also be released in the Xamarin components 

Please note that the unit tests are still being added so it's early days before and if you decide to use it you should now that some
paths of the code have not been tested. 

Let me know of your thoughts and if you have any questions.
