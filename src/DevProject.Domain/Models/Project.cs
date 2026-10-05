namespace DevProject.Domain.Models;

public class Project
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Objective { get; private set; }
    public string? Motivation { get; private set; }
    public DateOnly? PlannedStartDate { get; private set; }
    public DateOnly? PlannedEndDate { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public Guid OwnerId { get; private set; }

    public Project(
        string name,
        string objective,
        Guid ownerId,
        string? motivation = null,
        DateOnly? plannedStartDate = null,
        DateOnly? plannedEndDate = null)
    {
        if (ownerId.Equals(Guid.Empty))
            throw new ArgumentException("Project ownerId cannot be empty", nameof(ownerId));

        ValidatePlannedDates(plannedStartDate, plannedEndDate);

        var now = DateTimeOffset.UtcNow;

        Id = Guid.NewGuid();
        Name = NormalizeRequiredText(name, nameof(name));
        Objective = NormalizeRequiredText(objective, nameof(objective));
        Motivation = string.IsNullOrWhiteSpace(motivation) ? null : motivation.Trim();
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
        CreatedAt = now;
        UpdatedAt = now;
        OwnerId = ownerId;
    }

    public void UpdateName(string name)
    {
        Name = NormalizeRequiredText(name, nameof(name));

        UpdatedAt = DateTimeOffset.UtcNow;
    }
    public void UpdateObjective(string objective)
    {
        Objective = NormalizeRequiredText(objective, nameof(objective));

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateMotivation(string? motivation)
    {
        Motivation = string.IsNullOrWhiteSpace(motivation) ? null : motivation.Trim();

        UpdatedAt = DateTimeOffset.UtcNow;
    }
    public void UpdatePlannedDates(DateOnly? startDate, DateOnly? endDate)
    {
        ValidatePlannedDates(startDate, endDate);

        PlannedStartDate = startDate;
        PlannedEndDate = endDate;

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /* VALIDATION METHODS */

    private static string NormalizeRequiredText(string value, string param)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"Project {param} cannot be empty.", param);

        return value.Trim();
    }

    private static void ValidatePlannedDates(DateOnly? startDate, DateOnly? endDate)
    {
        if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            throw new ArgumentException(
                "Planned end date cannot be before planned start date.", nameof(endDate));

    }
}
