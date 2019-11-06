# Hourai Localization

An extension of [Unity3D's Localization](https://docs.unity3d.com/Packages/com.unity.localization@0.5/manual/index.html)
to more localizer friendly workflows. Currently supports Google Spreadsheets as 
as a source of truth in generating localization assets.

## Installation
In Unity 2019.3 and later, add the following to your `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.houraiteahouse.localization": "2.0.0"
  },
  "scopedRegistries": [
    {
      "name": "Hourai Teahouse",
      "url": "https://upm.houraiteahouse.net",
      "scopes": ["com.houraiteahouse"]
    }
  ]
}
```

## Usage

Hourai Localization only exists in the editor. Generator assets can be created
and configured via `Assets > Create > Hourai Teahouse > Localization Generator`.

Each generator is mapped to a `AssetTableCollection`. For every table collection, 
a new generator should be made.

To trigger a project wide regenration of localization data, use the menu item
`Hourai Teahouse > Localization > Generate All`. This can be called 
programmatically via `HouraiTeahouse.Localization.LocalizationGenerator.GenerateAll()`.
This can be useful in continuous integration enviroments to automatically regenerate
localization assets before builds.

Note that localization generators will overwrite all keys in the target collections.
