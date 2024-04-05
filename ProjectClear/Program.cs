
while (true)
{
    Console.Write("Input Root Folder Path:");

    string Path = null;
    while (string.IsNullOrWhiteSpace(Path))
        Path = Console.ReadLine();

    var Root = new DirectoryInfo(Path);

    var OrgLength = Root
        .EnumerateFiles("*", SearchOption.AllDirectories)
        .Sum(Item => Item.Length);
    var OrgSize = GetSize(OrgLength);

    Console.WriteLine($"Total Size: {OrgSize} ({OrgLength})");
    RCS_CleanCsproj(Root);

    var ResultLength = Root
        .EnumerateFiles("*", SearchOption.AllDirectories)
        .Sum(Item => Item.Length);
    var ResultSize = GetSize(ResultLength);

    Console.WriteLine("\nClean Finish");
    Console.WriteLine($"Total Size From: {OrgSize}({OrgLength}) => {ResultSize}({ResultLength})\n");
}


void RCS_CleanCsproj(DirectoryInfo Root)
{
    var Files = Root.GetFiles();
    var HasCsproj = Files
        .Any(Item => Item.Extension == ".csproj");

    if (HasCsproj)
    {
        var DeleteFolders = Root
            .GetDirectories()
            .Where(Item => Item.Name == "bin" || Item.Name == "obj");

        foreach (var Item in DeleteFolders)
        {
            var FullPath = Item.FullName.Replace("\\", "/");
            try
            {
                Item.Delete(true);
                Console.WriteLine($"Delete {FullPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete {FullPath} Error: {ex.Message}");
            }
        }
        return;
    }

    var NextFolder = Root.GetDirectories();
    foreach (var Folder in NextFolder)
        RCS_CleanCsproj(Folder);
}

string GetSize(long Length)
{
    var Kb = 1024;
    var Mb = Math.Pow(1024, 2);
    var Gb = Math.Pow(1024, 3);
    var Tb = Math.Pow(1024, 4);

    if (Length < Mb)
        return $"{Length / Kb:###,###.000} Kb";

    if (Length < Gb)
        return $"{Length / Mb:###,###.000} Mb";

    if (Length < Tb)
        return $"{Length / Gb:###,###.000} Gb";

    return null;
}