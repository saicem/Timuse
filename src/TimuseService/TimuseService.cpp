#include <iostream>
#include <UIAutomation.h>
#include "UIAutomationFocusChangedEventHandler.h"

HRESULT InitializeUIAutomation(IUIAutomation** ppAutomation);
HRESULT InitializeApplicationListener(void);
HRESULT PreventMultiInstance();
void CreateConsoleWhileDebug(void);

int WINAPI WinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPSTR lpCmdLine, _In_ int nShowCmd)
{
    CreateConsoleWhileDebug();

    HRESULT hr = S_OK;
    hr = PreventMultiInstance();
    if (FAILED(hr))
    {
        std::cout << "timuse service already launched." << std::endl;
        return hr;
    }

    hr = InitializeApplicationListener();
    if (FAILED(hr)) return hr;

    MSG msg = { };
    while (GetMessage(&msg, NULL, 0, 0))
    {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }
    return 0;
}

HRESULT InitializeApplicationListener()
{
    HRESULT hr;
    hr = CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);
    if (FAILED(hr)) return -1;

    IUIAutomation* pUIAutomation;
    hr = InitializeUIAutomation(&pUIAutomation);
    if (FAILED(hr)) return -2;

    IUIAutomationElement* pUIElementRoot;

    hr = pUIAutomation->GetRootElement(&pUIElementRoot);
    if (FAILED(hr)) return -3;

    HMODULE hLibService = LoadLibrary(L"TimuseService.Lib.dll");
    if (hLibService == NULL) return -4;
    auto onSwitch = (FOREGROUNDAPPLICATIONSWITCHEDPROC)GetProcAddress(hLibService, "OnSwitch");
    auto initService = GetProcAddress(hLibService, "InitService");
    if (!initService || !onSwitch) return -5;

    initService();

    UIAutomationFocusChangedEventHandler* pFocusHandler = new UIAutomationFocusChangedEventHandler(onSwitch);
    if (!pFocusHandler)
    {
        return E_OUTOFMEMORY;
    }
    pUIAutomation->AddFocusChangedEventHandler(NULL, pFocusHandler);

    return S_OK;
}

HRESULT InitializeUIAutomation(IUIAutomation** ppAutomation)
{
    return CoCreateInstance(CLSID_CUIAutomation, NULL,
        CLSCTX_INPROC_SERVER, IID_IUIAutomation,
        reinterpret_cast<void**>(ppAutomation));
}

HRESULT PreventMultiInstance()
{
    HANDLE hMutex = NULL;
    hMutex = OpenMutex(MUTEX_ALL_ACCESS, FALSE, L"MUTEXOFTIMEUSESERVICE");
    if (NULL == hMutex) hMutex = CreateMutex(0, FALSE, L"MUTEXOFTIMEUSESERVICE");
    else return -1;
    return S_OK;
}

void CreateConsoleWhileDebug()
{
    int argCnt;
    LPWSTR* argv = CommandLineToArgvW(GetCommandLine(), &argCnt);
    if (argv != NULL && argCnt >= 2 && wcscmp(argv[1], L"debug") == 0)
    {
        AllocConsole();
        FILE* stream;
        freopen_s(&stream, "CON", "r", stdin);
        freopen_s(&stream, "CON", "w", stdout);
        SetConsoleTitle(L"Timuse Service Console");
    }
}