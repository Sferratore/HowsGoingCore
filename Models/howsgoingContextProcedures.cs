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
    public partial class howsgoingContext
    {
        private IhowsgoingContextProcedures _procedures;

        public virtual IhowsgoingContextProcedures Procedures
        {
            get
            {
                if (_procedures is null) _procedures = new howsgoingContextProcedures(this);
                return _procedures;
            }
            set
            {
                _procedures = value;
            }
        }

        public IhowsgoingContextProcedures GetProcedures()
        {
            return Procedures;
        }

        protected void OnModelCreatingGeneratedProcedures(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GetFriendRequestsResult>().HasNoKey().ToView(null);
            modelBuilder.Entity<GetFriendsResult>().HasNoKey().ToView(null);
            modelBuilder.Entity<GetRecordsResult>().HasNoKey().ToView(null);
        }
    }

    public partial class howsgoingContextProcedures : IhowsgoingContextProcedures
    {
        private readonly howsgoingContext _context;

        public howsgoingContextProcedures(howsgoingContext context)
        {
            _context = context;
        }

        public virtual async Task<List<GetFriendRequestsResult>> GetFriendRequestsAsync(string username, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default)
        {
            var parameterreturnValue = new SqlParameter
            {
                ParameterName = "returnValue",
                Direction = System.Data.ParameterDirection.Output,
                SqlDbType = System.Data.SqlDbType.Int,
            };

            var sqlParameters = new []
            {
                new SqlParameter
                {
                    ParameterName = "username",
                    Size = 50,
                    Value = username ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.VarChar,
                },
                parameterreturnValue,
            };
            var _ = await _context.SqlQueryAsync<GetFriendRequestsResult>("EXEC @returnValue = [dbo].[GetFriendRequests] @username", sqlParameters, cancellationToken);

            returnValue?.SetValue(parameterreturnValue.Value);

            return _;
        }

        public virtual async Task<List<GetFriendsResult>> GetFriendsAsync(string username, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default)
        {
            var parameterreturnValue = new SqlParameter
            {
                ParameterName = "returnValue",
                Direction = System.Data.ParameterDirection.Output,
                SqlDbType = System.Data.SqlDbType.Int,
            };

            var sqlParameters = new []
            {
                new SqlParameter
                {
                    ParameterName = "username",
                    Size = 50,
                    Value = username ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.VarChar,
                },
                parameterreturnValue,
            };
            var _ = await _context.SqlQueryAsync<GetFriendsResult>("EXEC @returnValue = [dbo].[GetFriends] @username", sqlParameters, cancellationToken);

            returnValue?.SetValue(parameterreturnValue.Value);

            return _;
        }

        public virtual async Task<List<GetRecordsResult>> GetRecordsAsync(string username, OutputParameter<int> returnValue = null, CancellationToken cancellationToken = default)
        {
            var parameterreturnValue = new SqlParameter
            {
                ParameterName = "returnValue",
                Direction = System.Data.ParameterDirection.Output,
                SqlDbType = System.Data.SqlDbType.Int,
            };

            var sqlParameters = new []
            {
                new SqlParameter
                {
                    ParameterName = "username",
                    Size = 50,
                    Value = username ?? Convert.DBNull,
                    SqlDbType = System.Data.SqlDbType.VarChar,
                },
                parameterreturnValue,
            };
            var _ = await _context.SqlQueryAsync<GetRecordsResult>("EXEC @returnValue = [dbo].[GetRecords] @username", sqlParameters, cancellationToken);

            returnValue?.SetValue(parameterreturnValue.Value);

            return _;
        }
    }
}
