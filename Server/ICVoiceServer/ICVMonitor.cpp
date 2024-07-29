#include "ICVMonitor.h"

ICVMonitor::ICVMonitor(void)
{
	d_mutex = CreateMutexA(NULL, false, NULL);
	if (d_mutex == NULL)
		throw MessageBox(NULL, L" Mutex creation failed.", NULL, MB_OK);
}

ICVMonitor::ICVMonitor(char* name)
{
	d_mutex = CreateMutexA(NULL, false, name);
	if (d_mutex == NULL)
		throw MessageBox(NULL, L"Monitor() - Mutex creation failed.", NULL, MB_OK);
}

ICVMonitor::~ICVMonitor(void)
{
	if (d_mutex != NULL)
	{
		CloseHandle(d_mutex);
		d_mutex = NULL;
	}
}

void ICVMonitor::ON() const
{
	WaitForSingleObject(d_mutex, INFINITE);
}

void ICVMonitor::OFF() const
{
	ReleaseMutex(d_mutex);
}
