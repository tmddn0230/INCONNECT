#pragma once

#include "ICInclude.h"
#include "ICFramework.h"
#include "UserManager.h"

void Init();

BOOL WinSockStart();
UserManager gUser;

unsigned __stdcall ServerThread(void* pArg);
unsigned __stdcall UserThread(void* pArg);