

namespace Instashots.DataModel
{
    using Microsoft.WindowsAzure.MobileServices;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Windows.Storage;

    [DataTable(Name = "Pictures")]
    public class Picture
    {
        public int Id { get; set; }        

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }
    
        [DataMember(Name = "userid")]
        public string UserId { get; set; }

        [DataMember(Name = "imageurl")]
        public string Imageurl { get; set; }

        [DataMember(Name = "sasQueryString")]
        public string sasQueryString { get; set; }

        [DataMember(Name = "likes")]
        public int Likes { get; set; }
 
         [IgnoreDataMember]
         public List<Comment> Comments { get; set; }

    }
}
