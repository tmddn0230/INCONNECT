#pragma once

#include <Windows.h>

class ICVMonitor
{
protected:
	HANDLE d_mutex;

public:
	ICVMonitor(void);
	ICVMonitor(char* name);
	~ICVMonitor(void);

	void ON() const;
	void OFF() const;

};

