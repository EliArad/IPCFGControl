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
	typedef void(__cdecl * ProgressCallback)(unsigned char *buffer, int length);
	DLL void IPStartProcess();

	DLL int  IPGetResult(float *result);
	DLL void IPSetRowData(unsigned char *data, int size);
	DLL void  IPClose();
	DLL void  IPStop();


#ifdef __cplusplus
}
#endif