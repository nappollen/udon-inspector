# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.2.0] - 2026-05-01

### Added
- `AssetLoader` MonoBehaviour (Runtime) to load AssetBundles at runtime (scenes or prefabs)
- Menu item `Tools/Udon Inspector/Load AssetBundle` — opens a file picker, assigns the path, and marks the component dirty
- Menu item `Tools/Udon Inspector/Download All ByteCodes` — saves raw bytecode of all scene UdonBehaviours as `.bytecode` files under `Assets/UdonByteCodes/`
- Menu item `Tools/Udon Inspector/Download All Variables` — saves variables of all scene UdonBehaviours as CSV files (Symbol, Type, Value) under `Assets/UdonVariables/`
- `Utils.EscapeName` helper to sanitize file names (replaces characters other than letters, digits and `_` with `_`)
- `Open in ChatGPT` button next to `Copy` in the Assembly panel — builds a conversion prompt, copies it to clipboard, and opens ChatGPT

### Performance
- Assembly extraction deferred via `EditorApplication.delayCall` — no UI freeze and no cross-thread Unity API calls
- Assembly display capped at 32 768 characters to prevent `OutOfMemoryException` on very large programs; a truncation notice redirects to the Download menu
- Assembly and variables cached by behaviour instance ID — repeated focus/hierarchy-change refreshes no longer re-extract or re-render unchanged data
- Stale deferred results discarded when selection changes before the callback fires

### Fixed
- `AssetLoader` placed in `Runtime/` folder with a dedicated assembly definition — Unity does not allow MonoBehaviours inside the `Editor/` folder
- File names in Download All operations sanitized via `Utils.EscapeName` to avoid invalid path characters
- `UnityException: ToString can only be called from the main thread` — removed `Task.Run`; extraction runs on the main thread via `EditorApplication.delayCall`
- `OutOfMemoryException` when inspecting a large UdonBehaviour — fixed by capping the rendered UI to 32 768 chars

### Removed
- `Export Variables` menu item (superseded by `Download All Variables`)

## [1.1.0] - 2025-12-19

### Changed
- Update menu item path 
- Improve symbol sorting
- Remove unused code

## [1.0.0] - 2025-12-14

### Added

- Initial release
- Package template created with `nappollen.packager`
- Can inspect any UdonBehaviour components in the scene
- Displays Udon program source code in the inspector
- Event handler methods are highlighted
- Variable declarations and values are shown
- Search element to filter the UdonBehaviours by name
- Assembler code view from ByteCode if source code is not available
- HexRepresentation is present, but not used.

