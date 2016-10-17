

namespace Instashots.DataModel
{
    using Microsoft.WindowsAzure.MobileServices;
    using System;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Windows.Storage;

    [DataTable(Name = "Comments")]
    public class Comment
    {
        public int Id { get; set; }

        [DataMember(Name = "text")]
        public string CommentText { get; set; }      

        [DataMember(Name = "userid")]
        public int UserId { get; set; }

        [DataMember(Name = "pictureid")]
        public int PictureId { get; set; }

        [DataMember(Name = "createdDate")]
        public DateTime? CreatedDate { get; set; }

        [IgnoreDataMember]
        public User CommentedBy { get; set; }
    }
}
