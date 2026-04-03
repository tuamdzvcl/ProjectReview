# Angular folder structure (TicketEvent)

This project uses Angular standalone (`bootstrapApplication` + `app.config.ts`).

## `core/`
One-time, app-wide things (singletons): interceptors, guards, global services, layout shell.

## `shared/`
Reusable building blocks used by many features: UI components, pipes, directives, shared models/utils.

## `features/`
Business domains (event listing, booking, auth, ...). Each feature owns its pages, UI, and data-access.

