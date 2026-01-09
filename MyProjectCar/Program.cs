using System.Text.Json;

namespace MyProjectCar;

class Program
{
    private static string configFileLocation =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyProjectCar",
            "config.json");

    private static Configuration config = new Configuration();

    static void Main(string[] args)
    {
        if (!File.Exists(configFileLocation))
        {
            AskQuestions();
            string? directory = Path.GetDirectoryName(configFileLocation);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
                WriteIndented = true
            };
            string jsonString = JsonSerializer.Serialize(config, options);
            File.WriteAllText(configFileLocation, jsonString);
        }
        else
        {
            string jsonString = File.ReadAllText(configFileLocation);
            var options = new JsonSerializerOptions
            {
                IncludeFields = true
            };
            config = JsonSerializer.Deserialize<Configuration>(jsonString, options) ?? new Configuration();
        }
        //Ask the player if this mod is for My Summer Car, My Winter Car, or Both
        var (isForSummerCar, isForWinterCar) = AskWhichGame();
        //What is the project named?
        Console.WriteLine("What is the name of the Project/Mod?");
        string ModName;
        while (true)
        {
            ModName = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(ModName))
                break;
            Console.WriteLine("Mod name cannot be empty. Please enter a valid name:");
        }

        //Ask for the parent folder where the project will be created
        Console.WriteLine("\nWhere should the project be created? (Enter the parent directory path)");
        string projectParentDirectory;
        while (true)
        {
            projectParentDirectory = (Console.ReadLine() ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(projectParentDirectory))
            {
                Console.WriteLine("Directory path cannot be empty. Please enter a valid path:");
                continue;
            }

            if (!Directory.Exists(projectParentDirectory))
            {
                Console.WriteLine("That directory does not exist. Please enter a valid directory path:");
                continue;
            }

            break;
        }

        string projectDirectory = Path.Combine(projectParentDirectory, ModName);

        if (!Directory.Exists(projectDirectory))
        {
            Directory.CreateDirectory(projectDirectory);
        }
        
        //Copy the files from ProjectTemplate to the projectDirectory
        string templateDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProjectTemplate");
        
        if (!Directory.Exists(templateDirectory))
        {
            Console.WriteLine("Error: ProjectTemplate directory not found!");
            return;
        }

        //Determine @GAMES value based on game selection
        string gamesValue;
        if (isForSummerCar && isForWinterCar)
            gamesValue = "Game.MySummerCar_And_MyWinterCar";
        else if (isForSummerCar)
            gamesValue = "Game.MySummerCar";
        else
            gamesValue = "Game.MyWinterCar";

        //Determine @MSC_LOADER_ROOT path
        string mscLoaderRoot = string.Empty;
        string mscGameRoot = string.Empty;
        if (isForSummerCar && !string.IsNullOrEmpty(config.MySummerCarLocation))
        {
            mscGameRoot = config.MySummerCarLocation;
            mscLoaderRoot = Path.Combine(config.MySummerCarLocation, "mysummercar_Data", "Managed");
        }
        else if (isForWinterCar && !string.IsNullOrEmpty(config.MyWinterCarLocation))
        {
            mscGameRoot = config.MyWinterCarLocation;
            mscLoaderRoot = Path.Combine(config.MyWinterCarLocation, "mywintercar_Data", "Managed");
        }

        //Copy and process each file
        foreach (string filePath in Directory.GetFiles(templateDirectory, "*", SearchOption.AllDirectories))
        {
            string relativePath = Path.GetRelativePath(templateDirectory, filePath);
            string newFileName = relativePath.Replace("MPCLinuxTemplate", ModName);
            string destinationPath = Path.Combine(projectDirectory, newFileName);

            if (destinationPath.EndsWith(".csproj.tmp"))
            {
                destinationPath = destinationPath.Substring(0, destinationPath.Length - 4);
            }

            string? destinationDir = Path.GetDirectoryName(destinationPath);
            if (destinationDir != null && !Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            string fileContent = File.ReadAllText(filePath);
            fileContent = fileContent.Replace("@MOD_NAME", ModName);
            fileContent = fileContent.Replace("@GAMES", gamesValue);
            fileContent = fileContent.Replace("@MSC_LOADER_ROOT", mscLoaderRoot);
            fileContent = fileContent.Replace("@MSC_GAME_ROOT", mscGameRoot);
            fileContent = fileContent.Replace("@MODS_DIR", Path.Combine(mscGameRoot, "Mods"));

            File.WriteAllText(destinationPath, fileContent);
        }

        Console.WriteLine($"\nProject '{ModName}' created successfully at: {projectDirectory}");
    }

    private static (bool IsForSummerCar, bool IsForWinterCar) AskWhichGame()
    {
        Console.WriteLine("\nWhich game is this mod for?");
        Console.WriteLine("1. My Summer Car");
        Console.WriteLine("2. My Winter Car");
        Console.WriteLine("3. Both");

        while (true)
        {
            Console.Write("Enter your choice (1-3): ");
            var input = Console.ReadLine()?.Trim();

            switch (input)
            {
                case "1":
                    return (true, false);
                case "2":
                    return (false, true);
                case "3":
                    return (true, true);
                default:
                    Console.WriteLine("Invalid choice. Please enter 1, 2, or 3.");
                    break;
            }
        }
    }

    private static void AskQuestions()
    {
        var (hasSummer, summerDir) = PromptForGameInstallFolder(
            "My Summer Car", "mysummercar.exe");

        config.MySummerCar = hasSummer;
        config.MySummerCarLocation = summerDir;

        var (hasWinter, winterDir) = PromptForGameInstallFolder(
            "My Winter Car", "mywintercar.exe");

        config.MyWinterCar = hasWinter;
        config.MyWinterCarLocation = winterDir;
    }

    private static (bool HasGame, string DirectoryPath) PromptForGameInstallFolder(string gameName, string exeFileName)
    {
        while (true)
        {
            Console.WriteLine($"Please provide the path to {gameName} (Or nothing if you don't have it):");
            var input = (Console.ReadLine() ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(input))
                return (false, string.Empty);

            // Accept either a directory path OR a direct path to the exe
            var directory = input;
            if (File.Exists(input) && string.Equals(Path.GetFileName(input), exeFileName, StringComparison.OrdinalIgnoreCase))
                directory = Path.GetDirectoryName(input) ?? string.Empty;

            if (!Directory.Exists(directory))
            {
                Console.WriteLine("That directory does not exist. Please try again (or leave empty).");
                continue;
            }

            var exePath = Path.Combine(directory, exeFileName);
            if (!File.Exists(exePath))
            {
                Console.WriteLine($"Could not find '{exeFileName}' in that directory. Please try again (or leave empty).");
                continue;
            }

            return (true, directory);
        }
    }

    internal class Configuration
    {
        public bool MyWinterCar { get; set; } = true;
        public string MyWinterCarLocation { get; set; } = "";
        public bool MySummerCar { get; set; } = true;
        public string MySummerCarLocation { get; set; } = "";
    }
}