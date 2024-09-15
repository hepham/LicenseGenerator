using System;

public class Class1
{
	public Class1()
	{
        static string hashToUUID(string input)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                byte[] truncatedHash = new byte[16];
                Array.Copy(hash, truncatedHash, 16);
                return new Guid(truncatedHash).ToString();
            }
        }
        static void Main(string[] args)
        {
            string input = "exampleInput";
            string uuid = hashToUUID(input);
            Console.WriteLine("UUID: " + uuid);
        }


    }
}
