using DHubV.Core.Entity.UserAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHubV.Dal.EntityConfigration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Configure the User entity here if needed
            seeduser(builder);
        }


        private void seeduser(EntityTypeBuilder<User> builder)
        {
            List<User> seeduser = new List<User>
            {
                new User
                {
                    Id = "512f6df4-e4b2-4b1f-a01e-60ace1761cb3",
                    UserName = "Mohsen",
                    Email = "Mohsen@gmail.com",
                    NormalizedEmail = "Mohsen".ToUpper(),
                    NormalizedUserName = "Mohsen@gmail.com".ToUpper(),
                    EmailConfirmed = true,
                    PasswordHash =
                        "AQAAAAIAAYagAAAAEHe61zEw3gk2CVMdtPl5FAbQPzwxIYWhe96GqN3RywiDuojearPtXbaN0j6TC2mwIA==",
                    SecurityStamp = "WGKUB4EBB2DV7CYBY65WXVWRSODM3D6U",
                    ConcurrencyStamp = "ee1039b3-ee74-41a9-8b73-8b5f74e4a7fb",
                    FullName = "محمد محسن"

                }
               
            };
            builder.HasData(seeduser);
        }
    }
  }

