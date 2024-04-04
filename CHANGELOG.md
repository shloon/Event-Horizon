# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Added `Cubes` sample to core package.
- Added more unit tests to core (for `TrackableTransformComponent`)
- Added de/ser benchmarks for all packet types.
- Added tests comparing ser/de output to Unity's internal JSON writer for future consistency.
- Added empty test assemblies for MetaXR tests
- Added some miscellaneous tests.

### Fixed
- Fixed self-caching of `transform` in `TrackableTransformComponent`.
- Use self-cached transform in `TrackableTransformComponent`.
- Fixed bug in `TrackableTransformComponent` where `translationMultiply` didn't apply correctly.
- Changed `ListViewExample` to `TrackableManagerInspector` as filename dictates.
- Fixed `RecorderComponent` to create recording directory only if path is valid.

### Changed
- Enabled CI on multiple unity LTS versions.
- Changed projects settings to conform to Meta XR SDK recommendations.
- **BREAKING:** Removed file format v2 distinguishing
- **BREAKING:** Prevent deserialization of `frames` with `TransformControlAsset`.
- Moved trackables to their own namespace
- Split `PacketUtils` to their own file
- Move trackables' tests to their own directory.
- Enabled unmarked test in `ActivationTrackableComponentTests`

## [0.1.1] - 2024.01.09

### Fixed
- Improved README documentation

## [0.1.0] - 2024.01.08

Initial Release
