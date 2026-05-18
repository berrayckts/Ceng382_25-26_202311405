IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Caterers] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(120) NOT NULL,
    [Slug] nvarchar(450) NOT NULL,
    [BrandStory] nvarchar(600) NULL,
    [City] nvarchar(80) NULL,
    [AddressLine] nvarchar(200) NULL,
    [Latitude] decimal(9,6) NOT NULL,
    [Longitude] decimal(9,6) NOT NULL,
    CONSTRAINT [PK_Caterers] PRIMARY KEY ([Id])
);

CREATE TABLE [CuisineCategories] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(80) NOT NULL,
    [Slug] nvarchar(450) NOT NULL,
    [IconKey] nvarchar(40) NOT NULL,
    CONSTRAINT [PK_CuisineCategories] PRIMARY KEY ([Id])
);

CREATE TABLE [Customers] (
    [Id] int NOT NULL IDENTITY,
    [DisplayName] nvarchar(120) NOT NULL,
    [Email] nvarchar(140) NOT NULL,
    [City] nvarchar(80) NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([Id])
);

CREATE TABLE [SystemLogs] (
    [Id] int NOT NULL IDENTITY,
    [CreatedUtc] datetime2 NOT NULL,
    [Level] nvarchar(20) NOT NULL,
    [EventName] nvarchar(80) NOT NULL,
    [ContextData] nvarchar(1200) NULL,
    CONSTRAINT [PK_SystemLogs] PRIMARY KEY ([Id])
);

CREATE TABLE [MenuItems] (
    [Id] int NOT NULL IDENTITY,
    [CatererId] int NOT NULL,
    [CategoryId] int NOT NULL,
    [Name] nvarchar(120) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [BasePrice] decimal(10,2) NOT NULL,
    [ImagePath] nvarchar(240) NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_MenuItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MenuItems_Caterers_CatererId] FOREIGN KEY ([CatererId]) REFERENCES [Caterers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MenuItems_CuisineCategories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [CuisineCategories] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Orders] (
    [Id] int NOT NULL IDENTITY,
    [CustomerId] int NOT NULL,
    [CatererId] int NOT NULL,
    [CreatedUtc] datetime2 NOT NULL,
    [Status] nvarchar(40) NOT NULL,
    [TotalAmount] decimal(10,2) NOT NULL,
    [PaymentReference] nvarchar(80) NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Orders_Caterers_CatererId] FOREIGN KEY ([CatererId]) REFERENCES [Caterers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Orders_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [MenuItemOptionGroups] (
    [Id] int NOT NULL IDENTITY,
    [MenuItemId] int NOT NULL,
    [Title] nvarchar(80) NOT NULL,
    [AllowsMultipleSelections] bit NOT NULL,
    [IsRequired] bit NOT NULL,
    CONSTRAINT [PK_MenuItemOptionGroups] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MenuItemOptionGroups_MenuItems_MenuItemId] FOREIGN KEY ([MenuItemId]) REFERENCES [MenuItems] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [MenuItemReviews] (
    [Id] int NOT NULL IDENTITY,
    [OrderId] int NOT NULL,
    [MenuItemId] int NOT NULL,
    [ItemRating] int NOT NULL,
    [CatererRating] int NOT NULL,
    [Comment] nvarchar(500) NULL,
    [CreatedUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_MenuItemReviews] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MenuItemReviews_MenuItems_MenuItemId] FOREIGN KEY ([MenuItemId]) REFERENCES [MenuItems] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MenuItemReviews_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [OrderLines] (
    [Id] int NOT NULL IDENTITY,
    [OrderId] int NOT NULL,
    [MenuItemId] int NOT NULL,
    [Quantity] int NOT NULL,
    [UnitPrice] decimal(10,2) NOT NULL,
    CONSTRAINT [PK_OrderLines] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderLines_MenuItems_MenuItemId] FOREIGN KEY ([MenuItemId]) REFERENCES [MenuItems] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_OrderLines_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [MenuItemOptions] (
    [Id] int NOT NULL IDENTITY,
    [OptionGroupId] int NOT NULL,
    [Title] nvarchar(80) NOT NULL,
    [PriceDelta] decimal(10,2) NOT NULL,
    [IsRemovable] bit NOT NULL,
    CONSTRAINT [PK_MenuItemOptions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MenuItemOptions_MenuItemOptionGroups_OptionGroupId] FOREIGN KEY ([OptionGroupId]) REFERENCES [MenuItemOptionGroups] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [OrderLineOptions] (
    [Id] int NOT NULL IDENTITY,
    [OrderLineId] int NOT NULL,
    [OptionLabel] nvarchar(80) NOT NULL,
    [PriceDelta] decimal(10,2) NOT NULL,
    CONSTRAINT [PK_OrderLineOptions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderLineOptions_OrderLines_OrderLineId] FOREIGN KEY ([OrderLineId]) REFERENCES [OrderLines] ([Id]) ON DELETE CASCADE
);

CREATE UNIQUE INDEX [IX_Caterers_Slug] ON [Caterers] ([Slug]);

CREATE UNIQUE INDEX [IX_CuisineCategories_Slug] ON [CuisineCategories] ([Slug]);

CREATE INDEX [IX_MenuItemOptionGroups_MenuItemId] ON [MenuItemOptionGroups] ([MenuItemId]);

CREATE INDEX [IX_MenuItemOptions_OptionGroupId] ON [MenuItemOptions] ([OptionGroupId]);

CREATE INDEX [IX_MenuItemReviews_MenuItemId] ON [MenuItemReviews] ([MenuItemId]);

CREATE INDEX [IX_MenuItemReviews_OrderId] ON [MenuItemReviews] ([OrderId]);

CREATE INDEX [IX_MenuItems_CategoryId] ON [MenuItems] ([CategoryId]);

CREATE INDEX [IX_MenuItems_CatererId] ON [MenuItems] ([CatererId]);

CREATE INDEX [IX_OrderLineOptions_OrderLineId] ON [OrderLineOptions] ([OrderLineId]);

CREATE INDEX [IX_OrderLines_MenuItemId] ON [OrderLines] ([MenuItemId]);

CREATE INDEX [IX_OrderLines_OrderId] ON [OrderLines] ([OrderId]);

CREATE INDEX [IX_Orders_CatererId] ON [Orders] ([CatererId]);

CREATE INDEX [IX_Orders_CustomerId] ON [Orders] ([CustomerId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260424103748_Week1InitialProjectSchema', N'10.0.5');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [SystemLogs] ADD [ActorEmail] nvarchar(160) NULL;

ALTER TABLE [SystemLogs] ADD [UserId] int NULL;

ALTER TABLE [Orders] ADD [CompletedUtc] datetime2 NULL;

ALTER TABLE [Orders] ADD [DeliveryAddress] nvarchar(240) NULL;

ALTER TABLE [Orders] ADD [EventNotes] nvarchar(600) NULL;

ALTER TABLE [OrderLines] ADD [CustomizationSummary] nvarchar(600) NULL;

ALTER TABLE [Customers] ADD [Latitude] decimal(9,6) NOT NULL DEFAULT 0.0;

ALTER TABLE [Customers] ADD [Longitude] decimal(9,6) NOT NULL DEFAULT 0.0;

ALTER TABLE [Caterers] ADD [Email] nvarchar(160) NULL;

ALTER TABLE [Caterers] ADD [IsApproved] bit NOT NULL DEFAULT CAST(0 AS bit);

ALTER TABLE [Caterers] ADD [Phone] nvarchar(40) NULL;

CREATE TABLE [AppUsers] (
    [Id] int NOT NULL IDENTITY,
    [FullName] nvarchar(120) NOT NULL,
    [Email] nvarchar(160) NOT NULL,
    [PasswordHash] nvarchar(500) NOT NULL,
    [Role] nvarchar(30) NOT NULL,
    [IsActive] bit NOT NULL,
    [IsTwoFactorEnabled] bit NOT NULL,
    [TwoFactorCode] nvarchar(12) NULL,
    [TwoFactorExpiresUtc] datetime2 NULL,
    [CatererProfileId] int NULL,
    [CustomerProfileId] int NULL,
    [CreatedUtc] datetime2 NOT NULL,
    CONSTRAINT [PK_AppUsers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AppUsers_Caterers_CatererProfileId] FOREIGN KEY ([CatererProfileId]) REFERENCES [Caterers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_AppUsers_Customers_CustomerProfileId] FOREIGN KEY ([CustomerProfileId]) REFERENCES [Customers] ([Id]) ON DELETE NO ACTION
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CatererProfileId', N'CreatedUtc', N'CustomerProfileId', N'Email', N'FullName', N'IsActive', N'IsTwoFactorEnabled', N'PasswordHash', N'Role', N'TwoFactorCode', N'TwoFactorExpiresUtc') AND [object_id] = OBJECT_ID(N'[AppUsers]'))
    SET IDENTITY_INSERT [AppUsers] ON;
INSERT INTO [AppUsers] ([Id], [CatererProfileId], [CreatedUtc], [CustomerProfileId], [Email], [FullName], [IsActive], [IsTwoFactorEnabled], [PasswordHash], [Role], [TwoFactorCode], [TwoFactorExpiresUtc])
VALUES (1, NULL, '2026-01-01T00:00:00.0000000Z', NULL, N'admin@caterease.local', N'CaterEase Admin', CAST(1 AS bit), CAST(0 AS bit), N'PBKDF2$100000$AAAAAAAAAAAAAAAAAAAAAA==$iMGgUv1EvOOAQO682i83Gg2X3pL2BniTeSFy0dFj/Vs=', N'Admin', NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CatererProfileId', N'CreatedUtc', N'CustomerProfileId', N'Email', N'FullName', N'IsActive', N'IsTwoFactorEnabled', N'PasswordHash', N'Role', N'TwoFactorCode', N'TwoFactorExpiresUtc') AND [object_id] = OBJECT_ID(N'[AppUsers]'))
    SET IDENTITY_INSERT [AppUsers] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AddressLine', N'BrandStory', N'City', N'Email', N'IsApproved', N'Latitude', N'Longitude', N'Name', N'Phone', N'Slug') AND [object_id] = OBJECT_ID(N'[Caterers]'))
    SET IDENTITY_INSERT [Caterers] ON;
INSERT INTO [Caterers] ([Id], [AddressLine], [BrandStory], [City], [Email], [IsApproved], [Latitude], [Longitude], [Name], [Phone], [Slug])
VALUES (1, N'Besiktas, Istanbul', N'Seasonal event catering with polished mezze tables, warm mains, and careful service.', N'Istanbul', N'caterer@caterease.local', CAST(1 AS bit), 41.043, 29.0094, N'Istanbul Garden Catering', N'+90 212 000 00 00', N'istanbul-garden');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AddressLine', N'BrandStory', N'City', N'Email', N'IsApproved', N'Latitude', N'Longitude', N'Name', N'Phone', N'Slug') AND [object_id] = OBJECT_ID(N'[Caterers]'))
    SET IDENTITY_INSERT [Caterers] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'IconKey', N'Name', N'Slug') AND [object_id] = OBJECT_ID(N'[CuisineCategories]'))
    SET IDENTITY_INSERT [CuisineCategories] ON;
INSERT INTO [CuisineCategories] ([Id], [IconKey], [Name], [Slug])
VALUES (1, N'briefcase', N'Corporate Lunch', N'corporate-lunch'),
(2, N'sparkles', N'Wedding Table', N'wedding-table'),
(3, N'leaf', N'Vegan Feast', N'vegan-feast');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'IconKey', N'Name', N'Slug') AND [object_id] = OBJECT_ID(N'[CuisineCategories]'))
    SET IDENTITY_INSERT [CuisineCategories] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'City', N'DisplayName', N'Email', N'Latitude', N'Longitude') AND [object_id] = OBJECT_ID(N'[Customers]'))
    SET IDENTITY_INSERT [Customers] ON;
INSERT INTO [Customers] ([Id], [City], [DisplayName], [Email], [Latitude], [Longitude])
VALUES (1, N'Istanbul', N'Demo User', N'user@caterease.local', 41.0082, 28.9784);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'City', N'DisplayName', N'Email', N'Latitude', N'Longitude') AND [object_id] = OBJECT_ID(N'[Customers]'))
    SET IDENTITY_INSERT [Customers] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CatererProfileId', N'CreatedUtc', N'CustomerProfileId', N'Email', N'FullName', N'IsActive', N'IsTwoFactorEnabled', N'PasswordHash', N'Role', N'TwoFactorCode', N'TwoFactorExpiresUtc') AND [object_id] = OBJECT_ID(N'[AppUsers]'))
    SET IDENTITY_INSERT [AppUsers] ON;
INSERT INTO [AppUsers] ([Id], [CatererProfileId], [CreatedUtc], [CustomerProfileId], [Email], [FullName], [IsActive], [IsTwoFactorEnabled], [PasswordHash], [Role], [TwoFactorCode], [TwoFactorExpiresUtc])
VALUES (2, 1, '2026-01-01T00:00:00.0000000Z', NULL, N'caterer@caterease.local', N'Istanbul Garden Manager', CAST(1 AS bit), CAST(0 AS bit), N'PBKDF2$100000$AAAAAAAAAAAAAAAAAAAAAA==$iMGgUv1EvOOAQO682i83Gg2X3pL2BniTeSFy0dFj/Vs=', N'Caterer', NULL, NULL),
(3, NULL, '2026-01-01T00:00:00.0000000Z', 1, N'user@caterease.local', N'Demo User', CAST(1 AS bit), CAST(1 AS bit), N'PBKDF2$100000$AAAAAAAAAAAAAAAAAAAAAA==$iMGgUv1EvOOAQO682i83Gg2X3pL2BniTeSFy0dFj/Vs=', N'User', NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CatererProfileId', N'CreatedUtc', N'CustomerProfileId', N'Email', N'FullName', N'IsActive', N'IsTwoFactorEnabled', N'PasswordHash', N'Role', N'TwoFactorCode', N'TwoFactorExpiresUtc') AND [object_id] = OBJECT_ID(N'[AppUsers]'))
    SET IDENTITY_INSERT [AppUsers] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'BasePrice', N'CategoryId', N'CatererId', N'Description', N'ImagePath', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[MenuItems]'))
    SET IDENTITY_INSERT [MenuItems] ON;
INSERT INTO [MenuItems] ([Id], [BasePrice], [CategoryId], [CatererId], [Description], [ImagePath], [IsActive], [Name])
VALUES (1, 420.0, 1, 1, N'A polished mezze selection with warm flatbread, salads, and dips for office events.', N'/images/menu/mezze.svg', CAST(1 AS bit), N'Executive Mezze Box'),
(2, 780.0, 2, 1, N'Slow-roasted lamb with seasonal vegetables and event-ready plating.', N'/images/menu/lamb.svg', CAST(1 AS bit), N'Rosemary Lamb Service'),
(3, 360.0, 3, 1, N'Plant-forward mains, grains, and bright sauces for mixed guest lists.', N'/images/menu/vegan.svg', CAST(1 AS bit), N'Green Garden Banquet');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'BasePrice', N'CategoryId', N'CatererId', N'Description', N'ImagePath', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[MenuItems]'))
    SET IDENTITY_INSERT [MenuItems] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AllowsMultipleSelections', N'IsRequired', N'MenuItemId', N'Title') AND [object_id] = OBJECT_ID(N'[MenuItemOptionGroups]'))
    SET IDENTITY_INSERT [MenuItemOptionGroups] ON;
INSERT INTO [MenuItemOptionGroups] ([Id], [AllowsMultipleSelections], [IsRequired], [MenuItemId], [Title])
VALUES (1, CAST(1 AS bit), CAST(0 AS bit), 1, N'Removable Ingredients'),
(2, CAST(1 AS bit), CAST(0 AS bit), 1, N'Optional Extras'),
(3, CAST(0 AS bit), CAST(1 AS bit), 2, N'Service Style');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AllowsMultipleSelections', N'IsRequired', N'MenuItemId', N'Title') AND [object_id] = OBJECT_ID(N'[MenuItemOptionGroups]'))
    SET IDENTITY_INSERT [MenuItemOptionGroups] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'IsRemovable', N'OptionGroupId', N'PriceDelta', N'Title') AND [object_id] = OBJECT_ID(N'[MenuItemOptions]'))
    SET IDENTITY_INSERT [MenuItemOptions] ON;
INSERT INTO [MenuItemOptions] ([Id], [IsRemovable], [OptionGroupId], [PriceDelta], [Title])
VALUES (1, CAST(1 AS bit), 1, 0.0, N'Remove walnuts'),
(2, CAST(1 AS bit), 1, 0.0, N'Remove dairy sauce'),
(3, CAST(0 AS bit), 2, 95.0, N'Add dessert bites'),
(4, CAST(0 AS bit), 2, 120.0, N'Add premium drinks'),
(5, CAST(0 AS bit), 3, 0.0, N'Buffet setup'),
(6, CAST(0 AS bit), 3, 180.0, N'Plated service');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'IsRemovable', N'OptionGroupId', N'PriceDelta', N'Title') AND [object_id] = OBJECT_ID(N'[MenuItemOptions]'))
    SET IDENTITY_INSERT [MenuItemOptions] OFF;

CREATE INDEX [IX_AppUsers_CatererProfileId] ON [AppUsers] ([CatererProfileId]);

CREATE INDEX [IX_AppUsers_CustomerProfileId] ON [AppUsers] ([CustomerProfileId]);

CREATE UNIQUE INDEX [IX_AppUsers_Email] ON [AppUsers] ([Email]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260513083908_FullCaterEasePlatform', N'10.0.5');

COMMIT;
GO

BEGIN TRANSACTION;
UPDATE [AppUsers] SET [IsTwoFactorEnabled] = CAST(0 AS bit)
WHERE [Id] = 3;
SELECT @@ROWCOUNT;


INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260513084111_SeedDemoUserWithoutTwoFactor', N'10.0.5');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE TABLE [AspNetRoles] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetUsers] (
    [Id] int NOT NULL IDENTITY,
    [FullName] nvarchar(120) NOT NULL,
    [IsActive] bit NOT NULL,
    [CatererProfileId] int NULL,
    [CustomerProfileId] int NULL,
    [CreatedUtc] datetime2 NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUsers_Caterers_CatererProfileId] FOREIGN KEY ([CatererProfileId]) REFERENCES [Caterers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_AspNetUsers_Customers_CustomerProfileId] FOREIGN KEY ([CustomerProfileId]) REFERENCES [Customers] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] int NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserRoles] (
    [UserId] int NOT NULL,
    [RoleId] int NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserTokens] (
    [UserId] int NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);

CREATE INDEX [IX_AspNetUsers_CatererProfileId] ON [AspNetUsers] ([CatererProfileId]);

CREATE INDEX [IX_AspNetUsers_CustomerProfileId] ON [AspNetUsers] ([CustomerProfileId]);

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260513115029_AddAspNetIdentity', N'10.0.5');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE TABLE [PlaceCatererMaps] (
    [Id] int NOT NULL IDENTITY,
    [GooglePlaceId] nvarchar(160) NOT NULL,
    [CatererId] int NOT NULL,
    [LinkedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_PlaceCatererMaps] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PlaceCatererMaps_Caterers_CatererId] FOREIGN KEY ([CatererId]) REFERENCES [Caterers] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_PlaceCatererMaps_CatererId] ON [PlaceCatererMaps] ([CatererId]);

CREATE UNIQUE INDEX [IX_PlaceCatererMaps_GooglePlaceId] ON [PlaceCatererMaps] ([GooglePlaceId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260515114246_AddPlaceCatererMaps', N'10.0.5');

COMMIT;
GO

BEGIN TRANSACTION;
DROP TABLE [PlaceCatererMaps];

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AddressLine', N'BrandStory', N'City', N'Email', N'IsApproved', N'Latitude', N'Longitude', N'Name', N'Phone', N'Slug') AND [object_id] = OBJECT_ID(N'[Caterers]'))
    SET IDENTITY_INSERT [Caterers] ON;
INSERT INTO [Caterers] ([Id], [AddressLine], [BrandStory], [City], [Email], [IsApproved], [Latitude], [Longitude], [Name], [Phone], [Slug])
VALUES (101, N'Galata, Beyoglu, Istanbul', N'Golden-hour tasting menus and refined corporate catering near Galata.', N'Istanbul', N'galata@catera.local', CAST(1 AS bit), 41.0256, 28.9744, N'Galata Banquet Studio', N'+90 212 111 22 33', N'galata-banquet-studio'),
(102, N'Nisantasi, Sisli, Istanbul', N'Boutique dinner service and elegant celebration menus around Nisantasi.', N'Istanbul', N'nisantasi@catera.local', CAST(1 AS bit), 41.0504, 28.991, N'Nisantasi Private Table', N'+90 212 222 44 55', N'nisantasi-private-table');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AddressLine', N'BrandStory', N'City', N'Email', N'IsApproved', N'Latitude', N'Longitude', N'Name', N'Phone', N'Slug') AND [object_id] = OBJECT_ID(N'[Caterers]'))
    SET IDENTITY_INSERT [Caterers] OFF;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260515115751_ReplaceGooglePlacesWithLeafletLocations', N'10.0.5');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [Caterers] ADD [District] nvarchar(80) NULL;

UPDATE [Caterers] SET [District] = N'Besiktas'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;


UPDATE [Caterers] SET [District] = N'Beyoglu'
WHERE [Id] = 101;
SELECT @@ROWCOUNT;


UPDATE [Caterers] SET [District] = N'Sisli'
WHERE [Id] = 102;
SELECT @@ROWCOUNT;


INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260515121350_AddCatererDistrictProfileLocation', N'10.0.5');

COMMIT;
GO

BEGIN TRANSACTION;
UPDATE [AppUsers] SET [Role] = N'RestaurantOwner'
WHERE [Id] = 2;
SELECT @@ROWCOUNT;


UPDATE [AppUsers] SET [Role] = N'Customer'
WHERE [Id] = 3;
SELECT @@ROWCOUNT;


INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260515125310_AlignRoleNamesForCorporateRbac', N'10.0.5');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [MenuItemOptions] ADD [IsAvailable] bit NOT NULL DEFAULT CAST(1 AS bit);

ALTER TABLE [MenuItemOptions] ADD [IsDefault] bit NOT NULL DEFAULT CAST(0 AS bit);

ALTER TABLE [MenuItemOptionGroups] ADD [MaxSelection] int NOT NULL DEFAULT 0;

ALTER TABLE [MenuItemOptionGroups] ADD [MinSelection] int NOT NULL DEFAULT 0;

UPDATE [MenuItemOptionGroups] SET [MaxSelection] = 3
WHERE [Id] = 1;
SELECT @@ROWCOUNT;


UPDATE [MenuItemOptionGroups] SET [MaxSelection] = 3
WHERE [Id] = 2;
SELECT @@ROWCOUNT;


UPDATE [MenuItemOptionGroups] SET [MaxSelection] = 1, [MinSelection] = 1
WHERE [Id] = 3;
SELECT @@ROWCOUNT;


UPDATE [MenuItemOptions] SET [IsAvailable] = CAST(1 AS bit)
WHERE [Id] = 1;
SELECT @@ROWCOUNT;


UPDATE [MenuItemOptions] SET [IsAvailable] = CAST(1 AS bit)
WHERE [Id] = 2;
SELECT @@ROWCOUNT;


UPDATE [MenuItemOptions] SET [IsAvailable] = CAST(1 AS bit)
WHERE [Id] = 3;
SELECT @@ROWCOUNT;


UPDATE [MenuItemOptions] SET [IsAvailable] = CAST(1 AS bit)
WHERE [Id] = 4;
SELECT @@ROWCOUNT;


UPDATE [MenuItemOptions] SET [IsAvailable] = CAST(1 AS bit), [IsDefault] = CAST(1 AS bit)
WHERE [Id] = 5;
SELECT @@ROWCOUNT;


UPDATE [MenuItemOptions] SET [IsAvailable] = CAST(1 AS bit)
WHERE [Id] = 6;
SELECT @@ROWCOUNT;


INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260516121657_AddMenuOptionSelectionMetadata', N'10.0.5');

COMMIT;
GO

BEGIN TRANSACTION;
UPDATE [MenuItems]
SET [BasePrice] = [BasePrice] + 300

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260517130000_IncreaseMenuBasePrices', N'10.0.5');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [MenuItemOptions] ADD [CreatedAt] datetime2 NOT NULL DEFAULT (GETUTCDATE());

ALTER TABLE [MenuItemOptions] ADD [OptionType] nvarchar(24) NOT NULL DEFAULT N'Extra';

ALTER TABLE [MenuItemOptions] ADD [QuantityInfo] nvarchar(80) NULL;

UPDATE [MenuItemOptions] SET [CreatedAt] = '2026-01-01T00:00:00.0000000Z', [OptionType] = N'Removable', [QuantityInfo] = NULL
WHERE [Id] = 1;
SELECT @@ROWCOUNT;


UPDATE [MenuItemOptions] SET [CreatedAt] = '2026-01-01T00:00:00.0000000Z', [OptionType] = N'Removable', [QuantityInfo] = NULL
WHERE [Id] = 2;
SELECT @@ROWCOUNT;


UPDATE [MenuItemOptions] SET [CreatedAt] = '2026-01-01T00:00:00.0000000Z', [OptionType] = N'Extra', [QuantityInfo] = NULL
WHERE [Id] = 3;
SELECT @@ROWCOUNT;


UPDATE [MenuItemOptions] SET [CreatedAt] = '2026-01-01T00:00:00.0000000Z', [OptionType] = N'Extra', [QuantityInfo] = NULL
WHERE [Id] = 4;
SELECT @@ROWCOUNT;


UPDATE [MenuItemOptions] SET [CreatedAt] = '2026-01-01T00:00:00.0000000Z', [OptionType] = N'Extra', [QuantityInfo] = NULL
WHERE [Id] = 5;
SELECT @@ROWCOUNT;


UPDATE [MenuItemOptions] SET [CreatedAt] = '2026-01-01T00:00:00.0000000Z', [OptionType] = N'Extra', [QuantityInfo] = NULL
WHERE [Id] = 6;
SELECT @@ROWCOUNT;


INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260518060208_AddMenuOptionTypeMetadata', N'10.0.5');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [AspNetUsers] ADD [EmailVerificationCode] nvarchar(6) NULL;

ALTER TABLE [AspNetUsers] ADD [EmailVerificationCodeExpiresAt] datetime2 NULL;

ALTER TABLE [AspNetUsers] ADD [EmailVerifiedAt] datetime2 NULL;

ALTER TABLE [AspNetUsers] ADD [IsEmailVerified] bit NOT NULL DEFAULT CAST(1 AS bit);

ALTER TABLE [AspNetUsers] ADD [LastVerificationCodeSentAt] datetime2 NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260518142009_AddEmailVerificationFields', N'10.0.5');

COMMIT;
GO

