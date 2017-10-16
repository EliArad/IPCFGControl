#include "FGInterface.h"
#include <stdio.h>
 
 
DWORD   dwThreadIdArray;
HANDLE  hThreadArray; 

int pData[2];

ProgressCallback m_progressCallback;

DWORD WINAPI MyThreadFunction( LPVOID lpParam ) 
{
	 Sleep(pData[1] * 1000);

	if (m_progressCallback)
    {
        // send progress update
        m_progressCallback(192);
    } 

	return 0;

}

DLL void DoWork(ProgressCallback progressCallback, int num1, int num2)
{
    int counter = 0;

	pData[0] = num1;
	pData[1] = num2;

	m_progressCallback = progressCallback;

	hThreadArray = CreateThread( 
            NULL,                   // default security attributes
            0,                      // use default stack size  
            MyThreadFunction,       // thread function name
            NULL,					// argument to thread function 
            0,                      // use default creation flags 
            &dwThreadIdArray);      // returns the thread identifier 
	  
}
 
DLL void FGClose()
{

}

DLL void FGStop()
{

}
 
