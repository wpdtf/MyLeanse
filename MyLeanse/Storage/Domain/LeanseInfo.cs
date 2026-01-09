namespace MyLeanse.LocalDatabase.Domain;

/// <summary>
/// Информация по линзам
/// </summary>
class LeanseInfo
{
    /// <summary>
    /// id пользователя телеграмм
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Дата когда линзы надели
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Дата когда линзы сняли
    /// </summary>
    public DateTime? EndTime { get; set; }
}
