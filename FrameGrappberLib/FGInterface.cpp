#include "FGInterface.h"
#include "FrameGrabber.h"
#include <stdio.h>
 
 
DWORD   dwThreadIdArray;
HANDLE  hThreadArray; 

int pData[2];

ProgressCallback m_progressCallback;
int m_stop = 1;
#define FRAME_SIZE 1024 * 768 * 4

unsigned char frameBuffer[FRAME_SIZE];

DWORD WINAPI MyThreadFunction( LPVOID lpParam ) 
{
	Sleep(pData[1] * 1000);

	for (int i = 0; i < FRAME_SIZE; i++)
	{
		frameBuffer[i] = i;
	}
	if (m_stop == 1)
		return 0;

	if (m_progressCallback)
    {
        // send progress update
		m_progressCallback(frameBuffer, FRAME_SIZE);
    } 

	return 0;

}

DLL void DoWork(ProgressCallback progressCallback, int num1, int num2)
{
    int counter = 0;

	m_stop = 0;

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
	m_stop = 1;
}
 
