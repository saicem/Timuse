#pragma once
#include <unordered_set>
#include <regex>

namespace Timuse
{
	class Config
	{
	private:
		std::atomic<int> m_minDurationInTick;
		std::unordered_set<std::wstring> m_ignoredFileName;
		std::unordered_set<std::wstring> m_ignoredDirectory;
		std::unordered_set<std::wstring> m_ignoredFileNamePattern;
		std::unordered_set<std::wstring> m_ignoredDirectoryPattern;

	public:
		Config(const std::wstring& appDataPath) 
		{
			auto strBuffer = std::make_unique<TCHAR[]>(MAX_PATH);
			
			auto hr = PathCchCombine(strBuffer.get(), MAX_PATH, const_cast<LPWSTR>(appDataPath.c_str()), TEXT("IgnoredFiles.txt"));
			if (FAILED(hr))
			{
				throw std::exception("Failed to combine file path");
			}
			
			std::ifstream ignoredFile(strBuffer.get());
			if (ignoredFile.is_open())
			{
				std::string line;
				while (std::getline(ignoredFile, line))
				{
					if (line.empty()) continue;
					if (line[0] == '#') continue;
					// if line is surrounded by '()' then it is a regex pattern
					if (line[0] == '(' && line[line.size() - 1] == ')')
					{
						m_ignoredFileNamePattern.insert(std::wstring(line.begin() + 1, line.end() - 1));
					}
					else
					{
						m_ignoredFileName.insert(std::wstring(line.begin(), line.end()));
					}
				}
			}

			hr = PathCchCombine(strBuffer.get(), MAX_PATH, const_cast<LPWSTR>(appDataPath.c_str()), TEXT("IgnoredDirectories.txt"));
			if (FAILED(hr))
			{
				throw std::exception("Failed to combine file path");
			}

			std::ifstream ignoredDirectory(strBuffer.get());
			if (ignoredDirectory.is_open())
			{
				std::string line;
				while (std::getline(ignoredDirectory, line))
				{
					if (line.empty()) continue;
					if (line[0] == '#') continue;
					// if line is surrounded by '()' then it is a regex pattern
					if (line[0] == '(' && line[line.size() - 1] == ')')
					{
						m_ignoredDirectoryPattern.insert(std::wstring(line.begin() + 1, line.end() - 1));
					}
					else
					{
						m_ignoredDirectory.insert(std::wstring(line.begin(), line.end()));
					}
				}
			}
		}

		void SetMinDurationInTick(int minDurationInTick)
		{
			m_minDurationInTick = minDurationInTick;
		}

		int GetMinDurationInTick() const
		{
			return m_minDurationInTick;
		}

		void AddIgnoredFileName(const std::wstring& fileName)
		{
			m_ignoredFileName.insert(fileName);
		}

		void AddIgnoredDirectory(const std::wstring& directory)
		{
			m_ignoredDirectory.insert(directory);
		}

		bool IsIgnoredFileName(const std::wstring& fileName) const
		{
			if (m_ignoredFileName.find(fileName) != m_ignoredFileName.end()) return true;
			for (const auto& pattern : m_ignoredFileNamePattern)
			{
				if (std::regex_match(fileName, std::wregex(pattern))) return true;
			}

			return false;
		}

		bool IsIgnoredDirectory(const std::wstring& directory) const
		{
			if (m_ignoredDirectory.find(directory) != m_ignoredDirectory.end()) return true;
			for (const auto& pattern : m_ignoredDirectoryPattern)
			{
				if (std::regex_match(directory, std::wregex(pattern))) return true;
			}

			return false;
		}
	};
}