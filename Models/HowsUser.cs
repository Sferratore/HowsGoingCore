﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace HowsGoingCore.Models;

public partial class HowsUser
{
    public string Username { get; set; }

    public string Password { get; set; }

    public string Email { get; set; }

    public virtual ICollection<Friendrequest> FriendrequestUser1Navigation { get; set; } = new List<Friendrequest>();

    public virtual ICollection<Friendrequest> FriendrequestUser2Navigation { get; set; } = new List<Friendrequest>();

    public virtual ICollection<Friendship> FriendshipUser1Navigation { get; set; } = new List<Friendship>();

    public virtual ICollection<Friendship> FriendshipUser2Navigation { get; set; } = new List<Friendship>();

    public virtual ICollection<Record> Record { get; set; } = new List<Record>();
}