#pragma once
 

#ifdef _MSC_VER
#define _CRT_SECURE_NO_WARNINGS
#endif

#include "Windows.h"
#ifdef __cplusplus
extern "C"
{
#endif
 
    #define DLL __declspec(dllexport)
    typedef void (__cdecl * ProgressCallback)(unsigned char *buffer, int length);
    DLL void DoWork(ProgressCallback progressCallback, int num1, int num2);

	DLL void FGClose();
	DLL void FGStop();
    
 
#ifdef __cplusplus
}
#endif