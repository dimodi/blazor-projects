# blazor-projects

Blazor projects and sample files for testing and support:

* `AuthorizeView` - an app that uses `<AuthorizeView>`, `Roles`, and `@attribute [Authorize]` to render content and Telerik components conditionally.
* `BlazorWasmWithService` - a WebAssembly app with a client-side data service that gets its data from a remote server end point.
* `EntityFramework` - an app with an Entity Framework (SQLite) CRUD data service and Telerik databound components.
* `Files` - a collection of images and documents for testing of components like FileSelect, PDF Viewer, an Upload.
* `ODataSample` - a standalone WebAssembly app that makes OData requests to a remote end point. Used for testing OData URL serialization for all value types.
* `RazorClassLibrary` - a small Razor Component Library with static JS and CSS assets that can be added as a project dependency to another project.
* `ServerLocalization` - a localized app that shows two ways to use `IStringLocalizer` for general app messages, and a `ITelerikStringLocalizer` service for the Telerik components.
* `TelerikApp` - an app that is similar to the standard Blazor project template, but uses only Telerik components (Drawer, Menu, Button, Grid).
* `TelerikRazorTemplates` - a collection of isolated `.razor` files to quickly start experimenting with Telerik Blazor components.
* `UploadMinimalApi` - a WebAssembly app that uploads files to Minimal API with CORS and Antiforgery tokens. Make sure to set both `MinimalApi` and `WasmAppMinApi` as startup projects.
* `UserSessions` - an app that uses custom cookie-based persistent authentication.
