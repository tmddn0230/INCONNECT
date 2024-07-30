#pragma once

#include <SDKDDKVer.h>
#define WIN32_LEAN_AND_MEAN             // 거의 사용되지 않는 내용을 Windows 헤더에서 제외합니다.
// Windows 헤더 파일
#include <windows.h>
// C 런타임 헤더 파일입니다.
#include <stdlib.h>
#include <malloc.h>
#include <memory.h>
#include <tchar.h>

#include <winsock2.h>
#pragma comment(lib, "ws2_32")
//::TranmitFile() 함수를 사용하기 위한 헤더와 라이브러리 설정
#include <Mswsock.h>
#include <windows.h>
#include <list>
#include <iterator>
#pragma comment(lib, "Mswsock")

#include <string>
#include <vector>
#include <map>
#include <queue>

