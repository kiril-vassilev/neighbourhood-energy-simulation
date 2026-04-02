using Microsoft.Data.Sqlite;

namespace Simulation.DAL;

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

	public static void Insert(DateTime currentTime, double load)
	{
		EnsureInitialized();

		using var connection = new SqliteConnection(_connectionString);
		connection.Open();

		using var command = connection.CreateCommand();
		command.CommandText = "INSERT INTO History (CurrentTime, Load) VALUES ($time, $load);";
		command.Parameters.AddWithValue("$time", currentTime.ToString("O"));
		command.Parameters.AddWithValue("$load", load);
		command.ExecuteNonQuery();
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
	Load REAL NOT NULL
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
