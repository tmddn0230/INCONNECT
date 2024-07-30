#include "ICGlobal.h"

HWND gMainWnd = NULL;
HWND gListboxWindow = NULL;
HFONT gArialFont = NULL;

int gUserper = 0; 
int MAX_QUEUE = 0;
int MAX_THREAD = 0;
int gbUseUDP = 1;

char gHostName[256];
char gIP[20];
int gServerPort = 0;

SOCKET gServerSocket = NULL;
HANDLE gServerHandle = NULL;

WSAEVENT gEvent[MAX_USER];
HANDLE gUserHandle[MAX_USER];
int gServerNum = 0;

int gUDPPort = 0;

UserManager g_User;

int logcount = 0;

void Log(const char* format, ...)
{
	va_list argptr;
	char msg[4096]; 
	memset(msg, 0x00, sizeof(msg));
	va_start(argptr, format);
	vsprintf(msg, format, argptr);
	va_end(argptr);

	// SendMessage ( gListboxWindow , LB_ADDSTRING, 0 , (LPARAM)msg );
	int count = 0;
	count = SendMessage(gListboxWindow, LB_GETCOUNT, 0, 0);
	if (count > 1000)
	{
		SendMessage(gListboxWindow, LB_RESETCONTENT, 0, 0);
		logcount++;
	}

	SendMessage(gListboxWindow, LB_INSERTSTRING, 0, (LPARAM)msg);
	char buffer[1024];
	memset(buffer, 0x00, sizeof(buffer));
	sprintf(buffer, "Log\\Log%d.txt", logcount);
	DebugLog(buffer, "%s\n", msg);
}

void DebugLog(char* filename, const char* format, ...)
{
	va_list argptr;
	char msg[10000];
	memset(msg, 0x00, sizeof(msg));
	va_start(argptr, format);
	vsprintf(msg, format, argptr);
	va_end(argptr);

	FILE* fp;
	fp = fopen(filename, "at");
	fprintf(fp, msg);
	fclose(fp);
}

std::string AnsiToUtf8(std::string strAnsi)
{
	std::string ret;
	if (strAnsi.length() <= 0) // strAnsi invalid? 
		return ret;

	int nWideStrLength = MultiByteToWideChar(CP_ACP, 0, strAnsi.c_str(), -1, NULL, 0); // CP_ACP : Code Page _ Ansi Code Page
	WCHAR* pwszBuf = new WCHAR[nWideStrLength + 1];

	memset(pwszBuf, 0, nWideStrLength + 1); // Null?

	MultiByteToWideChar(CP_ACP, 0, strAnsi.c_str(), -1, pwszBuf, nWideStrLength + 1);
	int nUtf8Length = WideCharToMultiByte(CP_UTF8, 0, pwszBuf, -1, NULL, 0, NULL, FALSE);

	char* pszUtf8Buf = new char[nUtf8Length + 1];
	memset(pszUtf8Buf, 0, nUtf8Length + 1);

	WideCharToMultiByte(CP_UTF8, 0, pwszBuf, -1, pszUtf8Buf, nUtf8Length + 1, NULL, FALSE);

	ret = pszUtf8Buf;

	SAFE_DELETE_ARRAY(pszUtf8Buf);
	SAFE_DELETE_ARRAY(pwszBuf);

	return ret;
}

std::string Utf8ToAnsi(std::string strUTF8)
{
	std::string ret;
	if (strUTF8.length() <= 0)
		return ret;

	int nWideStrLength = MultiByteToWideChar(CP_UTF8, 0, strUTF8.c_str(), -1, NULL, 0);

	WCHAR* pwszBuf = new WCHAR[nWideStrLength + 1];

	memset(pwszBuf, 0, nWideStrLength + 1);
	MultiByteToWideChar(CP_UTF8, 0, strUTF8.c_str(), -1, pwszBuf, nWideStrLength + 1);

	int nAnsiStrLength = WideCharToMultiByte(CP_ACP, 0, pwszBuf, -1, NULL, 0, NULL, FALSE);

	char* pszAnsiBuf = new char[nAnsiStrLength + 1];
	memset(pszAnsiBuf, 0, nAnsiStrLength + 1);

	WideCharToMultiByte(CP_ACP, 0, pwszBuf, -1, pszAnsiBuf, nAnsiStrLength + 1, NULL, FALSE);
	ret = pszAnsiBuf;

	SAFE_DELETE_ARRAY(pszAnsiBuf);
	SAFE_DELETE_ARRAY(pwszBuf);

	return ret;
}

std::wstring Utf8ToWchar(std::string strAnsi)
{
	std::wstring ret;
	if (strAnsi.length() <= 0)
		return ret;

	int nWideStrLength = MultiByteToWideChar(CP_UTF8, 0, strAnsi.c_str(), -1, NULL, 0);

	WCHAR* pwszBuf = new WCHAR[nWideStrLength + 1];
	memset(pwszBuf, 0, nWideStrLength + 1);

	MultiByteToWideChar(CP_UTF8, 0, strAnsi.c_str(), -1, pwszBuf, nWideStrLength + 1);

	ret = pwszBuf;

	SAFE_DELETE_ARRAY(pwszBuf);
	return ret;
}

std::string WcharToAnsi(std::wstring strWchar)
{
	std::string ret;
	if (strWchar.length() <= 0)
		return ret;

	int iLength = WideCharToMultiByte(CP_ACP, 0, strWchar.c_str(), -1, NULL, 0, NULL, FALSE);

	char* pChar = new char[iLength + 1];
	memset(pChar, 0, iLength + 1);

	WideCharToMultiByte(CP_ACP, 0, strWchar.c_str(), -1, pChar, iLength + 1, NULL, FALSE);
	ret = pChar;

	SAFE_DELETE_ARRAY(pChar);

	return ret;
}

std::string WcharToUtf8(std::wstring strWchar)
{
	std::string ret;
	if (strWchar.length() <= 0)
		return ret;

	int iLength = WideCharToMultiByte(CP_UTF8, 0, strWchar.c_str(), -1, NULL, 0, NULL, FALSE);

	char* pChar = new char[iLength + 1];
	memset(pChar, 0, iLength + 1);

	WideCharToMultiByte(CP_UTF8, 0, strWchar.c_str(), -1, pChar, iLength + 1, NULL, FALSE);
	ret = pChar;

	SAFE_DELETE_ARRAY(pChar);

	return ret;
}
