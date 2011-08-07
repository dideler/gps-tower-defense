using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.IsolatedStorage;

namespace BingMaps
{
    /// <summary>
    /// Methods for reading and writing to isolated storage.
    /// </summary>
    public class StorageStuff
    {

        /// <summary>
        /// Write content to a file in the user store.
        /// </summary>
        /// <param name="fileName">Name of the file ot be accessed.</param>
        /// <param name="content">The file's data.</param>
        public void SaveToFile(string fileName, string content)
        {
            //get the user Store and then create the file in the store
            //finally write the content to the file also with a message box
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            using (var writeStream = new IsolatedStorageFileStream(fileName, FileMode.Create, store))
            using (var writer = new StreamWriter(writeStream))
            {
                writer.Write(content);
            }

        }

        /// <summary>
        /// Read data from a file.
        /// </summary>
        /// <param name="fileName">The file to be read.</param>
        /// <returns>The data in the file.</returns>
        public string LoadFromFile(string fileName)
        {
            //Setting the fileName
            //var fileName = "myWP7.dat";
            try
            {
                //get the user Store and then open the file in the store
                //finally read the content to the file and return it
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                using (var readStream = new IsolatedStorageFileStream(fileName, FileMode.Open, store))
                using (var reader = new StreamReader(readStream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (IsolatedStorageException)
            {
                //IsolatedStorageException catch if File cant be opened
                return null;
            }
        }

    }
}
