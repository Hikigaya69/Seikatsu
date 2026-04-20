namespace Seikatsu.Backend.Services
{
    public class SupabaseStorageService : IStorageService
    {
        private readonly Supabase.Client _supabase;
        private readonly string _bucketName;

        public SupabaseStorageService(Supabase.Client supabase, IConfiguration config)
        {
            _supabase = supabase;
            _bucketName = config["Supabase:BucketName"];
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder)
        {
            // Convert file to bytes
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            // Generate unique file name
            var fileName = $"{folder}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            // Upload to Supabase bucket
            await _supabase.Storage
                .From(_bucketName)
                .Upload(fileBytes, fileName, new Supabase.Storage.FileOptions
                {
                    ContentType = file.ContentType,
                    Upsert = false
                });

            // Return public URL
            var publicUrl = _supabase.Storage
                .From(_bucketName)
                .GetPublicUrl(fileName);

            return publicUrl;
        }
    }
}
