namespace CollisionChecker
{
    public interface IFilePathUtilities
    {
        bool CheckExistence(string filePath);
        int getFileTypeByExtension(string extension);
    }
}