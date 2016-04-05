using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Drawing;
using System.Globalization;

namespace ImageSort
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime dt;
            DateTime defaultDT = default(DateTime);
            bool move = false;
            bool separateModified = true;
            bool sortByYear = true;
            bool sortByMonth = true;
            bool sortByDay = true;
            bool sortByHour = false;
            bool sortByMinute = false;
            bool sortBySecond = false;
            bool abbreviateMonths = true;
            string month, day;
            string sourcePath = @"E:\OneDrive\Pictures\Ben & Holly";
            string sourceFile;
            string outDir = @"E:\Test";
            string targetPath, destFile;
            string fileName;
            IEnumerable<string> files;
            //string fileName = "0508142358f.jpg";
            if (!Directory.Exists(sourcePath))
            {
                Console.WriteLine("Path does not exist: " + sourcePath);
                Console.ReadKey();
                return;
            }
            try
            {
                files = Directory
                    .EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories)
                    .Where(file =>
                        file.ToLower().EndsWith("jpg")  ||
                        file.ToLower().EndsWith("jpeg") ||
                        file.ToLower().EndsWith("png")  ||
                        file.ToLower().EndsWith("bmp")  ||
                        file.ToLower().EndsWith("dib")
                        )
                    .ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception occurred: ");
                Console.WriteLine(e.ToString());
                Console.ReadKey();
                return;
            }

            foreach (string file in files)
            {
                fileName = file.Substring(sourcePath.Length + 1);
                sourceFile = Path.Combine(sourcePath, fileName);
                targetPath = outDir;
                dt = DateTaken(sourceFile) ?? defaultDT;
                if (separateModified && dt.Equals(defaultDT))
                {
                    targetPath = Path.Combine(targetPath, "DateModified");
                    dt = File.GetLastWriteTime(sourceFile);
                }

                if (sortByYear)
                {
                    targetPath = Path.Combine(targetPath, dt.Year.ToString());
                }
                if (sortByMonth)
                {
                    if (abbreviateMonths)
                    {
                        month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(dt.Month);
                    }
                    else
                    {
                        month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dt.Month);
                    }
                    targetPath = Path.Combine(targetPath, month);
                }
                if (sortByDay)
                {
                    if (sortByMonth) { day = dt.Day.ToString(); }
                    else { day = dt.DayOfYear.ToString(); }
                    targetPath = Path.Combine(targetPath, day);
                }
                if (sortByHour)
                {
                    targetPath = Path.Combine(targetPath, dt.Hour.ToString());
                }
                if (sortByMinute)
                {
                    targetPath = Path.Combine(targetPath, dt.Day.ToString());
                }
                if (sortBySecond)
                {
                    targetPath = Path.Combine(targetPath, dt.Second.ToString());
                }
                Directory.CreateDirectory(targetPath);
                destFile = Path.Combine(targetPath, fileName);

                if (move)
                {
                    if (File.Exists(destFile)) { File.Delete(destFile); }
                    File.Move(sourceFile, destFile);
                }
                else { File.Copy(sourceFile, destFile, true); }
            }
        }

        public static DateTime? DateTaken(string imagePath)
        {
            Image getImage = Image.FromFile(imagePath);
            int DateTakenValue = 0x9003; //36867;

            if (!getImage.PropertyIdList.Contains(DateTakenValue))
                return null;

            string dateTakenTag = Encoding.ASCII.GetString(getImage.GetPropertyItem(DateTakenValue).Value);
            if (dateTakenTag.Contains("."))
            {
                return Convert.ToDateTime(dateTakenTag);
            }
            else
            {
                string[] parts = dateTakenTag.Split(':', ' ');
                int year = int.Parse(parts[0]);
                int month = int.Parse(parts[1]);
                int day = int.Parse(parts[2]);
                int hour = int.Parse(parts[3]);
                int minute = int.Parse(parts[4]);
                int second = int.Parse(parts[5]);

                return new DateTime(year, month, day, hour, minute, second);
            }
        }

        /// <summary>
        /// A safe way to get all the files in a directory and sub directory without crashing on UnauthorizedException or PathTooLongException
        /// </summary>
        /// <param name="rootPath">Starting directory</param>
        /// <param name="patternMatch">Filename pattern match</param>
        /// <param name="searchOption">Search subdirectories or only top level directory for files</param>
        /// <returns>List of files</returns>
        public static IEnumerable<string> GetDirectoryFiles(string rootPath, string patternMatch, SearchOption searchOption)
        {
            var foundFiles = Enumerable.Empty<string>();

            if (searchOption == SearchOption.AllDirectories)
            {
                try
                {
                    IEnumerable<string> subDirs = Directory.EnumerateDirectories(rootPath);
                    foreach (string dir in subDirs)
                    {
                        foundFiles = foundFiles.Concat(GetDirectoryFiles(dir, patternMatch, searchOption)); // Add files in subdirectories recursively to the list
                    }
                }
                catch (UnauthorizedAccessException) { }
                catch (PathTooLongException) { }
            }

            try
            {
                foundFiles = foundFiles.Concat(Directory.EnumerateFiles(rootPath, patternMatch)); // Add files from the current directory
            }
            catch (UnauthorizedAccessException) { }

            return foundFiles;
        }
    }
}
