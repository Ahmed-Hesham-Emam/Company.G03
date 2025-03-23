namespace Company.G03.PL.Helpers
    {
    public class AttachmentSettings
        {

        public static string UploadFile(IFormFile file, string FolderName)
            {
            if (string.IsNullOrWhiteSpace(FolderName))
                throw new ArgumentException("File name cannot be null or empty."); // Check if the folder name is null or empty

            if (!File.Exists(FolderName))
                File.Create(FolderName).Dispose(); // Create a folder to save the file

            var FolderPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Files", FolderName); // Path to the folder where the file will be saved

            var FileName = $"{Guid.NewGuid()}{file.FileName}"; // Generate a unique file name for the file to be saved

            var FilePath = Path.Combine(FolderPath, FileName); // Path to the file to be saved

            using var FileStream = new FileStream(FilePath, FileMode.Create); // Create a file stream to save the file

            file.CopyTo(FileStream); // Copy the file to the file stream

            return FileName;
            }

        public static void DeleteFile(string FolderName, string FileName)
            {
            var FilePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Files", FolderName, FileName); // Path to the file to be deleted

            if (File.Exists(FilePath)) // Check if the file exists
                File.Delete(FilePath); // Delete the file
            }


        }
    }
