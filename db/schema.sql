CREATE TABLE dbo.Users (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    Email         NVARCHAR(256)  NOT NULL UNIQUE,
    CreatedAtUtc  DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE TABLE dbo.States (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    Abbreviation  CHAR(2)       NOT NULL UNIQUE,
    Name          NVARCHAR(50)  NOT NULL UNIQUE
);

INSERT INTO dbo.States (Abbreviation, Name) VALUES
('AL','Alabama'), ('AK','Alaska'), ('AZ','Arizona'), ('AR','Arkansas'),
('CA','California'), ('CO','Colorado'), ('CT','Connecticut'), ('DE','Delaware'),
('DC','District of Columbia'), ('FL','Florida'), ('GA','Georgia'), ('HI','Hawaii'),
('ID','Idaho'), ('IL','Illinois'), ('IN','Indiana'), ('IA','Iowa'),
('KS','Kansas'), ('KY','Kentucky'), ('LA','Louisiana'), ('ME','Maine'),
('MD','Maryland'), ('MA','Massachusetts'), ('MI','Michigan'), ('MN','Minnesota'),
('MS','Mississippi'), ('MO','Missouri'), ('MT','Montana'), ('NE','Nebraska'),
('NV','Nevada'), ('NH','New Hampshire'), ('NJ','New Jersey'), ('NM','New Mexico'),
('NY','New York'), ('NC','North Carolina'), ('ND','North Dakota'), ('OH','Ohio'),
('OK','Oklahoma'), ('OR','Oregon'), ('PA','Pennsylvania'), ('RI','Rhode Island'),
('SC','South Carolina'), ('SD','South Dakota'), ('TN','Tennessee'), ('TX','Texas'),
('UT','Utah'), ('VT','Vermont'), ('VA','Virginia'), ('WA','Washington'),
('WV','West Virginia'), ('WI','Wisconsin'), ('WY','Wyoming');

CREATE TABLE dbo.Profiles (
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    UserId              INT            NOT NULL UNIQUE REFERENCES dbo.Users(Id),
    DisplayName         NVARCHAR(100)  NOT NULL,
    BirthDate           DATE           NOT NULL,
    City                NVARCHAR(150)  NOT NULL,
    State               CHAR(2)        NOT NULL REFERENCES dbo.States(Abbreviation),
    ZipCode             CHAR(5)        NOT NULL,
    Pronouns            NVARCHAR(30)   NULL,
    Bio                 NVARCHAR(300)  NULL,
    PreferredDistance   NVARCHAR(30)   NOT NULL,
    CreatedAtUtc        DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAtUtc        DATETIME2      NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE TABLE dbo.ProfilePhotos (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    ProfileId   INT            NOT NULL REFERENCES dbo.Profiles(Id),
    FileName    NVARCHAR(260)  NOT NULL,
    BlobUrl     NVARCHAR(500)  NOT NULL,   -- points at the profile-photos container
    SortOrder   TINYINT        NOT NULL DEFAULT 0
);

CREATE TABLE dbo.Interests (
    Id    TINYINT      IDENTITY(1,1) PRIMARY KEY,
    Name  NVARCHAR(30) NOT NULL
);
INSERT INTO dbo.Interests (Name) VALUES
    ('Hiking'), ('Kayaking'), ('Climbing'), ('Backpacking'), ('Cycling'), ('Camping');

CREATE TABLE dbo.ExperienceLevel (
    Id  TINYINT     IDENTITY(1,1) PRIMARY KEY,
    ExperienceLevel NVARCHAR(30) NOT NULL
);

INSERT INTO dbo.ExperienceLevel (ExperienceLevel) VALUES
    ('Beginner'), ('Intermediate'), ('Advance');

CREATE TABLE dbo.ProfileInterests (
    ProfileId   INT     NOT NULL REFERENCES dbo.Profiles(Id),
    InterestId  TINYINT NOT NULL REFERENCES dbo.Interests(Id),
    ExperienceLevelId TINYINT NOT NULL REFERENCES dbo.ExperienceLevel(Id),
    PRIMARY KEY (ProfileId, InterestId)
);

CREATE TABLE dbo.Goals (
    Id    TINYINT      IDENTITY(1,1) PRIMARY KEY,
    Name  NVARCHAR(30) NOT NULL UNIQUE
);
INSERT INTO dbo.Goals (Name) VALUES
    ('Friends'), ('Adventure Partners'), ('Dating'), ('Group Activities');

CREATE TABLE dbo.ProfileGoals (
    ProfileId  INT     NOT NULL REFERENCES dbo.Profiles(Id),
    GoalId     TINYINT NOT NULL REFERENCES dbo.Goals(Id),
    PRIMARY KEY (ProfileId, GoalId)
);