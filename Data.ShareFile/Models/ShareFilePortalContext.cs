using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Data.ShareFile.Extensions;
using Data.ShareFile.DbFirstModels;

namespace Data.ShareFile.Models
{
    public partial class ShareFilePortalContext : DbContext
    {
        public ShareFilePortalContext() : base() { }

        public ShareFilePortalContext(DbContextOptions options) : base(options) { }

        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<PublicHoliday> PublicHolidays { get; set; }
        public virtual DbSet<SecurityCompany> SecurityCompanies { get; set; }
        public virtual DbSet<SecurityCompanyNew> SecurityCompanyNews { get; set; }
        public virtual DbSet<SecurityCompanyPermission> SecurityCompanyPermissions { get; set; }
        public virtual DbSet<SecurityUserCompany> SecurityUserCompanies { get; set; }
        public virtual DbSet<ShareFileDbGroup> ShareFileGroups { get; set; }
        public virtual DbSet<ShareFileDbGroupMember> ShareFileGroupMembers { get; set; }
        public virtual DbSet<SharefileAuth> SharefileAuths { get; set; }
        public virtual DbSet<DbPrincipal> Principals { get; set; }
        //public virtual DbSet<SfPrincipal> Principals { get; set; }
        //public virtual DbSet<SfContact> Contacts { get; set; }
        //public virtual DbSet<SfGroup> Groups { get; set; }
        //public virtual DbSet<SfUser> Users { get; set; }
        public virtual DbSet<SfAccessControl> AccessControls { get; set; }
        public virtual DbSet<SfItem> Items { get; set; }
        public virtual DbSet<ViewShareFileGroupMember> ViewShareFileGroupMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Application>(entity =>
            {
                entity.ToTable("Application");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Title).IsRequired();
            });

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("ApplicationUser");

                entity.Property(e => e.AdLogin).IsRequired();

                entity.Property(e => e.FirstName).IsRequired();

                entity.Property(e => e.LastName).IsRequired();
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.ToTable("Company");

                entity.HasIndex(e => e.DefaultApprover2Id, "IX_DefaultApprover2_Id");

                entity.HasIndex(e => e.DefaultApprover3Id, "IX_DefaultApprover3_Id");

                entity.HasIndex(e => e.DefaultApproverId, "IX_DefaultApprover_Id");

                entity.HasIndex(e => e.DefaultPreparerId, "IX_DefaultPreparer_Id");

                entity.HasIndex(e => e.DefaultReviewerId, "IX_DefaultReviewer_Id");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.DefaultApprover2Id)
                    .HasColumnName("DefaultApprover2_Id")
                    .HasDefaultValueSql("((4))");

                entity.Property(e => e.DefaultApprover3Id)
                    .HasColumnName("DefaultApprover3_Id")
                    .HasDefaultValueSql("((4))");

                entity.Property(e => e.DefaultApproverId)
                    .HasColumnName("DefaultApprover_Id")
                    .HasDefaultValueSql("((4))");

                entity.Property(e => e.DefaultPreparerId)
                    .HasColumnName("DefaultPreparer_Id")
                    .HasDefaultValueSql("((4))");

                entity.Property(e => e.DefaultReviewerId)
                    .HasColumnName("DefaultReviewer_Id")
                    .HasDefaultValueSql("((4))");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Title).IsRequired();

                entity.HasOne(d => d.DefaultApprover2)
                    .WithMany(p => p.CompanyDefaultApprover2s)
                    .HasForeignKey(d => d.DefaultApprover2Id)
                    .HasConstraintName("FK_dbo.Company_dbo.ApplicationUser_DefaultApprover2Id");

                entity.HasOne(d => d.DefaultApprover3)
                    .WithMany(p => p.CompanyDefaultApprover3s)
                    .HasForeignKey(d => d.DefaultApprover3Id)
                    .HasConstraintName("FK_dbo.Company_dbo.ApplicationUser_DefaultApprover3Id");

                entity.HasOne(d => d.DefaultApprover)
                    .WithMany(p => p.CompanyDefaultApprovers)
                    .HasForeignKey(d => d.DefaultApproverId)
                    .HasConstraintName("FK_dbo.Company_dbo.ApplicationUser_DefaultApproverId");

                entity.HasOne(d => d.DefaultPreparer)
                    .WithMany(p => p.CompanyDefaultPreparers)
                    .HasForeignKey(d => d.DefaultPreparerId)
                    .HasConstraintName("FK_dbo.Company_dbo.ApplicationUser_DefaultPreparerId");

                entity.HasOne(d => d.DefaultReviewer)
                    .WithMany(p => p.CompanyDefaultReviewers)
                    .HasForeignKey(d => d.DefaultReviewerId)
                    .HasConstraintName("FK_dbo.Company_dbo.ApplicationUser_DefaultReviewerId");
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.ToTable("Document");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(260)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.FolderPath).HasMaxLength(250);

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.RootPath).HasMaxLength(250);

                entity.Property(e => e.UploadedOn).HasColumnType("datetime");
            });

            modelBuilder.Entity<PublicHoliday>(entity =>
            {
                entity.ToTable("PublicHoliday");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Title).IsRequired();
            });

            modelBuilder.Entity<SecurityCompany>(entity =>
            {
                entity.ToTable("SecurityCompany");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.FinYearStartDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Taken).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Updated).HasColumnType("datetime");
            });

            modelBuilder.Entity<SecurityCompanyNew>(entity =>
            {
                entity.ToTable("SecurityCompany_New");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.FinYearStartDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.Property(e => e.Taken).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Updated).HasColumnType("datetime");
            });

            modelBuilder.Entity<SecurityCompanyPermission>(entity =>
            {
                entity.ToTable("SecurityCompanyPermission");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            });

            modelBuilder.Entity<SecurityUserCompany>(entity =>
            {
                entity.ToTable("SecurityUserCompany");

                entity.HasIndex(e => e.ApplicationUserId, "IX_ApplicationUserId");

                entity.HasIndex(e => e.CreatedById, "IX_CreatedById");

                entity.HasIndex(e => e.ModifiedById, "IX_ModifiedById");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.HasOne(d => d.ApplicationUser)
                    .WithMany(p => p.SecurityUserCompanyApplicationUsers)
                    .HasForeignKey(d => d.ApplicationUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.SecurityUserCompany_dbo.ApplicationUser_ApplicationUserId");

                entity.HasOne(d => d.CreatedBy)
                    .WithMany(p => p.SecurityUserCompanyCreatedBies)
                    .HasForeignKey(d => d.CreatedById)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.SecurityUserCompany_dbo.ApplicationUser_CreatedById");

                entity.HasOne(d => d.ModifiedBy)
                    .WithMany(p => p.SecurityUserCompanyModifiedBies)
                    .HasForeignKey(d => d.ModifiedById)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.SecurityUserCompany_dbo.ApplicationUser_ModifiedById");
            });

            modelBuilder.Entity<ShareFileDbGroup>(entity =>
            {
                entity.ToTable("ShareFileGroup");

                entity.HasIndex(e => e.Uid, "IX_ShareFileGroup_Uid");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Uid)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.Url).HasMaxLength(1000);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ShareFileGroupCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ShareFileGroup_CreatedBy");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.ShareFileGroupModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ShareFileGroup_ModifiedBy");
            });

            modelBuilder.Entity<ShareFileDbGroupMember>(entity =>
            {
                entity.ToTable("ShareFileGroupMember");

                entity.HasIndex(e => e.Uid, "IX_ShareFileGroupMember_Uid");

                entity.Property(e => e.Company).HasMaxLength(100);

                entity.Property(e => e.Email).HasMaxLength(500);

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.FullName).HasMaxLength(200);

                entity.Property(e => e.LastName).HasMaxLength(100);

                entity.Property(e => e.Uid)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.Url).HasMaxLength(1000);

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.ShareFileGroupMemberCreatedByNavigations)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ShareFileGroupMember_EGroupId");

                //entity.HasOne(d => d.Group)
                //    .WithMany(p => p.ShareFileGroupMembers)
                //    .HasForeignKey(d => d.GroupId)
                //    .HasConstraintName("FK_ShareFileGroupMember_GroupId");

                entity.HasOne(d => d.ModifiedByNavigation)
                    .WithMany(p => p.ShareFileGroupMemberModifiedByNavigations)
                    .HasForeignKey(d => d.ModifiedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ShareFileGroupMember_InvoiceUrlId");
            });

            modelBuilder.Entity<SharefileAuth>(entity =>
            {
                entity.ToTable("SharefileAuth");

                entity.HasIndex(e => e.CreatedById, "IX_CreatedById");

                entity.HasIndex(e => e.ModifiedById, "IX_ModifiedById");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Exipiry).HasColumnType("datetime");

                entity.Property(e => e.ModifiedOn).HasColumnType("datetime");

                entity.HasOne(d => d.CreatedBy)
                    .WithMany(p => p.SharefileAuthCreatedBies)
                    .HasForeignKey(d => d.CreatedById)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.SharefileAuth_dbo.ApplicationUser_CreatedById");

                entity.HasOne(d => d.ModifiedBy)
                    .WithMany(p => p.SharefileAuthModifiedBies)
                    .HasForeignKey(d => d.ModifiedById)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_dbo.SharefileAuth_dbo.ApplicationUser_ModifiedById");
            });

            //modelBuilder.Entity<SfPrincipal>(entity =>
            //{
            //    //entity.HasIndex(e => e.Id, "IX_Principal_Uid");
            //    entity.HasDiscriminator(d => d.Discriminator)
            //        .HasValue<SfPrincipal>((int)SfPrincipalType.Principal)
            //        .HasValue<SfUser>((int)SfPrincipalType.User)
            //        .HasValue<SfGroup>((int)SfPrincipalType.Group)
            //        .HasValue<SfContact>((int)SfPrincipalType.Contact)
            //        .IsComplete(false);
            //    //entity.Property(e => e.CreatedOn).HasDefaultValueSql("GETDATE()");
            //});

            modelBuilder.Entity<DbPrincipal>(entity =>
            {
                entity.Property(e => e.FullName).HasComputedColumnSql("CONCAT([LastName], ', ', [FirstName])");
            });

            //modelBuilder.Entity<SfContact>(entity =>
            //{
            //    entity.HasBaseType(typeof(SfPrincipal)); //(Type)null
            //    //entity.Property(e => e.FullName).HasComputedColumnSql("[LastName] + ', ' + [FirstName]");
            //});

            //modelBuilder.Entity<SfUser>(entity =>
            //{
            //    //entity.HasBaseType(typeof(SfPrincipal)); //(Type)null
            //    //entity.Property(e => e.FullName).HasComputedColumnSql("[LastName] + ', ' + [FirstName]");
            //});

            //modelBuilder.Entity<SfGroup>(entity =>
            //{
            //    //entity.HasBaseType(typeof(SfPrincipal)); //(Type)null
            //});

            modelBuilder.Entity<SfAccessControl>(entity =>
            {
                //entity.HasIndex(e => e.Uid, "IX_AccessControl_Uid");

                //entity.Property(e => e.CreatedOn).HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<SfItem>(entity =>
            {
                //entity.HasIndex(e => e.Uid, "IX_Item_Uid");

                //entity.Property(e => e.CreatedOn).HasDefaultValueSql("GETDATE()");

                //entity.Property(e => e.ItemCount).HasComputedColumnSql("Count([Children])", stored: true);
            });

            modelBuilder.Entity<ViewShareFileGroupMember>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_ShareFileGroupMember");

                entity.Property(e => e.Uid)
                    .HasMaxLength(36)
                    .IsUnicode(false);

                entity.Property(e => e.Company).HasMaxLength(100);

                entity.Property(e => e.Email).HasMaxLength(500);

                entity.Property(e => e.FirstName).HasMaxLength(100);

                entity.Property(e => e.FullName).HasMaxLength(200);

                entity.Property(e => e.LastName).HasMaxLength(100);
            });

            //modelBuilder.Seed();

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
