

namespace Instashots.DataModel
{
    using Microsoft.WindowsAzure.MobileServices;
    using System;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Windows.Storage;

    [DataTable(Name = "Follow")]
    public class Follow
    {
        public int Id { get; set; } 

        [DataMember(Name = "followeruserid")]
        public int FollowerUserId { get; set; }

        [DataMember(Name = "followinguserid")]
        public int FollowingUserId { get; set; }

        [DataMember(Name = "lastUpdated")]
        public DateTime? LastUpdatedDate { get; set; }        
    }
}
