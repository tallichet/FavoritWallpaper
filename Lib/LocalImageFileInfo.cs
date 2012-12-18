using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Lib
{
    public class LocalImageFileInfo : INotifyPropertyChanged
    {
        private Windows.Storage.StorageFile storageFile;

        public LocalImageFileInfo (Windows.Storage.StorageFile file) 
        {
            storageFile = file;
            loadImage();
        }

        public string Name
        {
            get
            {
                return storageFile.Name;
            }
        }

        public Uri ImageUri
        {
            get
            {
                return new Uri("ms-appdata:///Local/images/" + storageFile.Name, UriKind.Absolute);
            }
        }

        public BitmapImage Image
        {
            get; private set;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void loadImage()
        {
            using (var stream = await storageFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                Image = new BitmapImage();
                Image.SetSource(stream.AsStreamForRead());
            }

            PropertyChanged.Raise(this, () => this.Image);
        }
    }
}
