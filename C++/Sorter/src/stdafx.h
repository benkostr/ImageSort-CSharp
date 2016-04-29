// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//
// I've made this a forced included file
// (under project properties/Advanced in visual studio).
// This means that the compiler will automatically include this file
// at the beginning of every other file in the project.
//

#pragma once

#ifdef _MSC_VER
#define _BIND_TO_CURRENT_VCLIBS_VERSION 1
#include "targetver.h"
#endif

#ifdef _M_X64
#ifdef _DEBUG
//#pragma comment(lib, "additional_Library_x64_Debug.lib")
#else
//#pragma comment(lib, "additional_Library_x64_Release.lib")
#endif
#else
#ifdef _DEBUG
//#pragma comment(lib, "additional_Library_x86_Debug.lib")
#else
//#pragma comment(lib, "additional_Library_x86_Release.lib")
#endif
#endif

#include <iostream>
#include <fstream>
#include <iomanip>
#include <string>
#include <vector>
//#include <tchar.h>
//#include <stdio.h>
#include "../../include/easyexif/exif.h"

using namespace std;


// TODO: reference additional headers your program requires here
