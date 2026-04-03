using Microsoft.Data.Sqlite;
using System.Globalization;

namespace Simulation.DAL;

public sealed record HistoryRow(
	long Id,
	DateTime CurrentTime,
	double CurrentLoadKw,
	string Season,
	double Temperature,
	double CurrentLoadWithBatteryKw,
	double BatteryCurrentPowerKw,
	double BatteryStateOfChargeKwh,
	double TotalEnergyKwh,
	double PeakWithoutBatteryKwh,
	double PeakWithBatteryKwh);

public static class HistoryRepository
{
	private static readonly object Sync = new();
	private static string? _connectionString;
	private static bool _initialized;
	public static void ClearHistory()
	{
		EnsureInitialized();

		lock (Sync)
		{
			using var connection = new SqliteConnection(_connectionString);
			connection.Open();

			using var command = connection.CreateCommand();
			command.CommandText = "DELETE FROM History;";
			command.ExecuteNonQuery();
		}
	}

	public static void Insert(
		DateTime currentTime,
		double currentLoadKw,
		string season,
		double temperature,
		double currentLoadWithBatteryKw,
		double batteryCurrentPowerKw,
		double batteryStateOfChargeKwh,
		double totalEnergyKwh,
		double peakWithoutBatteryKwh,
		double peakWithBatteryKwh)
	{
		Insert(new HistoryRow(
			0,
			currentTime,
			currentLoadKw,
			season,
			temperature,
			currentLoadWithBatteryKw,
			batteryCurrentPowerKw,
			batteryStateOfChargeKwh,
			totalEnergyKwh,
			peakWithoutBatteryKwh,
			peakWithBatteryKwh));
	}

	public static void Insert(HistoryRow historyRow)
	{
		EnsureInitialized();

		using var connection = new SqliteConnection(_connectionString);
		connection.Open();

		using var command = connection.CreateCommand();

		command.CommandText = @"
			INSERT INTO History (
				CurrentTime,
				CurrentLoadKw,
				Season,
				Temperature,
				CurrentLoadWithBatteryKw,
				BatteryCurrentPowerKw,
				BatteryStateOfChargeKwh,
				TotalEnergyKwh,
				PeakWithoutBatteryKwh,
				PeakWithBatteryKwh
			) VALUES (
				$time,
				$currentLoadKw,
				$season,
				$temperature,
				$currentLoadWithBatteryKw,
				$batteryCurrentPowerKw,
				$batteryStateOfChargeKwh,
				$totalEnergyKwh,
				$peakWithoutBatteryKwh,
				$peakWithBatteryKwh
			);";

		command.Parameters.AddWithValue("$time", historyRow.CurrentTime.ToString("O"));
		command.Parameters.AddWithValue("$currentLoadKw", historyRow.CurrentLoadKw);
		command.Parameters.AddWithValue("$season", historyRow.Season);
		command.Parameters.AddWithValue("$temperature", historyRow.Temperature);
		command.Parameters.AddWithValue("$currentLoadWithBatteryKw", historyRow.CurrentLoadWithBatteryKw);
		command.Parameters.AddWithValue("$batteryCurrentPowerKw", historyRow.BatteryCurrentPowerKw);
		command.Parameters.AddWithValue("$batteryStateOfChargeKwh", historyRow.BatteryStateOfChargeKwh);
		command.Parameters.AddWithValue("$totalEnergyKwh", historyRow.TotalEnergyKwh);
		command.Parameters.AddWithValue("$peakWithoutBatteryKwh", historyRow.PeakWithoutBatteryKwh);
		command.Parameters.AddWithValue("$peakWithBatteryKwh", historyRow.PeakWithBatteryKwh);
		command.ExecuteNonQuery();
	}

	public static IReadOnlyList<HistoryRow> GetLatestHistory(int count = 96)
	{
		EnsureInitialized();

		if (count <= 0)
			return Array.Empty<HistoryRow>();

		using var connection = new SqliteConnection(_connectionString);
		connection.Open();

		using var command = connection.CreateCommand();
		command.CommandText = @"
			SELECT
				Id,
				CurrentTime,
				CurrentLoadKw,
				Season,
				Temperature,
				CurrentLoadWithBatteryKw,
				BatteryCurrentPowerKw,
				BatteryStateOfChargeKwh,
				TotalEnergyKwh,
				PeakWithoutBatteryKwh,
				PeakWithBatteryKwh
			FROM History
			ORDER BY Id DESC
			LIMIT $count;";
		command.Parameters.AddWithValue("$count", count);

		var rows = new List<HistoryRow>(count);

		using var reader = command.ExecuteReader();
		while (reader.Read())
		{
			var currentTime = DateTime.Parse(reader.GetString(1), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

			rows.Add(new HistoryRow(
				reader.GetInt64(0),
				currentTime,
				reader.GetDouble(2),
				reader.GetString(3),
				reader.GetDouble(4),
				reader.GetDouble(5),
				reader.GetDouble(6),
				reader.GetDouble(7),
				reader.GetDouble(8),
				reader.GetDouble(9),
				reader.GetDouble(10)));
		}

		rows.Reverse();
		return rows;
	}

	private static void EnsureInitialized()
	{
		if (_initialized)
			return;

		lock (Sync)
		{
			if (_initialized)
				return;

			var dbPath = ResolveDbPath();
			_connectionString = $"Data Source={dbPath}";

			using var connection = new SqliteConnection(_connectionString);
			connection.Open();

			using var command = connection.CreateCommand();
			command.CommandText = @"
				CREATE TABLE IF NOT EXISTS History (
					Id INTEGER PRIMARY KEY AUTOINCREMENT,
					CurrentTime TEXT NOT NULL,
					CurrentLoadKw REAL NOT NULL,
					Season TEXT NOT NULL DEFAULT '',
					Temperature REAL NOT NULL DEFAULT 0,
					CurrentLoadWithBatteryKw REAL NOT NULL DEFAULT 0,
					BatteryCurrentPowerKw REAL NOT NULL DEFAULT 0,
					BatteryStateOfChargeKwh REAL NOT NULL DEFAULT 0,
					TotalEnergyKwh REAL NOT NULL DEFAULT 0,
					PeakWithoutBatteryKwh REAL NOT NULL DEFAULT 0,
					PeakWithBatteryKwh REAL NOT NULL DEFAULT 0
				);";
			command.ExecuteNonQuery();

			_initialized = true;
		}
	}

	private static string ResolveDbPath()
	{
		var configuredPath = Environment.GetEnvironmentVariable("SIMULATION_DB_PATH");
		if (!string.IsNullOrWhiteSpace(configuredPath))
			return configuredPath;

		var slnDirectory = FindAncestorDirectoryContaining("Simulation.sln", Directory.GetCurrentDirectory())
			?? FindAncestorDirectoryContaining("Simulation.sln", AppContext.BaseDirectory);

		var baseDirectory = slnDirectory ?? Directory.GetCurrentDirectory();
		return Path.Combine(baseDirectory, "simulation.db");
	}

	private static string? FindAncestorDirectoryContaining(string fileName, string startDirectory)
	{
		try
		{
			var current = new DirectoryInfo(startDirectory);

			while (current != null)
			{
				var candidate = Path.Combine(current.FullName, fileName);
				if (File.Exists(candidate))
					return current.FullName;

				current = current.Parent;
			}
		}
		catch
		{
			return null;
		}

		return null;
	}
}
