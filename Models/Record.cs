﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace HowsGoingCore.Models;

public partial class Record
{
    public int RecordId { get; set; }

    public string RecordContent { get; set; }

    public int? Satisfaction { get; set; }

    public string UserId { get; set; }

    public DateTime? LastUpdate { get; set; }

    public virtual HowsUser User { get; set; }
}