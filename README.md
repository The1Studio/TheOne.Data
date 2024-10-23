# TheOne.Data

Data Manager for Unity

## Installation

### Option 1: Unity Scoped Registry (Recommended)

Add the following scoped registry to your project's `Packages/manifest.json`:

```json
{
  "scopedRegistries": [
    {
      "name": "TheOne Studio",
      "url": "https://upm.the1studio.org/",
      "scopes": [
        "com.theone"
      ]
    }
  ],
  "dependencies": {
    "com.theone.data": "1.1.0"
  }
}
```

### Option 2: Git URL

Add to Unity Package Manager:
```
https://github.com/The1Studio/TheOne.Data.git
```

## Features

- Type-safe data management with automatic serialization
- Multiple storage backends (Assets, PlayerPrefs, External files)
- Flexible serialization formats (JSON, CSV, Binary)
- Advanced type conversion system
- Async loading with progress tracking
- Integration with popular DI frameworks

## Dependencies

- TheOne.Extensions
- TheOne.Logging
- TheOne.ResourceManagement

## Usage

### Basic Data Management

```csharp
using TheOne.Data;

// Load data by type
var settings = dataManager.Load<GameSettings>();
var playerData = dataManager.Load<PlayerData>();

// Update data in memory
settings.MusicVolume = 0.8f;
dataManager.Update(settings);

// Save data to storage (in memory)
dataManager.Save<GameSettings>();

// Flush to persistent storage
dataManager.Flush<GameSettings>();

// Save and flush in one call
dataManager.SaveAndFlush<GameSettings>();

// Batch operations
dataManager.SaveAll();
dataManager.FlushAll();
```

### Custom Data Classes

```csharp
// Simple data class with JSON serialization
[Serializable]
public class PlayerData : IJsonData
{
    public string Name { get; set; }
    public int Level { get; set; }
    public float Experience { get; set; }
    public List<string> UnlockedItems { get; set; }
}

// CSV data with attributes
[CsvRow]
public class ItemData : ICsvData
{
    [CsvColumn("id")]
    public int Id { get; set; }
    
    [CsvColumn("name")]
    public string Name { get; set; }
    
    [CsvColumn("cost")]
    public int Cost { get; set; }
    
    [CsvIgnore]
    public Sprite Icon { get; set; } // Not serialized
    
    [CsvOptional]
    public string Description { get; set; } // Optional column
}
```

### Async Operations

```csharp
#if THEONE_UNITASK
// Load data asynchronously with progress
var progress = new Progress<float>(p => Debug.Log($"Loading: {p:P}"));
var settings = await dataManager.LoadAsync<GameSettings>(progress: progress);

// Save and flush asynchronously
await dataManager.SaveAndFlushAsync<PlayerData>();

// Batch async operations
await dataManager.SaveAllAsync();
await dataManager.FlushAllAsync();
#else
// Coroutine version
StartCoroutine(dataManager.LoadAsync<GameSettings>(
    callback: settings => 
    {
        Debug.Log($"Settings loaded: {settings.Language}");
    }
));
#endif
```

### Storage Configuration

```csharp
// Storage is automatically configured by DataManager based on data type
// Use DI extensions to register DataManager:

// VContainer
builder.RegisterDataManager();

// Zenject
container.BindDataManager();

// Custom DI
container.AddDataManager();

// For external storage (file system), also register:
container.AddExternalDataStorages(); // Custom DI
builder.RegisterExternalDataStorages(); // VContainer
container.BindExternalDataStorages(); // Zenject
```

### Type Conversion

```csharp
using TheOne.Data.Conversion;

// Convert string to various types
var intValue = converterManager.ConvertFromString<int>("42");
var vector = converterManager.ConvertFromString<Vector3>("1.5,2.0,3.5");
var color = converterManager.ConvertFromString<Color>("#FF5733");
var guid = converterManager.ConvertFromString<Guid>("550e8400-e29b-41d4-a716-446655440000");

// Convert objects to string
var vectorStr = converterManager.ConvertToString(new Vector3(1, 2, 3));
var dateStr = converterManager.ConvertToString(DateTime.Now);

// Collections
var list = converterManager.ConvertFromString<List<int>>("1,2,3,4,5");
var dict = converterManager.ConvertFromString<Dictionary<string, float>>("key1:1.5,key2:2.5");
```

### Custom Converters

```csharp
public class CustomTypeConverter : Converter<CustomType>
{
    protected override CustomType ConvertFromString(string str)
    {
        var parts = str.Split('|');
        return new CustomType 
        { 
            Id = int.Parse(parts[0]),
            Name = parts[1]
        };
    }
    
    protected override string ConvertToString(CustomType obj)
    {
        return $"{obj.Id}|{obj.Name}";
    }
}
```

### CSV Data Management

```csharp
#if THEONE_CSV
// Define CSV data container
public class ItemDatabase : CsvData<int, ItemData>
{
    // Automatically handles Dictionary<int, ItemData>
}

// Load CSV data
var itemDb = dataManager.Load<ItemDatabase>();
var sword = itemDb[1001]; // Access by key
foreach (var item in itemDb.Values)
{
    Debug.Log($"{item.Name}: {item.Cost} gold");
}

// Real-world example: Nested CSV data structures
[Preserve]
internal sealed class LevelBlueprint : CsvData<string, LevelRecord>
{
}

internal sealed record LevelRecord(string Id, CsvData<string, VariantRecord> Variants);
internal sealed record VariantRecord(string Variant, int Difficulty, float TimeLimit);

// Usage
var blueprint = dataManager.Load<LevelBlueprint>();
var levelInfo = blueprint["Level_01"];
var easyVariant = levelInfo.Variants["Easy"];
Debug.Log($"Difficulty: {easyVariant.Difficulty}, Time: {easyVariant.TimeLimit}");
#endif
```

## Architecture

### Folder Structure

```
TheOne.Data/
├── Scripts/
│   ├── IDataManager.cs                # Main data manager interface
│   ├── DataManager.cs                 # Data manager implementation
│   ├── Conversion/                    # Type conversion system
│   │   ├── Abstractions/
│   │   │   ├── IConverter.cs
│   │   │   └── IConverterManager.cs
│   │   └── Implementations/
│   │       ├── ConverterManager.cs
│   │       └── Converters/
│   │           ├── Numbers/           # Numeric type converters
│   │           ├── Collections/       # Collection converters
│   │           ├── Time/             # DateTime converters
│   │           ├── Tuples/           # Unity type converters
│   │           └── Others/           # Misc converters
│   ├── Serialization/                # Serialization system
│   │   ├── Abstractions/
│   │   │   └── ISerializer.cs
│   │   └── Implementations/
│   │       ├── Csv/                  # CSV serialization
│   │       ├── Json/                 # JSON serialization
│   │       └── Object/               # Binary serialization
│   ├── Storages/                     # Storage backends
│   │   ├── Abstractions/
│   │   │   ├── IDataStorage.cs
│   │   │   └── IWritableDataStorage.cs
│   │   └── Implementations/
│   │       ├── Asset/                # Unity asset storage
│   │       ├── External/             # File system storage
│   │       └── PlayerPrefs/          # PlayerPrefs storage
│   └── DI/                          # Dependency injection
│       ├── DataManagerDI.cs
│       ├── DataManagerVContainer.cs
│       └── DataManagerZenject.cs
```

### Core Classes

#### Interfaces

##### `IDataManager`
Central interface for data operations:
- `Load<T>()` - Load data from storage
- `Update<T>(data)` - Update data in memory
- `Save<T>()` - Save data to cache
- `Flush<T>()` - Write cache to persistent storage
- `SaveAndFlush<T>()` - Save and flush in one operation
- Async variants with UniTask/IEnumerator support

##### `IConverter`
Type conversion interface:
- `CanConvert(Type)` - Check if type is supported
- `ConvertFromString(string, Type)` - Parse string to object
- `ConvertToString(object, Type)` - Convert object to string
- `GetDefaultValue(Type)` - Get default value for type

##### `ISerializer`
Data serialization interface:
- `CanSerialize(Type)` - Check if type can be serialized
- `Deserialize(Type, rawData)` - Deserialize raw data
- `Serialize(data)` - Serialize to raw format
- Async variants for large data

##### `IDataStorage`
Storage backend interface:
- `Read(key)` - Read raw data
- `ReadAsync(key)` - Async read with progress

##### `IWritableDataStorage`
Writable storage interface:
- Extends `IDataStorage`
- `Write(key, value)` - Write data
- `Flush()` - Persist to storage
- Async variants available

### Storage Types

#### Asset Storage
- `AssetTextDataStorage` - Text assets from Resources
- `AssetBinaryDataStorage` - Binary assets
- `AssetBlobDataStorage` - Large binary blobs
- Read-only, loaded from Unity assets

#### PlayerPrefs Storage
- `PlayerPrefsDataStorage` - Unity PlayerPrefs backend
- Suitable for small user preferences
- Platform-specific persistence

#### External Storage
- `ExternalTextDataStorage` - Text files
- `ExternalBinaryDataStorage` - Binary files
- `ExternalFileVersionManager` - Version control
- Full read/write support

### Serialization Formats

#### JSON Serialization
- Supports any class implementing `IJsonData`
- Uses Unity's JsonUtility or Newtonsoft.Json
- Human-readable format

#### CSV Serialization
- Requires `THEONE_CSV` define symbol
- Supports `ICsvData` with attributes
- Ideal for tabular game data
- Column mapping via attributes

#### Binary Serialization
- Direct object serialization
- Smallest file size
- Fastest performance

### Type Converters

#### Built-in Converters
- **Numbers**: byte, int, float, double, decimal, etc.
- **Collections**: List<T>, Dictionary<K,V>, arrays
- **Unity Types**: Vector2/3/4, Color, Color32
- **Time**: DateTime, DateTimeOffset, TimeSpan
- **Others**: bool, string, Guid, Enum, Uri

#### Converter Features
- Configurable separators for collections
- Null-safe conversions
- Culture-invariant formatting
- Extensible converter system

### Design Patterns

- **Repository Pattern**: Centralized data access
- **Strategy Pattern**: Pluggable storage/serialization
- **Factory Pattern**: Dynamic converter selection
- **Async/Await Pattern**: Non-blocking I/O operations
- **Dependency Injection**: Flexible configuration

### Code Style & Conventions

- **Namespace**: All code under `TheOne.Data` namespace
- **Null Safety**: Uses `#nullable enable` directive
- **Conditional Compilation**: `#if THEONE_UNITASK`, `#if THEONE_CSV`, `#if THEONE_JSON`
- **Interfaces**: Prefixed with `I` (e.g., `IDataManager`)
- **Async Methods**: Suffixed with `Async`
- **Progress Reporting**: Optional `IProgress<float>` parameter

## Performance Considerations

- Data is cached in memory after loading
- Flush operations are batched when possible
- Async operations for large datasets
- Lazy loading with progress tracking
- Efficient binary serialization available

## Best Practices

1. **Data Modeling**: Keep data classes simple and serializable
2. **Storage Selection**: Choose appropriate backend per data type
3. **Batch Operations**: Use SaveAll/FlushAll for multiple changes
4. **Async Loading**: Use async methods for large datasets
5. **Version Management**: Implement migration for save data changes
6. **Error Handling**: Always handle load failures gracefully
7. **Testing**: Mock IDataManager for unit tests

## Advanced Usage

### Multi-Storage Selection

The DataManager automatically selects the appropriate storage backend based on your data type:

```csharp
// Data implementing IJsonData or ICsvData from Resources
// Automatically uses AssetTextDataStorage for read-only data
public class GameConfig : IJsonData { }

// Data stored in PlayerPrefs
// Uses PlayerPrefsDataStorage for small preferences
public class UserSettings { }

// Data stored in external files
// Uses ExternalBinaryDataStorage or ExternalTextDataStorage
public class PlayerSaveData { }

// The DataManager determines storage based on:
// 1. Serializer type (JSON, CSV, Binary)
// 2. Data interface implementation
// 3. Storage availability in DI container
```

### Integration with Services

```csharp
public class DifficultyConfigService : IAsyncEarlyLoadable
{
    private readonly IDataManager dataManager;
    private readonly IAssetsManager assetsManager;
    
    private LevelBlueprint levelBlueprint;
    
    // Load data during service initialization
    async UniTask IAsyncEarlyLoadable.LoadAsync(IProgress<float>? progress, CancellationToken cancellationToken)
    {
        this.levelBlueprint = await this.dataManager.LoadAsync<LevelBlueprint>(
            progress: progress, 
            cancellationToken: cancellationToken
        );
    }
    
    // Use loaded data for game logic
    public int GetDifficulty(string levelId)
    {
        var levelRecord = this.levelBlueprint[levelId];
        var variant = levelRecord.Variants["Default"];
        return variant.Difficulty;
    }
    
    // Combine with asset loading
    public async UniTask<LevelModel> LoadLevelAsync(string levelId)
    {
        var levelKey = $"Levels/{levelId}";
        return await this.assetsManager.LoadAsync<LevelModel>(levelKey);
    }
}
```

### Custom Storage Backend

```csharp
public class CloudStorage : IWritableDataStorage
{
    public Type RawDataType => typeof(byte[]);
    
    public bool CanStore(Type type) => true;
    
    public object? Read(string key)
    {
        // Download from cloud
        return CloudService.Download(key);
    }
    
    public void Write(string key, object value)
    {
        // Upload to cloud
        CloudService.Upload(key, (byte[])value);
    }
    
    public void Flush()
    {
        // Sync with cloud
        CloudService.Sync();
    }
}
```

### Encrypted Storage

```csharp
public class EncryptedStorage : WritableDataStorage
{
    private readonly IEncryption encryption;
    
    public override void Write(string key, object value)
    {
        var encrypted = encryption.Encrypt((byte[])value);
        base.Write(key, encrypted);
    }
    
    public override object? Read(string key)
    {
        var encrypted = base.Read(key);
        return encrypted != null 
            ? encryption.Decrypt((byte[])encrypted)
            : null;
    }
}
```