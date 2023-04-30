using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Data.WebPortal.Models
{
    public partial class ShareFileWebPortalContext : DbContext
    {
        public ShareFileWebPortalContext()
        {
        }

        public ShareFileWebPortalContext(DbContextOptions<ShareFileWebPortalContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Document> Documents { get; set; } = null!;
        public virtual DbSet<RecAllocationView> RecAllocationViews { get; set; } = null!;
        public virtual DbSet<RolloverAccount> RolloverAccounts { get; set; } = null!;
        public virtual DbSet<RolloverAccountView> RolloverAccountViews { get; set; } = null!;
        public virtual DbSet<RolloverObjectAccount> RolloverObjectAccounts { get; set; } = null!;
        public virtual DbSet<RolloverWorksheet> RolloverWorksheets { get; set; } = null!;
        public virtual DbSet<ViewTaskTemplate> ViewTaskTemplates { get; set; } = null!;
        public virtual DbSet<ViewTimetableTask> ViewTimetableTasks { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>(entity =>
            {
                entity.ToTable("Document");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.FileName)
                    .HasMaxLength(260)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.FolderPath).HasMaxLength(250);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.RootPath).HasMaxLength(250);

                entity.Property(e => e.UploadedOn).HasColumnType("datetime");
            });

            modelBuilder.Entity<RecAllocationView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("RecAllocationView");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.RolloverDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<RolloverAccount>(entity =>
            {
                entity.ToTable("RolloverAccount");

                entity.HasIndex(e => e.Approver2Id, "IX_Approver2Id");

                entity.HasIndex(e => e.Approver3Id, "IX_Approver3Id");

                entity.HasIndex(e => e.ApproverId, "IX_ApproverId");

                entity.HasIndex(e => e.CreatedById, "IX_CreatedById");

                entity.HasIndex(e => e.InternallyReviewedById, "IX_InternallyReviewedById");

                entity.HasIndex(e => e.ModifiedById, "IX_ModifiedById");

                entity.HasIndex(e => e.PreparerId, "IX_PreparerId");

                entity.HasIndex(e => e.RecAccountId, "IX_RecAccountId");

                entity.HasIndex(e => e.ReviewerId, "IX_ReviewerId");

                entity.HasIndex(e => e.RolloverId, "IX_RolloverId");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.InternallyReviewedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            });

            modelBuilder.Entity<RolloverAccountView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("RolloverAccountView");

                entity.Property(e => e.Approver2FullName).HasColumnName("Approver2_FullName");

                entity.Property(e => e.Approver2Id).HasColumnName("Approver2_Id");

                entity.Property(e => e.Approver3FullName).HasColumnName("Approver3_FullName");

                entity.Property(e => e.Approver3Id).HasColumnName("Approver3_Id");

                entity.Property(e => e.ApproverFullName).HasColumnName("Approver_FullName");

                entity.Property(e => e.ApproverId).HasColumnName("Approver_Id");

                entity.Property(e => e.CreatedByFullName).HasColumnName("CreatedBy_FullName");

                entity.Property(e => e.CreatedById).HasColumnName("CreatedBy_Id");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.CurrentMonthBalance).HasColumnType("decimal(38, 2)");

                entity.Property(e => e.LastMonthBalance).HasColumnType("decimal(38, 2)");

                entity.Property(e => e.ModifiedByFullName).HasColumnName("ModifiedBy_FullName");

                entity.Property(e => e.ModifiedById).HasColumnName("ModifiedBy_Id");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Movement).HasColumnType("decimal(38, 2)");

                entity.Property(e => e.PreparerFullName).HasColumnName("Preparer_FullName");

                entity.Property(e => e.PreparerId).HasColumnName("Preparer_Id");

                entity.Property(e => e.ReviewerFullName).HasColumnName("Reviewer_FullName");

                entity.Property(e => e.ReviewerId).HasColumnName("Reviewer_Id");

                entity.Property(e => e.RolloverDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<RolloverObjectAccount>(entity =>
            {
                entity.ToTable("RolloverObjectAccount");

                entity.HasIndex(e => e.CreatedById, "IX_CreatedById");

                entity.HasIndex(e => e.ModifiedById, "IX_ModifiedById");

                entity.HasIndex(e => e.ObjectAccountId, "IX_ObjectAccountId");

                entity.HasIndex(e => e.RolloverAccountId, "IX_RolloverAccountId");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Value).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.RolloverAccount)
                    .WithMany(p => p.RolloverObjectAccounts)
                    .HasForeignKey(d => d.RolloverAccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.RolloverObjectAccount_dbo.RolloverAccount_RolloverAccountId");
            });

            modelBuilder.Entity<RolloverWorksheet>(entity =>
            {
                entity.ToTable("RolloverWorksheet");

                entity.HasIndex(e => e.CreatedById, "IX_CreatedById");

                entity.HasIndex(e => e.ModifiedById, "IX_ModifiedById");

                entity.HasIndex(e => e.RolloverObjectAccountId, "IX_RolloverObjectAccountId");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("('1900-01-01T00:00:00.000')");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Value).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.RolloverObjectAccount)
                    .WithMany(p => p.RolloverWorksheets)
                    .HasForeignKey(d => d.RolloverObjectAccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.RolloverWorksheet_dbo.RolloverObjectAccount_RolloverObjectAccountId");
            });

            modelBuilder.Entity<ViewTaskTemplate>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_TaskTemplates");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Frequency)
                    .HasMaxLength(11)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ViewTimetableTask>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_TimetableTasks");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.DueDate).HasColumnType("datetime");

                entity.Property(e => e.TaskType)
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.TimetableTaskStatus)
                    .HasMaxLength(17)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
