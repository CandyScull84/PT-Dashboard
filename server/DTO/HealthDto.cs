namespace FullstackApp1.server.DTO;

public record HealthDto (
    bool DatabaseOk,
    bool SnapshotOk,
    DateTime CheckedAt
);