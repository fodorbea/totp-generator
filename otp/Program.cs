using System.Security.Cryptography;
using System.Text;


class TotpGenerator
{
    static string GeneratePassword(string userId)
    {
        //combine userId and timestamp
        string combinedSecretAsString = userId + DateTime.Now.ToString();

        //get byte array format of the secret
        byte[] combinedSecretAsByteArray = Encoding.UTF8.GetBytes(combinedSecretAsString);

        //use SHA512 HMAC method to get a 6 digit code
        var hmac = new HMACSHA512();
        var computedHash = hmac.ComputeHash(combinedSecretAsByteArray);

        int offset = computedHash[computedHash.Length - 1] & 0x0F;

        var otp = (computedHash[offset] & 0x7f) << 24
                | (computedHash[offset + 1] & 0xff) << 16
                | (computedHash[offset + 2] & 0xff) << 8
                | (computedHash[offset + 3] & 0xff) % 1000000;

        var truncatedValue = (int)otp % (int)Math.Pow(10, 6);

        return truncatedValue.ToString().PadLeft(6, '0');
    }



    static void GetCode(string id)
    {            
        var code = GeneratePassword(id);
        Console.WriteLine($"Your code is " + code + " and it expires at " + DateTime.UtcNow.AddSeconds(30).ToString("HH:mm:ss"));   
    }


    static void Main()
    {
        string userId;
        //read user input
        Console.Write("Enter userId: ");
        userId = Console.ReadLine();

        
        if(userId != null)
        {
            //initial
            GetCode(userId);

            //create autogeneration mechanism and run GetCode method every 30s
            var timer = new System.Timers.Timer();
            timer.Elapsed += (sender, args) => OnTimedEvent(sender, userId);
            timer.Interval = 30000;
            timer.Enabled = true;
        }
       

        Console.WriteLine("Starting autogeneration. Press \'q\' to stop generation.");
        while (Console.Read() != 'q') ;

    }

    private static void OnTimedEvent(object sender, string userId)
    {
        GetCode(userId);
    }
}