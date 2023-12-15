#include "AppRecorder.h"
#include "BinaryHelper.h"

std::chrono::system_clock::duration GetDurationSinceZeroClock(const std::chrono::system_clock::time_point& time)
{
	auto duration = time.time_since_epoch();
	auto durationMs = std::chrono::duration_cast<std::chrono::milliseconds>(duration);
	auto durationSinceZeroClock = durationMs - std::chrono::duration_cast<std::chrono::milliseconds>(durationMs % std::chrono::hours(24));
	return durationSinceZeroClock;
}

AppRecorder::AppRecorder()
{
	auto strAppData = std::make_unique<TCHAR[]>(MAX_PATH);
	// get appdata path
	auto success = SHGetSpecialFolderPath(NULL, strAppData.get(), CSIDL_LOCAL_APPDATA, true);
	if (!success)
	{
		throw std::exception("Failed to get appdata path");
	}
	// append app name
	auto hr = PathCchAppend(strAppData.get(), MAX_PATH, TEXT("\\Timuse"));
	if (FAILED(hr))
	{
		throw std::exception("Failed to append app name");
	}

	// if floder not exist, create it
	if (!PathFileExists(strAppData.get()))
	{
		success = CreateDirectory(strAppData.get(), NULL);
		if (!success)
		{
			throw std::exception("Failed to create directory");
		}
	}

	auto strBuffer = std::make_unique<TCHAR[]>(MAX_PATH);

	// get map file path
	hr = PathCchCombine(strBuffer.get(), MAX_PATH, strAppData.get(), TEXT("map.bin"));
	if (FAILED(hr))
	{
		throw std::exception("Failed to combine file path");
	}

    // open map file
	hMapFile = CreateFile(strBuffer.get(), GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ, NULL, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
	if (hMapFile == INVALID_HANDLE_VALUE)
	{
		throw std::exception("Failed to open map file");
	}
	
	// get map file size
	LARGE_INTEGER fileSize;
	success = GetFileSizeEx(hMapFile, &fileSize);
	if (!success)
	{
		throw std::exception("Failed to get map file size");
	}

	auto sizeMapFile = fileSize.QuadPart;
	LONGLONG readBytes = 0;
	BinaryHelper binaryHelper(hMapFile, false);

	// seek to begin of file
	SetFilePointer(hMapFile, 0, NULL, FILE_BEGIN);

	while (readBytes < sizeMapFile)
	{
		// read app id
		currentMaxId = binaryHelper.ReadUInt16();
		readBytes += sizeof(uint16_t);
		// read app path
		binaryHelper.ReadCchString(strBuffer.get(), MAX_PATH);
		readBytes += _tcslen(strBuffer.get()) * sizeof(TCHAR);
		auto strPath = GetUniString(strBuffer.get(), MAX_PATH);
		// read app name, discard
		binaryHelper.ReadCchString(strBuffer.get(), MAX_PATH);
		readBytes += _tcslen(strBuffer.get()) * sizeof(TCHAR);
		// add to map
		mapApp[strPath] = currentMaxId;
	}

	// get record file path
	hr = PathCchCombine(strBuffer.get(), MAX_PATH, strAppData.get(), TEXT("record.bin"));
	if (FAILED(hr))
	{
		throw std::exception("Failed to combine file path");
	}

	// open record file with shared read/write access
	hRecordFile = CreateFile(strBuffer.get(), GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ, NULL, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
	if (hRecordFile == INVALID_HANDLE_VALUE)
	{
		throw std::exception("Failed to open record file");
	}

	// seek to end of file
	SetFilePointer(hRecordFile, 0, NULL, FILE_END);

	// get index file path
	hr = PathCchCombine(strBuffer.get(), MAX_PATH, strAppData.get(), TEXT("index.bin"));
	if (FAILED(hr))
	{
		throw std::exception("Failed to combine file path");
	}

	// open index file with shared read/write access
	hIndexFile = CreateFile(strBuffer.get(), GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ, NULL, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
	if (hIndexFile == INVALID_HANDLE_VALUE)
	{
		throw std::exception("Failed to open index file");
	}

	// get index file size
	success = GetFileSizeEx(hIndexFile, &fileSize);
	if (!success)
	{
		throw std::exception("Failed to get index file size");
	}

	auto sizeIndexFile = fileSize.QuadPart;
	if (sizeIndexFile == 0)
	{
		// get current utc time with chrono
		auto now = std::chrono::system_clock::now();
		// get total days since epoch
		indexedDays = (uint32_t)(std::chrono::duration_cast<std::chrono::hours>(now.time_since_epoch()).count() / 24);
		// write days to index file
		DWORD bytesWritten = 0;
		success = WriteFile(hIndexFile, &indexedDays, sizeof(uint32_t), &bytesWritten, NULL);
		if (!success)
		{
			throw std::exception("Failed to write index file");
		}
	}
	else
	{
		// seek to begin of file
		SetFilePointer(hIndexFile, 0, NULL, FILE_BEGIN);

		// read days from index file
		DWORD bytesRead = 0;
		uint32_t startDays = 0;
		success = ReadFile(hIndexFile, &startDays, sizeof(uint32_t), &bytesRead, NULL);
		if (!success)
		{
			throw std::exception("Failed to read index file");
		}

		auto indexCount = (uint32_t)(sizeIndexFile / sizeof(uint32_t));
		indexedDays = startDays + indexCount - 1u;
	}
}

AppRecorder::~AppRecorder()
{
	CloseHandle(hRecordFile);
	CloseHandle(hMapFile);
	CloseHandle(hIndexFile);
}

void AppRecorder::Switch(const std::shared_ptr<TCHAR> strName, const std::shared_ptr<TCHAR> strPath)
{
	auto now = std::chrono::system_clock::now();

	if (strName == nullptr || strPath == nullptr)
	{
		return;
	}

	if (/*!_tcsclen(strName.get()) == 0 && */spFocusAppName && spFocusAppPath)
	{
		auto id = GetCurrentApplicationId();
		
		auto startDays = (uint32_t)(std::chrono::duration_cast<std::chrono::hours>(spFocusStartAt->time_since_epoch()).count() / 24);
		auto endDays = (uint32_t)(std::chrono::duration_cast<std::chrono::hours>(now.time_since_epoch()).count() / 24);
		
		auto startOfDay = GetDurationSinceZeroClock(*spFocusStartAt);
		auto endOfDay = GetDurationSinceZeroClock(now);

		if (startDays == endDays)
		{
			WriteRecord(startDays, id, startOfDay, endOfDay - startOfDay);
		}
		else
		{
			WriteRecord(startDays, id, startOfDay, std::chrono::hours(24) - startOfDay);
			for (auto day = startDays + 1; day < endDays; day++)
			{
				WriteRecord(day, id, std::chrono::milliseconds(0), std::chrono::hours(24));
			}
			WriteRecord(endDays, id, std::chrono::milliseconds(0), endOfDay);
		}
	}

	spFocusStartAt = std::make_shared<std::chrono::system_clock::time_point>(now);
	spFocusAppName = strName;
	spFocusAppPath = strPath;
}

void AppRecorder::WriteRecord(uint32_t day, uint16_t appId, const std::chrono::system_clock::duration& startTimeOfDay, const std::chrono::system_clock::duration& duration) const
{
	if (hRecordFile == INVALID_HANDLE_VALUE)
	{
		throw std::exception("Record file not opened");
	}

	// TrackIndex(day);

	// get total ms count of startTimeOfDay
	auto startMs = (uint32_t)std::chrono::duration_cast<std::chrono::milliseconds>(startTimeOfDay).count();
	// get total ms count of duration
	auto durationMs = (uint32_t)std::chrono::duration_cast<std::chrono::milliseconds>(duration).count();

	ApplicationRecord record(appId, startMs, durationMs);

	DWORD bytesWritten = 0;
	auto success = WriteFile(hRecordFile, &record, sizeof(ApplicationRecord), &bytesWritten, NULL);
	if (!success)
	{
		throw std::exception("Failed to write record file");
	}

	// flush record file
	success = FlushFileBuffers(hRecordFile);
	if (!success)
	{
		throw std::exception("Failed to flush record file");
	}

	std::wcout << "[" << startTimeOfDay << "] <" << duration << "> " << appId << ": " << spFocusAppName << std::endl;
}

void AppRecorder::TrackIndex(uint32_t today) const
{
}

void AppRecorder::SaveApplicationInfo(const std::shared_ptr<TCHAR> strName, const std::shared_ptr<TCHAR> strPath, uint16_t id) const
{
	if (hMapFile == INVALID_HANDLE_VALUE)
	{
		throw std::exception("Map file not opened");
	}

	BinaryHelper binaryHelper(hMapFile, false);

	// seek to end of file
	SetFilePointer(hMapFile, 0, NULL, FILE_END);

	// write app id
	binaryHelper.WriteUInt16(id);
	// write app path
	binaryHelper.WriteCchString(strPath.get(), MAX_PATH);
	// write app name
	binaryHelper.WriteCchString(strName.get(), MAX_PATH);

	// flush map file
	auto success = FlushFileBuffers(hMapFile);
	if (!success)
	{
		throw std::exception("Failed to flush map file");
	}
}

uint16_t AppRecorder::GetCurrentApplicationId() const
{
	// get current app id from map
	auto iter = mapApp.find(spFocusAppPath.get());
	if (iter != mapApp.end())
	{
		return iter->second;
	}
	
	// if not found, create new app id
	currentMaxId++;
	SaveApplicationInfo(spFocusAppName, spFocusAppPath, currentMaxId);
	mapApp[spFocusAppPath.get()] = currentMaxId;
	
	return currentMaxId;
}

std::wstring AppRecorder::GetUniString(const LPTSTR value, int cch)
{
	auto strLength = _tcslen(value);
	if (strLength >= cch)
	{
		throw std::exception("String too long");
	}

	std::wstring result;
	result.resize(strLength);
	
	for (int i = 0; i < strLength; i++)
	{
		result[i] = (wchar_t)(value[i]);
	}

	return result;
}
