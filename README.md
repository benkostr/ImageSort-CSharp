# ImageSort
Sort images by date with EXIF data

There is a (quicker) version in C++ with Qt5 that is also being developed for Linux: [ImageSort-Cpp](https://github.com/benkostr/ImageSort-Cpp).

### Features
- Sorts images into subdirectories based on date taken
- Options for month, day, hour, minute, and second
- Can copy or move files
- Optionally sorts non-EXIF file formats by date modified into a separate folder
- Written in C# with WPF
- 64bit

### Dependencies
- .NET 4.6

### Building
1. Open .sln in Visual Studio.
2. Compile solution for "x64" or for "Any CPU"
  - This will build a shared library and an executable into the binary output directory
3. Run ImageSort.exe

### To Do
- Write Documentation
- Comment Code
- Fix manual garbage collection calls
