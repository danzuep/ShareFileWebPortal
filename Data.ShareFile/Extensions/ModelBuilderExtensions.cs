using System;
using Microsoft.EntityFrameworkCore;
using Data.ShareFile.Models;

namespace Data.ShareFile.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            //var item1 = new SfItem { Id = 1, Name = "Test Item" };
            //var user1 = new SfPrincipal { Id = 1, Name = "New User" };
            //modelBuilder.Entity<SfItem>().HasData(item1);
            //modelBuilder.Entity<SfPrincipal>().HasData(user1);
            //modelBuilder.Entity<SfAccessControl>().HasData(
            //    new SfAccessControl { Id = 1, Principal = user1, Item = item1 }
            //    );
        }
    }
}
