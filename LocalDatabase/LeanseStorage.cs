using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyLeanse.LocalDatabase;

public class LeanseStorage
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly string filePath;

    public LeanseStorage(string relativePath = "leanse.json")
    {
        filePath = Path.Combine(AppContext.BaseDirectory, relativePath);
        EnsureFileExists();
    }

    public void Clear()
    {
        WriteAll(new List<LeanseInfo>());
    }

    public void Start()
    {
        var list = ReadAll();

        list.Add(new LeanseInfo
        {
            StartTime = DateTime.Now,
            EndTime = null
        });

        WriteAll(list);
    }

    public bool End()
    {
        var list = ReadAll();

        var active = list.FirstOrDefault(x => x.EndTime == null);
        if (active == null)
        {
            return false;
        }

        active.EndTime = DateTime.Now;
        WriteAll(list);

        return true;
    }

    public bool IsActive()
    {
        var list = ReadAll();

        var active = list.FirstOrDefault(x => x.EndTime == null);
        if (active == null)
        {
            return false;
        }

        return true;
    }

    public TimeSpan Info()
    {
        var list = ReadAll();
        var now = DateTime.Now;

        TimeSpan total = TimeSpan.Zero;

        foreach (var item in list)
        {
            var end = item.EndTime ?? now;
            if (end > item.StartTime)
            {
                total += end - item.StartTime;
            }
        }

        return total;
    }

    private List<LeanseInfo> ReadAll()
    {
        if (!File.Exists(filePath))
        {
            return new List<LeanseInfo>();
        }

        var json = File.ReadAllText(filePath);
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<LeanseInfo>();
        }

        return JsonSerializer.Deserialize<List<LeanseInfo>>(json, JsonOptions)
               ?? new List<LeanseInfo>();
    }

    private void WriteAll(List<LeanseInfo> list)
    {
        var json = JsonSerializer.Serialize(list, JsonOptions);
        File.WriteAllText(filePath, json);
    }

    private void EnsureFileExists()
    {
        if (!File.Exists(filePath))
        {
            WriteAll(new List<LeanseInfo>());
        }
    }
}
