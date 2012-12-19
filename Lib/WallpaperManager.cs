using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Search;
using System.Windows.Media.Imaging;

namespace Lib
{
    public class WallpaperManager : INotifyPropertyChanged
    {
        #region Singleton viewmodel
        private static WallpaperManager singleton;

        public static WallpaperManager Singleton
        {
            get
            {
                if (singleton == null)
                {
                    singleton = new WallpaperManager();
                }
                return singleton;
            }
        }
        #endregion


        private bool querying;
        private IEnumerable<LocalImageFileInfo> localImages;

        public IEnumerable<LocalImageFileInfo> LocalImages
        {
            get 
            {
                if (localImages == null && querying == false)
                {
                    updateLocalImages();
                }
                return localImages;
            }
        }


        public async void AddPictures(System.IO.Stream photoStream, string initialFilename)
        {
            var bitmap = new System.Windows.Media.Imaging.BitmapImage();
            bitmap.SetSource(photoStream);

            var folder = await getImagesFolder();
            var file = await folder.CreateFileAsync(Path.GetFileName(initialFilename), Windows.Storage.CreationCollisionOption.GenerateUniqueName);

            using (var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite)) 
            {
                new WriteableBitmap(bitmap).SaveJpeg(stream.AsStream(), bitmap.PixelWidth, bitmap.PixelHeight, 0, 100);
            }

            localImages = null;
            PropertyChanged.Raise(this, () => this.LocalImages);
        }

        private async void updateLocalImages()
        {
            querying = true;
            localImages = from file in await QueryLocalPictures()
                          select new LocalImageFileInfo(file);
            querying = false;

            PropertyChanged.Raise(this, () => this.LocalImages);
        }

        public static async Task<IReadOnlyList<Windows.Storage.StorageFile>> QueryLocalPictures()
        {
            var folder = await getImagesFolder();

            return await folder.GetFilesAsync();
        }

        private static IAsyncOperation<Windows.Storage.StorageFolder> getImagesFolder()
        {
            return Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync("images", Windows.Storage.CreationCollisionOption.OpenIfExists);
        }
    
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
