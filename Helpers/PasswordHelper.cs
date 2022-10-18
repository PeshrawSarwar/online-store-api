using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace online_store_api.Helpers{
    public static class PasswordHelper{
       public static PasswordHashResult HashPassword(string plaintext, string salt=""){
        // string? password = Console.ReadLine();

        // Generate a 128-bit salt using a sequence of
        // cryptographically strong random bytes.
        byte[] saltBytes; 
        if(salt == ""){
            saltBytes = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            salt = Convert.ToBase64String(saltBytes);

        }else{
            saltBytes = Convert.FromBase64String(salt);
        }
        
        // RandomNumberGenerator.GetBytes(128 / 8); // divide by 8 to convert bits to bytes
        // Console.WriteLine($"Salt: {Convert.ToBase64String(saltBytes)}");

        // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: plaintext!,
            salt: saltBytes,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));

                return new PasswordHashResult{
                    PasswordSalt = salt,
                    PasswordHash = hashed
                };

              
            }

        
    }

    public class PasswordHashResult{
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
    }


}