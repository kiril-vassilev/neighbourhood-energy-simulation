# Neighborhood Energy Simulation

A C# simulation project for modeling energy consumption and production in a neighborhood environment.

## Overview

This project simulates various energy assets and their interactions within a neighborhood, including:

- Residential houses with base loads
- Photovoltaic (PV) systems for solar energy generation
- Electric vehicle (EV) chargers
- Heat pumps for heating/cooling
- Public charging stations
- Weather simulation and its impact on energy usage

## Architecture

The project is structured as follows:

- **Simulation.BLL**: Business logic layer containing the core simulation engine, domain models, and main program
- **Core**: Contains simulation clock, context, engine, and factory classes
- **Domain**: Defines energy assets, neighborhood, and weather components

## Simulation Assumptions

This simulation models a simplified residential neighbourhood energy system. The goal is clarity and determinism rather than full physical accuracy. The following assumptions were made:

## EV Charging Behavior
Home EV Chargers
Charging occurs only during evening hours (18:00–23:00).
Charging power is fixed at 7 kW (typical single-phase residential charger).
Vehicles are assumed to:
Arrive home before charging window
Require charging every day
No smart charging or load balancing is implemented.

Rationale:
This reflects common real-world behavior where users plug in vehicles after work, creating an evening peak.

Public EV Chargers (6 units)
Each charger operates independently.
At each simulation step, usage is determined by a probabilistic model:
~30% chance of being occupied
When occupied, charger draws 22 kW (typical AC public charger).
No queuing or reservation system is modeled.

Rationale:
This provides a simple but dynamic shared infrastructure model without introducing complex agent-based behavior.

## PV (Solar Panels)
PV generation is modeled as:
A function of time of day (sinusoidal curve)
Scaled by a solar factor (weather-dependent)
Peak production occurs around midday.
No generation at night.
Energy Usage Model
PV generation is first used locally to offset household consumption.
Excess generation results in net export to the grid:
Represented as negative load at household/neighbourhood level
No battery storage is included.

Rationale:
This reflects a common grid-tied residential PV setup without storage.

## Heat Pump Model

Heat pump consumption is temperature-dependent:

No usage when temperature > 18°C
Below 18°C, consumption increases linearly:
Power ∝ (18 - Temperature)
Represents space heating demand only.
Does not model:
Building insulation differences
Thermal inertia
COP (Coefficient of Performance)

Rationale:
Provides a simple but effective link between weather and heating demand.

## Base Household Consumption
Every house has a baseline consumption profile:
Morning peak (07:00–09:00)
Evening peak (18:00–22:00)
Low consumption overnight

Rationale:
Captures typical residential usage patterns.

## Weather & Seasonality
Weather is synthetic and deterministic (no external API).
Temperature is based on month (season):
Winter: ~5°C
Summer: ~22°C
Solar production depends on:
Time of day
Simplified solar factor

Rationale:
Ensures reproducibility while still reflecting seasonal effects.

## Energy Accounting

Energy is accumulated using:

Energy (kWh) = Power (kW) × Time (hours)
Simulation uses 15-minute time steps (0.25h).
All assets track:
Instantaneous power (kW)
Cumulative energy (kWh)
🏘️ Neighbourhood Configuration
30 houses (fixed)
6 public EV chargers (fixed)
Asset distribution (deterministic via seed):
~40% PV systems
~30% heat pumps
~20% home EV chargers

Rationale:
Provides realistic diversity while keeping simulation reproducible.

## Limitations
No battery storage
No grid constraints or transformer limits
No dynamic pricing or demand response
No individual user behavior modeling
No EV state-of-charge tracking

## Design Philosophy

The simulation prioritizes:

Clarity and extensibility
Deterministic behavior (via fixed random seed)
Separation of concerns (assets, simulation engine, UI)


## Prerequisites

- .NET 10.0 SDK

## Building the Project

1. Clone or download the repository
2. Navigate to the project root directory
3. Run the following command:

```bash
dotnet build
```

## Running the Simulation

To run the simulation:

```bash
dotnet run --project Simulation.BLL
```

## Configuration

The simulation can be configured through the `SimulationContext` and various domain classes to adjust parameters like weather conditions, energy asset specifications, and simulation duration.