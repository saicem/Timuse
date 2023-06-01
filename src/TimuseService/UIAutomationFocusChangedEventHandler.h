#pragma once
#include <UIAutomation.h>

typedef void (*FOREGROUNDAPPLICATIONSWITCHEDPROC)(LPVOID lpName, LPVOID lpPath);

typedef struct
{
    WORD wLanguage;
    WORD wCodePage;
} LANGANDCODEPAGE;

typedef LANGANDCODEPAGE* LPLANGANDCODEPAGE;

const BSTR const ADDRESSBARACCKEY = SysAllocString(L"Ctrl+L");

class UIAutomationFocusChangedEventHandler :
    public IUIAutomationFocusChangedEventHandler
{
private:
    LONG _refCount;

    DWORD _lastPid;
    LPLANGANDCODEPAGE _lpLangAndCodePage;
    FOREGROUNDAPPLICATIONSWITCHEDPROC _applicationSwitchHandler;

    IUIAutomation* _uiAutomation;

    WCHAR _fileName[MAX_PATH] = { 0 };
    WCHAR _filePath[MAX_PATH] = { 0 };
    WCHAR lpCmdBuffer[41] = { 0 };

    HRESULT GetFileInfoByProcessId(DWORD pid, BSTR* fileName, BSTR* filePath);

public:
    UIAutomationFocusChangedEventHandler(IUIAutomation* uiAutomation, FOREGROUNDAPPLICATIONSWITCHEDPROC applicationSwitchedHandler)
        : _refCount(1), _lastPid(0), _fileName(), _filePath(), lpCmdBuffer(), _lpLangAndCodePage(NULL)
    {
        _applicationSwitchHandler = applicationSwitchedHandler;
        _uiAutomation = uiAutomation;
    }

    virtual ULONG STDMETHODCALLTYPE AddRef() override
    {
        ULONG ret = InterlockedIncrement(&_refCount);
        return ret;
    }

    virtual ULONG STDMETHODCALLTYPE Release() override
    {
        ULONG ret = InterlockedDecrement(&_refCount);
        if (ret == 0)
        {
            delete this;
            return 0;
        }
        return ret;
    }

    virtual HRESULT STDMETHODCALLTYPE QueryInterface(REFIID riid, void** ppInterface) override
    {
        if (riid == __uuidof(IUnknown))
            *ppInterface = static_cast<IUIAutomationFocusChangedEventHandler*>(this);
        else if (riid == __uuidof(IUIAutomationFocusChangedEventHandler))
            *ppInterface = static_cast<IUIAutomationFocusChangedEventHandler*>(this);
        else
        {
            *ppInterface = NULL;
            return E_NOINTERFACE;
        }
        this->AddRef();
        return S_OK;
    }

    virtual HRESULT STDMETHODCALLTYPE HandleFocusChangedEvent(IUIAutomationElement* sender) override;
};
