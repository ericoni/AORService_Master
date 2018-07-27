using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AORCommon
{
	public sealed class SecurePasswordManager
	{
		private const int SALT_SIZE = 8;

		private const int HASH_SIZE = 20;

		/// <summary>
		/// Creates a hash from a password
		/// </summary>
		/// <param name="password">the password</param>
		/// <param name="iterations">number of iterations</param>
		/// <returns>the hash</returns>
		public static string Hash(string password, int iterations)
		{
			byte[] salt = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
			//new RNGCryptoServiceProvider().GetBytes(salt = new byte[SALT_SIZE]);
			new RNGCryptoServiceProvider().GetBytes(salt);

			var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
			var hash = pbkdf2.GetBytes(HASH_SIZE);

			var hashBytes = new byte[SALT_SIZE + HASH_SIZE];
			Array.Copy(salt, 0, hashBytes, 0, SALT_SIZE);
			Array.Copy(hash, 0, hashBytes, SALT_SIZE, HASH_SIZE);

			//convert to base64
			var base64hash = Convert.ToBase64String(hashBytes);

			return string.Format("$USERHASH$V1${0}${1}", iterations, base64hash);
		}

		/// <summary>
		/// Creates a hash from a password with 3000 iterations
		/// </summary>
		/// <param name="password">the password</param>
		/// <returns>the hash</returns>
		public static string Hash(string password)
		{
			return Hash(password, 3000);
		}

		/// <summary>
		/// Check if hash is supported
		/// </summary>
		/// <param name="hashString">the hash</param>
		/// <returns>is supported?</returns>
		public static bool IsHashSupported(string hashString)
		{
			return hashString.Contains("$USERHASH$V1$");
		}

		/// <summary>
		/// verify a password against a hash
		/// </summary>
		/// <param name="password">the password</param>
		/// <param name="hashedPassword">the hash</param>
		/// <returns>could be verified?</returns>
		public static bool Veryfy(string password, string hashedPassword)
		{
			if (!IsHashSupported(hashedPassword))
				throw new NotSupportedException("The hashtype is not supported!");

			var splittedHashString = hashedPassword.Replace("$USERHASH$V1$", "").Split('$');
			var iterations = int.Parse(splittedHashString[0]);
			var base64Hash = splittedHashString[1];

			var hashBytes = Convert.FromBase64String(base64Hash);

			//get salt
			var salt = new byte[SALT_SIZE];
			Array.Copy(hashBytes, 0, salt, 0, SALT_SIZE);

			//create hash with given salt
			var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
			byte[] hash = pbkdf2.GetBytes(HASH_SIZE);

			//get result
			for (var i = 0; i < HASH_SIZE; i++)
			{
				if (hashBytes[i + SALT_SIZE] != hash[i])
				{
					return false;
				}
			}
			return true;
		}
	}
}
