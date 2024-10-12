using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace XEncDecTooL
{
    internal class Program
    {
        static string pwd;
        [STAThread]

        static void Main(string[] args)
        {



            while (true)
            {

                //Console.Clear();
                //Console.WriteLine("===@@@@ Video Encryption & Decryption Tool ===@@@@");

                string logo = @"
    _      _  ______ _   _  _____  _____   _____  ______  _______         _     
    x     x |  ____| \ | ||  __  |  __ \ |  __ \|  ____||__   __|       | |    
     x   x  | |__  |  \| || |  | || |  | || |  | || |__    | | ___   ___| | 
      x x   |  __| | . ` ||      || |  | || |  | ||  __|   | |/ _ \ / __| '
     x  x   | |____| |\  || |__| || |__| || |__| || |____  | | (_) | (__| !   
    x    x  |______|_| \_||_____/|_____/ |_____/|______|   |_|\___/ \___|_|_ |_|
                                                                            
                            @ X Encryption/Decryption Tool@
        ";
                // Print the logo to the console
                Console.ForegroundColor = ConsoleColor.Green;  // Set the text color
                Console.WriteLine(logo);
                Console.ResetColor();

                Console.WriteLine("1. Encrypt Video");
                Console.WriteLine("2. Decrypt Video");
                Console.WriteLine("3. Exit");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        EncryptVideo();
                        break;
                    case "2":
                        DecryptVideo();
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }

            }
        }

        static void EncryptVideo()
        {
            Console.Write("Enter the path of the video file to encrypt: ");
            string filePath = SelectFile("Select the video file to encrypt");
            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine("No file selected. Operation canceled.");
                Console.Clear();
                return;

            }

            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found.");
                return;
            }
            string pattern;
            string password;
            while (true)
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Console.WindowWidth)); // Clear the line by writing spaces
                Console.SetCursorPosition(0, Console.CursorTop - 1); // Move up to the line with the prompt
                Console.Write("At least 6 characters,1 uppercase,1 Special char,1 digit\n Enter the decryption password: ");
                password = ReadPasswordforencrption();
                pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{6,}$";

                if (IsValidPassword(password, pattern))
                {
                    Console.WriteLine("The password is valid.");
                    break;
                }
                else
                {
                    MessageBox.Show("The password is not valid. It must be at least 6 characters long and include at least one uppercase letter, one lowercase letter, and one digit.");

                    Console.Write(new string(' ', Console.WindowWidth)); // Clear the line again
                    Console.SetCursorPosition(0, Console.CursorTop); // Move to the start of the line

                    Console.WriteLine("Press any key to try again...");
                    Console.ReadKey(); // Wait for user input before continuing
                }
            }

                pwd = password;
            //var pass = password;
            var savefile = "password.txt";
            SaveToFile(pwd, savefile);


            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Password cannot be empty. Operation canceled.");
                return;
            }

            string encryptedFilePath = filePath + ".enc";

            try
            {
                EncryptFile(filePath, encryptedFilePath, password);
                // Use Invoke to ensure that MessageBox is called on the main thread
                Task.Run(() => MessageBox.Show($"Video file encrypted successfully!\nEncrypted file: {encryptedFilePath}"));
                File.Delete(filePath); // Ensure the original file is deleted after encryption
                Console.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error encrypting file: {ex.Message}");
            }

            //Console.WriteLine("Press any key to return to the main menu...");
            //Console.ReadKey();
        }

        static bool IsValidPassword(string password, string pattern)
        {
            Regex regex = new Regex(pattern);
            return regex.IsMatch(password);
        }

        static void SaveToFile(string text, string savefile)
        {
            // Write text to the specified file
            try
            {
                File.WriteAllText(savefile, text);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while writing to the file: " + ex.Message);
            }
        }

        static string ReadPasswordforencrption()
        {
            string password = "";
            ConsoleKeyInfo keyInfo;

            do
            {
                keyInfo = Console.ReadKey(intercept: true); // Read key without displaying it

                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        // Remove last character from the password and update display
                        password = password.Substring(0, password.Length - 1);
                        Console.Write("\b \b"); // Erase last '*' from the console
                    }
                }
                else if (keyInfo.Key != ConsoleKey.Enter)
                {
                    password += keyInfo.KeyChar; // Append the key character to the password
                    Console.Write("*"); // Display '*' for each entered character
                }

            } while (keyInfo.Key != ConsoleKey.Enter); // Continue until Enter key is pressed

            return password;
        }


        static void DecryptVideo()
        {
            Console.Write("Enter the path of the encrypted video file to decrypt: ");
            string filePath = SelectFile("Select the video file to decrypt");
            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine("No file selected. Operation canceled.");
                return;
            }

            if (!File.Exists(filePath))
            {
                Console.WriteLine("File not found.");
                return;
            }
            Console.WriteLine();
            Console.Write("Enter the decryption password: ");
            string password = ReadPasswordfordecrption();
            //var pass1 = password;

            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Password cannot be empty. Operation canceled.");
                return;
            }

            //if (pwd != password)
            //{
            //    MessageBox.Show("Invalid password: Please Enter Correct Passord");
            //    Console.Clear();
            //    Console.WriteLine("Press any key to return to the main menu...");
            //    Console.ReadKey();


            //}




            string decryptedFilePath = filePath.Replace(".enc", "");

            try
            {
                DecryptFile(filePath, decryptedFilePath, password);


                // Use Invoke to ensure that MessageBox is called on the main thread
                Task.Run(() => MessageBox.Show($"Video file decrypted successfully!\nDecrypted file: {decryptedFilePath}"));

                File.Delete(filePath); // Ensure the encrypted file is deleted after decryption
                Console.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error decrypting file:Password is incorrect {ex.Message}");
                Console.Clear();
            }

            //Console.WriteLine("Press any key to return to the main menu...");
            //Console.ReadKey();
        }



        static string ReadPasswordfordecrption()
        {
            string password = "";
            ConsoleKeyInfo keyInfo;

            do
            {
                keyInfo = Console.ReadKey(intercept: true); // Read key without displaying it

                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        // Remove last character from the password and update display
                        password = password.Substring(0, password.Length - 1);
                        Console.Write("\b \b"); // Erase last '*' from the console
                    }
                }
                else if (keyInfo.Key != ConsoleKey.Enter)
                {
                    password += keyInfo.KeyChar; // Append the key character to the password
                    Console.Write("*"); // Display '*' for each entered character
                }

            } while (keyInfo.Key != ConsoleKey.Enter); // Continue until Enter key is pressed

            return password;
        }

        static void EncryptFile(string inputFilePath, string outputFilePath, string password)
        {
            // Generate a random salt for this encryption
            byte[] salt = GenerateRandomSalt();

            // Derive the key and IV from the password and salt
            byte[] key, iv;
            DeriveKeyAndIV(password, salt, out key, out iv);

            using (FileStream inputStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            using (FileStream outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                // Write the salt at the beginning of the output file
                outputStream.Write(salt, 0, salt.Length);

                using (CryptoStream cryptoStream = new CryptoStream(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    const int buffersize = 12000;
                    byte[] buffer = new byte[buffersize];
                    int bytesRead;
                    long totalBytes = new FileInfo(inputFilePath).Length;
                    long processedBytes = 0;
                    Console.WriteLine($"Decrypting file: {inputFilePath} ({totalBytes / 1024 / 1024:F2} MB)");
                    int progressBarWidth = 50;
                    Console.Write("[");
                    for (int i = 0; i < progressBarWidth; i++)
                    {
                        Console.Write(" ");
                    }
                    Console.Write("] 0%");

                    while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        cryptoStream.Write(buffer, 0, bytesRead);
                        processedBytes += bytesRead;
                        double progress = (double)processedBytes / totalBytes;
                        int progressBarPosition = (int)(progress * progressBarWidth);
                        Console.SetCursorPosition(1, Console.CursorTop);
                        Console.Write("[");
                        for (int i = 0; i < progressBarWidth; i++)
                        {
                            if (i < progressBarPosition)
                            {
                                Console.Write("=");
                            }
                            else if (i == progressBarPosition)
                            {
                                Console.Write(">");
                            }
                            else
                            {
                                Console.Write(" ");
                            }
                        }
                        Console.Write("] " + (int)(progress * 100) + "%");


                        //Console.WriteLine($"Encryption Progress: {(processedBytes * 100 / totalBytes)}%");
                    }
                }
            }
        }

        static byte[] GenerateRandomSalt()
        {
            byte[] salt = new byte[16]; // 128-bit salt
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        static void DecryptFile(string inputFilePath, string outputFilePath, string password)
        {
            using (FileStream inputStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            using (FileStream outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            {
                // Read the salt from the beginning of the encrypted file
                byte[] salt = new byte[16]; // 128-bit salt
                inputStream.Read(salt, 0, salt.Length);

                // Derive the key and IV from the password and salt
                byte[] key, iv;
                DeriveKeyAndIV(password, salt, out key, out iv);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    using (CryptoStream cryptoStream = new CryptoStream(inputStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        const int buffersize = 12000;
                        byte[] buffer = new byte[buffersize];
                        int bytesRead;
                        long totalBytes = new FileInfo(inputFilePath).Length;
                        long processedBytes = salt.Length; // Skip salt bytes for progress calculation
                        Console.WriteLine();
                        Console.WriteLine($"Decrypting file: {inputFilePath} ({totalBytes / 1024 / 1024:F2} MB)");
                        int progressBarWidth = 50;
                        Console.Write("[");
                        for (int i = 0; i < progressBarWidth; i++)
                        {
                            Console.Write(" ");
                        }
                        Console.Write("] 0%");

                        DateTime startTime = DateTime.Now;

                        while ((bytesRead = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            outputStream.Write(buffer, 0, bytesRead);
                            processedBytes += bytesRead;


                            double progress = (double)processedBytes / totalBytes;
                            int progressBarPosition = (int)(progress * progressBarWidth);
                            Console.SetCursorPosition(1, Console.CursorTop);
                            Console.Write("[");
                            for (int i = 0; i < progressBarWidth; i++)
                            {
                                if (i < progressBarPosition)
                                {
                                    Console.Write("=");
                                }
                                else if (i == progressBarPosition)
                                {
                                    Console.Write(">");
                                }
                                else
                                {
                                    Console.Write(" ");
                                }
                            }
                            Console.Write("] " + (int)(progress * 100) + "%");
                            //Console.WriteLine($"Decryption Progress: {(processedBytes * 100 / totalBytes)}%");
                        }
                    }
                }
            }
        }
        static void DeriveKeyAndIV(string password, byte[] salt, out byte[] key, out byte[] iv)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000)) // 100,000 iterations
            {
                key = pbkdf2.GetBytes(32); // 256-bit key
                iv = pbkdf2.GetBytes(16);  // 128-bit IV
            }
        }


        private static string SelectFile(string title)
        {
            try
            {
                using (var openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Title = title;
                    openFileDialog.Filter = "All Files (*.*)|*.*"; // Modify as needed for specific file types
                    openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        return openFileDialog.FileName;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, log it, or show a message to the user
                Console.WriteLine($"An error occurred while selecting the file: {ex.Message}");
                return null;
            }
        }
    }
}

