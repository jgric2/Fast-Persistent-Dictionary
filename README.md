


# Fast Persistent Dictionary



Fast Persistent Dictionary, is an implementation of a persistent dictionary in C#. This dictionary-like data structure is designed to persist data to disk, serving as a reliable workhorse for cases where the application requires storing large amounts of data, but also needs to operate with high efficiency and minimize disk space usage.

The dictionary comes with a plethora of features and methods, giving the developer the flexibility to manipulate the data as per their requirements. Some of the key features include options to add or update a record, clear all records, load or save the dictionary, or perform common mathematical operations across the dataset. The dictionary also supports compression to help manage disk usage and provides a GROBUF Serializer for data serialization, facilitating efficient storage of complex objects. 

This dictionary makes use of the .NET `IEnumerable` and `ISerializable` interfaces, allowing it to be used in various scenarios where these interfaces are required, for example, in .NET collections or serialization operations.

The project also provides exception handling and recovery features. A dispose mechanism is incorporated ensuring that all resources are released when the instance is no longer required, enhancing the project’s stability and resilience.

Fast Persistent Dictionary is an open-source project crafted with care, providing a robust and efficient persistent dictionary in C#. It's an invaluable tool when building applications requiring efficient, reliable, and persistent data storage.


## Fast Persistent Dictionary Features
* **Single File Database**: the In use database and the saved and loadable format is all compiled in a single file.
* **Performance**: Fast Persistent Dictionary supports a high rate of updates and retrieves. Typically surpassing ESENT by a wide margin.
* **Simplicity**: a FastPersistentDictionary looks and behaves like the .NET Dictionary class. No extra method calls are required.
* **Concurrency**: each data structure can be accessed by multiple threads.
* **Scale**: Values can be up to 2gb in size.
* **No Serialization Flags**: Any key or value can be used as long as it is serializable by Grobuf.




# Benchmarks
```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4037/23H2/2023Update/SunValley3)
Intel Core i7-10875H CPU 2.30GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.200
  [Host]     : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2 [AttachedDebugger]
  Job-URLKUQ : .NET 8.0.2 (8.0.224.6711), X64 RyuJIT AVX2

InvocationCount=1  UnrollFactor=1  

```
| Method                          | N       | Mean              | Error          | StdDev         | Median            |
|-------------------------------- |-------- |------------------:|---------------:|---------------:|------------------:|
| **FastPersistentDictionary_Add**    | **10**      |         **35.712 μs** |      **1.0418 μs** |      **2.8869 μs** |         **34.700 μs** |
| StandardDictionary_Add          | 10      |          4.615 μs |      0.0963 μs |      0.1350 μs |          4.600 μs |
| EsentPersistentDictionary_Add         | 10      |        169.886 μs |      5.2269 μs |     14.5704 μs |        164.950 μs |
| FastPersistentDictionary_Get    | 10      |         29.895 μs |      0.9572 μs |      2.5550 μs |         30.300 μs |
| StandardDictionary_Get          | 10      |          3.429 μs |      0.0810 μs |      0.2312 μs |          3.350 μs |
| EsentPersistentDictionary_Get         | 10      |         49.512 μs |      1.2037 μs |      3.2544 μs |         49.300 μs |
| FastPersistentDictionary_Remove | 10      |          4.667 μs |      0.0940 μs |      0.1742 μs |          4.700 μs |
| StandardDictionary_Remove       | 10      |          3.397 μs |      0.1454 μs |      0.3981 μs |          3.300 μs |
| EsentPersistentDictionary_Remove      | 10      |         85.437 μs |      1.6982 μs |      3.5821 μs |         85.650 μs |
| **FastPersistentDictionary_Add**    | **100**     |        **246.110 μs** |      **4.8914 μs** |      **7.1697 μs** |        **245.000 μs** |
| StandardDictionary_Add          | 100     |         20.167 μs |      0.3757 μs |      0.2934 μs |         20.200 μs |
| EsentPersistentDictionary_Add         | 100     |      1,387.178 μs |     24.7951 μs |     41.4270 μs |      1,383.100 μs |
| FastPersistentDictionary_Get    | 100     |        103.832 μs |      6.6457 μs |     19.1743 μs |        107.050 μs |
| StandardDictionary_Get          | 100     |         10.550 μs |      0.2117 μs |      0.3037 μs |         10.500 μs |
| EsentPersistentDictionary_Get         | 100     |        376.640 μs |      7.4826 μs |     20.4835 μs |        375.100 μs |
| FastPersistentDictionary_Remove | 100     |         17.328 μs |      0.4785 μs |      1.2689 μs |         17.200 μs |
| StandardDictionary_Remove       | 100     |         10.529 μs |      0.2106 μs |      0.3217 μs |         10.600 μs |
| EsentPersistentDictionary_Remove      | 100     |        840.953 μs |     16.4776 μs |     30.5424 μs |        839.100 μs |
| **FastPersistentDictionary_Add**    | **1000**    |      **2,308.243 μs** |     **44.5800 μs** |     **53.0693 μs** |      **2,306.800 μs** |
| StandardDictionary_Add          | 1000    |        177.095 μs |      3.4542 μs |      3.9779 μs |        175.400 μs |
| EsentPersistentDictionary_Add         | 1000    |     12,976.556 μs |    491.4843 μs |  1,441.4385 μs |     12,218.100 μs |
| FastPersistentDictionary_Get    | 1000    |        790.672 μs |     32.5535 μs |     94.4435 μs |        814.500 μs |
| StandardDictionary_Get          | 1000    |         87.707 μs |      1.2218 μs |      1.0831 μs |         87.500 μs |
| EsentPersistentDictionary_Get         | 1000    |      3,355.380 μs |     92.6984 μs |    264.4738 μs |      3,264.600 μs |
| FastPersistentDictionary_Remove | 1000    |        200.779 μs |      5.6056 μs |     16.0834 μs |        196.950 μs |
| StandardDictionary_Remove       | 1000    |        106.619 μs |      5.9200 μs |     15.4915 μs |        106.600 μs |
| EsentPersistentDictionary_Remove      | 1000    |      7,758.348 μs |    203.6357 μs |    580.9842 μs |      7,583.750 μs |
| **FastPersistentDictionary_Add**    | **10000**   |     **22,342.905 μs** |    **910.8584 μs** |  **2,671.3902 μs** |     **21,158.300 μs** |
| StandardDictionary_Add          | 10000   |      1,814.397 μs |    120.8519 μs |    356.3347 μs |      1,636.200 μs |
| EsentPersistentDictionary_Add         | 10000   |    122,277.346 μs |  2,166.8262 μs |  4,570.5723 μs |    122,271.200 μs |
| FastPersistentDictionary_Get    | 10000   |      4,522.615 μs |    242.8226 μs |    668.8040 μs |      4,239.550 μs |
| StandardDictionary_Get          | 10000   |        710.010 μs |     50.2010 μs |    140.7690 μs |        662.100 μs |
| EsentPersistentDictionary_Get         | 10000   |     32,145.620 μs |    631.8258 μs |    591.0102 μs |     31,998.800 μs |
| FastPersistentDictionary_Remove | 10000   |        876.048 μs |     73.5746 μs |    215.7816 μs |        742.500 μs |
| StandardDictionary_Remove       | 10000   |        869.499 μs |     62.9090 μs |    184.5012 μs |        835.700 μs |
| EsentPersistentDictionary_Remove      | 10000   |     76,566.850 μs |  1,515.7748 μs |  1,745.5685 μs |     76,654.200 μs |
| **FastPersistentDictionary_Add**    | **100000**  |    **229,172.487 μs** |  **4,581.7610 μs** |  **5,794.4611 μs** |    **228,866.700 μs** |
| StandardDictionary_Add          | 100000  |     32,888.018 μs |  1,342.1441 μs |  3,696.6548 μs |     31,588.850 μs |
| EsentPersistentDictionary_Add         | 100000  |  1,249,948.586 μs | 19,460.0175 μs | 17,250.7987 μs |  1,249,580.750 μs |
| FastPersistentDictionary_Get    | 100000  |     47,154.111 μs |    884.0972 μs |  1,501.2643 μs |     47,125.200 μs |
| StandardDictionary_Get          | 100000  |     13,312.728 μs |    287.6890 μs |    830.0487 μs |     13,184.250 μs |
| EsentPersistentDictionary_Get         | 100000  |    328,134.381 μs |  5,873.8948 μs |  4,904.9682 μs |    328,413.050 μs |
| FastPersistentDictionary_Remove | 100000  |      8,773.308 μs |    175.0641 μs |    227.6328 μs |      8,712.500 μs |
| StandardDictionary_Remove       | 100000  |     13,193.624 μs |    335.9221 μs |    979.9007 μs |     13,229.400 μs |
| EsentPersistentDictionary_Remove      | 100000  |    803,621.169 μs | 15,802.8693 μs | 15,520.5307 μs |    800,826.950 μs |
| **FastPersistentDictionary_Add**    | **1000000** |  **2,295,026.753 μs** | **35,852.7067 μs** | **33,536.6441 μs** |  **2,292,892.900 μs** |
| StandardDictionary_Add          | 1000000 |    519,974.638 μs | 14,354.2653 μs | 41,872.0685 μs |    509,290.700 μs |
| EsentPersistentDictionary_Add         | 1000000 | 13,038,880.720 μs | 82,744.1640 μs | 77,398.9424 μs | 13,047,758.000 μs |
| FastPersistentDictionary_Get    | 1000000 |    493,184.675 μs |  9,822.8597 μs | 11,312.0199 μs |    492,070.800 μs |
| StandardDictionary_Get          | 1000000 |    151,635.342 μs |  3,024.3169 μs |  3,361.5202 μs |    152,587.600 μs |
| EsentPersistentDictionary_Get         | 1000000 |  3,279,782.257 μs | 55,003.8343 μs | 51,450.6208 μs |  3,301,896.650 μs |
| FastPersistentDictionary_Remove | 1000000 |    112,024.419 μs |  2,202.4345 μs |  3,087.5058 μs |    112,387.200 μs |
| StandardDictionary_Remove       | 1000000 |    152,094.443 μs |  2,997.1979 μs |  4,486.0651 μs |    152,306.050 μs |
| EsentPersistentDictionary_Remove      | 1000000 |  8,093,507.270 μs | 43,590.9941 μs | 40,775.0430 μs |  8,090,683.550 μs |


# Quick Start Demo

Setting up a FastPersistentDictionary is simple and straightforward and matches the implementation of a standard Dictionary in C# as much as possible with various additional extension methods.

```
int N = 100000;
private FastPersistentDictionary<string, string> fastPersistentDictionary= new FastPersistentDictionary<string, string>(path: fastPersistentDictionary);
//Strings for example but can work with any type.
 for (int i = 0; i < N; i++)
 {
     string key = $"Key{i}";
     string value = $"Value{i}";
     FastPersistentDictionary.Add(key, value); 
 }


 for (int i = 0; i < N; i++)
 {
     string key = $"Key{i}";
     string value;
     bool found = FastPersistentDictionary.TryGetValue(key, out value); 
 }
```


# Table of Contents
- [Project Title](#project-title)
- [Quick Start Demo](#quick-start-demo)
- [Table of Contents](#table-of-contents)
- [Usage](#usage)
- [Contribute](#contribute)
- [License](#license)


# Usage
[(Back to top)](#table-of-contents)

# FastPersistentDictionary

The `FastPersistentDictionary` is a highly efficient and flexible persistent dictionary for C# that provides a range of dictionary functionalities while ensuring data persistence on disk. Developed by James Grice, this library is designed to be utilized just like any other dictionary, but with the added benefit of persistency and reduced disk space usage.

## Installation

To install the `FastPersistentDictionary` library, you can download it from [GitHub](https://github.com/jgric2/Fast-Persistent-Dictionary) and include it in your project.

## Quick Start Guide

### 1. Creating a FastPersistentDictionary

Here's a simple example to create a `FastPersistentDictionary`:

```csharp
using FastPersistentDictionary;

var dictionary = new FastPersistentDictionary<string, int>("path/to/your/dictionary/file");
```

### 2. Adding Entries

To add entries to the dictionary:

```csharp
dictionary.Add("key1", 100);
dictionary.Add("key2", 200);
```

### 3. Accessing Entries

The `FastPersistentDictionary` can be accessed like a regular dictionary:

```csharp
int value = dictionary["key1"];
Console.WriteLine(value);  // Output: 100
```

### 4. Updating Entries

Updating existing entries is straightforward:

```csharp
dictionary["key1"] = 150;
```

### 5. Checking Existence of Keys and Values

You can check if a key or a value exists in the dictionary:

```csharp
bool hasKey = dictionary.ContainsKey("key1");    // true
bool hasValue = dictionary.ContainsValue(200);   // true
```

### 6. Removing Entries

Removing entries from the dictionary:

```csharp
dictionary.Remove("key1");
```

### 7. Iterating Over Entries

You can easily iterate over the entries in the dictionary:

```csharp
foreach (var kvp in dictionary)
{
    Console.WriteLine($"{kvp.Key} : {kvp.Value}");
}
```

### 8. Compacting the Database

To minimize the file size on the disk manually, you can compact the database:

```csharp
dictionary.CompactDatabaseFile();
```

### 9. Saving and Loading the Dictionary

You can save the current state of the dictionary to a file:

```csharp
dictionary.SaveDictionary("path/to/save/file");
```

And you can load an existing dictionary from a file:

```csharp
var loadedDictionary = new FastPersistentDictionary<string, int>("path/to/save/file");
loadedDictionary.LoadDictionary("path/to/save/file");
```

### 10. Disposing the Dictionary

Make sure to dispose of the dictionary properly when it is no longer needed:

```csharp
dictionary.Dispose();
```

## Advanced Features

### 1. Bulk Operations

You can perform bulk-read operations:

```csharp
var keys = new[] { "key1", "key2" };
var values = dictionary.GetBulk(keys);
```

### 2. Mathematical Operations

The `FastPersistentDictionary` supports mathematical operations:

```csharp
int min = dictionary.Min();
int sum = dictionary.Sum();
int max = dictionary.Max();
double average = dictionary.Average(kvp => kvp.Value);
```

### 3. Query and Filter

Advanced querying, filtering, and extracting subsets of the dictionary:

```csharp
var subset = dictionary.GetSubset((key, value) => value > 100);

bool any = dictionary.Any(kvp => kvp.Value > 150);
bool all = dictionary.All(kvp => kvp.Value > 50);
```

### 4. Importing Other Dictionaries

Import entries from another dictionary:

```csharp
var anotherDictionary = new Dictionary<string, int>
{
    { "key3", 300 },
    { "key4", 400 }
};
dictionary.Merge(anotherDictionary);
```

## Summary

The `FastPersistentDictionary` offers a wide range of functionalities for persistent data storage with minimal overhead on disk space. With the ease of use of any other dictionary and options for advanced operations, it is a powerful tool for large datasets and projects in C#.



# Contribute
[(Back to top)](#table-of-contents)

Your support is invaluable and truly welcomed! Here's how you can contribute:

- **Write Tests and Benchmarks**: Enhance code reliability and performance evaluation.
- **Enhance Documentation**: Assist others in comprehending and effectively using FastPersistentDictionary.
- **Submit Feature Requests and Bug Reports**: Suggest new ideas and report issues to help refine FastPersistentDictionary.
- **Optimize Performance**: Offer optimizations and improvements to existing features..


# License
[(Back to top)](#table-of-contents)

[MIT license](./LICENSE)


