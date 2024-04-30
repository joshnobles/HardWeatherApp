namespace HardWeatherApp.Entities
{
    public static class Logger
    {
        public static void Log(LogType t, string s)
        {
            try
            {
                if (!Config.LOGGING)
                    return;

                if (!Valid.Log(s))
                    return;

                bool append = new FileInfo(Config.LOG_FILE_PATH).Length < 1000000;

                using var sw = new StreamWriter(Config.LOG_FILE_PATH, append);

                sw.WriteLine($"[{t}] {s}");
            }
            catch
            {
                Console.WriteLine("Could not log");
            }
        }
    }
}
