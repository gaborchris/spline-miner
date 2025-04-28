# Spline Miner
A fun and interactive game designed to help players build intuition around splines while enjoying an engaging experience!

![Spline Miner Concept Art](https://github.com/gaborchris/spline-miner/blob/main/spline-miner.png)

---

## MVP Features
### Initial Features
- [ ] **Player Control**:
  - [x] Movement on track
  - [ ] Movement when dismounted from the track system
- [ ] **Spline System**:
  - [x] Basic spline system for track placement
  - [x] Ability to manipulate and drag splines
  - [ ] Splines should only be moveable in pending placement state
  - [ ] Once placed, splines should be fixed in position to prevent odd calculations.
  - [ ] Destructible track should create two splines on either side of the break point
- [ ] **Camera**:
  - [ ] Camera follows player on track
  - [ ] Camera follows player when dismounted
  - [ ] World spans a large area and camera can be moved around
- [ ] **Collision**:
  - [ ] Interaction with the floor
- [ ] **Destructible Blocks**:
  - [ ] Breakable world tiles for dynamic gameplay

---

## Out of Scope
The following features are not included in the current scope:
- [] Enemies
- [] Items or inventory system
- [] Advanced lighting effects

---

## Requirements
### Core Mechanics
- [x] **Spline Interaction**:
  - [x] Drag and manipulate curves (does not need to be a mathematically correct spline)
  - [x] Simple 3-point drag system
- [x] **Track Placement**:
  - [x] Ability to place tracks dynamically
- [ ] **Player Controls**:
  - [x] Keyboard-based control while on the track
  - [ ] Dismounting from the track system
- [ ] **Camera System**:
  - [ ] Smooth and intuitive camera behavior
- [ ] **Physics**:
  - [ ] Collision physics with the floor
  - [ ] Destructible world tiles for interactive environments
