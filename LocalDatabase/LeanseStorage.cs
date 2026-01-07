using MyLeanse.LocalDatabase.Domain;
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
        filePath = Path.Combine(AppContext.BaseDirectory, "data", relativePath);
        EnsureFileExists();
    }

    public void Clear(long userId)
    {
        var list = ReadAll();

        list = list.Where(x => x.UserId != userId).ToList();

        WriteAll(list);
    }

    public void Start(long userId)
    {
        var list = ReadAll();

        list.Add(new LeanseInfo
        {
            UserId = userId,
            StartTime = DateTime.Now,
            EndTime = null
        });

        WriteAll(list);
    }

    public bool End(long userId)
    {
        var list = ReadAll();

        var active = list.FirstOrDefault(x => x.UserId == userId && x.EndTime == null);
        if (active == null)
        {
            return false;
        }

        active.EndTime = DateTime.Now;
        WriteAll(list);

        return true;
    }

    public bool IsActive(long userId)
    {
        var list = ReadAll();

        var active = list.FirstOrDefault(x => x.UserId == userId && x.EndTime == null);
        if (active == null)
        {
            return false;
        }

        return true;
    }

    public TimeSpan Info(long userId)
    {
        var list = ReadAll();
        list = list.Where(x => x.UserId == userId).ToList();

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
