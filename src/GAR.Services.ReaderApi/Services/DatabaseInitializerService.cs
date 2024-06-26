﻿namespace GAR.Services.ReaderApi.Services;

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using GAR.Services.ReaderApi.Entities;
using Npgsql;

public class DatabaseInitializerService(
    NpgsqlDataSource dataSource,
    ILogger<DatabaseInitializerService> logger)
    : IDisposable
{
    private readonly NpgsqlDataSource _dataSource = dataSource;
    private readonly ILogger<DatabaseInitializerService> _logger = logger;

    private bool _disposed;

    public async Task EnsureCreatedAsync(CancellationToken cancellationToken = default)
    {
        await using var cmd = _dataSource.CreateCommand("SELECT 1 FROM pg_catalog.pg_database WHERE datname = 'gar-database'");
        var exists = await cmd.ExecuteScalarAsync(cancellationToken);

        if (exists is not null)
        {
            return;
        }

        cmd.CommandText = $"CREATE DATABASE 'gar-database'";
        await cmd.ExecuteNonQueryAsync(cancellationToken);

        _logger.LogInformation("Database was created.");
    }

    public async Task CreateSchemaAsync(CancellationToken cancellationToken = default)
    {
        await using var cmd = _dataSource.CreateCommand(@"
            CREATE SCHEMA IF NOT EXISTS public;
        ");

        await cmd.ExecuteNonQueryAsync(cancellationToken);
        _logger.LogInformation("Schema was created.");
    }

    public async Task DropSchemaAsync(CancellationToken cancellationToken = default)
    {
        await using var cmd = _dataSource.CreateCommand(@"DROP SCHEMA IF EXISTS public CASCADE;");

        await cmd.ExecuteNonQueryAsync(cancellationToken);
        _logger.LogInformation("Schema was dropped.");
    }

    public async Task CreateTablesAsync(CancellationToken cancellationToken = default)
    {
        var cw = Stopwatch.StartNew();

        await CreateAddressesTableAsync(cancellationToken);
        await CreateApartmentsTableAsync(cancellationToken);
        await CreateHierarchiesTableAsync(cancellationToken);
        await CreateHousesTableAsync(cancellationToken);
        await CreateRoomsTableAsync(cancellationToken);
        await CreateSteadsTableAsync(cancellationToken);

        _logger.LogInformation("Database initialization has ended in {Milliseconds}ms", cw.ElapsedMilliseconds);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _dataSource.Dispose();
        }

        _disposed = true;
    }

    private async Task CreateAddressesTableAsync(CancellationToken cancellationToken = default)
    {
        await using var cmd = _dataSource.CreateCommand($"""
            CREATE TABLE IF NOT EXISTS public."{nameof(AddressObject)}" (
                "{nameof(AddressObject.Id)}" SERIAL PRIMARY KEY,
                "{nameof(AddressObject.ObjectId)}" integer NOT NULL,
                "{nameof(AddressObject.FullName)}" text NOT NULL
            );
        """);

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    private async Task CreateApartmentsTableAsync(CancellationToken cancellationToken = default)
    {
        await using var cmd = _dataSource.CreateCommand($"""
            CREATE TABLE IF NOT EXISTS public."{nameof(Apartment)}" (
                "{nameof(Apartment.Id)}" SERIAL PRIMARY KEY,
                "{nameof(Apartment.ObjectId)}" integer NOT NULL,
                "{nameof(Apartment.FullName)}" text NOT NULL
            );
        """);

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    private async Task CreateHierarchiesTableAsync(CancellationToken cancellationToken = default)
    {
        await using var cmd = _dataSource.CreateCommand($"""
            CREATE TABLE IF NOT EXISTS public."{nameof(Hierarchy)}" (
                "{nameof(Hierarchy.Id)}" SERIAL PRIMARY KEY,
                "{nameof(Hierarchy.ObjectId)}" integer NOT NULL,
                "{nameof(Hierarchy.Path)}" text NOT NULL
           );
        """);

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    private async Task CreateHousesTableAsync(CancellationToken cancellationToken = default)
    {
        await using var cmd = _dataSource.CreateCommand($"""
            CREATE TABLE IF NOT EXISTS public."{nameof(House)}" (
                "{nameof(House.Id)}" SERIAL PRIMARY KEY,
                "{nameof(House.ObjectId)}" integer NOT NULL,
                "{nameof(House.FullName)}" text NOT NULL
            );
        """);

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    private async Task CreateRoomsTableAsync(CancellationToken cancellationToken = default)
    {
        await using var cmd = _dataSource.CreateCommand($"""
            CREATE TABLE IF NOT EXISTS public."{nameof(Room)}" (
                "{nameof(Room.Id)}" SERIAL PRIMARY KEY,
                "{nameof(Room.ObjectId)}" integer NOT NULL,
                "{nameof(Room.FullName)}" text NOT NULL
            );
        """);

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    private async Task CreateSteadsTableAsync(CancellationToken cancellationToken = default)
    {
        await using var cmd = _dataSource.CreateCommand($"""
            CREATE TABLE IF NOT EXISTS public."{nameof(Stead)}" (
                "{nameof(Stead.Id)}" SERIAL PRIMARY KEY,
                "{nameof(Stead.ObjectId)}" integer NOT NULL,
                "{nameof(Stead.FullName)}" text NOT NULL
            );
        """);

        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }
}
