# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.4.0] - 2017-08-30

### Added

- Added the option to create the start and the end object for the spawn of the enemies

### Changed

- Organized free and path cells in two different gamobjects at the instantiation

### Fixed

- Code is now more readable
- Each path is now connect with the others

## [1.3.0] - 2017-08-26

### Added

- Added a session in README.md to explain how to use the new changes

### Changed

- Replaced constant weight with public variables
- Maps are now serializableObject

### Removed

- Removed Constant file
- MaxSize variable removed for memory boost from class Map

### Fixed

- Code is now more readable

## [1.2.0] - 2017-08-26

### Changed

- Replaced Array with List
- Shuffle method now works for list
- Adapted methods to new list behaviours
- Code refactoring
- Memory boost

### Removed

- Removed node finder methods
- Removed Stopwatch diagnostic tool from code

### Fixed

- Fixed misleading variable name
- Code is now more readable

## [1.1.0] - 2017-08-26

### Changed

- Replaced Coord with Node class to improve performace
- Perfeormed research of the start and end points
- Performace improved of 95%

### Removed

- Removed Coord structure
- Removed Coord's Utility methods

## [1.0.4] - 2017-08-26

### Changed

- Performed comunication between Node class and Coord structure
- Boolean in pathfinding and map generator work now at the same way

### Fixed

- Code is now more readable

## [1.0.3] - 2017-08-26

### Removed

- README contributors section removed

## [1.0.2] - 2017-08-26

### Changed

- README is now more comprehensible

## [1.0.1] - 2017-08-26

### Fixed

- README not showed the .gif

## [1.0.0] - 2017-08-26

### Added

- Added project on GitHub
- Added level random generator scripts
- Added a README
- Added a MIT license