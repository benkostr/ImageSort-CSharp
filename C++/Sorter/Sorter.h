#pragma once
class Sorter
{
public:
    Sorter();
    ~Sorter();
    bool setMove, sortNonExif, separateNonExif, sortByYear, sortByMonth;
    bool sortbyDay, sortByHour, sortByMinute, sortBySecond, abbreviateMonths;
    string sourcePath, outDir;
private:
    string filename, sourceFile, month, day, targetPath, destFile;

};

