#include <iostream>
#include <UIAutomation.h>
#include <UIAutomationClient.h>
#include "UIAutomationFocusChangedEventHandler.h"

HRESULT InitializeUIAutomation(IUIAutomation** ppAutomation);
HRESULT InitializeApplicationListener(void);

int main()
{
    HRESULT hr = InitializeApplicationListener();
    if (FAILED(hr)) return hr;

    MSG msg = { };
    while (GetMessage(&msg, NULL, 0, 0))
    {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }
}

HRESULT InitializeApplicationListener()
{
    HRESULT hr;
    hr = CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);
    if (FAILED(hr)) return 1;

    IUIAutomation* pUIAutomation;
    hr = InitializeUIAutomation(&pUIAutomation);
    if (FAILED(hr)) return 2;

    IUIAutomationElement* pUIElementRoot;

    hr = pUIAutomation->GetRootElement(&pUIElementRoot);
    if (FAILED(hr)) return 3;

    HMODULE hLibService = LoadLibrary(L"TimuseService.Lib.dll");
    if (hLibService == NULL) return 4;
    auto onSwitch = (FOREGROUNDAPPLICATIONSWITCHEDPROC)GetProcAddress(hLibService, "OnSwitch");
    auto initService = GetProcAddress(hLibService, "InitService");
    if (!initService || !onSwitch) return 5;

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