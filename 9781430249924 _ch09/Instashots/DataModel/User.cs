

namespace Instashots.DataModel
{
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Runtime.Serialization;

[DataTable(Name = "User")]
public class User
{
    public int Id { get; set; }

    [DataMember(Name = "username")]
    public string UserName { get; set; }

    [DataMember(Name = "userid")]
    public string UserID { get; set; }

    [DataMember(Name = "lastaccessed")]
    public DateTime? LastAccessed { get; set; }
}
}
