﻿namespace Task3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var path = $@".{Path.DirectorySeparatorChar}Test";
            var subpath = "Test";
            var fileName = "TestFile";

            var deletedCounter = 0;
            
            const int deleteAfter = 30;

            try
            {
                // создаём стартовую иерархию каталогов и файлов
                CreateTestDirectoryAndFiles(ref path, ref subpath, ref fileName);

                DirectoryInfo directoryInfo = new DirectoryInfo(path);

                if (directoryInfo.Exists)
                {
                    Console.WriteLine($"Исходный размер каталога {directoryInfo.Name}: {GetSize(directoryInfo)} байт");

                    // получаем список вложеных папок в корневой каталог
                    DirectoryInfo[] dirsInfo = directoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);

                    // проверяем папки и файлы на срок годности рекурсивно, при необходимости удаляем
                    CheckAndDelete(deleteAfter, dirsInfo, 0, false, ref deletedCounter);

                    // получаем список файлов в каталоге их, если просрочены - удаляем
                    FileInfo[] filesInfo = directoryInfo.GetFiles("*");

                    foreach (FileInfo f in filesInfo)
                    {
                        var fileToDelete = f.LastAccessTime + TimeSpan.FromMinutes(deleteAfter) <= DateTime.Now;

                        Console.WriteLine($"{f.Name}\t {f.LastAccessTime}\t (будет ли удалён?: {(fileToDelete ? "да" : "нет")})");

                        if (fileToDelete)
                        {
                            f.Delete();
                            deletedCounter++;
                        }
                    }

                    Console.WriteLine($"Текущий размер каталога {directoryInfo.Name}: {GetSize(directoryInfo)} байт. Удалено {deletedCounter} файлов.");
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

        // метод для поиска всех папок (включая вложеные) и файлов в этих папках
        // при необходимости удаляем
        private static void CheckAndDelete(int deleteAfter, DirectoryInfo[] dirsInfo, byte deep, bool isLastDirectoryToDelete, ref int delConter)
        {
            foreach (DirectoryInfo d in dirsInfo)
            {
                var directoryToDelete = d.LastAccessTime + TimeSpan.FromMinutes(deleteAfter) <= DateTime.Now && isLastDirectoryToDelete;

                if (deep >= 0)
                {
                    Console.WriteLine($"{new('\t', count: deep)}{d.Name}\\\t {d.LastAccessTime}\t (будет ли удалён?: {(directoryToDelete ? "да" : "нет")})");
                }

                CheckAndDelete(deleteAfter, d.GetDirectories("*", SearchOption.TopDirectoryOnly), ++deep, directoryToDelete, ref delConter);

                deep = 0;

                FileInfo[] fInfo = d.GetFiles("*");

                foreach (FileInfo f in fInfo)
                {
                    var fileToDelete = f.LastAccessTime + TimeSpan.FromMinutes(deleteAfter) <= DateTime.Now;

                    Console.WriteLine($"{new('\t', count: deep + 1)}{f.Name}\t {f.LastAccessTime}\t (будет ли удалён?: {(fileToDelete ? "да" : "нет")})");

                    if (fileToDelete)
                    {
                        f.Delete();
                        Console.WriteLine($"{f.FullName} удалён");
                        delConter++;
                    }
                }

                if (directoryToDelete)
                {
                    d.Delete();
                    Console.WriteLine($"{d.FullName} удалёна");
                }
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

        // вспомогательный метод для создания иерархии папок и файлов
        static void CreateTestDirectoryAndFiles(ref string path, ref string subpath, ref string fileName)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            while (true)
            {
                if (directoryInfo.Exists)
                {
                    var userInput = "";

                    do
                    {
                        Console.WriteLine($"Папка {directoryInfo.Name} уже существует. Удалить эту папку со всем содержимым? (y/n)");
                        userInput = Console.ReadLine().ToLower();
                    } while (userInput != "y" && userInput != "n");

                    if (userInput == "y")
                    {
                        Console.WriteLine($"Удаление папки {directoryInfo.Name} со всем содержимым.");
                        directoryInfo.Delete(true);
                    }
                    else if (userInput == "n")
                    {
                        Console.WriteLine($"Удаление папки {directoryInfo.Name} со всем содержимым отменено.");
                        break;
                    }

                }
                else
                {
                    directoryInfo.Create();

                    using (FileStream fs = File.Create($@"{path}{Path.DirectorySeparatorChar}{fileName}.dat")) { }

                    for (int i = 1; i <= 3; i++)
                    {
                        directoryInfo.CreateSubdirectory($"{subpath}{i}");
                        using (FileStream fs = File.Create($@"{path}{Path.DirectorySeparatorChar}{subpath}{i}{Path.DirectorySeparatorChar}{fileName}{subpath}{i}.dat")) { }

                    }

                    Console.WriteLine($"Папка {directoryInfo.Name} создана.");
                    break;
                }
            }
        }
    }
}