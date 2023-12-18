#include "BinaryHelper.h"

int32_t BinaryHelper::Read7BitEncodedInt(int& cb)
{
	int32_t value = 0;
	int shift = 0;
	cb = 0;
	byte b;
	do
	{
		auto success = ReadFile(hFile, &b, 1, nullptr, nullptr);
		if (!success)
		{
			throw std::exception("Failed to read 7 bit encoded int");
		}
		value |= (b & 0x7F) << shift;
		shift += 7;
		cb++;
	} while ((b & 0x80) != 0);
	return value;
}

uint16_t BinaryHelper::ReadUInt16()
{
	uint16_t value = 0;
	auto buffer = reinterpret_cast<byte*>(&value);
	auto success = ReadFile(hFile, buffer, sizeof(uint16_t), nullptr, nullptr);
	if (!success)
	{
		throw std::exception("Failed to read uint16_t");
	}
	return value;
}

void BinaryHelper::ReadCchString(LPTSTR buffer, int cch, int& cb)
{
	cb = 0;
	auto strLength = Read7BitEncodedInt(cb);
	if (strLength >= cch)
	{
		throw std::exception("String too long");
	}

	DWORD bytesRead = 0;
	auto success = ReadFile(hFile, buffer, strLength * sizeof(TCHAR), &bytesRead, nullptr);

	// append null terminator
	auto charRead = bytesRead / sizeof(TCHAR);
	buffer[charRead] = 0;

	cb += bytesRead;

	if (!success)
	{
		throw std::exception("Failed to read string");
	}
}

void BinaryHelper::Write7BitEncodedInt(uint32_t value)
{
	do
	{
		byte b = value & 0x7F;
		value >>= 7;
		if (value != 0)
		{
			b |= 0x80;
		}
		WriteFile(hFile, &b, 1, nullptr, nullptr);
	} while (value != 0);
}

void BinaryHelper::WriteUInt16(uint16_t value)
{
	auto buffer = reinterpret_cast<byte*>(&value);
	auto success = WriteFile(hFile, buffer, sizeof(value), nullptr, nullptr);
	if (!success)
	{
		throw std::exception("Failed to write uint16_t");
	}
}

void BinaryHelper::WriteCchString(const LPTSTR value, int cch)
{
	auto strLength = (int)_tcslen(value);
	if (strLength >= cch)
	{
		throw std::exception("String too long");
	}

	Write7BitEncodedInt(strLength);
	auto success = WriteFile(hFile, value, strLength * sizeof(TCHAR), nullptr, nullptr);

	if (!success)
	{
		throw std::exception("Failed to write string");
	}
}
