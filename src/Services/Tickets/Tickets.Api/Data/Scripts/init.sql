USE DeskPilotTickets;
GO

CREATE TABLE Categories (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE()
);

CREATE TABLE Tickets (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    CustomerId UNIQUEIDENTIFIER NOT NULL,
    AssignedAgentId UNIQUEIDENTIFIER NULL,
    CategoryId UNIQUEIDENTIFIER NULL REFERENCES Categories(Id),
    Subject NVARCHAR(300) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    Status NVARCHAR(30) NOT NULL DEFAULT 'Open',   -- Open, InProgress, OnHold, Resolved, Closed
    Priority NVARCHAR(20) NOT NULL DEFAULT 'Medium', -- Low, Medium, High, Urgent
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    ResolvedAt DATETIME2 NULL
);

CREATE TABLE Comments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    TicketId UNIQUEIDENTIFIER NOT NULL REFERENCES Tickets(Id),
    AuthorId UNIQUEIDENTIFIER NOT NULL,
    Body NVARCHAR(MAX) NOT NULL,
    IsInternal BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE()
);

-- Index to speed up the paginated/filtered list endpoint (mirrors your resume's query optimization work)
CREATE INDEX IX_Tickets_Tenant_Status_Priority ON Tickets (TenantId, Status, Priority);
GO