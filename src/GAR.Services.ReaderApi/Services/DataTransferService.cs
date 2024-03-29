﻿namespace GAR.Services.ReaderApi.Services;

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using GAR.Services.ReaderApi.Data;

public class DataTransferService(
    ILogger<DataTransferService> logger,
    ZipXmlReaderService zipXmlReaderService,
    DataWriterService dataWriterService,
    DataMapHelper dataMapHelper)
    : IDisposable
{
    private readonly ILogger<DataTransferService> _logger = logger;
    private readonly ZipXmlReaderService _zipXmlReaderService = zipXmlReaderService;
    private readonly DataWriterService _dataWriterService = dataWriterService;
    private readonly DataMapHelper _dataMapHelper = dataMapHelper;

    private bool _disposed;

    public async Task ImportAsync(CancellationToken cancellationToken = default)
    {
        await _zipXmlReaderService.InitTypesAsync();

        var sw = Stopwatch.StartNew();

        await ImportObjectsAsync(cancellationToken);
        await ImportApartmentsAsync(cancellationToken);
        await ImportHierarchiesAsync(cancellationToken);
        await ImportHousesAsync(cancellationToken);
        await ImportRoomsAsync(cancellationToken);
        await ImportSteadsAsync(cancellationToken);

        _logger.LogInformation("Data transfer has ended in {Milliseconds}ms", sw.ElapsedMilliseconds);
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
            _zipXmlReaderService.Dispose();
        }

        _disposed = true;
    }

    private async Task ImportObjectsAsync(CancellationToken cancellationToken = default)
    {
        var dataMapper = _dataMapHelper.Addresses;
        var sw = Stopwatch.StartNew();

        while (_zipXmlReaderService.CanReadObjects)
        {
            var bucket = _zipXmlReaderService.ReadObjectsAsync();
            var saved = await _dataWriterService.ImportAsync(dataMapper, bucket, cancellationToken);

            _logger.LogInformation("Objects saved from bucket: {Saved}", saved);
        }

        _logger.LogInformation("Objects copying has ended in {Milliseconds}", sw.ElapsedMilliseconds);
    }

    private async Task ImportApartmentsAsync(CancellationToken cancellationToken = default)
    {
        var dataMapper = _dataMapHelper.Apartments;
        var sw = Stopwatch.StartNew();

        while (_zipXmlReaderService.CanReadApartments)
        {
            var bucket = _zipXmlReaderService.ReadApartmentsAsync();
            var saved = await _dataWriterService.ImportAsync(dataMapper, bucket, cancellationToken);

            _logger.LogInformation("Apartments saved from bucket: {Saved}", saved);
        }

        _logger.LogInformation("Apartments copying has ended in {Milliseconds}", sw.ElapsedMilliseconds);
    }

    private async Task ImportHierarchiesAsync(CancellationToken cancellationToken = default)
    {
        var dataMapper = _dataMapHelper.Hierarchies;
        var sw = Stopwatch.StartNew();

        while (_zipXmlReaderService.CanReadHierarchies)
        {
            var bucket = _zipXmlReaderService.ReadHierarchiesAsync();
            var saved = await _dataWriterService.ImportAsync(dataMapper, bucket, cancellationToken);

            _logger.LogInformation("Hierarchies saved from bucket: {Saved}", saved);
        }

        _logger.LogInformation("Hierarchies copying has ended in {Milliseconds}", sw.ElapsedMilliseconds);
    }

    private async Task ImportHousesAsync(CancellationToken cancellationToken = default)
    {
        var dataMapper = _dataMapHelper.Houses;
        var sw = Stopwatch.StartNew();

        while (_zipXmlReaderService.CanReadHouses)
        {
            var bucket = _zipXmlReaderService.ReadHousesAsync();
            var saved = await _dataWriterService.ImportAsync(dataMapper, bucket, cancellationToken);

            _logger.LogInformation("Houses saved from bucket: {Saved}", saved);
        }

        _logger.LogInformation("Houses copying has ended in {Milliseconds}", sw.ElapsedMilliseconds);
    }

    private async Task ImportRoomsAsync(CancellationToken cancellationToken = default)
    {
        var dataMapper = _dataMapHelper.Rooms;
        var sw = Stopwatch.StartNew();

        while (_zipXmlReaderService.CanReadRooms)
        {
            var bucket = _zipXmlReaderService.ReadRoomsAsync();
            var saved = await _dataWriterService.ImportAsync(dataMapper, bucket, cancellationToken);

            _logger.LogInformation("Rooms saved from bucket: {Saved}", saved);
        }

        _logger.LogInformation("Rooms copying has ended in {Milliseconds}", sw.ElapsedMilliseconds);
    }

    private async Task ImportSteadsAsync(CancellationToken cancellationToken = default)
    {
        var dataMapper = _dataMapHelper.Steads;
        var sw = Stopwatch.StartNew();

        while (_zipXmlReaderService.CanReadSteads)
        {
            var bucket = _zipXmlReaderService.ReadSteadsAsync();
            var saved = await _dataWriterService.ImportAsync(dataMapper, bucket, cancellationToken);

            _logger.LogInformation("Steads saved from bucket: {Saved}", saved);
        }

        _logger.LogInformation("Steads copying has ended in {Milliseconds}", sw.ElapsedMilliseconds);
    }
}
