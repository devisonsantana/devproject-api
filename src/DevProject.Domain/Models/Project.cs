namespace DevProject.Domain.Models;

public class Project
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Objective { get; private set; }
    public string? Description { get; private set; }
    public string? Motivation { get; private set; }
    public ProjectStatus Status { get; private set; }
    public DateOnly? PlannedStartDate { get; private set; }
    public DateOnly? PlannedEndDate { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public Project(
        string name,
        string objective,
        string? description = null,
        string? motivation = null,
        DateOnly? plannedStartDate = null,
        DateOnly? plannedEndDate = null)
    {
        ValidateName(name);
        ValidateObjective(objective);
        ValidatePlannedDates(plannedStartDate, plannedEndDate);

        var now = DateTimeOffset.Now;

        Id = Guid.NewGuid();
        Name = name.Trim();
        Objective = objective.Trim();
        Description = description;
        Motivation = motivation;
        Status = ProjectStatus.Planning;
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public void UpdateName(string name)
    {
        ValidateName(name);

        Name = name.Trim();

        UpdatedAt = DateTimeOffset.Now;
    }
    public void UpdateObjective(string objective)
    {
        ValidateObjective(objective);

        Objective = objective.Trim();

        UpdatedAt = DateTimeOffset.Now;
    }

    public void UpdatePlannedDates(DateOnly? startDate, DateOnly? endDate)
    {
        ValidatePlannedDates(startDate, endDate);

        PlannedStartDate = startDate;
        PlannedEndDate = endDate;

        UpdatedAt = DateTimeOffset.Now;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Project name cannot be empty.", nameof(name));
    }

    private static void ValidateObjective(string objective)
    {
        if (string.IsNullOrWhiteSpace(objective))
            throw new ArgumentException("Project objective cannot be empty.", nameof(objective));
    }

    private static void ValidatePlannedDates(DateOnly? startDate, DateOnly? endDate)
    {
        if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            throw new ArgumentException("Planned end date cannot be before planned start date.", nameof(endDate));

    }
}
