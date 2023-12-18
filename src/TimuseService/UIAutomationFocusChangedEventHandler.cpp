#include <memory>

HRESULT UIAutomationFocusChangedEventHandler::HandleFocusChangedEvent(IUIAutomationElement* sender)
{
    try
    {
        HRESULT hr = S_OK;
        int currentPid;
        hr = sender->get_CurrentProcessId(&currentPid);
        if (FAILED(hr)) return hr;

        if (currentPid != _lastPid)
        {
            BSTR fileName = nullptr, filePath = nullptr;
            hr = GetFileInfoByProcessId(currentPid, &fileName, &filePath);

            if (SUCCEEDED(hr)) _handler(fileName, filePath);
            _lastPid = currentPid;
        }

        return hr;
    }
    catch (std::exception& ex)
    {
        std::cout << ex.what() << std::endl;
        return -1;
    }
}

HRESULT UIAutomationFocusChangedEventHandler::GetFileInfoByProcessId(DWORD pid, BSTR* fileName, BSTR* filePath)
{
    HANDLE hProcess = OpenProcess(PROCESS_QUERY_LIMITED_INFORMATION, true, pid);

    if (!hProcess)
    {
        return -1;
    }
    
    memset(_fileName, 0, sizeof(_fileName));
    memset(_filePath, 0, sizeof(_filePath));

    DWORD dwPathLength = MAX_PATH;
    QueryFullProcessImageName(hProcess, 0, _filePath, &dwPathLength);

    DWORD dwFileVersionInfoSize = GetFileVersionInfoSize(_filePath, NULL);

    // no version info, use file name
    if (!dwFileVersionInfoSize)
    {
        *fileName = _filePath;
        *filePath = _filePath;
        return S_OK;
    }

    BYTE* lpVersionInfoBuffer = new BYTE[dwFileVersionInfoSize];
    GetFileVersionInfo(_filePath, NULL, dwFileVersionInfoSize, lpVersionInfoBuffer);

    DWORD dwDefaultLang = 0x040904E4;
    UINT cbTranslate = sizeof(dwDefaultLang);
    _lpLangAndCodePage = (LPLANGANDCODEPAGE)&dwDefaultLang;
    VerQueryValue(lpVersionInfoBuffer, L"\\VarFileInfo\\Translation", (LPVOID*)&_lpLangAndCodePage, &cbTranslate);

    LPVOID lpbuffer;
    swprintf_s(lpCmdBuffer, L"\\StringFileInfo\\%04x%04x\\FileDescription", _lpLangAndCodePage->wLanguage, _lpLangAndCodePage->wCodePage);
    auto success = VerQueryValue(lpVersionInfoBuffer, lpCmdBuffer, &lpbuffer, &cbTranslate);

    // query version info failed, use file name
    if (!success)
    {
        *fileName = _filePath;
        *filePath = _filePath;
        delete[](lpVersionInfoBuffer);
        return S_OK;
    }

    // query success, use filename and discription
    wcscpy_s(_fileName, (BSTR)lpbuffer);
    *fileName = _fileName;
    *filePath = _filePath;

    delete[](lpVersionInfoBuffer);
    return S_OK;
}