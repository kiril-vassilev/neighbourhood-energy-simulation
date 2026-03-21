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

This simulation models a simplified residential neighbourhood energy system. The goal is clarity, determinism, and extensibility rather than full physical accuracy. The following assumptions were made:

## EV Charging Behavior
Home EV Chargers
Charging occurs only during evening hours (18:00–23:00).
Charging power is fixed at 7 kW (typical residential charger).
Vehicles are assumed to:
Arrive home before the charging window
Require daily charging
No smart charging or scheduling optimization is implemented.

Rationale:
Represents common real-world behavior where EV charging contributes to evening peak demand.

Public EV Chargers (6 units)
Each charger operates independently.
At each simulation step, usage is determined by a probabilistic model (~30% occupancy).
When occupied, a charger draws 22 kW.
No queuing, reservations, or user prioritization is modeled.

Rationale:
Provides a dynamic shared infrastructure model without introducing complex agent-based simulation.

## PV (Solar Panels)
PV generation is modeled as a function of:
Time of day (sinusoidal production curve)
Weather (solar factor)
Peak generation occurs around midday.
No generation during nighttime.
Energy Usage Model
PV generation is first used locally to offset household demand.
Excess generation is exported to the grid, represented as negative load at neighbourhood level.
No battery storage is present at the household level.

Rationale:
Represents a typical grid-connected residential PV system without local storage.

## Heat Pump Model

Heat pump consumption depends on ambient temperature:

No consumption when temperature > 18°C
Below 18°C, consumption increases linearly:
Power ∝ (18 − Temperature)
Models space heating only.
Does not include:
Building insulation differences
Thermal inertia
Coefficient of performance (COP)

Rationale:
Provides a simple but effective relationship between weather and heating demand.

## Neighbourhood Battery (Peak Shaving)

A shared battery is introduced to reduce peak load at neighbourhood level.

Battery Characteristics
Defined by:
Capacity (kWh)
State of Charge (SoC)
Maximum charge/discharge power (kW)
Round-trip efficiency (~95%)
No degradation or lifecycle effects are modeled.
Control Strategy
The battery operates using a threshold-based peak shaving strategy:
When neighbourhood load exceeds a predefined threshold:
Battery discharges to reduce peak load
When load is below the threshold:
Battery charges (if capacity allows)
Constraints
Charging/discharging is limited by:
Max power limits
Available capacity / remaining SoC
State of charge is updated each simulation step.
Grid Interaction
Battery acts at neighbourhood level, not per household.
It reduces net load seen by the grid but does not alter individual household behavior.

Rationale:
Demonstrates a simple but realistic demand-side flexibility mechanism commonly used in smart grids.

## Base Household Consumption
Each house has a predefined daily consumption profile:
Morning peak (07:00–09:00)
Evening peak (18:00–22:00)
Lower demand overnight

Rationale:
Represents typical residential electricity usage patterns.

## Weather & Seasonality
Weather is synthetic and deterministic (no external APIs).
Temperature is determined by season (month-based):
Winter: ~5°C
Summer: ~22°C
Solar production depends on:
Time of day
Simplified solar irradiance factor

Rationale:
Ensures reproducibility while maintaining realistic seasonal influence.

## Energy Accounting

Energy is calculated as:

Energy (kWh) = Power (kW) × Time (hours)

## Simulation Time Step
The simulation uses a 15-minute time step (0.25 hours).

This choice is based on:

Industry practice: 15-minute intervals are commonly used in energy systems and smart metering.
Sufficient accuracy: Captures daily consumption patterns, EV charging behavior, and PV generation without unnecessary detail.
Performance: Provides a good balance between simulation speed and realism.
Visualization: Results in 96 data points per day, which is well-suited for charting the last 24 hours.


Each asset tracks:
Instantaneous power (kW)
Cumulative energy (kWh)
Neighbourhood load is tracked:
Without battery
With battery (net load)

Rationale:
Aligns with standard energy system modeling practices.

## Neighbourhood Configuration
Fixed configuration:
30 houses
6 public EV chargers
Asset distribution (deterministic via seeded randomness):
~40% PV systems
~30% heat pumps
~20% home EV chargers

Rationale:
Ensures reproducibility while maintaining diversity of assets.

## Limitations
No household-level battery storage
No grid constraints (e.g., transformer capacity)
No dynamic pricing or demand response
No EV state-of-charge tracking
No detailed building thermal modeling
Battery uses a simple heuristic (not optimized or predictive)

## Design Philosophy

The simulation prioritizes:

Clarity and modular design (asset-based architecture)
Deterministic behavior (fixed random seed)
Extensibility (easy to add new assets or strategies)
Explainability over physical accuracy

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

## Running Tests

To run the unit tests:

```bash
dotnet test
```

## Configuration

The simulation can be configured through the `SimulationContext` and various domain classes to adjust parameters like weather conditions, energy asset specifications, and simulation duration.