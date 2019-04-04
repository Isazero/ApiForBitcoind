using System;
using System.Data.Entity;
using System.Security.Cryptography;

namespace BitcoinApi.Database
{
    public class BitcoinApiContext : DbContext
    {
        public BitcoinApiContext()
            : base("name=BitcoinApiContext")
        {

            System.Data.Entity.Database.SetInitializer(new BitcoinApiDbInitializer());
        }

        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<Wallet> Wallets { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }

    public class BitcoinApiDbInitializer : DropCreateDatabaseAlways<BitcoinApiContext>
    {
        protected override void Seed(BitcoinApiContext context)
        {
            const string username = "isazero";
            const string password = "isa980822";
            var salt = new byte[16];
            new RNGCryptoServiceProvider().GetBytes(salt);
            var passwordRfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, 10000);
            var hash = passwordRfc2898DeriveBytes.GetBytes(20);
            var hashBytes = new byte[36];

            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            var hashedPassword = Convert.ToBase64String(hashBytes);
            var defaultUser = new User
            {
                Id = 1,
                Password = hashedPassword,
                Salt = salt,
                Username = username
            };

            context.Users.Add(defaultUser);

            base.Seed(context);
        }
    }
}