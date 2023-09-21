﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using HowsGoingCore.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace HowsGoingCore.Models
{
    public partial interface IhowsgoingContextProcedures
    {
        Task<List<GetFriendRequestsResult>> GetFriendRequestsAsync(string username, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetFriendsResult>> GetFriendsAsync(string username, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
        Task<List<GetRecordsResult>> GetRecordsAsync(string username, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default);
    }
}
