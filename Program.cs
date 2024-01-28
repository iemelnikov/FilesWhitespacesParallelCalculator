using System.Diagnostics;

namespace FilesWhitespacesParallelCalculator
{
    public class Program
    {
        static void Main(string[] args)
        {
            string tempFolderPath =  Path.GetTempPath(); // папка Temp для поиска файлов
            Console.WriteLine($"Folder for calculation whitespaces count in files:{Environment.NewLine}{tempFolderPath}");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            var stopwatch = new Stopwatch();
            int[] filesCountsToProcess = { 3, 0 };
            // Последовательно вычисляем кол-во пробелов для 3 случайных файлов в папке, а затем для всех файлов
            for (int i = 0; i < filesCountsToProcess.Length; i++)
            {
                stopwatch.Reset();
                stopwatch.Start();
                int filesCountToProcess = filesCountsToProcess[i];
                CalculateWhitespacesCountForFiles(tempFolderPath, "*.*", filesCountToProcess);
                stopwatch.Stop();
                Console.WriteLine($"Summary tasks execution time for processing {(filesCountToProcess > 0 ? $"{filesCountToProcess} random" : "all")} files in folder: {stopwatch.ElapsedMilliseconds} ms");
                Console.WriteLine($"Press Enter to {(i == filesCountsToProcess.Length - 1 ? "finish" : "continue..")}.");
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Функция, вычисляющая количество пробелов в файлах параллельно
        /// </summary>
        /// <param name="targetFilesDirectoryPath">Путь к папке с файлами</param>
        /// <param name="filesSearchPattern">Маска фильтрации по имени и расширению для поиска файлов</param>
        /// <param name="filesCountToProcess">Количество файлов для обработки. При значении 0 анализируются все файлы</param>
        static void CalculateWhitespacesCountForFiles(string targetFilesDirectoryPath, string filesSearchPattern, int filesCountToProcess)
        {
            IEnumerable<string> filesPathsToProcess = Directory.EnumerateFiles(targetFilesDirectoryPath, filesSearchPattern, SearchOption.AllDirectories);
            if (filesCountToProcess > 0)
            {
                var rnd = new Random();
                filesPathsToProcess = filesPathsToProcess.OrderBy(_ => rnd.Next()).Take(filesCountToProcess); // Получаем в случайном порядке n файлов
            }
            var tasksToCalculateFileWhitespacesCount = new List<Task>();
            foreach (string filePathToProcess in filesPathsToProcess)
            {
                var task = Task.Run(() =>
                {
                    try
                    {
                        string fileContent = File.ReadAllText(filePathToProcess); // Содержимое файла в текстовом виде
                        int whitespacesCount = fileContent.Count(_ => _ == ' '); // Количество пробелов в файле
                        Console.WriteLine($"Whitespaces count for file {filePathToProcess}: {whitespacesCount}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"The file {filePathToProcess} was skipped due to the following error:{Environment.NewLine}{ex.Message}");
                    }
                });
                tasksToCalculateFileWhitespacesCount.Add(task);
            }
            Task.WaitAll(tasksToCalculateFileWhitespacesCount.ToArray());
        }
    }
}
