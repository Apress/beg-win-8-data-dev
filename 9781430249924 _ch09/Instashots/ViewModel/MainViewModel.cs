using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.WindowsAzure.MobileServices;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using Windows.UI.Popups;
using System.Net.Http;
using Windows.Data.Json;
using Microsoft.Live;
using Windows.Storage;
 
using AviarySDKsWindows8;
using Aviary.SDKs.Windows8.Common;
using Windows.Storage.Streams;
using Windows.Storage.Pickers;
using GalaSoft.MvvmLight.Command;
using Instashots.DataModel;
using Instashots.Helper;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;


using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
namespace Instashots.ViewModel
{
     
    public class MainViewModel : ViewModelBase
    {
        LiveAuthClient liveIdClient = new LiveAuthClient("https://instashots.azure-mobile.net/");
        private LiveConnectSession session;

        string selectedFileName = string.Empty;

        private IMobileServiceTable<User> userTable = App.MobileService.GetTable<User>();
        private IMobileServiceTable<Picture> pictureTable = App.MobileService.GetTable<Picture>();
        private IMobileServiceTable<Comment> commentTable = App.MobileService.GetTable<Comment>();

        private ObservableCollection<Picture> myPictures = new ObservableCollection<Picture>();
        private ObservableCollection<Comment> pictureComments = new ObservableCollection<Comment>();
        
        private IRandomAccessStream m_aviaryPhotoStream;

        private User CurrentUser;
        public RelayCommand SignoutCommand { get; private set; }
        public RelayCommand EditPhotoCommand { get; private set; }
        public RelayCommand UploadPhotoCommand { get; private set; }
        public RelayCommand AddCommentCommand { get; private set; }
        public RelayCommand ShowCommentCommand { get; private set; }
        public RelayCommand LikeCommand { get; private set; }

       
        public MainViewModel()
        {
            if (!IsInDesignMode)
            {              
               this.SignoutCommand = new RelayCommand(this.SignOutAction, CanSignOut);
               this.UploadPhotoCommand = new RelayCommand(this.UploadAction, CanUpload);
               this.EditPhotoCommand = new RelayCommand(this.EditPhotoAction , CanEditPhoto);
               this.AddCommentCommand = new RelayCommand(this.AddCommentAction);
               this.ShowCommentCommand = new RelayCommand(this.ShowCommentAction);
               this.LikeCommand = new RelayCommand(this.LikeAction);
               Authenticate();
            }
        }

        //User welcome text
        public string WelcomeTitle
        {
            get
            {
                return welcomeTitle;
            }
            set
            {
                welcomeTitle = value;
                RaisePropertyChanged("WelcomeTitle");
            }
        }string welcomeTitle;

        //Uploading Photo Title
        public string PhotoTitle
        {
            get
            {
                return photoTitle;
            }
            set
            {
                photoTitle = value;
                RaisePropertyChanged("PhotoTitle");
            }
        }string photoTitle;

        //Photo Like count
        public string SelectedLikeCount
        {
            get
            {
                return selectedLikeCount;
            }
            set
            {
                selectedLikeCount = value;
                RaisePropertyChanged("SelectedLikeCount");
            }
        }string selectedLikeCount;

        //Show popup UI to entry title and upload 
        public bool ShowUploadPopup
        {
            get
            {
                return showUploadPopup;
            }
            set
            {
                showUploadPopup = value;
                RaisePropertyChanged("ShowUploadPopup");
            }
        }bool showUploadPopup;

        //Show popup UI to add comment
        public bool ShowCommentPopup
        {
            get
            {
                return showCommentPopup;
            }
            set
            {
                showCommentPopup = value;
                RaisePropertyChanged("ShowCommentPopup");
            }
        }bool showCommentPopup;


        //Selected Photo from the Feed
        public Picture SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                selectedItem = value;
                SelectedTitle = selectedItem.Title;
                SelectedPhoto = SelectedItem.Imageurl ;
                ShowSelectedPhoto = Visibility.Visible;
                SelectedLikeCount = string.Format("{0} {1}"
                    , SelectedItem.Likes, SelectedItem.Likes > 1 ? "Likes" : "Like");
                RaisePropertyChanged("SelectedItem");
                LoadComment();
            }
        }Picture selectedItem;


        //Set visibility of the Selected Photo section
        public Visibility ShowSelectedPhoto
        {
            get
            {
                return showSelectedPhoto;
            }
            set
            {
                showSelectedPhoto = value;
                RaisePropertyChanged("ShowSelectedPhoto");
            }
        }Visibility showSelectedPhoto = Visibility.Collapsed;

        //Selected Photo title from the feed
        public string SelectedTitle
        {
            get
            {
                return selectedTitle;
            }
            set
            {
                selectedTitle = value;
                RaisePropertyChanged("SelectedTitle");
            }
        }string selectedTitle;

        //Comment to be added to the selected photo
        public string CommentText
        {
            get
            {
                return commentText;
            }
            set
            {
                commentText = value;
                RaisePropertyChanged("CommentText");
            }
        }string commentText;

        //Azure storage url of the selected photo from the feed
        public string SelectedPhoto
        {
            get
            {
                return selectedPhoto;
            }
            set
            {
                selectedPhoto = value;
                RaisePropertyChanged("SelectedPhoto");
            }
        }string selectedPhoto;

        //WritableBitmap of the edited photot from Aviary UI
        public WriteableBitmap EditedImage
        {
            get
            {
                return editedImage;
            }
            set
            {
                editedImage = value;
                RaisePropertyChanged("EditedImage");
            }
        }WriteableBitmap editedImage;


        public bool CanEditPhoto()
        {
            return session != null;
        }        

        public bool CanSignOut()
        {
            return liveIdClient.CanLogout;
        }

        public async void SignOutAction()
        {
            session = null;
            await Authenticate();
        }

        public void ShowCommentAction()
        {
            showCommentPopup = true;
            RaisePropertyChanged("ShowCommentPopup");
        }

        private async void LoadComment()
        {
            var comments = await commentTable
                .Where(b => b.PictureId == SelectedItem.Id)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
            pictureComments.Clear();
            foreach (var c in comments)
            {
                c.CommentedBy = await userTable.LookupAsync(c.UserId);
                pictureComments.Add(c);
            }
            RaisePropertyChanged("PictureComments");
        }

        public async void AddCommentAction()
        {
            var comment = new Comment { CommentText = commentText
                , PictureId = SelectedItem.Id
                ,UserId=CurrentUser.Id
                ,  CreatedDate= DateTime.Now };
            await App.MobileService.GetTable<Comment>().InsertAsync(comment);
            pictureComments.Add(comment);
        }       

        public async void LikeAction()
        {
            await App.MobileService.GetTable<Picture>().UpdateAsync(SelectedItem);
        }

        public bool CanUpload()
        {
            //return ((!String.IsNullOrEmpty(PhotoTitle)) && EditedImage != null);
            return true;
          
        }

        public async void UploadAction()
        {            
            string fileName = string.Format("{0}_{1}"
                    , Guid.NewGuid()
                    , selectedFileName);
            var picture = new Picture { Name = fileName
                , Title= PhotoTitle };
            await App.MobileService.GetTable<Picture>().InsertAsync(picture);
            string container = "instashots";
            string imageUrl = string.Format("http://{0}.blob.core.windows.net/{1}/{2}"
                , "splcricket"
                , container
                , fileName);
            StorageCredentials cred = new StorageCredentials(picture.sasQueryString);
            var imageUri = new Uri(picture.Imageurl);
            // Instantiate a Blob store container based on the info in the returned item.
            CloudBlobContainer cloudcontainer = new CloudBlobContainer(
            new Uri(string.Format("https://{0}/{1}"
                , imageUri.Host
                , container))
                , cred);
            CloudBlockBlob blobFromSASCredential =
            cloudcontainer.GetBlockBlobReference(fileName);

                //Save File to local folder.
            await EditedImage.SaveToFile(
                            ApplicationData.Current.LocalFolder,
                            fileName,
                            CreationCollisionOption.GenerateUniqueName); 

            var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var savedFile = await localFolder.GetFileAsync(fileName);

            using (var fileStream = await savedFile.OpenStreamForReadAsync())
            {
                await blobFromSASCredential.UploadFromStreamAsync(fileStream.AsInputStream());
            }     
            MyPictures.Add(picture); 
        }


       
        private async System.Threading.Tasks.Task Authenticate()
        {   
            while (session == null)
            {
                // Force a logout to make it easier to test with multiple Microsoft Accounts
                if (liveIdClient.CanLogout)
                    liveIdClient.Logout();
                LiveLoginResult result = await liveIdClient.LoginAsync(new[] { "wl.basic" });
                if (result.Status == LiveConnectSessionStatus.Connected)
                {
                    session = result.Session;
                    LiveConnectClient client = new LiveConnectClient(result.Session);
                    LiveOperationResult meResult = await client.GetAsync("me");
                    //assigning the token generated by LiveConnectClient to the MobileServiceUser
                    MobileServiceUser loginResult = await App.MobileService.LoginAsync(result.Session.AuthenticationToken);
                    var results = await userTable.ToListAsync();

                    if (results.Count == 0)
                    {
                        var user = new User { LastAccessed = DateTime.Now, UserName = meResult.Result["first_name"].ToString() };
                    }
                    else
                    {
                        CurrentUser = results.First();
                    }
                    WelcomeTitle = string.Format("Welcome {0}!", meResult.Result["first_name"]);
                    //Get the photos uploaded by logged in user.
                    var getPictures = await GetMyPhotos();                     
                    foreach (var p in getPictures)
                    {
                        myPictures.Add(p);
                    }
                    RaisePropertyChanged("MyPictures");                
                }
                else
                {
                    session = null;
                    var dialog = new MessageDialog("You must log in.", "Login Required");
                    dialog.Commands.Add(new UICommand("OK"));
                    await dialog.ShowAsync();
                }
            }
        }

       
        public ObservableCollection<Picture> MyPictures
        {
            get
            {
                return myPictures;
            }
        }

        public ObservableCollection<Comment> PictureComments
        {
            get
            {
                return pictureComments;
            }
        }


        public async void EditPhotoAction()
        {
            editedImage = null;
            PhotoTitle = null;
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                selectedFileName = file.Name;
                m_aviaryPhotoStream = await file.OpenReadAsync();
                await LaunchAviarySDK();
            }
        }

        private async System.Threading.Tasks.Task LaunchAviarySDK()
        {
            if (m_aviaryPhotoStream == null)
            {
                return;
            }
            //Load editor with IRandomAccessStream from a picture file
            AviaryEditorTask.SetAviaryEditorAccentColor(Windows.UI.Colors.LightGray, true);
            AviaryEditorTask task = await AviaryEditorTask.FromRandomAccessStream(m_aviaryPhotoStream, true);           
            task.Completed += PhotoEditCompleted;
            task.Show();
        }

        private void PhotoEditCompleted(object sender, AviaryTaskCompletedEventArgs e)
        {
            //check the Result to see if editing was successfully completed before accessing the Edited Photo
            if (e.Result == AviaryTaskResult.Completed)
            {            
                EditedImage = e.EditedPhoto.Image;
                ShowUploadPopup = true;
            }
        }

        private async Task<List<Picture>> GetMyPhotos()
        {       
            return await pictureTable.ToListAsync();             
        }
    }


    public static class WriteableBitmapSaveExtensions
    {
        /// <summary>
        /// Saves the WriteableBitmap to a png file with a unique file name.
        /// </summary>
        /// <param name="writeableBitmap">The writeable bitmap.</param>
        /// <returns>The file the bitmap was saved to.</returns>
        public static async Task<StorageFile> SaveToFile(
            this WriteableBitmap writeableBitmap)
        {
            return await writeableBitmap.SaveToFile(
                KnownFolders.PicturesLibrary,
                string.Format(
                    "{0}_{1}.png",
                    DateTime.Now.ToString("yyyyMMdd_HHmmss_fff"),
                    Guid.NewGuid()),
                CreationCollisionOption.GenerateUniqueName);
        }

        /// <summary>
        /// Saves the WriteableBitmap to a png file in the given folder with a unique file name.
        /// </summary>
        /// <param name="writeableBitmap">The writeable bitmap.</param>
        /// <param name="storageFolder">The storage folder.</param>
        /// <returns>The file the bitmap was saved to.</returns>
        public static async Task<StorageFile> SaveToFile(
            this WriteableBitmap writeableBitmap,
            StorageFolder storageFolder)
        {
            return await writeableBitmap.SaveToFile(
                storageFolder,
                string.Format(
                    "{0}_{1}.png",
                    DateTime.Now.ToString("yyyyMMdd_HHmmss_fff"),
                    Guid.NewGuid()),
                CreationCollisionOption.GenerateUniqueName);
        }

        /// <summary>
        /// Saves the WriteableBitmap to a file in the given folder with the given file name.
        /// </summary>
        /// <param name="writeableBitmap">The writeable bitmap.</param>
        /// <param name="storageFolder">The storage folder.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="options">
        /// The enum value that determines how responds if the fileName is the same
        /// as the name of an existing file in the current folder. Defaults to ReplaceExisting.
        /// </param>
        /// <returns></returns>
        public static async Task<StorageFile> SaveToFile(
            this WriteableBitmap writeableBitmap,
            StorageFolder storageFolder,
            string fileName,
            CreationCollisionOption options = CreationCollisionOption.ReplaceExisting)
        {
            StorageFile outputFile =
                await storageFolder.CreateFileAsync(
                    fileName,
                    options);

            Guid encoderId;

            var ext = Path.GetExtension(fileName);

            if (new[] { ".bmp", ".dib" }.Contains(ext))
            {
                encoderId = BitmapEncoder.BmpEncoderId;
            }
            else if (new[] { ".tiff", ".tif" }.Contains(ext))
            {
                encoderId = BitmapEncoder.TiffEncoderId;
            }
            else if (new[] { ".gif" }.Contains(ext))
            {
                encoderId = BitmapEncoder.TiffEncoderId;
            }
            else if (new[] { ".jpg", ".jpeg", ".jpe", ".jfif", ".jif" }.Contains(ext))
            {
                encoderId = BitmapEncoder.TiffEncoderId;
            }
            else if (new[] { ".hdp", ".jxr", ".wdp" }.Contains(ext))
            {
                encoderId = BitmapEncoder.JpegXREncoderId;
            }
            else //if (new [] {".png"}.Contains(ext))
            {
                encoderId = BitmapEncoder.PngEncoderId;
            }

            await writeableBitmap.SaveToFile(outputFile, encoderId);

            return outputFile;
        }

        /// <summary>
        /// Saves the WriteableBitmap to the given file with the specified BitmapEncoder ID.
        /// </summary>
        /// <param name="writeableBitmap">The writeable bitmap.</param>
        /// <param name="outputFile">The output file.</param>
        /// <param name="encoderId">The encoder id.</param>
        /// <returns></returns>
        public static async Task SaveToFile(
            this WriteableBitmap writeableBitmap,
            StorageFile outputFile,
            Guid encoderId)
        {
            Stream stream = writeableBitmap.PixelBuffer.AsStream();
            byte[] pixels = new byte[(uint)stream.Length];
            await stream.ReadAsync(pixels, 0, pixels.Length);

            using (var writeStream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(encoderId, writeStream);
                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Premultiplied,
                    (uint)writeableBitmap.PixelWidth,
                    (uint)writeableBitmap.PixelHeight,
                    96,
                    96,
                    pixels);
                await encoder.FlushAsync();

                using (var outputStream = writeStream.GetOutputStreamAt(0))
                {
                    await outputStream.FlushAsync();
                }
            }
        }
    }
}



 