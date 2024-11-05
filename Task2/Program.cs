namespace Task2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var path = $@".{Path.DirectorySeparatorChar}Test";

            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);

                if (directoryInfo.Exists)
                {
                    Console.WriteLine($"Размер каталога {path}: {GetSize(directoryInfo)}");
                }
                else
                {
                    Console.WriteLine("Каталог не найден.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        // метод возвращающий размер каталога в байтах
        static long GetSize(DirectoryInfo dir)
        {
            if (dir == null || !dir.Exists)
            {
                throw new DirectoryNotFoundException("Каталог не найден.");
            }

            long size = 0;

            try
            {
                size += dir.GetFiles().Sum(file => file.Length);
                size += dir.GetDirectories().Sum(GetSize);
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine($"Ошибка доступа к каталогу {dir}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обрабатывания каталога {dir}:");
                Console.WriteLine(ex.Message);
            }

            return size;
        }
    }
}
