﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HowsGoingCore.Models;

public partial class howsgoingContext : DbContext
{
    public howsgoingContext(DbContextOptions<howsgoingContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Friendrequest> Friendrequest { get; set; }

    public virtual DbSet<Friendship> Friendship { get; set; }

    public virtual DbSet<HowsUser> HowsUser { get; set; }

    public virtual DbSet<Record> Record { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Friendrequest>(entity =>
        {
            entity.HasKey(e => e.FriendrequestId).HasName("PK__Friendre__F383ECCC335798D3");

            entity.Property(e => e.FriendrequestId)
                .ValueGeneratedNever()
                .HasColumnName("FriendrequestID");
            entity.Property(e => e.RequestReceiver)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RequestSender)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.RequestReceiverNavigation).WithMany(p => p.FriendrequestRequestReceiverNavigation)
                .HasForeignKey(d => d.RequestReceiver)
                .HasConstraintName("FK__Friendreq__User1__2D27B809");

            entity.HasOne(d => d.RequestSenderNavigation).WithMany(p => p.FriendrequestRequestSenderNavigation)
                .HasForeignKey(d => d.RequestSender)
                .HasConstraintName("FK__Friendreq__User2__2E1BDC42");
        });

        modelBuilder.Entity<Friendship>(entity =>
        {
            entity.HasKey(e => e.FriendshipId).HasName("PK__Friendsh__4D531A747420DBEE");

            entity.Property(e => e.FriendshipId)
                .ValueGeneratedNever()
                .HasColumnName("FriendshipID");
            entity.Property(e => e.User1)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.User2)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.User1Navigation).WithMany(p => p.FriendshipUser1Navigation)
                .HasForeignKey(d => d.User1)
                .HasConstraintName("FK__Friendshi__User1__29572725");

            entity.HasOne(d => d.User2Navigation).WithMany(p => p.FriendshipUser2Navigation)
                .HasForeignKey(d => d.User2)
                .HasConstraintName("FK__Friendshi__User2__2A4B4B5E");
        });

        modelBuilder.Entity<HowsUser>(entity =>
        {
            entity.HasKey(e => e.Username).HasName("PK__HowsUser__536C85E5752FDE25");

            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("password");
        });

        modelBuilder.Entity<Record>(entity =>
        {
            entity.HasKey(e => e.RecordId).HasName("PK__Record__FBDF78C902092F93");

            entity.Property(e => e.RecordId)
                .ValueGeneratedNever()
                .HasColumnName("RecordID");
            entity.Property(e => e.RecordContent)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Record)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Record__UserID__267ABA7A");
        });

        OnModelCreatingGeneratedProcedures(modelBuilder);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}