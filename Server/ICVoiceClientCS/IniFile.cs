using System;
using System.IO;
using System.Text.RegularExpressions;


namespace ICVoiceClientCS
{
    class IniFile
    {
        private readonly string _filePath;

        public IniFile(string fileName)
        {
            // 프로젝트의 실행 디렉터리 내에 INI 파일을 설정
            _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            EnsureFileExists();
        }

        public string Read(string section, string key, string defaultValue)
        {
            try
            {
                string[] lines = File.ReadAllLines(_filePath);
                bool isInSection = false;

                foreach (string line in lines)
                {
                    // 섹션 헤더 찾기
                    if (line.Trim().StartsWith($"[{section}]"))
                    {
                        isInSection = true;
                        continue;
                    }

                    if (isInSection)
                    {
                        // 섹션이 끝났으면 종료
                        if (line.Trim().StartsWith("[") && line.Trim().EndsWith("]"))
                        {
                            break;
                        }

                        // 키-값 쌍 찾기
                        var match = Regex.Match(line, @"^\s*(?<key>[^\s=]+)\s*=\s*(?<value>.*)\s*$");
                        if (match.Success)
                        {
                            string m_key = match.Groups["key"].Value;
                            string m_value = match.Groups["value"].Value;

                            if (m_key.Equals(key, StringComparison.OrdinalIgnoreCase))
                            {
                                return m_value;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"INI 파일 읽기 오류: {ex.Message}");
            }

            return defaultValue;
        }


        private void EnsureFileExists()
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine($"INI 파일이 존재하지 않습니다. 새로 생성합니다: {_filePath}");
                CreateDefaultIniFile();
            }
        }

        private void CreateDefaultIniFile()
        {
            try
            {
                using (var writer = new StreamWriter(_filePath))
                {
                    writer.WriteLine("[Ports]");
                    writer.WriteLine("LocalPort=5001");
                    writer.WriteLine("ServerPort=5002");
                    writer.WriteLine("ServerIpAddress=127.0.0.1");
                }
                Console.WriteLine("기본 설정으로 INI 파일을 생성했습니다.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"INI 파일 생성 오류: {ex.Message}");
            }
        }
    }

}
