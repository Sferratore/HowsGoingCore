﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HowsGoingCore.Models
{
    public partial class GetRecordsResult
    {
        public int RecordID { get; set; }
        public string RecordContent { get; set; }
        public int? Satisfaction { get; set; }
        public string UserID { get; set; }
        public DateTime? LastUpdate { get; set; }
    }
}
