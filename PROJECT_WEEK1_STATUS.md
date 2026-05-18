# Project Week 1 Status

This document tracks the first implementation step for the CENG 382 catering project on the `project` branch.

## Source of Scope

- `Ceng 382 Web Project (1).pdf`
  - Week 1 roadmap
  - Create a new project branch
  - Start a fresh MVC project direction
  - Decide the database scheme
  - Decide the project brand
  - Build navbar, left bar, footer, icons, and starter CSS
- `Ceng 382 Web Project.pdf`
  - Full project requirements
  - Submission requirements
  - References requirement

## Week 1 Decisions

- Brand: `Mira Feast`
- Domain: premium catering marketplace
- Database approach: `Code First`
- Starter visual language:
  - warm ivory background
  - deep burgundy panels
  - brass accent color
  - left navigation + premium dashboard storefront

## Week 1 Implementation

- `project` branch created
- Home page redesigned as the new branded storefront
- Shared layout replaced with project-specific header/footer shell
- `CateringProjectContext` added for the project database
- Core entities added:
  - `CatererProfile`
  - `CustomerProfile`
  - `CuisineCategory`
  - `MenuItemEntity`
  - `MenuItemOptionGroup`
  - `MenuItemOption`
  - `OrderEntity`
  - `OrderLineEntity`
  - `OrderLineOptionEntity`
  - `MenuItemReview`
  - `SystemLogEntry`
- Roadmap and references page added under `Home/Privacy`

## References

- Course PDF: `Ceng 382 Web Project.pdf`
- Course roadmap PDF: `Ceng 382 Web Project (1).pdf`
- User-shared theme screenshot used as inspiration only
- Existing ASP.NET Core MVC and EF Core project structure in this repository
