#pragma once

typedef enum _FG_ERRORS
{
	FG_ERR,
	FG_OK
} FG_ERRORS;

class CFrameGrabber
{
public:
	CFrameGrabber(void);
	~CFrameGrabber(void);

	FG_ERRORS Start();
};

