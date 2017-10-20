#include "IPInterface.h"
#include <stdio.h>


DLL void IPStartProcess()
{

}

DLL int IPGetResult(float *result)
{
	*result = 24;
	return 1;
}

DLL void IPSetRowData(unsigned char *data, int size)
{
	FILE *handle = fopen("c:\\1.bin" , "w+b");
	fwrite(data , size , 1 , handle);
	fclose(handle);
}

DLL void  IPClose()
{

}

DLL void  IPStop()
{

}