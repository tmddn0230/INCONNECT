#pragma once

#pragma warning ( disable : 4244 )
#pragma warning ( disable : 4800 )
#pragma warning ( disable : 4996 )
#pragma warning ( disable : 4018 )
#pragma warning ( disable : 4065 )

#include <vector>
#include <map>

#include <winsock2.h>
#include <windows.h>
#include <mmsystem.h>
#include <time.h>
#include <process.h>
#include <commctrl.h>
#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include <deque>

#include <queue>
#include <string>

#include <tchar.h>
#include <wininet.h >
#include <WinUser.h>
#include <direct.h>
#include "framework.h"
#include "ICVoiceServer.h"

#include "ICVProtocol.h"
#include "ICVPacket.h"
#include "ICVDefine.h"	//
#include "ICVThread.h"
#include "ICVMonitor.h"
#include "UDPServer.h"
#include "User.h"
#include "UserManager.h"

#include <iostream>
#include <fstream>
#include <sstream>
#include <codecvt>
#include <boost/locale.hpp>
#include "ICVStruct.h"


//---------------------------------ÀýÃë¼±------------------------------
using namespace std;
using std::vector;
using std::queue;
using std::string;

#include "Global.h"