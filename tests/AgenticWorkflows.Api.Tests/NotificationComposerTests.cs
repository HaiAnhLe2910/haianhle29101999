using AgenticWorkflows.Api.Models;
using AgenticWorkflows.Api.Services;

namespace AgenticWorkflows.Api.Tests;

public sealed class NotificationComposerTests
{
    [Fact]
    public void Notifications_truncate_descriptions_longer_than_ninety_characters()
    {
        var description = new string('a', 91);
        var item = CreateWorkItem(description: description, dueDate: new DateOnly(2026, 6, 15));
        var expectedDescription = $"Description: {new string('a', 87)}...";

        var createdNotification = NotificationComposer.BuildCreatedNotification(item);
        var dueSoonNotification = NotificationComposer.BuildDueSoonNotification(item);

        Assert.Contains(expectedDescription, createdNotification);
        Assert.Contains(expectedDescription, dueSoonNotification);
    }

    [Fact]
    public void Notifications_omit_due_date_line_when_due_date_is_null()
    {
        var item = CreateWorkItem(description: "Track rollout details", dueDate: null);

        var createdNotification = NotificationComposer.BuildCreatedNotification(item);
        var dueSoonNotification = NotificationComposer.BuildDueSoonNotification(item);

        Assert.DoesNotContain("Due date:", createdNotification);
        Assert.DoesNotContain("Due date:", dueSoonNotification);
    }

    [Fact]
    public void Created_and_due_soon_notifications_include_expected_next_steps()
    {
        var item = CreateWorkItem(description: "Follow up with stakeholders", dueDate: new DateOnly(2026, 6, 20));

        var createdNotification = NotificationComposer.BuildCreatedNotification(item);
        var dueSoonNotification = NotificationComposer.BuildDueSoonNotification(item);

        Assert.Contains("Next step: Review the backlog and assign an owner.", createdNotification);
        Assert.Contains("Next step: Confirm the item still belongs in this sprint.", dueSoonNotification);
    }

    private static WorkItem CreateWorkItem(string? description, DateOnly? dueDate) =>
        new(
            Guid.NewGuid(),
            "Prepare release brief",
            description,
            3,
            WorkItemStatus.Todo,
            dueDate);
}
