#include <Windows.h>
#include <iostream>

void LoadHook();
void WinEventproc(HWINEVENTHOOK, DWORD, HWND, LONG, LONG, DWORD, DWORD);

typedef void (*OnSwitchProc)(LPVOID lpName, LPVOID lpPath);

OnSwitchProc onSwitch;
FARPROC initService;

WCHAR lpPathBuffer[MAX_PATH];
WCHAR lpCmdBuffer[44];

int main()
{
	HMODULE hLibService = LoadLibrary(L"TimeuseService.dll");
	if (hLibService == NULL) return 1;

	onSwitch = (OnSwitchProc)GetProcAddress(hLibService, "OnSwitch");
	initService = GetProcAddress(hLibService, "InitService");

	if (!initService || !onSwitch) return 1;

	initService();
	LoadHook();

	MSG msg = { };

	while (GetMessage(&msg, NULL, 0, 0) > 0)
	{
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}

	return 0;
}

void LoadHook()
{
	auto hModule = GetModuleHandle(nullptr);
	SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, NULL, &WinEventproc, 0, 0, WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);
}

void WinEventproc(HWINEVENTHOOK hWinEventHook, DWORD event, HWND hwnd, LONG idObject, LONG idChild, DWORD idEventThread, DWORD dwmsEventTime)
{
	DWORD dwProcessId;
	GetWindowThreadProcessId(hwnd, &dwProcessId);
	auto hProcess = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, true, dwProcessId);

	DWORD dwPathLength = MAX_PATH;
	QueryFullProcessImageName(hProcess, 0, lpPathBuffer, &dwPathLength);

	DWORD dwFileVersionInfoSize = GetFileVersionInfoSize(lpPathBuffer, nullptr);

	BYTE* lpVersionInfoBuffer = new BYTE[dwFileVersionInfoSize];
	GetFileVersionInfo(lpPathBuffer, NULL, dwFileVersionInfoSize, lpVersionInfoBuffer);

	LPVOID lpbuffer;
	UINT bufferLeng;
	BOOL success = VerQueryValue(lpVersionInfoBuffer, L"\\VarFileInfo\\Translation", &lpbuffer, &bufferLeng);
	
	DWORD dwLang = 0x040904E4;
	if (success) 
	{
		dwLang = *((LPDWORD)lpbuffer);
		dwLang = ((dwLang & 0xffff) << 16) | (dwLang >> 16);
	}

	swprintf_s(lpCmdBuffer, L"\\\\StringFileInfo\\\\%08x\\\\FileDescription", dwLang);

	success = VerQueryValue(lpVersionInfoBuffer, lpCmdBuffer, &lpbuffer, &bufferLeng);
	if (!success)
	{
		delete[](lpVersionInfoBuffer);
		return;
	}

	onSwitch(lpbuffer, lpPathBuffer);
	delete[](lpVersionInfoBuffer);
}
