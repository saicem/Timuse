#pragma once
class BinaryHelper
{
public:
	BinaryHelper(HANDLE hFile, bool releaseHandle = false) : hFile(hFile), releaseHandle(releaseHandle) { }
	~BinaryHelper() 
	{
		if (hFile != INVALID_HANDLE_VALUE && releaseHandle)
		{
			CloseHandle(hFile);
		}
	}

	int32_t Read7BitEncodedInt();
	uint16_t ReadUInt16();
	void ReadCchString(LPTSTR buffer, int cch);

	void Write7BitEncodedInt(uint32_t value);
	void WriteUInt16(uint16_t value);
	void WriteCchString(const LPTSTR value, int cch);

private:
	HANDLE hFile;
	bool releaseHandle;
};