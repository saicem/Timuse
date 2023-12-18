#pragma once

// 1/100 second 
typedef std::chrono::duration<int64_t, std::centi> record_tick_unit_t;

class AppRecorder
{
public:
    AppRecorder();
    ~AppRecorder();

    void Switch(const BSTR strName, const BSTR strPath);

private:
    HANDLE hRecordFile;
    HANDLE hMapFile;
    HANDLE hIndexFile;
    
    std::shared_ptr<std::chrono::utc_clock::time_point> spFocusStartAt = nullptr;
    uint16_t lastAppId = 0;

    mutable uint32_t indexedDays = 0;
    mutable std::unordered_map<std::wstring, uint16_t> mapApp;
    mutable uint16_t currentMaxId = 0;

    void WriteRecord(uint32_t day, uint16_t appId, const std::chrono::utc_clock::duration& startTimeOfDay, const std::chrono::utc_clock::duration& duration) const;
    void TrackIndex(uint32_t today) const;
    void SaveApplicationInfo(const BSTR strName, const BSTR strPath, uint16_t id) const;
    uint16_t GetApplicationId(BSTR strName, BSTR strPath) const;
    bool GetAppNameById(uint16_t appId, std::wstring& appName) const;
    
    std::wstring GetUniString(const LPTSTR value, int cch);

private:
    std::wstring CreateAppDataPath() const;
    bool InitializeAppMap(const std::wstring& strAppData);
    bool InitializeRecordFile(const std::wstring& strAppData);
    bool InitializeIndexFile(const std::wstring& strAppData);

private:
    struct ApplicationRecord
    {
    public:

        uint64_t data;

        ApplicationRecord() : data(0) { }
        ApplicationRecord(uint64_t data) : data(data) { }
        ApplicationRecord(uint16_t appId, uint32_t start, uint32_t duration) : data(0)
        {
            data |= (appId & 0xFFFFU);
            data |= ((uint64_t)duration & 0xFFFFFFU) << 16;
            data |= ((uint64_t)start & 0xFFFFFFU) << 40;
        }
        
        uint16_t GetAppId() const
        {
            return (uint16_t)(data & 0xFFFFU);
        }

        uint32_t GetDurationTicks() const
        {
            return (uint32_t)((data & 0x000000FFFFFF0000U) >> 16);
        }

        uint32_t GetRecordStartTicks() const
        {
            return (uint32_t)((data & 0xFFFFFF0000000000) >> 40);
        }
    };
};