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