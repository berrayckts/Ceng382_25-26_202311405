# Week 5 Requirements Map

This file maps the current `week5` implementation to the expected Week 5 deliverables.

Note: there is no separate assignment sheet inside this repository, so the checklist below is inferred from the implemented work and common Week 5 MVC + EF Core lab flow.

## Inferred Week 5 Goals

- Connect the MVC project to the Northwind database
- Scaffold Northwind entities with Entity Framework Core
- Add a custom module/table beyond the original Northwind schema
- Build full CRUD screens for that custom entity
- Relate the custom entity to an existing Northwind table
- Validate input in forms
- Make the module reachable from the UI

## Mapping

### 1. MVC project connected to Northwind

Status: Done

- Connection string is configured in [appsettings.json](C:\Users\berra nur\Desktop\Ceng382_25-26_202311405\Northwind.Mvc\appsettings.json)
- `NorthwindContext` is registered in [Program.cs](C:\Users\berra nur\Desktop\Ceng382_25-26_202311405\Northwind.Mvc\Program.cs)

### 2. Northwind database scaffolded with EF Core

Status: Done

- Scaffolded entity set exists under the `Models` folder
- Main context is in [NorthwindContext.cs](C:\Users\berra nur\Desktop\Ceng382_25-26_202311405\Northwind.Mvc\Models\NorthwindContext.cs)
- Existing Northwind entities such as `Customer`, `Order`, `Product`, and `Shipper` are present in `Northwind.Mvc/Models`

### 3. Custom entity added for Week 5

Status: Done

- Custom entity: [ShippersContactInfo.cs](C:\Users\berra nur\Desktop\Ceng382_25-26_202311405\Northwind.Mvc\Models\ShippersContactInfo.cs)
- EF Core mapping for the custom entity: [NorthwindContext.ShippersContactInfo.cs](C:\Users\berra nur\Desktop\Ceng382_25-26_202311405\Northwind.Mvc\Models\NorthwindContext.ShippersContactInfo.cs)

### 4. Relationship with existing Northwind table

Status: Done

- `ShippersContactInfo.Shipper` is a foreign key to `Shippers`
- Navigation property: `ShipperNavigation`
- FK mapping is configured with `HasForeignKey(d => d.Shipper)` in [NorthwindContext.ShippersContactInfo.cs](C:\Users\berra nur\Desktop\Ceng382_25-26_202311405\Northwind.Mvc\Models\NorthwindContext.ShippersContactInfo.cs)

### 5. CRUD controller

Status: Done

- Controller: [ShippersContactInfoController.cs](C:\Users\berra nur\Desktop\Ceng382_25-26_202311405\Northwind.Mvc\Controllers\ShippersContactInfoController.cs)
- Implemented actions:
  - `Index`
  - `Details`
  - `Create`
  - `Edit`
  - `Delete`

### 6. CRUD Razor views

Status: Done

- [Index.cshtml](C:\Users\berra nur\Desktop\Ceng382_25-26_202311405\Northwind.Mvc\Views\ShippersContactInfo\Index.cshtml)
- [Create.cshtml](C:\Users\berra nur\Desktop\Ceng382_25-26_202311405\Northwind.Mvc\Views\ShippersContactInfo\Create.cshtml)
- [Edit.cshtml](C:\Users\berra nur\Desktop\Ceng382_25-26_202311405\Northwind.Mvc\Views\ShippersContactInfo\Edit.cshtml)
- [Details.cshtml](C:\Users\berra nur\Desktop\Ceng382_25-26_202311405\Northwind.Mvc\Views\ShippersContactInfo\Details.cshtml)
- [Delete.cshtml](C:\Users\berra nur\Desktop\Ceng382_25-26_202311405\Northwind.Mvc\Views\ShippersContactInfo\Delete.cshtml)

### 7. Form validation

Status: Done

- Data annotation validation is applied in [ShippersContactInfo.cs](C:\Users\berra nur\Desktop\Ceng382_25-26_202311405\Northwind.Mvc\Models\ShippersContactInfo.cs)
- Razor validation helpers are used in the create/edit views

### 8. UI navigation

Status: Done

- Home page entry exists in [Index.cshtml](C:\Users\berra nur\Desktop\Ceng382_25-26_202311405\Northwind.Mvc\Views\Home\Index.cshtml)
- Navbar link exists in [_Layout.cshtml](C:\Users\berra nur\Desktop\Ceng382_25-26_202311405\Northwind.Mvc\Views\Shared\_Layout.cshtml)

### 9. Build verification

Status: Done

- `dotnet build Northwind.Mvc.sln` succeeds with 0 errors and 0 warnings

### 10. Database creation script included in repo

Status: Added now

- SQL script added: [create_shippers_contact_infos.sql](C:\Users\berra nur\Desktop\Ceng382_25-26_202311405\sql\create_shippers_contact_infos.sql)

## Current Week 5 Summary

The repository currently satisfies the inferred Week 5 requirements with a Northwind-connected MVC app and a complete custom `ShippersContactInfo` CRUD module tied to the `Shippers` table.
