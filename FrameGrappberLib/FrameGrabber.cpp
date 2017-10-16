#include "FrameGrabber.h"
#include <stdio.h>

CFrameGrabber::CFrameGrabber(void)
{
}


CFrameGrabber::~CFrameGrabber(void)
{
}

FG_ERRORS CFrameGrabber::Start()
{
	 
#if 0 
    if (getPath)
    {
        // get file path...
        char* path = getPath("Text Files|*.txt");
        // open the file for reading
        FILE *file = fopen(path, "r");
        // read buffer
        char line[1024];
 
        // print file info to the screen
        printf("File path: %s\n", path ? path : "N/A");
        printf("File content:\n");
 
        while(fgets(line, 1024, file) != NULL)
        {
            printf("%s", line);
        }
 
        // close the file
        fclose(file);
		return FG_OK;
	}   
	#endif 

	return FG_ERR;
}
