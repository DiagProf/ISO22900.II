# Why ISO22900.II-Sharp.Abstractions was merged back (1.4.1-beta)

In 1.4.0-beta the interfaces, data classes, enums and exceptions were moved into a separate package
`ISO22900.II-Sharp.Abstractions`. With 1.4.1-beta this package is **discontinued** and everything is part of
`ISO22900.II-Sharp` again.

## Why

- **Two packages, always the same version.** The implementation uses internal members of the abstractions, so both
  packages had to be released together and referenced with exactly the same version. Mixing versions caused
  restore errors.
- **No real benefit.** The abstractions package was meant for your own implementations (simulators, test doubles).
  The interfaces are still available in `ISO22900.II-Sharp` for that.
- **Harder to maintain.** Type forwarders, version pinning and two READMEs and release notes for one library.

## What this means for you

- **Namespaces are unchanged** (`ISO22900.II`, `ISO22900.II.Interface`). In most cases you don't need to change your code.
- **Remove** any `PackageReference` to `ISO22900.II-Sharp.Abstractions`.
- **Update** `ISO22900.II-Sharp` to `1.4.1-beta` or later.
- Rebuild your project.

If both `ISO22900.II-Sharp.Abstractions` and `ISO22900.II-Sharp` 1.4.1-beta are referenced (directly or through
another package), the compiler reports ambiguous or duplicate types. Removing the Abstractions reference fixes this.

`ISO22900.II-Sharp.Abstractions` and `ISO22900.II-Sharp 1.4.0-beta` are unlisted on NuGet.org.
