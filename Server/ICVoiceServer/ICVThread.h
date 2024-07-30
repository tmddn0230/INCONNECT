#pragma once

#include "ICVMonitor.h"

class ICVThread : public ICVMonitor
{
public:
	DWORD mThreadID;
	bool bRun;
	HANDLE mThreadHandle;

};

