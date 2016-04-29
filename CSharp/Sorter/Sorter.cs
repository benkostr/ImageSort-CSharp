using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Drawing;
using System.Globalization;

namespace Sorter
{
    public class Sorter
    {
        public bool move { get; set; }
        public bool sortNonExif { get; set; }
        public bool separateNonExif { get; set; }
        public bool sortByYear { get; set; }
        public bool sortByMonth { get; set; }
        public bool sortByDay { get; set; }
        public bool sortByHour { get; set; }
        public bool sortByMinute { get; set; }
        public bool sortBySecond { get; set; }
        public bool abbreviateMonths { get; set; }
        public string sourcePath { get; set; }
        public string outDir { get; set; }

        private DateTime dt;
        private DateTime defaultDT = default(DateTime);
        private IEnumerable<string> files;
        private string fileName, sourceFile;
        private string month, day;
        private string targetPath, destFile;

        public Sorter(string sourcePath, string outDir)
        {
            this.move = false;
            this.sortNonExif = true;
            this.separateNonExif = true;
            this.sortByYear = true;
            this.sortByMonth = true;
            this.sortByDay = true;
            this.sortByHour = false;
            this.sortByMinute = false;
            this.sortBySecond = false;
            this.abbreviateMonths = true;
            this.sourcePath = sourcePath;
            this.outDir = outDir;
        }

        public void Sort()
        {
            files = Directory
                .EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories)
                .Where(file =>
                    file.ToLower().EndsWith("jpg") ||
                    file.ToLower().EndsWith("jpeg") ||
                    file.ToLower().EndsWith("png") ||
                    file.ToLower().EndsWith("bmp") ||
                    file.ToLower().EndsWith("dib")
                    )
                .ToList();

            foreach (string file in files)
            {
                fileName = file.Substring(sourcePath.Length + 1);
                sourceFile = Path.Combine(sourcePath, fileName);
                targetPath = outDir;
                dt = DateTaken(sourceFile) ?? defaultDT;
                if (dt.Equals(defaultDT))
                {
                    if (!sortNonExif)
                    {
                        continue;
                    }
                    else if (separateNonExif)
                    {
                        targetPath = Path.Combine(targetPath, "NonEXIF");
                        dt = File.GetLastWriteTime(sourceFile);
                    }
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
                GC.Collect();
            }
        }

        private static DateTime? DateTaken(string imagePath)
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
    }
}
