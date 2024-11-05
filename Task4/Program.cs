namespace Task4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // создаём список студентов из файла
            List<Student> studentsToRead = ReadStudentsFromBinFile("students.dat");
            
            // читаем исходные данные полученые из файла
            foreach (Student studentProp in studentsToRead)
            {
                Console.WriteLine(studentProp.Name + " " + studentProp.Group + " " + studentProp.DateOfBirth + " " + studentProp.AverageScore);
            }

            // обрабатываем и записываем данные студентов
            ProccessAndWriteStudentsFiles(studentsToRead);
        }

        private static void ProccessAndWriteStudentsFiles(List<Student> studentsToRead)
        {
            // группируем студентов по группам
            var studentsGroups = studentsToRead.GroupBy(s => s.Group);

            // получаем путь к Рабочему столу пользователя и указание адреса новой паки
            var dirName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Students");

            // создаём папку Students на Рабочем столе
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
            else
            {
                Console.WriteLine($"Каталог {dirName} уже существует.");
            }

            // перебор групп студентов
            foreach (var studentGroup in studentsGroups)
            {
                Console.WriteLine($"Создаём текстовый файл для группы {studentGroup.Key}");

                // создаём файл группы с полной перезаписью файла
                using (StreamWriter writer = new StreamWriter($"{dirName}{Path.DirectorySeparatorChar}{studentGroup.Key}.txt", false))
                {
                    // записываем данные студентов в текстовый файл группы
                    foreach (var student in studentGroup)
                    {
                        writer.WriteLine($"{student.Name}, {student.DateOfBirth}, {student.AverageScore}");
                    }
                    
                }
            }
        }

        // метод возвращающий список студентов из файла
        static List<Student> ReadStudentsFromBinFile(string fileName)
        {
            List<Student> result = new();
            using FileStream fs = new FileStream(fileName, FileMode.Open);
            using StreamReader sr = new StreamReader(fs);

            fs.Position = 0;

            BinaryReader br = new BinaryReader(fs);

            while (fs.Position < fs.Length)
            {
                Student student = new Student();
                student.Name = br.ReadString();
                student.Group = br.ReadString();
                long dt = br.ReadInt64();
                student.DateOfBirth = DateTime.FromBinary(dt);
                student.AverageScore = br.ReadDecimal();

                result.Add(student);
            }

            fs.Close();
            return result;
        }
    }

    internal class Student
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public DateTime DateOfBirth { get; set; }
        public decimal AverageScore { get; set; }
    }
}
