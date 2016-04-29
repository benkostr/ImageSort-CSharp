using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
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

        private List<string>[] partitionedFiles;
        private List<Thread> threads;
        private int numProcs = Environment.ProcessorCount;
        private int numThreads;
        private IEnumerable<string> files;

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

        public void Start()
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

            // Number of threads equals number of logical processors unless
            // there are less files than logical processes.
            numThreads = (numProcs <= files.Count()) ? numProcs : files.Count();
            partitionedFiles = PartitionList(files.ToList<string>(), numThreads);

            threads = new List<Thread>();
            for (int i = 0; i < numThreads; i++)
            {
                threads.Add(new Thread(new ParameterizedThreadStart(Sort)));
            }
            for (int i = 0; i < numThreads; i++) { threads[i].Start(partitionedFiles[i]); }
            for (int i = 0; i < numThreads; i++) { threads[i].Join(); }
            threads.Clear();
        }

        public static List<T>[] PartitionList<T>(List<T> list, int totalPartitions)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (totalPartitions < 1)
                throw new ArgumentOutOfRangeException("totalPartitions");

            List<T>[] partitions = new List<T>[totalPartitions];

            int maxSize = (int)Math.Ceiling(list.Count / (double)totalPartitions);
            int k = 0;

            for (int i = 0; i < partitions.Length; i++)
            {
                partitions[i] = new List<T>();
                for (int j = k; j < k + maxSize; j++)
                {
                    if (j >= list.Count)
                        break;
                    partitions[i].Add(list[j]);
                }
                k += maxSize;
            }

            return partitions;
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

        private void Sort(object imageFiles)
        {
            DateTime dt;
            DateTime defaultDT = default(DateTime);
            string fileName, sourceFile;
            string month, day;
            string targetPath, destFile;
            foreach (string file in (List<string>)imageFiles)
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
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                GC.WaitForPendingFinalizers();
            }
        }
    }
}
