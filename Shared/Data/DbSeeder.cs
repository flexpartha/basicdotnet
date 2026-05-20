using Microsoft.EntityFrameworkCore;
using UserApi.Features.Users;

namespace UserApi.Shared.Data;

public static class DbSeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "Patricia Lebsack", Username = "Karianne", Email = "Julianne.OConner@kory.org", Phone = "493-170-9623 x156", Website = "kale.biz" },
            new User { Id = 2, Name = "Chelsey Dietrich", Username = "Kamren", Email = "Lucio_Hettinger@annie.ca", Phone = "(254)954-1289", Website = "demarco.info" },
            new User { Id = 3, Name = "Partha Chakraborty", Username = "partha.chakra2", Email = "partha.chakra2@gmail.com", Phone = "9862029378", Website = "nil" }
        );

        modelBuilder.Entity<User>().OwnsOne(u => u.Address).HasData(
            new { UserId = 1L, Street = "Hoeger Mall", Suite = "Apt. 692", City = "South Elvis", Zipcode = "53919-4257" },
            new { UserId = 2L, Street = "Skiles Walks", Suite = "Suite 351", City = "Roscoeview", Zipcode = "33263" },
            new { UserId = 3L, Street = "rippon", Suite = "A1", City = "kolkata", Zipcode = "700132" }
        );

        modelBuilder.Entity<User>().OwnsOne(u => u.Address).OwnsOne(a => a.Geo).HasData(
            new { AddressUserId = 1L, Lat = "29.4572", Lng = "-164.2990" },
            new { AddressUserId = 2L, Lat = "-31.8129", Lng = "62.5342" },
            new { AddressUserId = 3L, Lat = "89", Lng = "87" }
        );

        modelBuilder.Entity<User>().OwnsOne(u => u.Company).HasData(
            new { UserId = 1L, Name = "Robel-Corkery", CatchPhrase = "Multi-tiered zero tolerance productivity", Bs = "transition cutting-edge web services" },
            new { UserId = 2L, Name = "Keebler LLC", CatchPhrase = "User-centric fault-tolerant solution", Bs = "revolutionize end-to-end systems" },
            new { UserId = 3L, Name = "nil", CatchPhrase = "nil", Bs = "nil" }
        );
    }
}
