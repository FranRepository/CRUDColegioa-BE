﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CTS_ReturnsApp.Models;

public partial class MexicanaHoldsContext : DbContext
{
    public MexicanaHoldsContext(DbContextOptions<MexicanaHoldsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CtsOu> CtsOu { get; set; }

    public virtual DbSet<CtsReturns> CtsReturns { get; set; }

    public virtual DbSet<LogCtsReturns> LogCtsReturns { get; set; }

    public virtual DbSet<LogHold> LogHold { get; set; }

    public virtual DbSet<Mails> Mails { get; set; }

    public virtual DbSet<MaterialCts> MaterialCts { get; set; }

    public virtual DbSet<Ou> Ou { get; set; }

    public virtual DbSet<StatusCts> StatusCts { get; set; }

    public virtual DbSet<UnitHistoryCts> UnitHistoryCts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CtsOu>(entity =>
        {
            entity.HasKey(e => e.Itemid).HasName("PK__cts_ou__56A22C924C804A93");

            entity.ToTable("cts_ou");

            entity.Property(e => e.Itemid).HasColumnName("itemid");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.CtsOuRepair)
                .HasColumnType("datetime")
                .HasColumnName("cts_ou_repair");
            entity.Property(e => e.CtsOuRepairComment)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("cts_ou_repair_comment");
            entity.Property(e => e.CtsOuRepairEditBy)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("cts_ou_repair_edit_by");
            entity.Property(e => e.CtsRetursId).HasColumnName("cts_returs_id");
            entity.Property(e => e.OuId).HasColumnName("ou_id");
            entity.Property(e => e.UserRepairOu)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("user_repair_ou");
        });

        modelBuilder.Entity<CtsReturns>(entity =>
        {
            entity.HasKey(e => e.Itemid).HasName("PK__cts_retu__56A22C92F58A024B");

            entity.ToTable("cts_returns");

            entity.Property(e => e.Itemid).HasColumnName("itemid");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.CommentRepair)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("comment_repair");
            entity.Property(e => e.Customer)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("customer");
            entity.Property(e => e.Discrepancy)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("discrepancy");
            entity.Property(e => e.EtaCommentCts)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("eta_comment_cts");
            entity.Property(e => e.EtaCommentCtsRepair)
                .HasColumnType("datetime")
                .HasColumnName("eta_comment_cts_repair");
            entity.Property(e => e.EtaFinishCtsQa)
                .HasColumnType("datetime")
                .HasColumnName("eta_finish_cts_qa");
            entity.Property(e => e.FinderBy).HasColumnName("finder_by");
            entity.Property(e => e.FinishCts)
                .HasColumnType("datetime")
                .HasColumnName("finish_cts");
            entity.Property(e => e.ItemAndCode)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("item_and_code");
            entity.Property(e => e.Material).HasColumnName("material");
            entity.Property(e => e.NoteRejectToIngressByPlant)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("note_reject_to_ingress_byPlant");
            entity.Property(e => e.NoteRejectToReleaseByPlant)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("note_reject_to_release_byPlant");
            entity.Property(e => e.ReleaseMfgDate)
                .HasColumnType("datetime")
                .HasColumnName("release_mfg_date");
            entity.Property(e => e.RequestedCts)
                .HasColumnType("date")
                .HasColumnName("requested_cts");
            entity.Property(e => e.StartChasisDate)
                .HasColumnType("datetime")
                .HasColumnName("start_chasis_date");
            entity.Property(e => e.StartCts)
                .HasColumnType("datetime")
                .HasColumnName("start_cts");
            entity.Property(e => e.StatusCts).HasColumnName("status_cts");
            entity.Property(e => e.UserAcceptIngress)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("user_accept_ingress");
            entity.Property(e => e.UserAcceptRepair)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("user_accept_repair");
            entity.Property(e => e.UserEdit)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("user_edit");
            entity.Property(e => e.UserRequestedCts)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("user_requested_cts");
            entity.Property(e => e.Vin)
                .IsRequired()
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("vin");
            entity.Property(e => e.WasAcceptedToIngressByPlant).HasColumnName("was_accepted_to_ingress_byPlant");
            entity.Property(e => e.WasRejectToReleaseByPlant).HasColumnName("was_reject_to_release_byPlant");

            entity.HasOne(d => d.FinderByNavigation).WithMany(p => p.CtsReturns)
                .HasForeignKey(d => d.FinderBy)
                .HasConstraintName("FinderByFK2");
        });

        modelBuilder.Entity<LogCtsReturns>(entity =>
        {
            entity.HasKey(e => e.Itemid).HasName("PK__log_cts___56A22C929DA1A66F");

            entity.ToTable("log_cts_returns");

            entity.Property(e => e.Itemid).HasColumnName("itemid");
            entity.Property(e => e.Date)
                .HasColumnType("date")
                .HasColumnName("date");
            entity.Property(e => e.TableLog)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("table_log");
            entity.Property(e => e.TextLog)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("text_log");
            entity.Property(e => e.UserLog)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("user_log");
        });

        modelBuilder.Entity<LogHold>(entity =>
        {
            entity.HasKey(e => e.Itemid).HasName("PK__log_hold__56A22C9251D60930");

            entity.ToTable("log_hold");

            entity.Property(e => e.Itemid).HasColumnName("itemid");
            entity.Property(e => e.PosixTimestamp).HasColumnName("posix_timestamp");
            entity.Property(e => e.TableLog)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("table_log");
            entity.Property(e => e.TextLog)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("text_log");
            entity.Property(e => e.UserLog)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("user_log");
        });

        modelBuilder.Entity<Mails>(entity =>
        {
            entity.HasKey(e => e.Itemid).HasName("PK__mails__56A22C92E2612DE1");

            entity.ToTable("mails");

            entity.Property(e => e.Itemid).HasColumnName("itemid");
            entity.Property(e => e.Enabled).HasColumnName("enabled");
            entity.Property(e => e.Mails1)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("mails");
            entity.Property(e => e.Notes)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("notes");
            entity.Property(e => e.OuId).HasColumnName("ou_id");
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("title");
        });

        modelBuilder.Entity<MaterialCts>(entity =>
        {
            entity.HasKey(e => e.Itemid).HasName("PK__material__56A22C92AB55F5F1");

            entity.ToTable("material_cts");

            entity.Property(e => e.Itemid).HasColumnName("itemid");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.Chargeback)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("chargeback");
            entity.Property(e => e.Comment)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("comment");
            entity.Property(e => e.Costo)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("costo");
            entity.Property(e => e.CtsRetursId).HasColumnName("cts_returs_id");
            entity.Property(e => e.DateRequested)
                .HasColumnType("date")
                .HasColumnName("date_requested");
            entity.Property(e => e.DayMaterialArrived)
                .HasColumnType("date")
                .HasColumnName("day_material_arrived");
            entity.Property(e => e.Dm)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("dm");
            entity.Property(e => e.Eta)
                .HasColumnType("date")
                .HasColumnName("eta");
            entity.Property(e => e.NumberParts)
                .HasColumnType("numeric(18, 0)")
                .HasColumnName("number_parts");
            entity.Property(e => e.PartNumber)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("part_number");
            entity.Property(e => e.Rga)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("rga");
        });

        modelBuilder.Entity<Ou>(entity =>
        {
            entity.HasKey(e => e.Itemid).HasName("PK__ou__56A22C926085BA3F");

            entity.ToTable("ou");

            entity.Property(e => e.Itemid).HasColumnName("itemid");
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.MailTo)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("MAIL_TO");
            entity.Property(e => e.OuName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ou_name");
            entity.Property(e => e.OuNumber).HasColumnName("ou_number");
        });

        modelBuilder.Entity<StatusCts>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("status_cts");

            entity.Property(e => e.DescripcionStatus)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("descripcion_status");
            entity.Property(e => e.IdStatusReferenceSecuence).HasColumnName("id_status_reference_secuence");
            entity.Property(e => e.Itemid)
                .ValueGeneratedOnAdd()
                .HasColumnName("itemid");
        });

        modelBuilder.Entity<UnitHistoryCts>(entity =>
        {
            entity.HasKey(e => e.Itemid).HasName("PK__unit_his__56A22C926986BE30");

            entity.ToTable("unit_history_cts");

            entity.Property(e => e.Itemid).HasColumnName("itemid");
            entity.Property(e => e.CtsRetursId).HasColumnName("cts_returs_id");
            entity.Property(e => e.Notes)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("notes");
            entity.Property(e => e.OuActual)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ou_actual");
            entity.Property(e => e.PosixTimestamp).HasColumnName("posix_timestamp");
            entity.Property(e => e.UserHistory)
                .IsRequired()
                .HasMaxLength(7)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("user_history");
            entity.Property(e => e.Vin)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("vin");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}