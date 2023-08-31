namespace album_photo_web_api.Helper
{
    public class CommonPath
    {
        public static string GetCurrentDirectory()
        {
            var result = Directory.GetCurrentDirectory();
            return result;
        }

        public static string GetStaticContentDirectory()
        {
            var result = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Data\\Photos\\");
            if (!Directory.Exists(result))
                Directory.CreateDirectory(result);
            return result;
        }

        public static string GetFilePath(string fileName)
        {
            var getStaticContentDirectory = GetStaticContentDirectory();
            var result = System.IO.Path.Combine(getStaticContentDirectory, fileName);
            return result;
        }





    }
}
