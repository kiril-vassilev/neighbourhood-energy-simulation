# Neighborhood Energy Simulation

A C# simulation project for modeling energy consumption and production in a neighborhood environment.

## Overview

This project simulates various energy assets and their interactions within a neighborhood, including:

- Residential houses with base loads
- Photovoltaic (PV) systems for solar energy generation
- Electric vehicle (EV) chargers
- Heat pumps for heating/cooling
- Public charging stations
- Neighbourhood battery storage for peak shaving
- Weather simulation and its impact on energy usage

Beyond simulation, the project serves as an **interactive decision-support tool**. Through the AI-powered chatbot interface (`Simulation.CHAT`), users can explore energy scenarios and optimization strategies using natural language — querying historical data, comparing seasonal patterns, and reasoning about the impact of different configurations without writing any code.

## Architecture

The project is structured as follows:

- **Simulation.BLL**: Business logic layer containing the core simulation engine, domain models, and main program
  - **Core**: Contains simulation clock, context, engine, and factory classes
  - **Domain**: Defines energy assets, neighborhood, and weather components
- **Simulation.DAL**: Data access layer; responsible for persisting and retrieving simulation history from the database
- **Simulation.REPORT**: CLI reporting tool; generates summary, per-season, and tabular history reports from stored data
- **Simulation.UI**: Blazor web UI that visualizes the live simulation, including current load, temperature, and 24-hour history charts
- **Simulation.CHAT**: Interactive console chatbot that answers natural language questions about the simulation using AI-powered tools
- **Simulation.TEST**: Unit test project covering energy calculations, domain models, and simulation behavior


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
Battery Characteristics

The battery is modeled as a centralized neighbourhood asset with the following properties:

Capacity (kWh) — total stored energy capacity
State of Charge (SoC, kWh) — current stored energy
Maximum charge/discharge power (kW) — limits instantaneous power flow
Round-trip efficiency (~90–95%) — modeled explicitly via charge and discharge losses

Efficiency is applied asymmetrically:

Charging stores less energy than drawn from the grid
Discharging delivers less energy than removed from the battery

This ensures energy conservation and realistic loss modeling.

Battery degradation and lifecycle effects are not modeled.

Control Strategy

The battery uses a threshold-based peak shaving strategy with deadband control.

When neighbourhood load exceeds the upper threshold:
The battery discharges to reduce peak load
When load is below the lower threshold:
The battery charges, if capacity allows
When load is within the deadband:
The battery remains idle

This prevents oscillations and avoids artificial load amplification.

Constraints

Battery operation is constrained by:

Power limits
Charge ≤ MaxChargekW
Discharge ≤ MaxDischargekW
State of Charge (SoC)
Cannot exceed capacity
Cannot drop below zero
Efficiency-aware limits
Maximum charge/discharge power is adjusted based on available SoC and efficiency

State of charge is updated at each simulation step using:

Charging: SoC += P × Δt × η
Discharging: SoC += P × Δt / η
Energy Accounting

The simulation tracks:

Total charged energy (kWh)
Total discharged energy (kWh)
Total battery throughput (kWh)
System-level energy losses

Energy balance is enforced:

Total energy with battery = baseline consumption + battery losses

Grid Interaction
The battery operates at the neighbourhood level, not per household
It modifies the aggregate load profile seen by the grid
Individual household consumption remains unchanged

The primary effects are:

Peak load reduction
Load smoothing (reduced variability)
Increased total energy consumption due to efficiency losses
Key Insights
Peak reduction is primarily limited by battery power (kW), not capacity (kWh)
Battery utilization depends on load variability and threshold selection
Larger batteries may be underutilized if peak events are infrequent

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

## Data Model

The simulation uses a modular, asset-based model:

* Neighbourhood
Contains all houses, public chargers, and an optional battery. Tracks total load, energy, history, and peak values.
* House
Represents a residential unit with a collection of energy assets.
* Energy Assets (IEnergyAsset)
All assets share a common interface and track power (kW) and energy (kWh).
* Includes:
    - BaseLoad (baseline consumption)
    - HeatPump (temperature-driven demand)
    - PvSystem (solar generation, modeled as negative load)
    - EvCharger (home charging)
    - PublicCharger (shared, probabilistic usage)
* BatteryStorage
Optional neighbourhood battery used for peak shaving via charge/discharge control.
* SimulationContext
Provides time, step duration, and weather data for each simulation step.

## Prerequisites

- .NET 10.0 SDK

## Building the Project

1. Clone or download the repository
2. Navigate to the project root directory
3. Run the following command:

```bash
dotnet build
```

## Running Simulation

Run the default simulation mode:

```bash
dotnet run --project Simulation.BLL
```

Show command help:

```bash
dotnet run --project Simulation.BLL -- --help
```

Run data generation mode:

```bash
dotnet run --project Simulation.BLL -- --generate-data
```

### Generate-data mode requirements

- `simulation.endTime` must be set (not null).

If this condition is not met, Simulation.BLL exits with an error message.

### Behavior by mode

- Default mode: detailed console presentation + sleep delay (`runtime.consoleLoopSleepMs`).
- Generate-data mode: no sleep, calls simulation steps without presentation output, and prints simple progress from 0% to 100%.

## Running the UI Visualization

The repository also includes a Blazor UI that visualizes the running neighborhood simulation, including the current time, temperature, live load values, and the last 24 hours of load history.

From the repository root, start the UI with:

```bash
dotnet run --project Simulation.UI
```

Then open one of the local URLs exposed by the app:

- `http://localhost:5264`

If you run the project in Development mode, the browser may open automatically.

## Running Tests

To run the unit tests:

```bash
dotnet test
```

## Running Reports

The `Simulation.REPORT` project is a CLI tool for generating reports from the simulation history stored in the database.

Show command help (also shown when no parameter is provided):

```bash
dotnet run --project Simulation.REPORT -- help
dotnet run --project Simulation.REPORT
```

### Available Parameters

| Parameter | Description |
|-----------|-------------|
| `1` | Print History Summary Report |
| `2` | Print History Summary Report per Season |
| `3` | Print History Report (first 100 rows) |
| `help` | Show help message |

### Examples

```bash
dotnet run --project Simulation.REPORT -- 1
dotnet run --project Simulation.REPORT -- 2
dotnet run --project Simulation.REPORT -- 3
dotnet run --project Simulation.REPORT -- help
```

> **Note:** The database must be populated first by running `Simulation.BLL` in generate-data mode before reports can show data.

## Running the Chatbot

The `Simulation.CHAT` project is an interactive console chatbot that lets you ask natural language questions about the neighborhood energy simulation.

Start the chatbot with:

```bash
dotnet run --project Simulation.CHAT
```

Once running, type a question and press Enter to receive a response. Type `quit` to exit.

```
Neighborhood Energy Simulation Chatbot
Type your question and press Enter. Type 'quit' to exit.
--------------------------------------------------
You: During which season do we use the most energy?
Bot: ...
```

> **Prerequisites:** The following fields must be configured in `simulation.json` before running the chatbot:
> ```json
> "chatbot": {
>   "azureOpenAI_Endpoint": "<your Azure OpenAI endpoint>",
>   "azureOpenAI_DeploymentName": "<your deployment name>"
> }
> ```
> Authentication uses `DefaultAzureCredential` — ensure your environment is authenticated (e.g. via Azure CLI `az login`).

> **Note:** The database should be populated first by running `Simulation.BLL` in generate-data mode so the chatbot has data to reason about.

## Configuration

Simulation settings are centralized in the root `simulation.json` file.

The runtime reads configuration from:

2. `simulation.json` found by walking up from the current working directory
3. `simulation.json` found by walking up from the app base directory

If no configuration is found, built-in defaults are used.

### Date Range Settings

Use the `simulation` section to control start/end behavior:

- `startTime`: simulation start date/time (ISO format)
- `endTime`: simulation end date/time (ISO format) or `null`

Default behavior is to run indefinitely by setting:

```json
"endTime": null
```

To run within a fixed date range, set an explicit end time, for example:

```json
"simulation": {
    "startTime": "2024-01-01T00:00:00",
    "endTime": "2024-12-31T23:45:00",
    "stepMinutes": 15
}
```

When `endTime` is set, both console and UI simulation loops stop automatically after the configured end date is reached.

### Other Configurable Areas

You can also configure:

- Neighbourhood size and asset distribution probabilities
- Battery capacity, SoC, power limits, efficiency, and target load
- Asset-specific behavior (base load profile, heat pump, EV charging, PV peak)
- Weather season base temperatures
- Runtime loop intervals for BLL and UI
